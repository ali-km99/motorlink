using CarDealer.API.Features.Users.DTOs;
using CarDealer.API.Shared.Common;
using CarDealer.API.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarDealer.API.Features.Users.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Roles.Owner)]
    public class UsersController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        public UsersController(IPermissionService permissionService) => _permissionService = permissionService;

        [HttpGet("~/api/permissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var result = await _permissionService.GetAllPermissionsAsync();
            return Ok(ApiResponse<List<PermissionDto>>.Ok(result));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            var result = await _permissionService.CreateUserWithPermissionsAsync(dto);
            if (result is null)
                return BadRequest(ApiResponse<object>.Fail("فشل إنشاء المستخدم"));
            return Ok(ApiResponse<UserWithPermissionsDto>.Ok(result));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _permissionService.GetAllUsersAsync();
            return Ok(ApiResponse<List<UserWithPermissionsDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var result = await _permissionService.GetUserWithPermissionsAsync(id);
            if (result is null) return NotFound(ApiResponse<object>.Fail("المستخدم غير موجود"));
            return Ok(ApiResponse<UserWithPermissionsDto>.Ok(result));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var result = await _permissionService.UpdateUserAsync(id, dto);
            if (result is null) return NotFound(ApiResponse<object>.Fail("المستخدم غير موجود"));
            return Ok(ApiResponse<UserWithPermissionsDto>.Ok(result));
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var result = await _permissionService.ToggleUserStatusAsync(id);

            if (!result)
                return NotFound();

            return Ok(new { message = "تم تعديل حالة المستخدم " });
        }

        [HttpPatch("{id}/permissions")]
        public async Task<IActionResult> UpdatePermissions(int id, [FromBody] UpdateUserPermissionsDto dto)
        {
            var result = await _permissionService.UpdateUserPermissionsAsync(id, dto.PermissionIds);
            if (result is null) return NotFound(ApiResponse<object>.Fail("المستخدم غير موجود"));
            return Ok(ApiResponse<UserWithPermissionsDto>.Ok(result));
        }


    }
}
