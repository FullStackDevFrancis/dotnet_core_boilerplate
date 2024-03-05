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
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseHttpsRedirection();
        
        app.UseSwagger();

        // Enable middleware to serve Swagger-ui (HTML, JS, CSS, etc.),
        // specifying the Swagger JSON endpoint
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AUTH"));

        
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}