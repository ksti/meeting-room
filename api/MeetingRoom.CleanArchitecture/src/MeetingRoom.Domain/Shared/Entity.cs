namespace MeetingRoom.Domain.Shared;

/// <summary>
/// 实体基类
/// 实体是具有唯一标识的对象，其同一性由ID确定而非属性值
/// </summary>
public abstract class Entity
{
    public string Id { get; protected set; }
    
    protected Entity()
    {
        Id = Guid.NewGuid().ToString();
    }
    
    protected Entity(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("实体ID不能为空", nameof(id));
        }
        
        Id = id;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Entity other)
            return false;
        
        if (ReferenceEquals(this, other))
            return true;
        
        if (GetType() != other.GetType())
            return false;
        
        if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(other.Id))
            return false;
        
        return Id == other.Id;
    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
            return true;
        
        if (left is null || right is null)
            return false;
        
        return left.Equals(right);
    }
    
    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}
