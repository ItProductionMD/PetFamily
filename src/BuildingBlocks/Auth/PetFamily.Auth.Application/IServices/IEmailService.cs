using PetFamily.Auth.Application.Email;

namespace PetFamily.Auth.Application.IServices;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
}
