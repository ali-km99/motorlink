using CarDealer.API.Shared.Services.Interfaces;


namespace CarDealer.API.Shared.Services.Implementations
{
    /// <summary>
    /// Implementation of ICurrentTenantService that extracts tenant context from HTTP user claims.
    /// </summary>
    public class CurrentTenantService : ICurrentTenantService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        /// <summary>
        /// Gets the current tenant ID from the "tenant_id" claim in the user's identity.
        /// Returns null if the claim is not found or if the user is a platform admin.
        /// </summary>
        public int? TenantId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null) return null;

                // Platform admins have cross-tenant access; TenantId is null
                if (IsPlatformAdmin) return null;

                var tenantIdClaim = user.FindFirst("tenant_id");
                if (tenantIdClaim != null && int.TryParse(tenantIdClaim.Value, out var tenantId))
                {
                    return tenantId;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the current user is a platform administrator.
        /// Extracts from "is_platform_admin" claim (lowercase with underscore).
        /// </summary>
        public bool IsPlatformAdmin
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                if (user == null) return false;

                var platformAdminClaim = user.FindFirst("is_platform_admin");
                if (platformAdminClaim != null && bool.TryParse(platformAdminClaim.Value, out var isPlatformAdmin))
                {
                    return isPlatformAdmin;
                }

                return false;
            }
        }
    }
}
