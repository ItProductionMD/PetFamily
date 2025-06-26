using PetFamily.Auth.Domain.Entities.RoleAggregate;
using PetFamily.Auth.Domain.Enums;
using PetFamily.Auth.Domain.Options;
using PetFamily.Auth.Domain.ValueObjects;
using PetFamily.SharedKernel.Abstractions;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Uniqness;
using PetFamily.SharedKernel.ValueObjects;
using PetFamily.SharedKernel.ValueObjects.Ids;
using static PetFamily.Auth.Domain.Validations.Validations;



namespace PetFamily.Auth.Domain.Entities.UserAggregate;

public class User : Entity<UserId>, ISoftDeletable, IHasUniqueFields
{
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
    public DateTime? DeletedAt { get; set; }
    public int LoginAttempts { get; set; } = 0;
    public IReadOnlyList<SocialNetworkInfo> SocialNetworks { get; private set; }

    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles.AsReadOnly();

    private const int MAX_LOGIN_ATTEMPTS = 5;

    private User() : base(UserId.NewGuid()) { }//EfCore need this

    private User(
        UserId id,
        string email,
        string login,
        Phone phone,
        string hashedPassword,
        ProviderType providerType,
        List<UserRole> userRoles,
        List<SocialNetworkInfo> socialNetworks
        ) : base(id)
    {
        Email = email;
        Phone = phone;
        Login = login;
        HashedPassword = hashedPassword;
        ProviderType = providerType;
        _userRoles = userRoles;
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
        IEnumerable<RoleId> rolesIds,
        ProviderType providerType)
    {
        var userRoles = rolesIds
            .Select(roleId => new UserRole(id, roleId))
            .ToList();

        var validationResult = ValidateUser(email, login, hashedPassword, LoginOptions.Default);

        var user = new User(
            id,
            email,
            login,
            phone,
            hashedPassword,
            providerType,
            userRoles,
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

    public void SoftDelete()
    {
        UserStatus = UserStatus.Disabled;
    }

    public void HardDelete()
    {
        UserStatus = UserStatus.Deleted;
    }

    public void Update(string email, List<Role> roles, Phone? phone = null)
    {
        Email = email;
        //update roles
        Phone = phone;
    }

    public void EnableTwoFactor() => IsTwoFactorEnabled = true;

    public void SetPhoneNumber(Phone phone) => Phone = phone;

    public void Restore()
    {
        throw new NotImplementedException();
    }

    public void AddRole(RoleId roleId)
    {
        if (!_userRoles.Any(r => r.RoleId == roleId))
            _userRoles.Add(new UserRole(Id, roleId));
    }

    public void RemoveRoleId(RoleId roleId)
    {
        _userRoles.RemoveAll(r => r.RoleId == roleId);
    }

    public void ConfirmEmail()
    {
        IsEmailConfirmed = true;
    }
    public static string[] GetUniqueFields()
    {
        throw new NotImplementedException();
    }

    public void UpdateRoles(IEnumerable<RoleId> roleIds)
    {

        _userRoles.Clear();
        var userRoles = roleIds.Select(rId => new UserRole(Id, rId)).ToList();
        _userRoles.AddRange(userRoles);
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


}
public record Profile(
    string Login,
    Phone Phone,
    List<SocialNetworkInfo> SocialNetworks
);
