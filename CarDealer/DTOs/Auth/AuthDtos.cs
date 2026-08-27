namespace CarDealer.API.DTOs.Auth;

public record LoginDto(
    string Email,
    string Password
);

public record RegisterDto(
    string Username,
    string Email,
    string Password,
    string Role = "Admin"
);

public record AuthResponseDto(
    int Id,
    string Username,
    string Email,
    string Role,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry
);

public record RefreshTokenDto(
    string RefreshToken
);

public record ChangePasswordDto(
    string CurrentPassword,
    string NewPassword
);

/// <summary>
/// DTO for registering a new dealership with owner user.
/// Creates both Tenant and AppUser records simultaneously.
/// </summary>
public record RegisterDealershipDto(
    string DealershipName,
    string DealershipSlug,
    string OwnerUsername,
    string OwnerEmail,
    string OwnerPassword
);

/// <summary>
/// Response after successful dealership registration.
/// Returns TenantId and JWT credentials for the new owner.
/// </summary>
public record DealershipRegistrationResponseDto(
    int TenantId,
    string TenantName,
    string TenantSlug,
    int OwnerId,
    string OwnerUsername,
    string OwnerEmail,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiry
);

