using TeamCraft.Application.DTOs.Auth;
using TeamCraft.Application.Repositories;
using TeamCraft.Application.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using TeamCraft.Domain.Entities;

namespace TeamCraft.Application.Services.Implementations;

    public class AuthService : IAuthService
    {
        private const string DefaultRole = "Employee";

        private readonly IUserAccountRepository _userAccountRepository;
        private readonly ITokenService _tokenService;   
        private readonly IEmployeeRepository _employeeRepository;

        public AuthService(IUserAccountRepository userAccountRepository, ITokenService tokenService, IEmployeeRepository employeeRepository)
        {
            _userAccountRepository = userAccountRepository;
            _tokenService = tokenService;
            _employeeRepository = employeeRepository;
        }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _userAccountRepository.GetByEmailAsync(dto.Email);

        if (user == null) {
            return null;
        }

        var hasher = new PasswordHasher<UserAccount>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var token = _tokenService.GenerateToken(user);

        var authResponse = new AuthResponseDto
        {
            Token = token,
            Email = user.Email,
            Role = user.Role
        };

        return authResponse;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        var employee = await _employeeRepository.GetByEmailAsync(dto.Email);

        if (employee == null) 
        {
            return null;
        }

        var userAccount = await _userAccountRepository.GetByEmailAsync(dto.Email);

        if (userAccount != null)
        {
            return null;
        }

        var newUserAccount = new UserAccount
        {
            Id = Guid.NewGuid(),
            EmployeeId = employee.Id,
            Email = dto.Email,
            Role = DefaultRole
        };

        var hasher = new PasswordHasher<UserAccount>();
        var result = hasher.HashPassword(newUserAccount, dto.Password);

        newUserAccount.PasswordHash = result;

        await _userAccountRepository.AddAsync(newUserAccount);

        var authResponse = new AuthResponseDto
        {
            Token = _tokenService.GenerateToken(newUserAccount),
            Email = newUserAccount.Email,
            Role = newUserAccount.Role
        };

        return authResponse;
    }
}

