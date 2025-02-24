using Microsoft.EntityFrameworkCore;
using MeetingRoom.Api.Data;
using MeetingRoom.Api.Extensions;
using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    /// <summary>
    /// 当前用户邮箱
    /// </summary>
    public string? UserId => _httpContextAccessor.HttpContext?.User?.Identity?.Name;

    /// <summary>
    /// 当前用户ID
    /// </summary>
    public string? TenantId => _httpContextAccessor.HttpContext?.User?.FindFirst("id")?.Value;

    /// <summary>
    /// 是否已认证
    /// </summary>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    public async Task<UserModel?> GetCurrentUserAsync()
    {
        if (!IsAuthenticated || string.IsNullOrEmpty(UserId))
        {
            return null;
        }

        var user = await _context.Users
            .AsNoTracking()
            .Include(u => u.Tokens)
            .Include(u => u.Devices)
            .FirstOrDefaultAsync(u => u.Id == UserId);

        return user?.MapToModel();
    }
}
