using PetFamily.SharedKernel.Results;

namespace PetFamily.SharedApplication.Abstractions.CQRS;


public interface ICommandHandler<TResponse, in TCommand> where TCommand : ICommand
{
    public Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
}
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    public Task<UnitResult> Handle(TCommand command, CancellationToken cancellationToken);
}