using FluentValidation;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Users.Commands.RegisterUser;

/// <summary>
/// 用户注册命令验证器
/// </summary>
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IUserRepository _userRepository;
    
    public RegisterUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;
        
        RuleFor(v => v.Username)
            .NotEmpty().WithMessage("用户名不能为空")
            .MinimumLength(3).WithMessage("用户名长度不能少于3个字符")
            .MaximumLength(50).WithMessage("用户名长度不能超过50个字符")
            .MustAsync(BeUniqueUsername).WithMessage("用户名已被使用");
        
        RuleFor(v => v.Email)
            .NotEmpty().WithMessage("电子邮件不能为空")
            .EmailAddress().WithMessage("电子邮件格式不正确")
            .MustAsync(BeUniqueEmail).WithMessage("电子邮件已被使用");
        
        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("密码不能为空")
            .MinimumLength(8).WithMessage("密码长度不能少于8个字符")
            .Matches("[A-Za-z]").WithMessage("密码必须包含至少一个字母")
            .Matches("[0-9]").WithMessage("密码必须包含至少一个数字");
        
        RuleFor(v => v.FirstName)
            .MaximumLength(50).WithMessage("姓长度不能超过50个字符");
        
        RuleFor(v => v.LastName)
            .MaximumLength(50).WithMessage("名长度不能超过50个字符");
        
        RuleFor(v => v.Contact)
            .MaximumLength(50).WithMessage("联系方式长度不能超过50个字符");
    }
    
    /// <summary>
    /// 验证用户名是否唯一
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否唯一</returns>
    private async Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken)
    {
        return !await _userRepository.IsUsernameExistsAsync(username, null, cancellationToken);
    }
    
    /// <summary>
    /// 验证电子邮件是否唯一
    /// </summary>
    /// <param name="email">电子邮件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否唯一</returns>
    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return !await _userRepository.IsEmailExistsAsync(email, null, cancellationToken);
    }
}
