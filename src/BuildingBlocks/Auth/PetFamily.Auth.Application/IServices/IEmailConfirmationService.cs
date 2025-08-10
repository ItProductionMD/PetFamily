using PetFamily.Auth.Domain.Entities.UserAggregate;
using PetFamily.SharedKernel.Results;

namespace PetFamily.Auth.Application.IServices;

public interface IEmailConfirmationService
{
    Task SendEmailConfirmationMessage(User user, CancellationToken ct = default);
    Result<Guid> GetUserIdFromConfirmationToken(string emailConfirmationToken);
}
