using TeamCraft.Application.DTOs.Competency;

namespace TeamCraft.Application.Services.Interfaces;

public interface ICompetencyService
{
    Task<IEnumerable<CompetencyDto>> GetAllAsync();
    Task<CompetencyDto?> GetByIdAsync(Guid id);
    Task<CompetencyDto> CreateAsync(CreateCompetencyDto dto);
    Task UpdateAsync(Guid id, CreateCompetencyDto dto);
    Task DeleteAsync(Guid id);
}