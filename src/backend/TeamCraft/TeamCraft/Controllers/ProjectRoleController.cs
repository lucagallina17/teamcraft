using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamCraft.Application.DTOs.ProjectRole;
using TeamCraft.Application.Services.Interfaces;

namespace TeamCraft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectRolesController : ControllerBase
{
    private readonly IProjectRoleService _service;

    public ProjectRolesController(IProjectRoleService service) => _service = service;

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectRoleDto>> GetById(Guid id)
    {
        var role = await _service.GetByIdAsync(id);
        if (role == null) return NotFound();
        return Ok(role);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectRoleDto>>> GetAll() => Ok(await _service.GetAllAsync());

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProjectRoleDto>> Create([FromBody] CreateProjectRoleDto dto) =>
        Ok(await _service.CreateAsync(dto));

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateProjectRoleDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        if (!deleted)
            return Conflict(new { message = "Il ruolo è utilizzato in uno o più progetti e non può essere eliminato." });

        return NoContent();
    }
}