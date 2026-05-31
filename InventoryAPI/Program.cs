using Microsoft.OpenApi;
using InventoryAPI.Data;
using Microsoft.EntityFrameworkCore;
using InventoryAPI.Services;
using Swashbuckle.AspNetCore.SwaggerUI;
using InventoryAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace InventoryAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //DBContext registrieren (EF Core):
        var dbContext = builder.Services.AddDbContext<InventoryDbContext>(options =>
         options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        //Langfristig sollten Verbindungsdetails in Secrets liegen, hier aber der Einfachheit halber in appsettings.json
        // -> Wenn kein EF Core verwendet wird (veralteter Ansatz)
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Jwt:Audience"],

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)
                    ),

                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
               };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy =>
                policy.RequireRole("Admin"));
            options.AddPolicy("User", policy =>
                policy.RequireRole("Admin", "User"));
        });

        builder.Services.AddScoped(sp => 
            new AuthService(
                sp.GetRequiredService<InventoryDbContext>(),
                builder.Configuration["Jwt:Secret"]!,
                builder.Configuration["Jwt:Issuer"]!,
                builder.Configuration["Jwt:Audience"]!
            )
        );

        //Services Scoped registrieren(wird für jede Instanz der Controller neu erstellt, da sie auf DbContext zugreifen, welcher auch Scoped ist):
        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<WarehouseService>();
        builder.Services.AddScoped<MovementService>();
        builder.Services.AddScoped<InventoryService>();
        builder.Services.AddScoped<UserService>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Please enter the Token from the login EP (without Bearer)",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(doc =>
            {
                var securityRequirement = new OpenApiSecurityRequirement();
                securityRequirement.Add(new OpenApiSecuritySchemeReference("Bearer", doc, null), new List<string>());
                return securityRequirement;
            });
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(options => 
            options.DocExpansion(DocExpansion.None)
        );

        //Exception Handling Middleware vor Controllern registrieren
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}