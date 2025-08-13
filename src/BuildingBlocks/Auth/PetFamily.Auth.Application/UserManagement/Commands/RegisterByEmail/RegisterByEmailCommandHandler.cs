using Microsoft.Extensions.Logging;
using PetFamily.Auth.Application.IRepositories;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Domain.Enums;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Authorization;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using User = PetFamily.Auth.Domain.Entities.UserAggregate.User;

namespace PetFamily.Auth.Application.UserManagement.Commands.RegisterByEmail;

public class RegisterByEmailCommandHandler(
    IUserWriteRepository userWriteRepository,
    IUserReadRepository userReadRepository,
    IRoleReadRepository roleReadRepository,
    IPasswordHasher passwordHasher,
    ILogger<RegisterByEmailCommandHandler> logger,
    IEmailConfirmationService emailConfirmationService,
    IJwtProvider jwtProvider) : ICommandHandler<RegisterByEmailCommand>
{
    private readonly IUserWriteRepository _userWriteRepository = userWriteRepository;
    private readonly IUserReadRepository _userReadRepository = userReadRepository;
    private readonly IRoleReadRepository _roleReadRepository = roleReadRepository;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<RegisterByEmailCommandHandler> _logger = logger;
    private readonly IEmailConfirmationService _emailConfirmationService = emailConfirmationService;
    private const string ROLE_CODE_TO_ASSIGN = RoleCodes.UNCONFIRMED_USER;

    public async Task<UnitResult> Handle(RegisterByEmailCommand cmd, CancellationToken ct)
    {
        RegisterByEmailCommandValidator.Validate(cmd);

        var phone = Phone.CreateNotEmpty(cmd.phoneNumber, cmd.phoneRegionCode).Data!;

        var checkUniqueFields = await _userReadRepository.CheckUniqueFields(
            cmd.Email,
            cmd.Login,
            phone,
            ct);
        if (checkUniqueFields.IsFailure)
            return checkUniqueFields;

        var hashedPassword = _passwordHasher.Hash(cmd.Password);

        var getRoleDto = await _roleReadRepository.GetByCodeAsync(ROLE_CODE_TO_ASSIGN, ct);
        if (getRoleDto.IsFailure)
            return Result.Fail(getRoleDto.Error);

        var roleDto = getRoleDto.Data!;
        var roleId = RoleId.Create(roleDto.RoleId).Data!;

        var socialNetworkList = cmd.SocialNetworksList
            .Select(dto => SocialNetworkInfo.Create(dto.Name, dto.Url).Data!).ToList();

        var user = User.Create(
            UserId.NewGuid(),
            cmd.Login,
            cmd.Email,
            phone,
            hashedPassword,
            socialNetworkList,
            roleId,
            ProviderType.Local
        ).Data!;

        await _userWriteRepository.AddAsync(user, ct);

        await _userWriteRepository.SaveChangesAsync(ct);

        await _emailConfirmationService.SendEmailConfirmationMessage(user, ct);

        _logger.LogInformation("User registered successfully. UserId: {UserId}, Email: {Email}",
            user.Id, user.Email);

        return UnitResult.Ok();
    }
}

