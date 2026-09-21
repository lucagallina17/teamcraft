using TeamCraft.Application.DTOs.Team;

namespace TeamCraft.Application.Services.Interfaces;

public interface ITeamService
{
    Task<IEnumerable<TeamDto>> GetByProjectIdAsync(Guid projectId);
    Task<TeamDto?> GetByIdAsync(Guid id);
    Task<TeamDto> CreateFromProposalAsync(Guid projectId, CreateTeamFromProposalDto dto);
    Task<TeamDto?> UpdateStatusAsync(Guid teamId, UpdateTeamStatusDto dto);
    Task<TeamDto?> RemoveMemberAsync(Guid teamId, Guid employeeId);
}