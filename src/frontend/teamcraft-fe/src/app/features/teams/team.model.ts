export enum TeamStatus {
    Active = 'Active',
    Proposed = 'Proposed',
    Closed = 'Closed'
}

export interface TeamDto {
    id: string;
    projectId: string;
    projectName: string;
    score: number;
    status: TeamStatus;
    createdAt: string;
    members: TeamMemberDto[];
}

export interface TeamMemberDto {
    id: string;
    employeeId: string;
    fullName: string;
    roleName: string;
}

export interface TeamProposalDto {
    proposalId: string;
    totalScore: number;
    isComplete: boolean;
    members: TeamProposalMemberDto[];
}

export interface TeamProposalMemberDto {
    employeeId: string;
    fullName: string;
    projectRoleRequirementId: string;
    projectRoleId: string;
    roleName: string;
    competencyScore: number;
}

export interface CreateTeamMemberDto {
    employeeId: string;
    projectRoleRequirementId: string;
    projectRoleId: string;
}

export interface CreateTeamFromProposalDto {
    members: CreateTeamMemberDto[];
}

export interface UpdateTeamStatusDto {
    status: TeamStatus;
}