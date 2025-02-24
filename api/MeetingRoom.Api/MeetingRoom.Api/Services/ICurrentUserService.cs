using MeetingRoom.Api.Models;

namespace MeetingRoom.Api.Services;

public interface ICurrentUserService
{
    string? Username { get; }

    /// <summary>
    /// 获取当前用户信息
    /// </summary>
    /// <returns>当前用户信息，如果未登录则返回null</returns>
    Task<UserModel?> GetCurrentUserAsync();
}
