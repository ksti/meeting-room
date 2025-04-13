using FluentValidation;

namespace MeetingRoom.Application.Features.Users.Commands.LoginUser;

/// <summary>
/// 用户登录命令验证器
/// </summary>
public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(v => v.UsernameOrEmail)
            .NotEmpty().WithMessage("用户名或电子邮件不能为空");
        
        RuleFor(v => v.Password)
            .NotEmpty().WithMessage("密码不能为空");
        
        // 设备信息验证，如果提供了设备标识符，则设备名称也必须提供
        When(v => !string.IsNullOrWhiteSpace(v.DeviceIdentifier), () =>
        {
            RuleFor(v => v.DeviceName)
                .NotEmpty().WithMessage("设备名称不能为空");
        });
    }
}
