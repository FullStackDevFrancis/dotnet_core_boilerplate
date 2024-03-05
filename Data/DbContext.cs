using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class DbContext: IdentityDbContext<AppUser>
{
    public DbContext(DbContextOptions<DbContext> options):base(options)
    {
            
    }

    protected DbContext()
    {
    }
}