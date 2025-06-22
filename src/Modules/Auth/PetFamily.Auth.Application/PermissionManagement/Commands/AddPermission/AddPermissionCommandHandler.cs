using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Domain.Entities;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.PermissionManagement.Commands.AddPermission;

public class AddPermissionCommandHandler(
    IPermissionWriteRepository repository,
    ILogger<AddPermissionCommandHandler> logger) : ICommandHandler<Guid, AddPermissionCommand>
{
    private readonly IPermissionWriteRepository _repository = repository;
    private readonly ILogger<AddPermissionCommandHandler> _logger = logger;

    public async Task<Result<Guid>> Handle(AddPermissionCommand cmd, CancellationToken ct)
    {
        //TODO Validate command

        var permission = Permission.Create(cmd.Code).Data!;

        await _repository.AddPermissionAsync(permission, ct);

        _logger.LogInformation("Add new permission successful! Id:{Id}", permission.Id.Value);

        return Result.Ok(permission.Id.Value);
    }
}
