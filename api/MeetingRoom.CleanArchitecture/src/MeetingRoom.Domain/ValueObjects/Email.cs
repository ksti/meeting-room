using System.Text.RegularExpressions;
using MeetingRoom.Domain.Exceptions;
using MeetingRoom.Domain.Shared;

namespace MeetingRoom.Domain.ValueObjects;

/// <summary>
/// 电子邮件值对象
/// 表示一个电子邮件地址
/// 作为值对象，Email是不可变的，一旦创建就不能修改
/// </summary>
public class Email : ValueObject
{
    /// <summary>
    /// 电子邮件地址
    /// </summary>
    public string Value { get; }
    
    private Email(string value)
    {
        Value = value;
    }
    
    /// <summary>
    /// 创建电子邮件
    /// </summary>
    /// <param name="email">电子邮件地址</param>
    /// <returns>电子邮件值对象</returns>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new EmailException("电子邮件地址不能为空");
        }
        
        email = email.Trim();
        
        if (!IsValidEmail(email))
        {
            throw new EmailException("电子邮件地址格式不正确");
        }
        
        return new Email(email);
    }
    
    /// <summary>
    /// 验证电子邮件地址格式
    /// </summary>
    /// <param name="email">电子邮件地址</param>
    /// <returns>是否有效</returns>
    private static bool IsValidEmail(string email)
    {
        // 使用正则表达式验证电子邮件格式
        var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        return regex.IsMatch(email);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value.ToLowerInvariant();
    }
    
    public override string ToString()
    {
        return Value;
    }
}

/// <summary>
/// 电子邮件相关的领域异常
/// </summary>
public class EmailException : DomainException
{
    public EmailException(string message) : base(message)
    {
    }
}
