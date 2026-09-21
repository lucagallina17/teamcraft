using Microsoft.EntityFrameworkCore;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Exceptions;

namespace TeamCraft.Infrastructure.Persistence.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

   public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            // Un vincolo di integrità referenziale (DeleteBehavior.Restrict) ha bloccato l'eliminazione
            // perché l'entità è referenziata altrove — traduciamo in un'eccezione di dominio
            // così Application non deve conoscere EF Core per gestire questo scenario
            throw new EntityInUseException(
                $"Impossibile eliminare l'elemento perché è utilizzato altrove nel sistema.");
        }
    }
}