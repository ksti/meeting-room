using System.Text.RegularExpressions;
using MeetingRoom.Domain.Exceptions;
using MeetingRoom.Domain.Shared;
using BCrypt.Net;

namespace MeetingRoom.Domain.ValueObjects;

/// <summary>
/// 密码值对象
/// 表示用户的密码
/// 作为值对象，Password是不可变的，一旦创建就不能修改
/// </summary>
public class Password : ValueObject
{
    /// <summary>
    /// 密码哈希值
    /// </summary>
    public string PasswordHash { get; }
    
    private Password(string passwordHash)
    {
        PasswordHash = passwordHash;
    }
    
    /// <summary>
    /// 创建密码（从明文密码）
    /// </summary>
    /// <param name="plainPassword">明文密码</param>
    /// <returns>密码值对象</returns>
    public static Password Create(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
        {
            throw new PasswordException("密码不能为空");
        }
        
        if (!IsValidPassword(plainPassword))
        {
            throw new PasswordException("密码必须至少包含8个字符，并且包含字母和数字");
        }
        
        // 使用BCrypt哈希密码
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        
        return new Password(passwordHash);
    }
    
    /// <summary>
    /// 从已有的密码哈希创建密码对象
    /// </summary>
    /// <param name="passwordHash">密码哈希</param>
    /// <returns>密码值对象</returns>
    public static Password CreateFromHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new PasswordException("密码哈希不能为空");
        }
        
        return new Password(passwordHash);
    }
    
    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="plainPassword">明文密码</param>
    /// <returns>密码是否正确</returns>
    public bool Verify(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
        {
            return false;
        }
        
        return BCrypt.Net.BCrypt.Verify(plainPassword, PasswordHash);
    }
    
    /// <summary>
    /// 验证密码格式
    /// </summary>
    /// <param name="password">明文密码</param>
    /// <returns>密码格式是否有效</returns>
    private static bool IsValidPassword(string password)
    {
        // 密码至少8个字符，包含字母和数字
        var regex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d).{8,}$");
        return regex.IsMatch(password);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PasswordHash;
    }
    
    // 不重写ToString方法，避免泄露密码信息
}

/// <summary>
/// 密码相关的领域异常
/// </summary>
public class PasswordException : DomainException
{
    public PasswordException(string message) : base(message)
    {
    }
}
