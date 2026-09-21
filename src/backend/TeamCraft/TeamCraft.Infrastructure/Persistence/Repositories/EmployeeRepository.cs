using Microsoft.EntityFrameworkCore;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext context) : base(context) { }

    public async Task<Employee?> GetByIdWithCompetenciesAsync(Guid id)
    {
        return await _context.Employees
            .Include(e => e.CompetencyAssessments)
                .ThenInclude(a => a.Competency)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == email);
    }

    public async Task<IEnumerable<Employee>> GetAllWithCompetenciesAsync()
    {
        return await _context.Employees
            .Include(e => e.CompetencyAssessments)
                .ThenInclude(a => a.Competency)
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName) // aggiunto
            .ToListAsync();
    }

    public async Task AddCompetencyAssessmentAsync(EmployeeCompetencyAssessment assessment)
    {
        _context.Set<EmployeeCompetencyAssessment>().Add(assessment);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveCompetencyAssessmentAsync(Guid assessmentId)
    {
        var assessment = await _context.Set<EmployeeCompetencyAssessment>().FindAsync(assessmentId);
        if (assessment == null) return;
        _context.Set<EmployeeCompetencyAssessment>().Remove(assessment);
        await _context.SaveChangesAsync();
    }

    public async Task<Employee?> GetByIdWithTeamHistoryAsync(Guid id)
    {
        return await _context.Employees
            .Include(e => e.TeamMembers)
                .ThenInclude(tm => tm.Team)
                    .ThenInclude(t => t.Project)
            .Include(e => e.TeamMembers)
                .ThenInclude(tm => tm.ProjectRole)
            .FirstOrDefaultAsync(e => e.Id == id);
    }
}