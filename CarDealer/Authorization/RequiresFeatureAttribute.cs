using CarDealer.API.DTOs;
using CarDealer.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CarDealer.API.Authorization;

public class RequiresFeatureAttribute : TypeFilterAttribute
{
    public RequiresFeatureAttribute(string featureCode) : base(typeof(RequiresFeatureFilter))
    {
        Arguments = new object[] { featureCode };
    }
}

public class RequiresFeatureFilter : IAsyncAuthorizationFilter
{
    private readonly string _featureCode;
    private readonly ITenantFeatureService _featureService;

    public RequiresFeatureFilter(string featureCode, ITenantFeatureService featureService)
    {
        _featureCode = featureCode;
        _featureService = featureService;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var user = context.HttpContext.User;

        if (!user.Identity?.IsAuthenticated == true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Platform Admin لا ينتمي لأي معرض ولا يخضع لخطط الاشتراك
        var isPlatformAdmin = user.HasClaim(c => c.Type == "is_platform_admin" && c.Value == "true");
        if (isPlatformAdmin)
            return;

        var allowed = await _featureService.HasFeatureAsync(_featureCode);
        if (!allowed)
        {
            context.Result = new ObjectResult(
                ApiResponse<object>.Fail("هذه الميزة غير متاحة ضمن خطة اشتراك معرضك الحالية"))
            { StatusCode = StatusCodes.Status403Forbidden };
        }
    }
}