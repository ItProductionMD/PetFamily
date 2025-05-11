using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using PetFamily.Application.Abstractions;
using PetFamily.Application.IRepositories;
using PetFamily.Domain.PetManagment.Root;
using PetFamily.Domain.PetManagment.ValueObjects;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Infrastructure.Contexts;
using System.Runtime.CompilerServices;

namespace PetFamily.IntegrationTests;

public abstract class BaseTest(TestWebApplicationFactory factory)
    : IClassFixture<TestWebApplicationFactory>, IAsyncLifetime
{
    protected readonly TestWebApplicationFactory _factory = factory;
    protected IServiceScope? _scope { get; set; }
    protected IServiceProvider _services => _scope!.ServiceProvider;
    protected WriteDbContext _dbContext = null!;
    
    public virtual Task InitializeAsync()
    {
        _scope = _factory.Services.CreateScope();
        _dbContext = _scope.ServiceProvider.GetService<WriteDbContext>()!;
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
