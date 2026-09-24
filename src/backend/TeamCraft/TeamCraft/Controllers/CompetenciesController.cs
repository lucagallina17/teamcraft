using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamCraft.Application.DTOs.Competency;
using TeamCraft.Application.Services.Interfaces;

namespace TeamCraft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompetenciesController : ControllerBase
{
    private readonly ICompetencyService _competencyService;

    public CompetenciesController(ICompetencyService competencyService)
    {
        _competencyService = competencyService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompetencyDto>>> GetAll()
    {
        var competencies = await _competencyService.GetAllAsync();
        return Ok(competencies);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<CompetencyDto>> GetById(Guid id)
    {
        var competency = await _competencyService.GetByIdAsync(id);

        if (competency == null) return NotFound();

        return Ok(competency);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CompetencyDto>> Create([FromBody] CreateCompetencyDto dto)
    {
        var competency = await _competencyService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = competency.Id }, competency);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateCompetencyDto dto)
    {
        await _competencyService.UpdateAsync(id, dto);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _competencyService.DeleteAsync(id);
        return NoContent();
    }
}