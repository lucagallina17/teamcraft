using TeamCraft.Application.DTOs.ProjectRole;

namespace TeamCraft.Application.Services.Interfaces;

public interface IProjectRoleService
{
    Task<ProjectRoleDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<ProjectRoleDto>> GetAllAsync();
    Task<ProjectRoleDto> CreateAsync(CreateProjectRoleDto dto);
    Task UpdateAsync(Guid Id, CreateProjectRoleDto dto);
    Task<bool> DeleteAsync(Guid Id);
}