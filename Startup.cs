using System.Text;
using dotnet_core_boilerplate.StartUpExtensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace dotnet_core_boilerplate;

public class Startup
{
    private readonly IConfiguration _configuration;


    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHealthChecks();
        Console.WriteLine("configuring services");
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My boilerplate c# project", Version = "v1" });
        });
        services.ConfigureDependencyInjection(_configuration);
        services.AddIdentity<AppUser,IdentityRole>().AddEntityFrameworkStores<DbContext>()
            .AddDefaultTokenProviders();
        services.AddAuthentication(opt=>{
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt=>{
            opt.SaveToken = true;
            opt.RequireHttpsMetadata = false;
            opt.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime =true,
                ValidateIssuerSigningKey = true,
                ValidAudience  = _configuration.GetSection("JWTSetting")["ValidAudience"],
                ValidIssuer= _configuration.GetSection("JWTSetting")["ValidIssuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWTSetting")["securityKey"]!))
            };
        });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();

        app.UseSwagger();

        // Enable middleware to serve Swagger-ui (HTML, JS, CSS, etc.),
        // specifying the Swagger JSON endpoint
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AUTH"));


        app.UseRouting();
        app.UseAuthentication();

        app.UseAuthorization();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}