using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeamCraft.Application.Commands;
using TeamCraft.Application.DTOs.Team;
using TeamCraft.Application.Services.Interfaces;
using TeamCraft.Domain.Enums;

namespace TeamCraft.API.Controllers;

[ApiController]
[Route("api/projects/{projectId}/teams")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;
    private readonly ITeamMatchingService _teamMatchingService;
    public readonly IMediator _mediator;

    public TeamsController(ITeamService teamService, ITeamMatchingService teamMatchingService, IMediator mediator)
    {
        _teamService = teamService;
        _teamMatchingService = teamMatchingService;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeamDto>>> GetByProject(Guid projectId)
    {
        var teams = await _teamService.GetByProjectIdAsync(projectId);
        return Ok(teams);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDto>> GetById(Guid projectId, Guid id)
    {
        var team = await _teamService.GetByIdAsync(id);

        if (team == null) return NotFound();

        return Ok(team);
    }

    /// Genera 3 proposte di team basate su competenze e affinità — non salva nulla
    [HttpGet("proposals")]
    public async Task<ActionResult<List<TeamProposalDto>>> GetProposals(Guid projectId)
    {
        var proposals = await _teamMatchingService.GenerateProposalsAsync(projectId);
        return Ok(proposals);
    }

    /// Salva la proposta scelta dall'HR come team effettivo
    [HttpPost]
    public async Task<ActionResult<TeamDto>> Create(Guid projectId, [FromBody] CreateTeamFromProposalDto dto)
    {
        var team = await _teamService.CreateFromProposalAsync(projectId, dto);
        return CreatedAtAction(nameof(GetById), new { projectId, id = team.Id }, team);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<TeamDto>> UpdateStatus(Guid projectId, Guid id, [FromBody] UpdateTeamStatusDto dto)
    {
        var status = Enum.Parse<TeamStatus>(dto.Status);
        TeamDto? team;

        if (status == TeamStatus.Active)
        {
            var result = await _mediator.Send(new ActivateTeamCommand(id));
            team = await _teamService.GetByIdAsync(result.Id);
        }
        else
        {
            team = await _teamService.UpdateStatusAsync(id, dto);
        }

        if (team == null) return NotFound();
        return Ok(team);
    }

    [HttpDelete("{id}/employees/{employeeId}")]
    public async Task<ActionResult<TeamDto>> RemoveMember(Guid projectId, Guid id, Guid employeeId)
    {
        var team = await _teamService.RemoveMemberAsync(id, employeeId);
        if (team == null) return NotFound();
        return Ok(team);
    }
}