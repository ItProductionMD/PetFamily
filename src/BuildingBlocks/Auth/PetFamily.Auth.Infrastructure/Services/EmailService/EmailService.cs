using Microsoft.Extensions.Logging;
using PetFamily.Auth.Application.Email;
using PetFamily.Auth.Application.IServices;

namespace PetFamily.Auth.Infrastructure.Services.EmailService;

public class EmailService(
    IJwtProvider jwtProvider,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<EmailService> _logger = logger;

    public async Task SendAsync(EmailMessage message, CancellationToken ct = default)
    {
        _logger.LogInformation("EMAIL SERVICE  send message to: {To} Subject: {Subject} Body: {Body}",
            message.To, message.Subject, message.Body);

        await Task.CompletedTask; // Simulate sending email
    }

}
