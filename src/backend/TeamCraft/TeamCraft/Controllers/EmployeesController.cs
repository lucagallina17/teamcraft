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

    /// <summary>
    /// Restituisce la lista di tutti i dipendenti
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
    {
        var employees = await _employeeService.GetAllAsync();
        return Ok(employees);
    }

    /// <summary>
    /// Restituisce il dettaglio di un dipendente con le sue competenze
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EmployeeDetailDto>> GetById(Guid id)
    {
        var employee = await _employeeService.GetByIdAsync(id);

        if (employee == null) return NotFound();

        return Ok(employee);
    }

    /// <summary>
    /// Crea un nuovo dipendente
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EmployeeDto>> Create([FromBody] CreateEmployeeDto dto)
    {
        var employee = await _employeeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = employee.Id }, employee);
    }

    /// <summary>
    /// Aggiorna un dipendente esistente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateEmployeeDto dto)
    {
        await _employeeService.UpdateAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Elimina un dipendente
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _employeeService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/competencies")]
    public async Task<IActionResult> AddCompetency(Guid id, [FromBody] AddCompetencyAssessmentDto dto)
    {
        await _employeeService.AddCompetencyAssessmentAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("competencies/{assessmentId}")]
    public async Task<IActionResult> RemoveCompetency(Guid assessmentId)
    {
        await _employeeService.RemoveCompetencyAssessmentAsync(assessmentId);
        return NoContent();
    }
}