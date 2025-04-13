using MeetingRoom.Domain.Exceptions;
using MeetingRoom.Domain.Shared;

namespace MeetingRoom.Domain.ValueObjects;

/// <summary>
/// 人名值对象
/// 表示一个人的姓名，包含姓和名
/// 作为值对象，PersonName是不可变的，一旦创建就不能修改
/// </summary>
public class PersonName : ValueObject
{
    /// <summary>
    /// 姓
    /// </summary>
    public string FirstName { get; }
    
    /// <summary>
    /// 名
    /// </summary>
    public string LastName { get; }
    
    /// <summary>
    /// 全名
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
    
    private PersonName(string firstName, string lastName)
    {
        FirstName = firstName?.Trim() ?? string.Empty;
        LastName = lastName?.Trim() ?? string.Empty;
    }
    
    /// <summary>
    /// 创建人名
    /// </summary>
    /// <param name="firstName">姓</param>
    /// <param name="lastName">名</param>
    /// <returns>人名值对象</returns>
    public static PersonName Create(string? firstName, string? lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
        {
            throw new PersonNameException("姓名不能为空");
        }
        
        return new PersonName(firstName ?? string.Empty, lastName ?? string.Empty);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }
    
    public override string ToString()
    {
        return FullName;
    }
}

/// <summary>
/// 人名相关的领域异常
/// </summary>
public class PersonNameException : DomainException
{
    public PersonNameException(string message) : base(message)
    {
    }
}
