using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using InventoryAPI.Data;
using Microsoft.EntityFrameworkCore;
using InventoryAPI.Services;

namespace InventoryAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //DBContext registrieren (EF Core):
        builder.Services.AddDbContext<InventoryDbContext>(options =>
         options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        //Langfristig sollten Verbindungsdetails in Secrets liegen, hier aber der Einfachheit halber in appsettings.json
        // -> Wenn kein EF Core verwendet wird (veralteter Ansatz)
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddControllers();

        //Services Scoped registrieren(wird für jede Instanz der Controller neu erstellt, da sie auf DbContext zugreifen, welcher auch Scoped ist):
        builder.Services.AddScoped<ProductService>();

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