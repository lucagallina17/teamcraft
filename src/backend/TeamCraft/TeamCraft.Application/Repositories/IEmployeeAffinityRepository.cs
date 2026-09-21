using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Application.Interfaces.Repositories;

public interface IEmployeeAffinityRepository : IGenericRepository<EmployeeAffinity>
{
    Task<IEnumerable<EmployeeAffinity>> GetForEmployeesAsync(IEnumerable<Guid> employeeIds);
    Task<EmployeeAffinity?> GetByEmployeePairAsync(Guid employeeId1, Guid employeeId2);
    Task<IEnumerable<(Guid EmployeeId1, Guid EmployeeId2)>> GetColleaguePairsAsync();
}