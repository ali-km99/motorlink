using CarDealer.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using CarDealer.API.Common;
namespace CarDealer.API.Authorization;

public class HasPermissionAttribute : TypeFilterAttribute
{
    public HasPermissionAttribute(string permissionCode) : base(typeof(HasPermissionFilter))
    {
        Arguments = new object[] { permissionCode };
    }
}

public class HasPermissionFilter : IAsyncAuthorizationFilter
{
    private readonly string _permissionCode;

    public HasPermissionFilter(string permissionCode) => _permissionCode = permissionCode;

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated == true)
        {
            context.Result = new UnauthorizedResult();
            return Task.CompletedTask;
        }

        var isOwner = user.IsInRole(Roles.Owner);
        var hasPermission = user.HasClaim("permission", _permissionCode);

        if (!isOwner && !hasPermission)
        {
            context.Result = new ObjectResult(
                ApiResponse<object>.Fail("ليس لديك صلاحية لتنفيذ هذه العملية"))
            { StatusCode = StatusCodes.Status403Forbidden };
        }

        return Task.CompletedTask;
    }
}