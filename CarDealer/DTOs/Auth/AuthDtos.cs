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
