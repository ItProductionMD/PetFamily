using PetFamily.Domain.Results;

namespace PetFamily.Application.Abstractions;

public interface IQueryHandler<TResponse, in TQuery> where TQuery : IQuery
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken token);
}
public interface IQueryHandler<in TQuery> where TQuery : IQuery
{
    Task<UnitResult> Handle(TQuery query, CancellationToken token);
}
