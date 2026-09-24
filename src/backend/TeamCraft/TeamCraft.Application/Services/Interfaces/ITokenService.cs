using TeamCraft.Domain.Entities;

namespace TeamCraft.Application.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(UserAccount user);
    }
}
