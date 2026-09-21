using TeamCraft.Application.DTOs.Employee;

namespace TeamCraft.Application.Services.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDetailDto?> GetByIdAsync(Guid id);
    Task<EmployeeDto> CreateAsync(CreateEmployeeDto dto);
    Task UpdateAsync(Guid id, CreateEmployeeDto dto);
    Task DeleteAsync(Guid id);
    Task AddCompetencyAssessmentAsync(Guid employeeId, AddCompetencyAssessmentDto dto);
    Task RemoveCompetencyAssessmentAsync(Guid assessmentId);
}