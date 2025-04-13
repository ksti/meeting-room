using MeetingRoom.Domain.Enums;
using MeetingRoom.Domain.Events;
using MeetingRoom.Domain.Exceptions;
using MeetingRoom.Domain.Shared;
using MeetingRoom.Domain.ValueObjects;

namespace MeetingRoom.Domain.Aggregates.UserAggregate;

/// <summary>
/// 用户聚合根
/// 用户是一个聚合根，包含用户的基本信息和相关操作
/// </summary>
public class User : AuditableAggregateRoot
{
    /// <summary>
    /// 用户姓名
    /// </summary>
    public PersonName? Name { get; private set; }
    
    /// <summary>
    /// 电子邮件
    /// </summary>
    public Email Email { get; private set; }
    
    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; private set; }
    
    /// <summary>
    /// 密码
    /// </summary>
    public Password Password { get; private set; }
    
    /// <summary>
    /// 联系方式
    /// </summary>
    public string? Contact { get; private set; }
    
    /// <summary>
    /// 头像
    /// </summary>
    public string? Avatar { get; private set; }
    
    /// <summary>
    /// 用户角色
    /// </summary>
    public UserRole Role { get; private set; }
    
    /// <summary>
    /// 用户状态
    /// </summary>
    public UserStatus Status { get; private set; }
    
    /// <summary>
    /// 用户的令牌集合
    /// </summary>
    private readonly List<Token> _tokens = new();
    
    /// <summary>
    /// 用户的设备集合
    /// </summary>
    private readonly List<Device> _devices = new();
    
    /// <summary>
    /// 令牌集合（只读）
    /// </summary>
    public IReadOnlyCollection<Token> Tokens => _tokens.AsReadOnly();
    
    /// <summary>
    /// 设备集合（只读）
    /// </summary>
    public IReadOnlyCollection<Device> Devices => _devices.AsReadOnly();
    
    // 私有构造函数，防止直接创建实例
    private User() 
    { 
        Username = string.Empty;
        Email = Email.Create("placeholder@example.com");
        Password = Password.CreateFromHash("$2a$11$placeholder_hash_value_for_empty_constructor");
        Role = UserRole.User;
        Status = UserStatus.Disabled;
    }
    
    private User(
        string username,
        Email email,
        Password password,
        UserRole role = UserRole.User,
        UserStatus status = UserStatus.Active)
    {
        Username = username;
        Email = email;
        Password = password;
        Role = role;
        Status = status;
        
        // 添加用户创建事件
        AddDomainEvent(new UserCreatedEvent(Id, username, email.Value));
    }
    
    /// <summary>
    /// 创建新用户
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="email">电子邮件</param>
    /// <param name="password">密码</param>
    /// <param name="role">用户角色</param>
    /// <returns>用户聚合根</returns>
    public static User Create(
        string username,
        string email,
        string password,
        UserRole role = UserRole.User)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new UserDomainException("用户名不能为空");
        }
        
        var emailObj = Email.Create(email);
        var passwordObj = Password.Create(password);
        
        return new User(username, emailObj, passwordObj, role);
    }
    
    /// <summary>
    /// 从现有数据重建用户
    /// </summary>
    public static User Reconstitute(
        string id,
        string username,
        string email,
        string passwordHash,
        string? firstName,
        string? lastName,
        string? contact,
        string? avatar,
        UserRole role,
        UserStatus status)
    {
        var user = new User
        {
            Id = id,
            Username = username,
            Email = Email.Create(email),
            Password = Password.CreateFromHash(passwordHash),
            Contact = contact,
            Avatar = avatar,
            Role = role,
            Status = status
        };
        
        if (!string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName))
        {
            user.Name = PersonName.Create(firstName, lastName);
        }
        
        return user;
    }
    
    /// <summary>
    /// 更新用户信息
    /// </summary>
    /// <param name="firstName">姓</param>
    /// <param name="lastName">名</param>
    /// <param name="contact">联系方式</param>
    /// <param name="avatar">头像</param>
    public void UpdateProfile(string? firstName, string? lastName, string? contact, string? avatar)
    {
        if (!string.IsNullOrWhiteSpace(firstName) || !string.IsNullOrWhiteSpace(lastName))
        {
            Name = PersonName.Create(firstName, lastName);
        }
        
        Contact = contact;
        Avatar = avatar;
        
        AddDomainEvent(new UserUpdatedEvent(Id, Username, Email.Value));
    }
    
    /// <summary>
    /// 更改密码
    /// </summary>
    /// <param name="currentPassword">当前密码</param>
    /// <param name="newPassword">新密码</param>
    public void ChangePassword(string currentPassword, string newPassword)
    {
        if (!Password.Verify(currentPassword))
        {
            throw new UserDomainException("当前密码不正确");
        }
        
        Password = Password.Create(newPassword);
        
        // 密码更改后，吊销所有令牌
        RevokeAllTokens();
        
        AddDomainEvent(new UserPasswordChangedEvent(Id));
    }
    
    /// <summary>
    /// 重置密码
    /// </summary>
    /// <param name="newPassword">新密码</param>
    public void ResetPassword(string newPassword)
    {
        Password = Password.Create(newPassword);
        
        // 密码重置后，吊销所有令牌
        RevokeAllTokens();
        
        AddDomainEvent(new UserPasswordResetEvent(Id));
    }
    
    /// <summary>
    /// 更改用户角色
    /// </summary>
    /// <param name="role">新角色</param>
    public void ChangeRole(UserRole role)
    {
        if (Role == role)
        {
            return;
        }
        
        Role = role;
        
        AddDomainEvent(new UserRoleChangedEvent(Id, role));
    }
    
    /// <summary>
    /// 更改用户状态
    /// </summary>
    /// <param name="status">新状态</param>
    public void ChangeStatus(UserStatus status)
    {
        if (Status == status)
        {
            return;
        }
        
        Status = status;
        
        // 如果用户被禁用或锁定，吊销所有令牌
        if (status == UserStatus.Disabled || status == UserStatus.Locked)
        {
            RevokeAllTokens();
        }
        
        AddDomainEvent(new UserStatusChangedEvent(Id, status));
    }
    
    /// <summary>
    /// 添加令牌
    /// </summary>
    /// <param name="token">令牌</param>
    public void AddToken(Token token)
    {
        _tokens.Add(token);
    }
    
    /// <summary>
    /// 吊销令牌
    /// </summary>
    /// <param name="tokenId">令牌ID</param>
    public void RevokeToken(string tokenId)
    {
        var token = _tokens.FirstOrDefault(t => t.Id == tokenId);
        
        if (token != null)
        {
            token.Revoke();
        }
    }
    
    /// <summary>
    /// 吊销所有令牌
    /// </summary>
    public void RevokeAllTokens()
    {
        foreach (var token in _tokens)
        {
            token.Revoke();
        }
    }
    
    /// <summary>
    /// 添加设备
    /// </summary>
    /// <param name="device">设备</param>
    public void AddDevice(Device device)
    {
        var existingDevice = _devices.FirstOrDefault(d => d.DeviceInfo.DeviceIdentifier == device.DeviceInfo.DeviceIdentifier);
        
        if (existingDevice != null)
        {
            // 更新现有设备
            existingDevice.UpdateLastActivity();
        }
        else
        {
            _devices.Add(device);
        }
    }
    
    /// <summary>
    /// 禁用设备
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    public void DisableDevice(string deviceId)
    {
        var device = _devices.FirstOrDefault(d => d.Id == deviceId);
        
        if (device != null)
        {
            device.Disable();
        }
    }
    
    /// <summary>
    /// 验证密码
    /// </summary>
    /// <param name="password">密码</param>
    /// <returns>密码是否正确</returns>
    public bool VerifyPassword(string password)
    {
        return Password.Verify(password);
    }
    
    /// <summary>
    /// 检查用户是否活跃
    /// </summary>
    /// <returns>用户是否活跃</returns>
    public bool IsActive()
    {
        return Status == UserStatus.Active;
    }
}

/// <summary>
/// 用户相关的领域异常
/// </summary>
public class UserDomainException : DomainException
{
    public UserDomainException(string message) : base(message)
    {
    }
}

/// <summary>
/// 用户创建事件
/// </summary>
public class UserCreatedEvent : DomainEvent
{
    public string UserId { get; }
    public string Username { get; }
    public string Email { get; }
    
    public UserCreatedEvent(string userId, string username, string email)
    {
        UserId = userId;
        Username = username;
        Email = email;
    }
}

/// <summary>
/// 用户更新事件
/// </summary>
public class UserUpdatedEvent : DomainEvent
{
    public string UserId { get; }
    public string Username { get; }
    public string Email { get; }
    
    public UserUpdatedEvent(string userId, string username, string email)
    {
        UserId = userId;
        Username = username;
        Email = email;
    }
}

/// <summary>
/// 用户密码更改事件
/// </summary>
public class UserPasswordChangedEvent : DomainEvent
{
    public string UserId { get; }
    
    public UserPasswordChangedEvent(string userId)
    {
        UserId = userId;
    }
}

/// <summary>
/// 用户密码重置事件
/// </summary>
public class UserPasswordResetEvent : DomainEvent
{
    public string UserId { get; }
    
    public UserPasswordResetEvent(string userId)
    {
        UserId = userId;
    }
}

/// <summary>
/// 用户角色更改事件
/// </summary>
public class UserRoleChangedEvent : DomainEvent
{
    public string UserId { get; }
    public UserRole NewRole { get; }
    
    public UserRoleChangedEvent(string userId, UserRole newRole)
    {
        UserId = userId;
        NewRole = newRole;
    }
}

/// <summary>
/// 用户状态更改事件
/// </summary>
public class UserStatusChangedEvent : DomainEvent
{
    public string UserId { get; }
    public UserStatus NewStatus { get; }
    
    public UserStatusChangedEvent(string userId, UserStatus newStatus)
    {
        UserId = userId;
        NewStatus = newStatus;
    }
}
