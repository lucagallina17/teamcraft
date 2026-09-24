using TeamCraft.Application.DTOs.Auth;

namespace TeamCraft.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
        Task<AuthResponseDto?> RegisterAsync(RegisterDto dto);
    }
}
