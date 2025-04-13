using MeetingRoom.Domain.Exceptions;
using MeetingRoom.Domain.Shared;

namespace MeetingRoom.Domain.ValueObjects;

/// <summary>
/// 设备信息值对象
/// 表示一个设备的基本信息
/// 作为值对象，DeviceInfo是不可变的，一旦创建就不能修改
/// </summary>
public class DeviceInfo : ValueObject
{
    /// <summary>
    /// 设备标识符
    /// </summary>
    public string DeviceIdentifier { get; }
    
    /// <summary>
    /// 设备名称
    /// </summary>
    public string DeviceName { get; }
    
    /// <summary>
    /// 平台
    /// </summary>
    public string Platform { get; }
    
    /// <summary>
    /// 操作系统
    /// </summary>
    public string OperatingSystem { get; }
    
    /// <summary>
    /// 操作系统版本
    /// </summary>
    public string OsVersion { get; }
    
    private DeviceInfo(string deviceIdentifier, string deviceName, string platform, string operatingSystem, string osVersion)
    {
        DeviceIdentifier = deviceIdentifier;
        DeviceName = deviceName;
        Platform = platform;
        OperatingSystem = operatingSystem;
        OsVersion = osVersion;
    }
    
    /// <summary>
    /// 创建设备信息
    /// </summary>
    /// <param name="deviceIdentifier">设备标识符</param>
    /// <param name="deviceName">设备名称</param>
    /// <param name="platform">平台</param>
    /// <param name="operatingSystem">操作系统</param>
    /// <param name="osVersion">操作系统版本</param>
    /// <returns>设备信息值对象</returns>
    public static DeviceInfo Create(string deviceIdentifier, string deviceName, string platform, string operatingSystem, string osVersion)
    {
        if (string.IsNullOrWhiteSpace(deviceIdentifier))
        {
            throw new DeviceInfoException("设备标识符不能为空");
        }
        
        if (string.IsNullOrWhiteSpace(deviceName))
        {
            throw new DeviceInfoException("设备名称不能为空");
        }
        
        return new DeviceInfo(
            deviceIdentifier.Trim(),
            deviceName.Trim(),
            platform?.Trim() ?? string.Empty,
            operatingSystem?.Trim() ?? string.Empty,
            osVersion?.Trim() ?? string.Empty
        );
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return DeviceIdentifier;
        yield return DeviceName;
        yield return Platform;
        yield return OperatingSystem;
        yield return OsVersion;
    }
    
    public override string ToString()
    {
        return $"{DeviceName} ({Platform} {OperatingSystem} {OsVersion})";
    }
}

/// <summary>
/// 设备信息相关的领域异常
/// </summary>
public class DeviceInfoException : DomainException
{
    public DeviceInfoException(string message) : base(message)
    {
    }
}
