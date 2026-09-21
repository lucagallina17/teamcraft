using TeamCraft.Application.DTOs.Team;
using TeamCraft.Application.Services.Interfaces;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;
using TeamCraft.Domain.Enums;
using TeamCraft.Domain.Exceptions;

namespace TeamCraft.Application.Services.Implementations
{
    public class TeamService : ITeamService
    {
        private readonly ITeamAggregateRepository _teamAggregateRepository;
        private readonly ITeamReadRepository _teamReadRepository;
        private readonly ITeamMatchingService _teamMatchingService;
        private readonly IProjectRepository _projectRepository;

        public TeamService(ITeamAggregateRepository teamAggregateRepository, ITeamReadRepository teamReadRepository, ITeamMatchingService teamMatchingService, IProjectRepository projectRepository)
        {
            _teamAggregateRepository = teamAggregateRepository;
            _teamReadRepository = teamReadRepository;
            _teamMatchingService = teamMatchingService;
            _projectRepository = projectRepository;
        }

        public async Task<IEnumerable<TeamDto>> GetByProjectIdAsync(Guid projectId)
        {
            var team = await _teamReadRepository.GetByProjectIdAsync(projectId);

            return team;
        }

        public async Task<TeamDto?> GetByIdAsync(Guid id)
        {
            var team = await _teamReadRepository.GetByIdWithMembersAsync(id);

            if (team == null) return null;

            return team;
        }

        public async Task<TeamDto> CreateFromProposalAsync(Guid projectId, CreateTeamFromProposalDto dto)
        {
            var assignments = dto.Members
                .Select(m => (m.EmployeeId, m.ProjectRoleRequirementId))
                .ToList();

            var score = await _teamMatchingService.RecalculateScoreAsync(projectId, assignments);

            var team = new TeamAggregate(projectId);

            foreach (var member in dto.Members)
            {
                team.AddMember(member.EmployeeId, member.ProjectRoleId);
            }

            team.SetScore((float)score);

            await _teamAggregateRepository.AddAsync(team);

            var saved = await _teamReadRepository.GetByIdWithMembersAsync(team.Id);
            return saved!;
        }

        public async Task<TeamDto?> UpdateStatusAsync(Guid teamId, UpdateTeamStatusDto dto)
        {
            var team = await _teamAggregateRepository.GetByIdAsync(teamId);
            if (team == null) return null;

            var newStatus = Enum.Parse<TeamStatus>(dto.Status);

            switch (newStatus)
            {
                case TeamStatus.Active: { team.Activate(); } break;
                case TeamStatus.Closed: { team.Close(); } break;
                case TeamStatus.Proposed: { throw new DomainRuleViolationException("Un team non può essere riportato allo stato Proposed"); }
            }

            await _teamAggregateRepository.UpdateAsync(team);

            // Il progetto diventa Active solo quando il team passa da Proposed ad Approved —
            // cioè quando l'HR conferma effettivamente la proposta, non quando viene solo generata
            if (newStatus == TeamStatus.Active)
            {
                var project = await _projectRepository.GetByIdAsync(team.ProjectId);
                if (project != null && project.Status == ProjectStatus.Draft)
                {
                    project.Status = ProjectStatus.Active;
                    await _projectRepository.UpdateAsync(project);
                }
            }

            var updated = await _teamReadRepository.GetByIdWithMembersAsync(teamId);
            return updated;
        }

        public async Task<TeamDto?> RemoveMemberAsync(Guid teamId, Guid employeeId)
        {
            var team = await _teamAggregateRepository.GetByIdAsync(teamId);
            if (team == null) return null;

            team.RemoveMember(employeeId);
            await _teamAggregateRepository.UpdateAsync(team);

            var updated = await _teamReadRepository.GetByIdWithMembersAsync(teamId);
            return updated;
        }
    }
}
