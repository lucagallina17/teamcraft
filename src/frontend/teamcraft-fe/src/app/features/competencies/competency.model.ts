export enum CompetencyType {
    Technical = 'Technical',
    SoftSkill = 'SoftSkill'
}

export interface CompetencyDto {
    id: string;
    name: string;
    type: CompetencyType;
}

export interface CreateCompetencyDto {
    name: string;
    type: CompetencyType;
}