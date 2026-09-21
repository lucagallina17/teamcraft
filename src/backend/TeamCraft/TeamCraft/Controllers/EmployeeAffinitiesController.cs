using Microsoft.AspNetCore.Mvc;
using TeamCraft.Application.DTOs.Employee;
using TeamCraft.Application.Services.Interfaces;

namespace TeamCraft.API.Controllers;

[ApiController]
[Route("api/employee-affinities")]
public class EmployeeAffinitiesController : ControllerBase
{
    private readonly IEmployeeAffinityService _service;

    public EmployeeAffinitiesController(IEmployeeAffinityService service) => _service = service;

    [HttpGet("colleagues")]
    public async Task<ActionResult<IEnumerable<ColleaguePairDto>>> GetColleagues()
    {
        return Ok(await _service.GetColleaguesWithAffinityAsync());
    }

    [HttpPost]
    public async Task<ActionResult<ColleaguePairDto>> SetAffinity([FromBody] SetAffinityDto dto)
    {
        return Ok(await _service.SetAffinityAsync(dto));
    }
}