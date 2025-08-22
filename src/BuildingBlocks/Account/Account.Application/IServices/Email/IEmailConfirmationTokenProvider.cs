using PetFamily.SharedKernel.Results;
using System.Security.Claims;

namespace Account.Application.IServices.Email;

public interface IEmailConfirmationTokenProvider
{
    string CreateEmailConfirmationToken(Guid userId);
    Result<Guid> GetUserIdFromEmailConfirmationToken(string emailConfirmationToken);
    UnitResult ValidateEmailConfirmationToken(string emailConfirmationToken);
}
