using TeamCraft.Application.DTOs.Team;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Application.Repositories
{
    public interface ITeamReadRepository
    {
        Task<TeamDto?> GetByIdWithMembersAsync(Guid id);
        Task<IEnumerable<TeamDto>> GetByProjectIdAsync(Guid projectId);
        Task<IEnumerable<Guid>> GetEmployeeIdsInActiveTeamsAsync();
    }
}
