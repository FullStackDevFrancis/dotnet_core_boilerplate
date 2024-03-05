using Microsoft.AspNetCore.Identity;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }
}