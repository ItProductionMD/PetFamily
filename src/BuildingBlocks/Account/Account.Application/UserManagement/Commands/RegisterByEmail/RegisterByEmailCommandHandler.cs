using Microsoft.Extensions.Logging;
using Account.Application.IRepositories;
using Account.Application.IServices;
using Account.Application.IServices.Email;
using Account.Domain.Entities.UserAggregate;
using Account.Domain.Enums;
using PetFamily.SharedApplication.Abstractions.CQRS;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;

namespace Account.Application.UserManagement.Commands.RegisterByEmail;

public class RegisterByEmailCommandHandler(
    IUserWriteRepository userWriteRepo,
    IUserReadRepository userReadRepo,
    IPasswordHasher passwordHasher,
    IEmailService emailService,
    ILogger<RegisterByEmailCommandHandler> _logger) : ICommandHandler<RegisterByEmailCommand>
{
    public async Task<UnitResult> Handle(RegisterByEmailCommand cmd, CancellationToken ct)
    {
        cmd.Validate();

        var phone = Phone.CreateNotEmpty(cmd.phoneNumber, cmd.phoneRegionCode).Data!;

        var checkUniqueFields = await userReadRepo.CheckUniqueFields(
            cmd.Email,
            cmd.Login,
            phone,
            ct);
        if (checkUniqueFields.IsFailure)
            return checkUniqueFields;

        var hashedPassword = passwordHasher.Hash(cmd.Password);

        var socialNetworkList = cmd.SocialNetworksList
            .Select(dto => SocialNetworkInfo.Create(dto.Name, dto.Url).Data!).ToList();

        var user = User.Create(
            UserId.NewGuid(),
            cmd.Login,
            cmd.Email,
            phone,
            hashedPassword,
            socialNetworkList,
            ProviderType.Local
        ).Data!;        

        await userWriteRepo.AddAndSaveAsync(user, ct);
        
        await emailService.SendEmailConfirmationTokenAsync(user.Id.Value,user.Email, ct);

        _logger.LogInformation("User registered successfully. UserId: {UserId}, Email: {Email}",
            user.Id, user.Email);

        return UnitResult.Ok();
    }
}

