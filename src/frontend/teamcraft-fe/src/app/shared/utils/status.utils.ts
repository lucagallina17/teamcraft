import { TeamStatus } from '../../features/teams/team.model';

/**
 * Etichetta leggibile in italiano per uno stato del team.
 * Centralizzata qui per evitare duplicazioni tra TeamDetail, ProjectDetail
 * e qualunque altro componente che mostri lo stato di un team.
 */
export function teamStatusLabel(status: TeamStatus): string {
    const labels: Record<TeamStatus, string> = {
        [TeamStatus.Proposed]: 'Proposed',
        [TeamStatus.Active]: 'Active',
        [TeamStatus.Closed]: 'Closed',
    };
    return labels[status];
}

/**
 * Severity PrimeNG (colore del p-tag) associata a uno stato del team.
 */
export function teamStatusSeverity(
    status: TeamStatus
): 'secondary' | 'success' | 'danger' {
    const severities: Record<TeamStatus, 'secondary' | 'success' | 'danger'> = {
        [TeamStatus.Proposed]: 'secondary',
        [TeamStatus.Active]: 'success',
        [TeamStatus.Closed]: 'danger',
    };
    return severities[status];
}