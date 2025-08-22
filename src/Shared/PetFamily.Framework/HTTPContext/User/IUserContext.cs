namespace PetFamily.Framework.HTTPContext.User;

public interface IUserContext
{
    bool HasPermission(string permission);
    string Email { get; }
    string Phone { get; }

    Guid GetUserId();
}
