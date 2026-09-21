using TeamCraft.Application.DTOs.ProjectRole;
using TeamCraft.Application.Services.Interfaces;
using TeamCraft.Domain.Entities;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Exceptions;

namespace TeamCraft.Application.Services.Implementations;

public class ProjectRoleService : IProjectRoleService
{
    private readonly IProjectRoleRepository _repository;

    public ProjectRoleService(IProjectRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProjectRoleDto?> GetByIdAsync(Guid id)
    {
        var role = await _repository.GetByIdAsync(id);
        if (role == null) return null;
        return new ProjectRoleDto { Id = role.Id, Name = role.Name, Description = role.Description };
    }

    public async Task<IEnumerable<ProjectRoleDto>> GetAllAsync()
    {
        var roles = await _repository.GetAllAsync();
        return roles.Select(r => new ProjectRoleDto { Id = r.Id, Name = r.Name, Description = r.Description });
    }

    public async Task<ProjectRoleDto> CreateAsync(CreateProjectRoleDto dto)
    {
        var role = new ProjectRole { Id = Guid.NewGuid(), Name = dto.Name, Description = dto.Description };
        await _repository.AddAsync(role);
        return new ProjectRoleDto { Id = role.Id, Name = role.Name, Description = role.Description };
    }

    public async Task UpdateAsync(Guid id, CreateProjectRoleDto dto)
    {
        var role = await _repository.GetByIdAsync(id);
        if (role == null) return;

        role.Name = dto.Name;
        role.Description = dto.Description;

        await _repository.UpdateAsync(role);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var role = await _repository.GetByIdAsync(id);
        if (role == null) return true;

        try
        {
            await _repository.DeleteAsync(role);
            return true;
        }
        catch (EntityInUseException)
        {
            // Il ruolo è referenziato da ProjectRoleRequirement o TeamMember (DeleteBehavior.Restrict)
            return false;
        }
    }
}