﻿using Microsoft.Extensions.Logging;
using PetFamily.Application.Abstractions.CQRS;
using PetFamily.Auth.Application.Dtos;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.SharedKernel.Results;
using static PetFamily.SharedKernel.Validations.ValidationExtensions;

namespace PetFamily.Auth.Application.UserManagement.Queries.GetUserAccountInfo;

public class GetUserAccountInfoHandler(
    IUserReadRepository userReadRepository,
    ILogger<GetUserAccountInfoHandler> logger )
    : ICommandHandler<UserAccountInfoDto, GetUserAccountInfoCommand>
{
    private readonly ILogger<GetUserAccountInfoHandler> _logger = logger;
    private readonly IUserReadRepository _userReadRepository = userReadRepository;
    public async Task<Result<UserAccountInfoDto>> Handle(GetUserAccountInfoCommand cmd, CancellationToken ct)
    {
        var validateCommandResult = ValidateIfGuidIsNotEpmty(cmd.userId, "User Id");
        if (validateCommandResult.IsFailure)
        {
            _logger.LogWarning("User Id is empty!");
            return Result.Fail(validateCommandResult.Error);
        }
        return await _userReadRepository.GetUserAccountInfo(cmd.userId, ct);       
    }
}
