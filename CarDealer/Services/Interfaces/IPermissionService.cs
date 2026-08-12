using CarDealer.API.DTOs.Permission;

public interface IPermissionService
{
    Task<List<PermissionDto>> GetAllPermissionsAsync();
    Task<UserWithPermissionsDto?> CreateUserWithPermissionsAsync(CreateUserDto dto);
    Task<UserWithPermissionsDto?> UpdateUserPermissionsAsync(int userId, List<int> permissionIds);
    Task<UserWithPermissionsDto?> GetUserWithPermissionsAsync(int userId);
    Task<List<UserWithPermissionsDto>> GetAllUsersAsync();
    Task<UserWithPermissionsDto?> UpdateUserAsync(int userId, UpdateUserDto dto);
    Task<bool> ToggleUserStatusAsync(int userId);
}