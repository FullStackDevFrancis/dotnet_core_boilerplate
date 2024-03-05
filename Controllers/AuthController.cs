using dotnet_core_boilerplate.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_core_boilerplate.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;

    public AuthController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(RegisterDto registerDto)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new AppUser{
            Email = registerDto.Email,
            FullName = registerDto.FullName,
            UserName = registerDto.Email
        };

        var result = await _userManager.CreateAsync(user,registerDto.Password);

        if(!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
            
        if(registerDto.Roles is null){
            await _userManager.AddToRoleAsync(user,"User");
        }else{
            foreach(var role in registerDto.Roles)
            {
                await _userManager.AddToRoleAsync(user,role);
            }
        }
    

        return Ok(new AuthResponseDto{
            IsSuccess = true,
            Message = "Account Created Sucessfully!"
        });

    }
}