using Microsoft.EntityFrameworkCore;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Repositories;

public class CompetencyRepository : GenericRepository<Competency>, ICompetencyRepository
{
    public CompetencyRepository(AppDbContext context) : base(context) { }

    public async Task<Competency?> GetByNameAsync(string name)
    {
        return await _context.Competencies
            .FirstOrDefaultAsync(c => c.Name == name);
    }
}