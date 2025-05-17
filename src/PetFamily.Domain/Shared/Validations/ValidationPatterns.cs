namespace PetFamily.Domain.Shared.Validations;

public static class ValidationPatterns
{
    // Allows letters and spaces. Suitable for color names (e.g., "Light Blue").
    public const string COLOR_PATTERN = @"^[A-Za-z\s]+$";

    // Validates a standard email address format (e.g., "example@domain.com").
    public const string EMAIL_PATTERN = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

    // Allows letters and hyphens (e.g., "John-Doe").
    public const string NAME_PATTERN = @"^[A-Za-z\-]+$";

    // Allows international phone region codes (e.g., "+1", "+44", "380").
    // A leading "+" is optional, followed by 1–3 digits.
    public const string PHONE_REGION_PATTERN = @"^\+?[1-9]\d{0,2}$";

    // Allows phone numbers consisting of 7 to 15 digits without spaces or dashes (e.g., "1234567890").
    // Does not include region code; use PHONE_REGION_PATTERN for that.
    public const string PHONE_NUMBER_PATTERN = @"^\d{7,15}$";

    public const string STREET_PATTERN = @"^(?=.*[A-Za-z])[-A-Za-z0-9 ]+$";

    //Allows for numbers and letters that present home address(e.g. 55/2 ,18b ,11)
    public const string ADRESS_NUMBER_PATTERN = @"^\d+[a-zA-Z]?(/[a-zA-Z0-9]+)?$";
}
