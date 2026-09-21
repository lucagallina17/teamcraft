using Microsoft.AspNetCore.Mvc;
using TeamCraft.Application.DTOs.Project;
using TeamCraft.Application.Services.Interfaces;

namespace TeamCraft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetAll()
    {
        var projects = await _projectService.GetAllAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDetailDto>> GetById(Guid id)
    {
        var project = await _projectService.GetByIdAsync(id);

        if (project == null) return NotFound();

        return Ok(project);
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(
        [FromBody] CreateProjectDto dto,
        [FromQuery] Guid createdBy)
    {
        var project = await _projectService.CreateAsync(dto, createdBy);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateProjectDto dto)
    {
        await _projectService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var project = await _projectService.GetByIdAsync(id);
        if (project == null) return NotFound();

        var deleted = await _projectService.DeleteAsync(id);
        if (!deleted)
        {
            var message = project.Status == 0 // Draft
                ? "Impossibile eliminare il progetto: è referenziato altrove."
                : "Un progetto non più in bozza non può essere eliminato — usa invece lo stato 'Annullato'.";

            return Conflict(new { message });
        }

        return NoContent();
    }

    [HttpPost("{id}/requirements")]
    public async Task<ActionResult<ProjectRoleRequirementDto>> AddRequirement(Guid id, [FromBody] AddRequirementDto dto)
    {
        var requirement = await _projectService.AddRequirementAsync(id, dto);
        return Ok(requirement);
    }

    [HttpPost("requirements/{requirementId}/competencies")]
    public async Task<IActionResult> AddRequirementCompetency(Guid requirementId, [FromBody] AddRequirementCompetencyDto dto)
    {
        await _projectService.AddRequirementCompetencyAsync(requirementId, dto);
        return NoContent();
    }

    [HttpDelete("requirements/{requirementId}")]
    public async Task<IActionResult> RemoveRequirement(Guid requirementId)
    {
        await _projectService.RemoveRequirementAsync(requirementId);
        return NoContent();
    }

    [HttpPut("requirements/competencies/{id}")]
    public async Task<IActionResult> UpdateRequirementCompetency(Guid id, [FromBody] AddRequirementCompetencyDto dto)
    {
        await _projectService.UpdateRequirementCompetencyAsync(id, dto);
        return NoContent();
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<ProjectDto>> UpdateStatus(Guid id, [FromBody] UpdateProjectStatusDto dto)
    {
        try
        {
            var project = await _projectService.UpdateStatusAsync(id, dto);
            if (project == null) return NotFound();
            return Ok(project);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}