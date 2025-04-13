namespace MeetingRoom.Domain.Shared;

/// <summary>
/// 值对象基类
/// 值对象是没有唯一标识的对象，其同一性由所有属性值确定
/// 值对象应该是不可变的，一旦创建就不能修改
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// 获取值对象的组成部分，用于比较相等性
    /// </summary>
    /// <returns>组成值对象的属性值集合</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();
    
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
            return false;
        
        var other = (ValueObject)obj;
        
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
    
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }
    
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;
        
        if (left is null || right is null)
            return false;
        
        return left.Equals(right);
    }
    
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}
