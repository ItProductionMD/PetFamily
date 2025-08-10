using PetFamily.SharedApplication.Abstractions.CQRS;

namespace PetFamily.IntegrationTests;

public abstract class CommandHandlerTest<TCommand>(
    TestWebApplicationFactory factory) : BaseTest(factory) where TCommand : ICommand
{
    protected ICommandHandler<TCommand> _sut = null!;
    public async override Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = GetCommandHandler<TCommand>();
    }
}

public abstract class CommandHandlerTest<TResponse, TCommand>(
    TestWebApplicationFactory factory) : BaseTest(factory) where TCommand : ICommand
{
    protected ICommandHandler<TResponse, TCommand> _sut = null!;
    public async override Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = GetCommandHandler<TResponse, TCommand>();
    }
}