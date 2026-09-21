export enum ProjectStatus {
    Draft = "Draft",
    Active = "Active",
    Completed = "Completed",
    Cancelled = "Cancelled"
}

export enum RequirementType {
    Required = "Required",
    Preferred = "Preferred",
}

export interface ProjectDto {
    id: string;
    name: string;
    description: string;
    status: ProjectStatus;
    startDate: string;
    endDate: string;
}

export interface ProjectDetailDto extends ProjectDto {
    roleRequirements: ProjectRoleRequirementDto[];
}

export interface CreateProjectDto {
    name: string;
    description: string;
    startDate: string;
    endDate: string;
}

export interface ProjectRoleRequirementDto {
    id: string;
    roleName: string;
    quantity: number;
    competencies: RequirementCompetencyDto[];
}

export interface RequirementCompetencyDto {
    id: string;
    competencyId: string;
    competencyName: string;
    minimumLevel: number;
    weight: number;
    requirementType: RequirementType;
}

export interface AddRequirementDto {
    projectRoleId: string;
    quantity: number;
}

export interface AddRequirementCompetencyDto {
    competencyId: string;
    minimumLevel?: number;
    weight?: number;
    requirementType: RequirementType;
}

export interface UpdateProjectStatusDto {
    status: ProjectStatus;
}