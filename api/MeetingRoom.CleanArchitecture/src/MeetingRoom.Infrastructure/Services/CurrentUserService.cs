using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MeetingRoom.Application.Interfaces;

namespace MeetingRoom.Infrastructure.Services;

/// <summary>
/// 当前用户服务实现
/// 用于获取当前请求的用户信息
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue("id");
    
    /// <summary>
    /// 获取当前用户名
    /// </summary>
    public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
    
    /// <summary>
    /// 检查当前用户是否已认证
    /// </summary>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    
    /// <summary>
    /// 检查当前用户是否具有指定角色
    /// </summary>
    /// <param name="role">角色名称</param>
    /// <returns>是否具有指定角色</returns>
    public bool IsInRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }
}
