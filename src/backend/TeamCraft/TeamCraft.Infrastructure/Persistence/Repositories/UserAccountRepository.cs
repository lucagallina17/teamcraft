using Microsoft.EntityFrameworkCore;
using TeamCraft.Application.Repositories;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Infrastructure.Persistence.Repositories;

public class UserAccountRepository : GenericRepository<UserAccount>, IUserAccountRepository
{
    public UserAccountRepository(AppDbContext context) : base(context) {}

    public async Task<UserAccount?> GetByEmailAsync(string email)
    {
         return await _context.UserAccounts
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}
