using Microsoft.Extensions.DependencyInjection;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.Auth.Infrastructure.Contexts;
using PetFamily.VolunteerRequests.Infrastructure.Contexts;
using PetSpecies.Infrastructure.Contexts;
using Volunteers.Infrastructure.Contexts;
using PetFamily.IntegrationTests.WebApplicationFactory;
using Microsoft.EntityFrameworkCore;

namespace PetFamily.IntegrationTests.Fixtures;

public abstract class BaseTest(TestWebApplicationFactory factory)
    : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    protected readonly TestWebApplicationFactory _factory = factory;
    protected IServiceScope? _scope { get; set; }
    protected IServiceProvider _services => _scope!.ServiceProvider;
    protected VolunteerWriteDbContext _volunteerDbContext = null!;
    protected VolunteerRequestWriteDbContext _volunteerRequestDbContext = null!;
    protected SpeciesWriteDbContext _speciesDbContext = null!;
    protected AuthWriteDbContext _authDbContext = null!;

    public virtual Task InitializeAsync()
    {
        _scope = _factory.Services.CreateScope();

        _volunteerDbContext = _scope.ServiceProvider.GetService<VolunteerWriteDbContext>()!;
        _speciesDbContext = _scope.ServiceProvider.GetService<SpeciesWriteDbContext>()!;
        _authDbContext = _scope.ServiceProvider.GetRequiredService<AuthWriteDbContext>()!;
        _volunteerRequestDbContext = _scope.ServiceProvider.GetRequiredService<VolunteerRequestWriteDbContext>()!;

        return Task.CompletedTask;
    }
    public virtual async Task DisposeAsync()
    {
        await _factory.ResetCheckpoint();
        _scope?.Dispose();
    }

    protected ICommandHandler<TResponse, T> GetCommandHandler<TResponse, T>() where T : ICommand =>
        _services.GetRequiredService<ICommandHandler<TResponse, T>>();

    protected ICommandHandler<T> GetCommandHandler<T>() where T : ICommand =>
        _services.GetRequiredService<ICommandHandler<T>>();

    protected IQueryHandler<TResponse, T> GetQueryHandler<TResponse, T>() where T : IQuery =>
        _services.GetRequiredService<IQueryHandler<TResponse, T>>();

    protected IQueryHandler<T> GetQueryHandler<T>() where T : IQuery =>
        _services.GetRequiredService<IQueryHandler<T>>();


    public async Task SeedAsync<T>(Type dbContextType, T entity) where T : class
    {
        using var scope = _factory.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService(dbContextType) as DbContext;
        if (dbContext == null)
            throw new InvalidOperationException($"DbContext of type {dbContextType.Name} not registered");

        dbContext.Set<T>().Add(entity);

        await dbContext.SaveChangesAsync();
    }
}
