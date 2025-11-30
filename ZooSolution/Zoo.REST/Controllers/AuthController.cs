using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Zoo.Infrastructure;

namespace Zoo.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Неправильна електронна адреса або пароль" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Неправильна електронна адреса або пароль" });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.UserName ?? "")
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Ключ JWT не налаштований");
        var jwtIssuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Видавець JWT не налаштований");
        var jwtAudience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Аудиторія JWT не налаштована");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            accessToken = tokenString,
            tokenType = "Bearer",
            expiresIn = 3600,
            email = user.Email,
            roles = roles
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return BadRequest(new { message = "Паролі не збігаються" });
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Реєстрація не вдалася", errors = result.Errors });
        }

        await _userManager.AddToRoleAsync(user, "User");

        return Ok(new 
        { 
            message = "Користувач зареєстрований успішно", 
            id = user.Id,
            email = user.Email,
            role = "User"
        });
    }

    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return NotFound(new { message = "Користувач не знайдений" });
        }

        var roleExists = await _userManager.IsInRoleAsync(user, request.Role);
        if (roleExists)
        {
            return BadRequest(new { message = $"Користувач вже має роль '{request.Role}'" });
        }

        var result = await _userManager.AddToRoleAsync(user, request.Role);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Не вдалося призначити роль", errors = result.Errors });
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        return Ok(new 
        { 
            message = "Роль успішно призначено",
            userId = user.Id,
            email = user.Email,
            roles = userRoles
        });
    }

    [HttpPost("remove-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveRole([FromBody] AssignRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return NotFound(new { message = "Користувач не знайдений" });
        }

        var result = await _userManager.RemoveFromRoleAsync(user, request.Role);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Не вдалося видалити роль", errors = result.Errors });
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        return Ok(new 
        { 
            message = "Роль успішно видалено",
            userId = user.Id,
            email = user.Email,
            roles = userRoles
        });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class AssignRoleRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

