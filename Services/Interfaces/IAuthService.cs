using System;
using wealthify.DTOs.User;

namespace wealthify.Services.Interfaces;

public interface IAuthService
{
    Task RegisterAsync(CreateUserDto dto, CancellationToken cancellationToken = default);
    Task<string?> LoginAsync(LoginUserDto dto, CancellationToken cancellationToken = default);
}
