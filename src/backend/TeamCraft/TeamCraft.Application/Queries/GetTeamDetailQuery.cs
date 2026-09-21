

using TeamCraft.Application.Repositories;

namespace TeamCraft.Application.Queries
{
    public class TeamReadResultDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
    }

    public record GetTeamDetailQuery(Guid Id);
    public class GetTeamDetailQueryHandler
    {
        private readonly ITeamReadRepository _teamReadRepository;
        public GetTeamDetailQueryHandler(ITeamReadRepository teamReadRepository)
        {
            _teamReadRepository = teamReadRepository;
        }

        public async Task<TeamReadResultDto?> Handle(GetTeamDetailQuery query)
        {
            var team = await _teamReadRepository.GetByIdWithMembersAsync(query.Id);

            if (team == null)
            {
                return null;
            }

            var teamResultDto = new TeamReadResultDto
                {
                    Id = team.Id,
                    ProjectId = team.ProjectId,
                };
                
            return teamResultDto;
        }
    }
}
