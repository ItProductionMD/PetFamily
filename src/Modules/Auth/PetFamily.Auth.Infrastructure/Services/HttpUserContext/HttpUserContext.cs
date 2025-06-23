using Microsoft.AspNetCore.Http;
using PetFamily.Auth.Application.IServices;
using PetFamily.Auth.Public.IContracts;
using System.Security.Claims;
using PetFamily.Auth.Domain.Constants;
using PetFamily.SharedKernel.Results;
using PetFamily.SharedKernel.Errors;

namespace PetFamily.Auth.Infrastructure.Services.HttpUserContext;

public class HttpUserContext : IUserContext
{
    private readonly IHttpContextAccessor _accessor;

    public HttpUserContext(IHttpContextAccessor accessor) => _accessor = accessor;

    public Result<Guid> GetUserId()
    {
        var parsedOk = Guid.TryParse(
            _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), 
            out var id);

        if (parsedOk == false || id == Guid.Empty)
            return Result.Fail(Error.GuidIsEmpty("UserId in HttpContext"));

        return Result.Ok(id);
    }

    public bool HasPermission(string permission) =>
        _accessor.HttpContext!.User.HasClaim(PermissionPolicy.NAME, permission);

    public string Phone =>  
        _accessor.HttpContext!.User.FindFirstValue(ClaimTypes.MobilePhone)
        ??string.Empty;

    public string Email => 
        _accessor.HttpContext!.User.FindFirstValue(ClaimTypes.Email)
        ??string.Empty;
}

