using Microsoft.AspNetCore.Http;
using PetFamily.SharedApplication.IUserContext;
using PetFamily.SharedKernel.Errors;
using PetFamily.SharedKernel.Results;
using System.Security.Claims;

namespace PetFamily.SharedInfrastructure.HttpContext;

public class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _accessor;

    public HttpUserContext(IHttpContextAccessor accessor) => _accessor = accessor;

    public Result<Guid> GetUserId()
    {
        var parsedOk = Guid.TryParse(
            _accessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            out var id);

        if (parsedOk == false || id == Guid.Empty)
            return Result.Fail(Error.GuidIsEmpty("UserId in HttpContext"));

        return Result.Ok(id);
    }

    public bool HasPermission(string permission) =>
        _accessor.HttpContext!.User.HasClaim("permission", permission);

    public string Phone =>
        _accessor.HttpContext!.User.FindFirst(ClaimTypes.MobilePhone)?.Value
        ?? string.Empty;

    public string Email =>
        _accessor.HttpContext!.User.FindFirst(ClaimTypes.Email)?.Value
        ?? string.Empty;
}
