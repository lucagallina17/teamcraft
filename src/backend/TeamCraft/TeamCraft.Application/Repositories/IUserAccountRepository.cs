using TeamCraft.Domain.Entities;

namespace TeamCraft.Application.Repositories
{
    public interface IUserAccountRepository : IGenericRepository<UserAccount>
    {
        Task<UserAccount?> GetByEmailAsync(string email);
    }
}
