using TeamCraft.Application.DTOs.Project;
using TeamCraft.Application.Repositories;
using TeamCraft.Application.Services.Interfaces;
using TeamCraft.Domain.Entities;
using TeamCraft.Domain.Enums;
using TeamCraft.Domain.Exceptions;

namespace TeamCraft.Application.Services.Implementations;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync()
    {
        var projects = await _projectRepository.GetAllAsync();

        return projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Status = p.Status,
            StartDate = p.StartDate,
            EndDate = p.EndDate
        });
    }

    public async Task<ProjectDetailDto?> GetByIdAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdWithRequirementsAsync(id);

        if (project == null) return null;

        return new ProjectDetailDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            RoleRequirements = project.ProjectRoleRequirements.Select(r => new ProjectRoleRequirementDto
            {
                Id = r.Id,
                RoleName = r.ProjectRole.Name,
                Quantity = r.Quantity,
                Competencies = r.RequirementCompetencies.Select(c => new RequirementCompetencyDto
                {
                    Id = c.Id,
                    CompetencyId = c.CompetencyId,
                    CompetencyName = c.Competency.Name,
                    MinimumLevel = c.MinimumLevel,
                    Weight = c.Weight,
                    RequirementType = c.RequirementType
                }).ToList()
            }).ToList()
        };
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, Guid createdBy)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Status = ProjectStatus.Draft,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            CreatedBy = createdBy
        };

        await _projectRepository.AddAsync(project);

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate
        };
    }

    public async Task UpdateAsync(Guid id, CreateProjectDto dto)
    {
        var project = await _projectRepository.GetByIdAsync(id);

        if (project == null) return;

        project.Name = dto.Name;
        project.Description = dto.Description;
        project.StartDate = dto.StartDate;
        project.EndDate = dto.EndDate;

        await _projectRepository.UpdateAsync(project);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null) return true;

        // Un progetto può essere eliminato fisicamente solo mentre è ancora in bozza —
        // dopo, l'unica via distruttiva prevista è lo stato Cancelled, che preserva lo storico
        if (project.Status != ProjectStatus.Draft)
        {
            return false;
        }

        try
        {
            await _projectRepository.DeleteAsync(project);
            return true;
        }
        catch (EntityInUseException)
        {
            return false;
        }
    }

    public async Task<ProjectRoleRequirementDto> AddRequirementAsync(Guid projectId, AddRequirementDto dto)
    {
        var requirement = new ProjectRoleRequirement
        {
            Id = Guid.NewGuid(),
            ProjectId = projectId,
            ProjectRoleId = dto.ProjectRoleId,
            Quantity = dto.Quantity
        };

        await _projectRepository.AddRequirementAsync(requirement);
        var saved = await _projectRepository.GetRequirementByIdAsync(requirement.Id);

        return new ProjectRoleRequirementDto
        {
            Id = saved!.Id,
            RoleName = saved.ProjectRole.Name,
            Quantity = saved.Quantity,
            Competencies = new List<RequirementCompetencyDto>()
        };
    }

    public async Task AddRequirementCompetencyAsync(Guid requirementId, AddRequirementCompetencyDto dto)
    {
        var competency = new ProjectRoleRequirementCompetency
        {
            Id = Guid.NewGuid(),
            ProjectRoleRequirementId = requirementId,
            CompetencyId = dto.CompetencyId,
            MinimumLevel = dto.MinimumLevel,
            Weight = dto.Weight,
            RequirementType = Enum.Parse<RequirementType>(dto.RequirementType)
        };

        await _projectRepository.AddRequirementCompetencyAsync(competency);
    }

    public async Task RemoveRequirementAsync(Guid requirementId)
    {
        var requirement = await _projectRepository.GetRequirementByIdAsync(requirementId);
        if (requirement == null) return;
        await _projectRepository.RemoveRequirementAsync(requirement);
    }

    public async Task UpdateRequirementCompetencyAsync(Guid id, AddRequirementCompetencyDto dto)
    {
        var competency = await _projectRepository.GetRequirementCompetencyByIdAsync(id);
        if (competency == null) return;

        competency.MinimumLevel = dto.MinimumLevel;
        competency.Weight = dto.Weight;
        competency.RequirementType = Enum.Parse<RequirementType>(dto.RequirementType);

        await _projectRepository.UpdateRequirementCompetencyAsync(competency);
    }

    public async Task<ProjectDto?> UpdateStatusAsync(Guid id, UpdateProjectStatusDto dto)
    {
        var project = await _projectRepository.GetByIdAsync(id);
        if (project == null) return null;

        var newStatus = Enum.Parse<ProjectStatus>(dto.Status);

        var allowedTransitions = new Dictionary<ProjectStatus, ProjectStatus[]>
        {
            [ProjectStatus.Draft] = new[] { ProjectStatus.Cancelled },
            [ProjectStatus.Active] = new[] { ProjectStatus.Completed, ProjectStatus.Cancelled },
            [ProjectStatus.Completed] = Array.Empty<ProjectStatus>(),
            [ProjectStatus.Cancelled] = Array.Empty<ProjectStatus>()
        };

        if (!allowedTransitions[project.Status].Contains(newStatus))
        {
            throw new InvalidOperationException(
                $"Non è possibile passare da {project.Status} a {newStatus}.");
        }

        project.Status = newStatus;
        await _projectRepository.UpdateAsync(project);

        return new ProjectDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Status = project.Status,
            StartDate = project.StartDate,
            EndDate = project.EndDate
        };
    }
}