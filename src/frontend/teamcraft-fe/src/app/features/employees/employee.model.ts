import { CompetencyType } from "../competencies/competency.model";
import { TeamStatus } from "../teams/team.model";

export enum AssessmentSource {
    HRAssessment = 'HRAssessment',
    SelfAssessment = 'SelfAssessment',
    ProjectFeedback = 'ProjectFeedback'
}

export interface EmployeeDto {
    id: string;
    firstName: string;
    lastName: string;
    email: string;
}

export interface EmployeeDetailDto extends EmployeeDto {
    competencies: EmployeeCompetencyDto[];
}

export interface EmployeeCompetencyDto {
    id: string;
    competencyId: string;
    competencyName: string;
    competencyType: CompetencyType;
    level: number;
    source: AssessmentSource;
    date: string;
}

export interface CreateEmployeeDto {
    firstName: string;
    lastName: string;
    email: string;
}

export interface AddCompetencyAssessmentDto {
    competencyId: string;
    level: number;
    source: AssessmentSource;
    date: string;
}

export interface EmployeeTeamHistoryDto {
    teamId: string;
    projectId: string;
    projectName: string;
    roleName: string;
    teamStatus: TeamStatus;
    teamCreatedAt: string;
}

export interface EmployeeDetailDto extends EmployeeDto {
    competencies: EmployeeCompetencyDto[];
    teamHistory: EmployeeTeamHistoryDto[];
}