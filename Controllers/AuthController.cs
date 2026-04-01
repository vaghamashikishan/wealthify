using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using wealthify.DTOs.User;
using wealthify.Models;
using wealthify.Services.Interfaces;

namespace wealthify.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController(IAuthService service) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse>> RegisterUser([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
    {
        await service.RegisterAsync(dto, cancellationToken);
        return Ok(new ApiResponse { Message = "User has been successfully registered" });
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<string>>> LoginUser([FromBody] LoginUserDto dto, CancellationToken cancellationToken)
    {
        var jwtToken = await service.LoginAsync(dto, cancellationToken);

        if (jwtToken is null)
        {
            return NotFound(new ApiResponse<string> { Success = false, Errors = ["Username or Password is wrong"] });
        }
        return Ok(new ApiResponse<string> { Data = jwtToken, Message = "Logged in successfully" });
    }
}

