using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.Common.Exceptions;
using MeetingRoom.Application.Interfaces;
using MeetingRoom.Domain.Aggregates.UserAggregate;
using MeetingRoom.Domain.Enums;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Users.Commands.RegisterUser;

/// <summary>
/// 用户注册命令处理程序
/// </summary>
public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IDateTime _dateTime;
    
    public RegisterUserCommandHandler(IUserRepository userRepository, IDateTime dateTime)
    {
        _userRepository = userRepository;
        _dateTime = dateTime;
    }
    
    /// <summary>
    /// 处理用户注册命令
    /// </summary>
    /// <param name="request">注册命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>结果，包含用户ID</returns>
    public async Task<Result<string>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 检查用户名和电子邮件是否已存在
            if (await _userRepository.IsUsernameExistsAsync(request.Username, null, cancellationToken))
            {
                return Result<string>.Failure("用户名已被使用");
            }
            
            if (await _userRepository.IsEmailExistsAsync(request.Email, null, cancellationToken))
            {
                return Result<string>.Failure("电子邮件已被使用");
            }
            
            // 创建用户
            var user = User.Create(
                request.Username,
                request.Email,
                request.Password,
                UserRole.User);
            
            // 设置用户的其他信息
            if (!string.IsNullOrWhiteSpace(request.FirstName) || !string.IsNullOrWhiteSpace(request.LastName))
            {
                user.UpdateProfile(request.FirstName, request.LastName, request.Contact, null);
            }
            
            // 设置审计信息
            user.SetCreated(request.Username);
            
            // 保存用户
            await _userRepository.AddAsync(user, cancellationToken);
            
            return Result<string>.Success(user.Id);
        }
        catch (Exception ex)
        {
            // 处理领域异常
            if (ex is Domain.Exceptions.DomainException domainException)
            {
                return Result<string>.Failure(domainException.Message);
            }
            
            // 处理其他异常
            throw new ApplicationServiceException("注册用户时发生错误", ex);
        }
    }
}
