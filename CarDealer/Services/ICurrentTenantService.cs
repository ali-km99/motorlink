namespace CarDealer.API.Services
{
    /// <summary>
    /// Provides access to the current tenant context from the HTTP request.
    /// Extracts tenant information from JWT claims in the user's identity.
    /// </summary>
    public interface ICurrentTenantService
    {
        /// <summary>
        /// Gets the current tenant ID from the user's claims.
        /// Returns null for Platform Admins (who have cross-tenant access).
        /// </summary>
        int? TenantId { get; }

        /// <summary>
        /// Gets a value indicating whether the current user is a platform administrator.
        /// Platform admins can access data across all tenants.
        /// </summary>
        bool IsPlatformAdmin { get; }
    }
}
