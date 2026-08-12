namespace CarDealer.API.DTOs.Permission;

public record PermissionDto(int Id, string Code, string Name, string Category);

public record CreateUserDto(
    string Username,
    string Email,
    string Password,
    List<int> PermissionIds
);

public record UpdateUserPermissionsDto(List<int> PermissionIds);

public record UserWithPermissionsDto(
    int Id, string Username, string Email, string Role,
     bool IsActive,
    List<PermissionDto> Permissions
);


public record UpdateUserDto(string Username, string Email, string Role);