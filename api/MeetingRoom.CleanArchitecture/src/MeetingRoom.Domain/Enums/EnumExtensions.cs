using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MeetingRoom.Domain.Enums;

/// <summary>
/// 枚举扩展方法
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// 获取枚举值的显示名称
    /// </summary>
    /// <param name="value">枚举值</param>
    /// <returns>显示名称</returns>
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        
        if (field == null)
            return value.ToString();
        
        var attribute = field.GetCustomAttribute<DisplayAttribute>();
        
        return attribute?.Name ?? value.ToString();
    }
    
    /// <summary>
    /// 从显示名称获取枚举值
    /// </summary>
    /// <typeparam name="TEnum">枚举类型</typeparam>
    /// <param name="displayName">显示名称</param>
    /// <returns>枚举值</returns>
    public static TEnum GetEnumFromDisplayName<TEnum>(string displayName) where TEnum : Enum
    {
        var type = typeof(TEnum);
        
        foreach (var field in type.GetFields())
        {
            if (field.GetCustomAttribute<DisplayAttribute>() is DisplayAttribute attribute && attribute.Name == displayName)
            {
                return (TEnum)field.GetValue(null)!;
            }
            
            if (field.Name == displayName)
            {
                return (TEnum)field.GetValue(null)!;
            }
        }
        
        throw new ArgumentException($"找不到显示名称为 {displayName} 的枚举值", nameof(displayName));
    }
}
