using PetFamily.Auth.Domain.Enums;
using PetFamily.Auth.Domain.Options;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Uniqness;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using static PetFamily.Auth.Domain.Validations.Validations;



namespace PetFamily.Auth.Domain.Entities.UserAggregate;

public class User : SoftDeletable, IEntity<UserId>, IHasUniqueFields
{
    public UserId Id { get; set; }
    [Unique]
    public string Login { get; set; }
    public string HashedPassword { get; set; }
    [Unique]
    public Phone Phone { get; set; }
    public ProviderType ProviderType { get; set; }
    public UserStatus UserStatus { get; set; }
    public bool IsEmailConfirmed { get; set; }
    [Unique]
    public string Email { get; set; }
    public bool IsTwoFactorEnabled { get; set; }
    public bool IsBlocked { get; set; } = false;
    public DateTime? BlockedAt { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int LoginAttempts { get; set; } = 0;
    public IReadOnlyList<SocialNetworkInfo> SocialNetworks { get; private set; }
    public RoleId RoleId { get; set; }
    private const int MAX_LOGIN_ATTEMPTS = 5;
    private User() { }//EfCore need this

    private User(
        UserId id,
        string email,
        string login,
        Phone phone,
        string hashedPassword,
        ProviderType providerType,
        RoleId roleId,
        List<SocialNetworkInfo> socialNetworks
        )
    {
        Id = id;
        Email = email;
        Phone = phone;
        Login = login;
        HashedPassword = hashedPassword;
        ProviderType = providerType;
        RoleId = roleId;
        CreatedAt = DateTime.UtcNow;
        SocialNetworks = socialNetworks;
    }

    public static Result<User> Create(
        UserId id,
        string login,
        string email,
        Phone phone,
        string hashedPassword,
        List<SocialNetworkInfo> socialNetworks,
        RoleId roleId,
        ProviderType providerType)
    {
        var validationResult = ValidateUser(email, login, hashedPassword, LoginOptions.Default);

        var user = new User(
            id,
            email,
            login,
            phone,
            hashedPassword,
            providerType,
            roleId,
            socialNetworks);

        return Result.Ok(user);
    }

    private static UnitResult ValidateUser(
        string email,
        string login,
        string password,
        LoginOptions options) =>

        UnitResult.FromValidationResults(
            () => ValidateLogin(login, options),
            () => ValidateEmail(email),
            () => ValidatePassword(password));

    override public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
    }

    override public void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
    }

    public void HardDelete()
    {
        UserStatus = UserStatus.Deleted;
    }

    public void Update(string email, Phone? phone = null)
    {
        //TODO check if Role is Volunteer - phone must be not null and should update volunteer data
        Email = email;
        Phone = phone;
    }

    public void EnableTwoFactor() => IsTwoFactorEnabled = true;

    public void SetPhoneNumber(Phone phone) => Phone = phone;

    public void ConfirmEmail()
    {
        IsEmailConfirmed = true;
    }
    public static string[] GetUniqueFields()
    {
        throw new NotImplementedException();
    }

    public void ChangeRole(RoleId roleId)
    {
        RoleId = roleId;
    }

    public void ErrorAttemptLogin()
    {
        LoginAttempts++;
        if (LoginAttempts >= MAX_LOGIN_ATTEMPTS)
        {
            IsBlocked = true;
            BlockedAt = DateTime.UtcNow;
        }
    }

    public int GetRemainingAttempts()
    {
        if (IsBlocked)
            return 0;
        return MAX_LOGIN_ATTEMPTS - LoginAttempts;
    }

    public void UpdateProfile(Profile profile)
    {
        Login = profile.Login;
        Phone = profile.Phone;
        SocialNetworks = SocialNetworks;
    }

    public void SetSuccessfulLogin()
    {
        LoginAttempts = 0;
        LastLoginDate = DateTime.UtcNow;
    }

}


public record Profile(
    string Login,
    Phone Phone,
    List<SocialNetworkInfo> SocialNetworks
);
