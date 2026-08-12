using CarDealer.API.Data;
using CarDealer.API.DTOs.Permission;
using CarDealer.API.Entities;
using Microsoft.EntityFrameworkCore;

public class PermissionService : IPermissionService
{
    private readonly AppDbContext _context;
    public PermissionService(AppDbContext context) => _context = context;

    public async Task<List<PermissionDto>> GetAllPermissionsAsync() =>
        await _context.Permissions
            .OrderBy(p => p.Category).ThenBy(p => p.Name)
            .Select(p => new PermissionDto(p.Id, p.Code, p.Name, p.Category))
            .ToListAsync();

    public async Task<UserWithPermissionsDto?> CreateUserWithPermissionsAsync(CreateUserDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
            throw new InvalidOperationException("اسم المستخدم أو البريد الإلكتروني مستخدم مسبقًا");

        var user = new AppUser
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password), // حسب مكتبة الهاش المستخدمة عندكم فعليًا
            Role = "User" // السوبر أدمن يُنشأ يدويًا فقط بقاعدة البيانات، مو عبر هذا الـ endpoint
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        if (dto.PermissionIds?.Any() == true)
        {
            var permissions = dto.PermissionIds.Distinct()
                .Select(pid => new UserPermission { UserId = user.Id, PermissionId = pid });
            await _context.UserPermissions.AddRangeAsync(permissions);
            await _context.SaveChangesAsync();
        }

        return await GetUserWithPermissionsAsync(user.Id);
    }

    public async Task<UserWithPermissionsDto?> UpdateUserAsync(int userId, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return null;

        if (user.Role == "SuperAdmin" && dto.Role != "SuperAdmin")
            throw new InvalidOperationException("لا يمكن تغيير رتبة حساب السوبر أدمن");

        var username = dto.Username.Trim();
        var email = dto.Email.Trim().ToLower();

        // ─── منع التكرار (باستثناء المستخدم نفسه) ─────────────────────
        var duplicateExists = await _context.Users
            .AnyAsync(u => u.Id != userId && (u.Username == username || u.Email == email));

        if (duplicateExists)
            throw new InvalidOperationException("اسم المستخدم أو البريد الإلكتروني مستخدم مسبقًا");

        user.Username = username;
        user.Email = email;
        user.Role = dto.Role;

        await _context.SaveChangesAsync();

        return await GetUserWithPermissionsAsync(userId);
    }

    public async Task<UserWithPermissionsDto?> UpdateUserPermissionsAsync(int userId, List<int> permissionIds)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null) return null;

        if (user.Role == "SuperAdmin")
            throw new InvalidOperationException("لا يمكن تعديل صلاحيات حساب السوبر أدمن");

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            // استبدال كامل للصلاحيات (Replace-All Pattern) — أبسط وأضمن من مقارنة Diff
            await _context.UserPermissions
                .Where(up => up.UserId == userId)
                .ExecuteDeleteAsync();

            if (permissionIds?.Any() == true)
            {
                var newPermissions = permissionIds.Distinct()
                    .Select(pid => new UserPermission { UserId = userId, PermissionId = pid });
                await _context.UserPermissions.AddRangeAsync(newPermissions);
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        });

        return await GetUserWithPermissionsAsync(userId);
    }

    public async Task<UserWithPermissionsDto?> GetUserWithPermissionsAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Permissions).ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return null;

        return new UserWithPermissionsDto(
            user.Id, user.Username, user.Email, user.Role,
              user.IsActive,
            user.Permissions.Select(up => new PermissionDto(
                up.Permission.Id, up.Permission.Code, up.Permission.Name, up.Permission.Category)).ToList());
    }

    public async Task<bool> ToggleUserStatusAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);

        if (user is null)
            return false;

        user.IsActive = !user.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<List<UserWithPermissionsDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .Include(u => u.Permissions).ThenInclude(up => up.Permission)
            .OrderBy(u => u.Username)
            .ToListAsync();

        return users.Select(user => new UserWithPermissionsDto(
            user.Id, user.Username, user.Email, user.Role,
              user.IsActive,
            user.Permissions.Select(up => new PermissionDto(
                up.Permission.Id, up.Permission.Code, up.Permission.Name, up.Permission.Category)).ToList()
        )).ToList();
    }
}