using TeamCraft.Application.DTOs.Team;

namespace TeamCraft.Application.Services.Interfaces;

public interface ITeamMatchingService
{
    Task<List<TeamProposalDto>> GenerateProposalsAsync(Guid projectId);

    Task<double> RecalculateScoreAsync(
        Guid projectId,
        List<(Guid EmployeeId, Guid ProjectRoleRequirementId)> assignments);
}