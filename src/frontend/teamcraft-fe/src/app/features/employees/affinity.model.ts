export interface ColleaguePairDto {
    employee1Id: string;
    employee1Name: string;
    employee2Id: string;
    employee2Name: string;
    hasAffinityDefined: boolean;
    score: number | null;
    status: string;
}

export interface SetAffinityDto {
    employeeId1: string;
    employeeId2: string;
    score: number;
}