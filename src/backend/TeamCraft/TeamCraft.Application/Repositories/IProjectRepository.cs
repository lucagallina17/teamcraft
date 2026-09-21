using TeamCraft.Domain.Entities;

namespace TeamCraft.Application.Repositories
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<Project?> GetByIdWithRequirementsAsync(Guid id);
        Task<IEnumerable<Project>> GetAllWithRequirementsAsync();
        Task<ProjectRoleRequirement> AddRequirementAsync(ProjectRoleRequirement requirement);
        Task<ProjectRoleRequirement?> GetRequirementByIdAsync(Guid requirementId);
        Task AddRequirementCompetencyAsync(ProjectRoleRequirementCompetency competency);
        Task RemoveRequirementAsync(ProjectRoleRequirement requirement);
        Task<ProjectRoleRequirementCompetency?> GetRequirementCompetencyByIdAsync(Guid id);
        Task UpdateRequirementCompetencyAsync(ProjectRoleRequirementCompetency competency);
    }
}
