using TeamCraft.Domain.Entities;

namespace TeamCraft.Application.Repositories
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee> GetByIdWithCompetenciesAsync(Guid id);
        Task<Employee> GetByEmailAsync(string email);
        Task<IEnumerable<Employee>> GetAllWithCompetenciesAsync();
        Task AddCompetencyAssessmentAsync(EmployeeCompetencyAssessment assessment);
        Task RemoveCompetencyAssessmentAsync(Guid assessmentId);
        Task<Employee?> GetByIdWithTeamHistoryAsync(Guid id);
    }
}
