using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;

namespace InventoryAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //Langfristig sollten Verbindungsdetails in Secrets liegen, hier aber der Einfachheit halber in appsettings.json
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddControllers();

        //Repos Scoped registrieren:
        builder.Services.AddScoped<ProductRepository>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Inventory API",
                Version = "v1",
                Description = "Backend Portfolio API"
            });
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();

        app.Run();
    }
}