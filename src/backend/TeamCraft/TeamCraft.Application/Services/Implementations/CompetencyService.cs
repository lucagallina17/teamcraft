using TeamCraft.Application.DTOs.Competency;
using TeamCraft.Application.Repositories;
using TeamCraft.Application.Services.Interfaces;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Application.Services.Implementations;

public class CompetencyService : ICompetencyService
{
    private readonly ICompetencyRepository _competencyRepository;

    public CompetencyService(ICompetencyRepository competencyRepository)
    {
        _competencyRepository = competencyRepository;
    }

    public async Task<IEnumerable<CompetencyDto>> GetAllAsync()
    {
        var competencies = await _competencyRepository.GetAllAsync();

        return competencies.Select(c => new CompetencyDto
        {
            Id = c.Id,
            Name = c.Name,
            Type = c.Type
        });
    }

    public async Task<CompetencyDto?> GetByIdAsync(Guid id)
    {
        var competency = await _competencyRepository.GetByIdAsync(id);

        if (competency == null) return null;

        return new CompetencyDto
        {
            Id = competency.Id,
            Name = competency.Name,
            Type = competency.Type
        };
    }

    public async Task<CompetencyDto> CreateAsync(CreateCompetencyDto dto)
    {
        var competency = new Competency
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Type = dto.Type
        };

        await _competencyRepository.AddAsync(competency);

        return new CompetencyDto
        {
            Id = competency.Id,
            Name = competency.Name,
            Type = competency.Type
        };
    }

    public async Task UpdateAsync(Guid id, CreateCompetencyDto dto)
    {
        var competency = await _competencyRepository.GetByIdAsync(id);

        if (competency == null) return;

        competency.Name = dto.Name;
        competency.Type = dto.Type;

        await _competencyRepository.UpdateAsync(competency);
    }

    public async Task DeleteAsync(Guid id)
    {
        var competency = await _competencyRepository.GetByIdAsync(id);

        if (competency == null) return;

        await _competencyRepository.DeleteAsync(competency);
    }
}