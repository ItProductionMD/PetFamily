using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Account.Infrastructure.Contexts;
using PetSpecies.Infrastructure.Contexts;
using Volunteers.Infrastructure.Contexts;
using static PetFamily.Tools.ToolsExtensions;

namespace PetFamily.Tools.Seeders;

public class ContextsCreator
{
    public static (IServiceScope scope, SpeciesWriteDbContext context) CreateSpeciesDbContext()
    {
        Console.WriteLine("###GetConnectionString...###");

        var connectionString = GetConnectionString();
        Console.WriteLine("###Creating host...###");

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddScoped(_ =>
                new SpeciesWriteDbContext(connectionString));
            })
       .Build();

        var scope = host.Services.CreateScope();

        Console.WriteLine("###Getting Species dbContext...###");

        var _dbContext = scope.ServiceProvider.GetRequiredService<SpeciesWriteDbContext>();
        if (_dbContext == null)
            throw new Exception("###species _dbContext is null!###");

        return (scope, _dbContext);
    }

    public static (IServiceScope scope, VolunteerWriteDbContext context) CreateVolunteerDbContext()
    {
        var connectionString = GetConnectionString();
        Console.WriteLine("###Creating host...###");

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddScoped(_ =>
                new VolunteerWriteDbContext(connectionString));
            })
       .Build();

        var scope = host.Services.CreateScope();

        Console.WriteLine("###Getting Species dbContext...###");

        var _dbContext = scope.ServiceProvider.GetRequiredService<VolunteerWriteDbContext>();
        if (_dbContext == null)
            throw new Exception("###species _dbContext is null!###");

        return (scope, _dbContext);
    }

    public static (IServiceScope scope, UserWriteDbContext context) CreateAuthDbContext()
    {
        var connectionString = GetConnectionString();
        Console.WriteLine("###Creating host...###");
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddScoped(_ =>
                new UserWriteDbContext(connectionString));
            })
       .Build();
        var scope = host.Services.CreateScope();
        Console.WriteLine("###Getting Auth dbContext...###");
        var _dbContext = scope.ServiceProvider.GetRequiredService<UserWriteDbContext>();
        if (_dbContext == null)
            throw new Exception("###auth _dbContext is null!###");

        return (scope, _dbContext);
    }
}
