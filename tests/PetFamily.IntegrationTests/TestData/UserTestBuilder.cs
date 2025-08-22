using global::Account.Domain.Enums;
using global::Account.Domain.ValueObjects;
using global::PetFamily.SharedKernel.ValueObjects;
using global::PetFamily.SharedKernel.ValueObjects.Ids;
using Account.Domain.Entities.UserAggregate;

namespace PetFamily.IntegrationTests.TestData;

public class UserTestBuilder
{
    private UserId _id = UserId.NewGuid();
    private string _login = "test_user";
    private string _email = "test@example.com";
    private Phone _phone = Phone.CreateEmpty();
    private string _hashedPassword = "hashed_password";
    private List<SocialNetworkInfo> _socialNetworks = new();
    private RoleId _roleId;
    private ProviderType _providerType = ProviderType.Local;

    public UserTestBuilder WithId(Guid id)
    {
        _id = UserId.Create(id).Data!;
        return this;
    }

    public UserTestBuilder WithLogin(string login)
    {
        _login = login;
        return this;
    }

    public UserTestBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public UserTestBuilder WithPhone(Phone phone)
    {
        _phone = phone;
        return this;
    }

    public UserTestBuilder WithHashedPassword(string password)
    {
        _hashedPassword = password;
        return this;
    }

    public UserTestBuilder WithRole(RoleId roleId)
    {
        _roleId = roleId;
        return this;
    }

    public UserTestBuilder WithSocialNetworks(List<SocialNetworkInfo> socialNetworks)
    {
        _socialNetworks = socialNetworks;
        return this;
    }

    public UserTestBuilder WithProviderType(ProviderType type)
    {
        _providerType = type;
        return this;
    }

    public User Build()
    {
        var result = User.Create(
            _id,
            _login,
            _email,
            _phone,
            _hashedPassword,
            _socialNetworks,
            _providerType);

        return result.Data!;
    }
}

