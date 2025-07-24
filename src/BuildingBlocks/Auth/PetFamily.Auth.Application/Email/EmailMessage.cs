namespace PetFamily.Auth.Application.Email;

public class EmailMessage(
    string To,
    string Subject,
    string Body,
    bool IsHtml = true
)
{
    public string To { get; } = To;
    public string Subject { get; } = Subject;
    public string Body { get; } = Body;
    public bool IsHtml { get; } = IsHtml;


    public static EmailMessage CreateConfirmationEmailMessage(
        string to,
        string confirmationToken
    )
    {
        return new EmailMessage(
            to,
            "Email Confirmation",
            $"Please confirm your email by clicking this link: " +
            $"http://127.0.0.1:5287/api/users/confirm_email/{confirmationToken}",
            true
        );
    }
};

