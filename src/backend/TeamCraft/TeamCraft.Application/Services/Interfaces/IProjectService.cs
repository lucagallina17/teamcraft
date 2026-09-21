using TeamCraft.Application.DTOs.Project;

namespace TeamCraft.Application.Services.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllAsync();
    Task<ProjectDetailDto?> GetByIdAsync(Guid id);
    Task<ProjectDto> CreateAsync(CreateProjectDto dto, Guid createdBy);
    Task UpdateAsync(Guid id, CreateProjectDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<ProjectRoleRequirementDto> AddRequirementAsync(Guid projectId, AddRequirementDto dto);
    Task AddRequirementCompetencyAsync(Guid requirementId, AddRequirementCompetencyDto dto);
    Task RemoveRequirementAsync(Guid requirementId);
    Task UpdateRequirementCompetencyAsync(Guid id, AddRequirementCompetencyDto dto);
    Task<ProjectDto?> UpdateStatusAsync(Guid id, UpdateProjectStatusDto dto);
}