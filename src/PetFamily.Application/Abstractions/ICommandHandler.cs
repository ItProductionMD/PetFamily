﻿using PetFamily.Domain.Results;

namespace PetFamily.Application.Abstractions;

interface ICommandHandler<TResponse,in TCommand> where TCommand : ICommand
{
    public Task<Result<TResponse>> Handle(TCommand command,CancellationToken cancellationToken);
}
interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    public Task<UnitResult> Handle(TCommand command, CancellationToken cancellationToken);
}