using PetFamily.SharedKernel.Results;

namespace PetFamily.Application.Abstractions.CQRS;

public interface IQueryHandler<TResponse, in TQuery> where TQuery : IQuery
{
    Task<Result<TResponse>> Handle(TQuery query, CancellationToken token);
}
public interface IQueryHandler<in TQuery> where TQuery : IQuery
{
    Task<UnitResult> Handle(TQuery query, CancellationToken token);
}
