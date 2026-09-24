using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeamCraft.Application.DTOs.Employee;
using TeamCraft.Application.Services.Interfaces;

namespace TeamCraft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
    {
        var employees = await _employeeService.GetAllAsync();
        return Ok(employees);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDetailDto>> GetById(Guid id)
    {
        var employee = await _employeeService.GetByIdAsync(id);

        if (employee == null) return NotFound();

        return Ok(employee);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeDto dto)
    {
        var employee = await _employeeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateEmployeeDto dto)
    {
        await _employeeService.UpdateAsync(id, dto);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _employeeService.DeleteAsync(id);
        return NoContent();
    }

    [Authorize]
    [HttpPost("{id}/competencies")]
    public async Task<IActionResult> AddCompetency(Guid id, [FromBody] AddCompetencyAssessmentDto dto)
    {
        await _employeeService.AddCompetencyAssessmentAsync(id, dto);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("competencies/{assessmentId}")]
    public async Task<IActionResult> RemoveCompetency(Guid assessmentId)
    {
        await _employeeService.RemoveCompetencyAssessmentAsync(assessmentId);
        return NoContent();
    }
}