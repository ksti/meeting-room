namespace MeetingRoom.Application.Interfaces;

/// <summary>
/// 当前用户服务接口
/// 用于获取当前请求的用户信息
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// 获取当前用户ID
    /// </summary>
    string? UserId { get; }
    
    /// <summary>
    /// 获取当前用户名
    /// </summary>
    string? Username { get; }
    
    /// <summary>
    /// 检查当前用户是否已认证
    /// </summary>
    bool IsAuthenticated { get; }
    
    /// <summary>
    /// 检查当前用户是否具有指定角色
    /// </summary>
    /// <param name="role">角色名称</param>
    /// <returns>是否具有指定角色</returns>
    bool IsInRole(string role);
}
