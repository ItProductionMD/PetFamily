using PetFamily.Application.Abstractions.CQRS;

namespace PetFamily.IntegrationTests;

public abstract class QueryHandlerTest<TQuery>(
    TestWebApplicationFactory factory) : BaseTest(factory) where TQuery : IQuery
{
    protected IQueryHandler<TQuery> _sut = null!;
    public async override Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = GetQueryHandler<TQuery>();
    }
}

public abstract class QueryHandlerTest<TResponse, TQuery>(
    TestWebApplicationFactory factory) : BaseTest(factory) where TQuery : IQuery
{
    protected IQueryHandler<TResponse, TQuery> _sut = null!;
    public async override Task InitializeAsync()
    {
        await base.InitializeAsync();
        _sut = GetQueryHandler<TResponse, TQuery>();
    }
}

