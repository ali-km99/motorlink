using CarDealer.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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

        // السوبر أدمن يتجاوز كل فحص صلاحيات تلقائيًا
        var isSuperAdmin = user.IsInRole("SuperAdmin");
        var hasPermission = user.HasClaim("permission", _permissionCode);

        if (!isSuperAdmin && !hasPermission)
        {
            context.Result = new ObjectResult(
                ApiResponse<object>.Fail("ليس لديك صلاحية لتنفيذ هذه العملية"))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }

        return Task.CompletedTask;
    }
}