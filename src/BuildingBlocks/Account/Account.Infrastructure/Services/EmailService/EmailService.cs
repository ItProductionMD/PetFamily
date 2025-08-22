using Microsoft.Extensions.Logging;
using Account.Application.IServices.Email;
using Account.Domain.Entities.UserAggregate;

namespace Account.Infrastructure.Services.EmailService;

public class EmailService(
    IEmailConfirmationTokenProvider emailTokenProvider,
    ILogger<EmailService> logger) : IEmailService
{

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        logger.LogInformation("EMAIL SERVICE  send message to: {To} Subject: {Subject} Body: {Body}",
            message.To, message.Subject, message.Body);

        await Task.CompletedTask; // Simulate sending email
    }

    public async Task SendEmailConfirmationTokenAsync(Guid userId, string toEmail, CancellationToken ct = default)
    {
        var emailConfirmationToken = emailTokenProvider.CreateEmailConfirmationToken(userId);

        var emailConfirmationMessage = EmailMessage.CreateConfirmationEmailMessage(
            toEmail,
            emailConfirmationToken);

        await SendAsync(emailConfirmationMessage, ct);// Simulate sending email
    }
}
