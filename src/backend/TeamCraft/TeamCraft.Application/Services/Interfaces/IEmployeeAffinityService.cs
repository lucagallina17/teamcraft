using TeamCraft.Application.DTOs.Employee;

namespace TeamCraft.Application.Services.Interfaces;

public interface IEmployeeAffinityService
{
    Task<IEnumerable<ColleaguePairDto>> GetColleaguesWithAffinityAsync();
    Task<ColleaguePairDto> SetAffinityAsync(SetAffinityDto dto);
}