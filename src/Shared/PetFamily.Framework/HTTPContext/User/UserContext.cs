using Microsoft.AspNetCore.Http;
using PetFamily.SharedApplication.Exceptions;
using System.Security.Claims;

namespace PetFamily.Framework.HTTPContext.User;

public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _accessor;

    public UserContext(IHttpContextAccessor accessor) => _accessor = accessor;

    public bool HasPermission(string permission) =>
        _accessor.HttpContext!.User.HasClaim("permission", permission);

    public Guid GetUserId()
    {
        var parsedOk = Guid.TryParse(
            _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out var id);

        if (parsedOk == false || id == Guid.Empty)
            throw new UserNotAuthenticatedException();

        return id;
    }

    public string Phone =>
        _accessor.HttpContext!.User.FindFirst(ClaimTypes.MobilePhone)?.Value
        ?? string.Empty;

    public string Email =>
        _accessor.HttpContext!.User.FindFirst(ClaimTypes.Email)?.Value
        ?? string.Empty;
}
