using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.Common.Exceptions;
using MeetingRoom.Application.Interfaces;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Users.Commands.LoginUser;

/// <summary>
/// 用户注销命令处理程序
/// </summary>
public class LogoutUserCommandHandler : IRequestHandler<LogoutUserCommand, Result<bool>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    
    public LogoutUserCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }
    
    /// <summary>
    /// 处理用户注销命令
    /// </summary>
    /// <param name="request">注销命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>结果，包含是否成功</returns>
    public async Task<Result<bool>> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 获取用户ID
            var userId = request.UserId ?? _currentUserService.UserId;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Result<bool>.Failure("未能获取用户信息");
            }
            
            // 查找用户
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            
            if (user == null)
            {
                return Result<bool>.Failure("用户不存在");
            }
            
            // 清除用户的所有令牌
            user.RevokeAllTokens();
            
            // 更新用户
            await _userRepository.UpdateAsync(user, cancellationToken);
            
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            // 处理领域异常
            if (ex is Domain.Exceptions.DomainException domainException)
            {
                return Result<bool>.Failure(domainException.Message);
            }
            
            // 处理其他异常
            throw new ApplicationServiceException("注销时发生错误", ex);
        }
    }
}
