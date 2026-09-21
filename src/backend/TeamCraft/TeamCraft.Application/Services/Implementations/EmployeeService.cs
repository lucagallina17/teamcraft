using TeamCraft.Application.Services.Interfaces;
using TeamCraft.Application.DTOs.Employee;
using TeamCraft.Domain.Entities;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Enums;

namespace TeamCraft.Application.Services.Implementations;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        var employees = await _employeeRepository.GetAllAsync();

        return employees.Select(e => new EmployeeDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Email = e.Email
        });
    }

    public async Task<EmployeeDetailDto?> GetByIdAsync(Guid id)
    {
        var employee = await _employeeRepository.GetByIdWithCompetenciesAsync(id);

        if (employee == null) return null;

        // Serve una seconda query per lo storico team, dato che GetByIdWithCompetenciesAsync
        // non carica i TeamMembers — separiamo le due Include per evitare un cartesian explosion
        // nella query SQL generata da EF Core quando si combinano più collection Include insieme
        var employeeWithTeams = await _employeeRepository.GetByIdWithTeamHistoryAsync(id);

        return new EmployeeDetailDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Competencies = employee.CompetencyAssessments.Select(a => new EmployeeCompetencyDto
            {
                Id = a.Id, // ← aggiunto
                CompetencyId = a.CompetencyId,
                CompetencyName = a.Competency.Name,
                CompetencyType = a.Competency.Type,
                Level = a.Level,
                Source = a.Source.ToString(),
                Date = a.Date
            }).ToList(),
            TeamHistory = employeeWithTeams!.TeamMembers.Select(tm => new EmployeeTeamHistoryDto
            {
                TeamId = tm.TeamId,
                ProjectId = tm.Team.ProjectId,
                ProjectName = tm.Team.Project.Name,
                RoleName = tm.ProjectRole.Name,
                TeamStatus = tm.Team.Status.ToString(),
                TeamCreatedAt = tm.Team.CreatedAt
            }).OrderByDescending(t => t.TeamCreatedAt).ToList()
        };
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto)
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            CreatedAt = DateTime.UtcNow
        };

        await _employeeRepository.AddAsync(employee);

        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email
        };
    }

    public async Task UpdateAsync(Guid id, CreateEmployeeDto dto)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null) return;

        employee.FirstName = dto.FirstName;
        employee.LastName = dto.LastName;
        employee.Email = dto.Email;

        await _employeeRepository.UpdateAsync(employee);
    }

    public async Task DeleteAsync(Guid id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);

        if (employee == null) return;

        await _employeeRepository.DeleteAsync(employee);
    }
    public async Task AddCompetencyAssessmentAsync(Guid employeeId, AddCompetencyAssessmentDto dto)
    {
        var assessment = new EmployeeCompetencyAssessment
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            CompetencyId = dto.CompetencyId,
            Level = dto.Level,
            Source = Enum.Parse<AssessmentSource>(dto.Source),
            Date = dto.Date
        };

        await _employeeRepository.AddCompetencyAssessmentAsync(assessment);
    }

    public async Task RemoveCompetencyAssessmentAsync(Guid assessmentId)
    {
        await _employeeRepository.RemoveCompetencyAssessmentAsync(assessmentId);
    }
}