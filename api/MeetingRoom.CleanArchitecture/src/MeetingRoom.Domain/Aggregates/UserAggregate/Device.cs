using MeetingRoom.Domain.Enums;
using MeetingRoom.Domain.Shared;
using MeetingRoom.Domain.ValueObjects;

namespace MeetingRoom.Domain.Aggregates.UserAggregate;

/// <summary>
/// 设备实体
/// 表示用户的登录设备
/// 作为实体，Device有唯一标识，但生命周期由User聚合根管理
/// </summary>
public class Device : AuditableEntity
{
    /// <summary>
    /// 设备信息
    /// </summary>
    public DeviceInfo DeviceInfo { get; private set; }
    
    /// <summary>
    /// 设备状态
    /// </summary>
    public DeviceStatus Status { get; private set; }
    
    /// <summary>
    /// 最后活动时间
    /// </summary>
    public DateTime LastActivityAt { get; private set; }
    
    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; private set; }
    
    // 导航属性，由ORM框架使用
    public User? User { get; private set; }
    
    // 私有构造函数，防止直接创建实例
    private Device() 
    { 
        DeviceInfo = DeviceInfo.Create(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        UserId = string.Empty;
        Status = DeviceStatus.Disabled;
        LastActivityAt = DateTime.MinValue;
    }
    
    private Device(DeviceInfo deviceInfo, string userId)
    {
        DeviceInfo = deviceInfo;
        UserId = userId;
        Status = DeviceStatus.Active;
        LastActivityAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// 创建新设备
    /// </summary>
    /// <param name="deviceIdentifier">设备标识符</param>
    /// <param name="deviceName">设备名称</param>
    /// <param name="platform">平台</param>
    /// <param name="operatingSystem">操作系统</param>
    /// <param name="osVersion">操作系统版本</param>
    /// <param name="userId">用户ID</param>
    /// <returns>设备实体</returns>
    public static Device Create(
        string deviceIdentifier,
        string deviceName,
        string platform,
        string operatingSystem,
        string osVersion,
        string userId)
    {
        var deviceInfo = DeviceInfo.Create(
            deviceIdentifier,
            deviceName,
            platform,
            operatingSystem,
            osVersion);
        
        return new Device(deviceInfo, userId);
    }
    
    /// <summary>
    /// 更新最后活动时间
    /// </summary>
    public void UpdateLastActivity()
    {
        LastActivityAt = DateTime.UtcNow;
    }
    
    /// <summary>
    /// 禁用设备
    /// </summary>
    public void Disable()
    {
        Status = DeviceStatus.Disabled;
    }
    
    /// <summary>
    /// 启用设备
    /// </summary>
    public void Enable()
    {
        Status = DeviceStatus.Active;
    }
    
    /// <summary>
    /// 检查设备是否活跃
    /// </summary>
    /// <returns>是否活跃</returns>
    public bool IsActive()
    {
        return Status == DeviceStatus.Active;
    }
}
