namespace Account.Application.IServices.Email;

public interface IEmailService
{
    Task SendAsync(EmailMessage message, CancellationToken ct = default);
    Task SendEmailConfirmationTokenAsync(Guid userId, string toEmail, CancellationToken ct = default);
}
