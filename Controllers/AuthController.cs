using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using dotnet_core_boilerplate.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace dotnet_core_boilerplate.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<AppUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(RegisterDto registerDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new AppUser
        {
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            UserName = registerDto.Email
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        if (registerDto.Roles is null)
        {
            await _userManager.AddToRoleAsync(user, "User");
        }
        else
        {
            foreach (var role in registerDto.Roles)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
        }


        return Ok(new AuthResponseDto
        {
            IsSuccess = true,
            Message = "Account Created Sucessfully!"
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _userManager.FindByEmailAsync(loginDto.Email);

        if (user is null)
        {
            return Unauthorized(new AuthResponseDto
            {
                IsSuccess = false,
                Message = "User not found with this email",
            });
        }

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!result)
        {
            return Unauthorized(new AuthResponseDto
            {
                IsSuccess = false,
                Message = "Invalid Password."
            });
        }


        var token = GenerateToken(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            IsSuccess = true,
            Message = "Login Success."
        });
    }


    private string GenerateToken(AppUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII
            .GetBytes(_configuration.GetSection("JWTSetting").GetSection("securityKey").Value!);

        var roles = _userManager.GetRolesAsync(user).Result;

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName ?? ""),
            new Claim(JwtRegisteredClaimNames.NameId, user.Id ?? ""),
            new Claim(JwtRegisteredClaimNames.Aud, _configuration["JWTSetting:validAudience"]),
            new Claim(JwtRegisteredClaimNames.Iss, _configuration["JWTSetting:validIssuer"])
        };


        foreach (var role in roles)

        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256
            )
        };


        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}