using Data;
using Endpoints;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
        //     options.UseSqlite(builder.Configuration.GetConnectionString("AppDbContext") ?? throw new InvalidOperationException("Connection string 'AppDbContext' not found.")));

        // builder.Services.AddScoped(implementationFactory: sp => sp
        //     .GetRequiredService<IDbContextFactory<AppDbContext>>()
        //     .CreateDbContext());

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("AppDbContext") ?? 
            throw new InvalidOperationException("Connection string 'AppDbContext' not found."))
        );

        // Add services to the container.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapPersonEndpoints();

        app.Run();
    }
}
