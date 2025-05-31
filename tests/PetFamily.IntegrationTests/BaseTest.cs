using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.Abstractions.CQRS;
using PetSpecies.Infrastructure.Contexts;
using Volunteers.Infrastructure.Contexts;

namespace PetFamily.IntegrationTests;

public abstract class BaseTest(TestWebApplicationFactory factory)
    : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    protected readonly TestWebApplicationFactory _factory = factory;
    protected IServiceScope? _scope { get; set; }
    protected IServiceProvider _services => _scope!.ServiceProvider;
    protected VolunteerWriteDbContext _volunteerDbContext = null!;
    protected SpeciesWriteDbContext _speciesDbContext = null!;

    public virtual Task InitializeAsync()
    {
        _scope = _factory.Services.CreateScope();

        _volunteerDbContext = _scope.ServiceProvider.GetService<VolunteerWriteDbContext>()!;
        _speciesDbContext = _scope.ServiceProvider.GetService<SpeciesWriteDbContext>()!;

        return Task.CompletedTask;
    }
    public virtual async Task DisposeAsync()
    {
        await _factory.ResetCheckpoint();
        _scope?.Dispose();
    }

    protected T GetService<T>() where T : notnull => _services.GetRequiredService<T>();

    protected ICommandHandler<TResponse, T> GetCommandHandler<TResponse, T>() where T : ICommand =>
        _services.GetRequiredService<ICommandHandler<TResponse, T>>();

    protected ICommandHandler<T> GetCommandHandler<T>() where T : ICommand =>
        _services.GetRequiredService<ICommandHandler<T>>();

    protected IQueryHandler<TResponse, T> GetQueryHandler<TResponse, T>() where T : IQuery =>
        _services.GetRequiredService<IQueryHandler<TResponse, T>>();

    protected IQueryHandler<T> GetQueryHandler<T>() where T : IQuery =>
        _services.GetRequiredService<IQueryHandler<T>>();

}
