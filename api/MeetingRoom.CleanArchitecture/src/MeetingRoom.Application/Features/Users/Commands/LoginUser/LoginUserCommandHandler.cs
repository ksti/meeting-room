using AutoMapper;
using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.Common.Exceptions;
using MeetingRoom.Application.Interfaces;
using MeetingRoom.Domain.Aggregates.UserAggregate;
using MeetingRoom.Domain.Interfaces.Repositories;
using MeetingRoom.Domain.ValueObjects;

namespace MeetingRoom.Application.Features.Users.Commands.LoginUser;

/// <summary>
/// 用户登录命令处理程序
/// </summary>
public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<LoginResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ITokenService _tokenService;
    private readonly IDateTime _dateTime;
    
    public LoginUserCommandHandler(
        IUserRepository userRepository,
        IMapper mapper,
        ITokenService tokenService,
        IDateTime dateTime)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _tokenService = tokenService;
        _dateTime = dateTime;
    }
    
    /// <summary>
    /// 处理用户登录命令
    /// </summary>
    /// <param name="request">登录命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>结果，包含登录响应</returns>
    public async Task<Result<LoginResponseDto>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 查找用户
            var user = await FindUserAsync(request.UsernameOrEmail, cancellationToken);
            
            if (user == null)
            {
                return Result<LoginResponseDto>.Failure("用户名或密码不正确");
            }
            
            // 验证密码
            if (!user.VerifyPassword(request.Password))
            {
                return Result<LoginResponseDto>.Failure("用户名或密码不正确");
            }
            
            // 检查用户状态
            if (!user.IsActive())
            {
                return Result<LoginResponseDto>.Failure("用户已被禁用或锁定");
            }
            
            // 生成令牌
            var (accessToken, refreshToken, expiresIn) = await _tokenService.GenerateTokensAsync(user);
            
            // 创建令牌实体
            var token = Token.Create(
                accessToken,
                "Bearer",
                _dateTime.UtcNow.AddSeconds(expiresIn),
                user.Id,
                refreshToken,
                _dateTime.UtcNow.AddDays(30));
            
            // 添加令牌到用户
            user.AddToken(token);
            
            // 如果提供了设备信息，则添加或更新设备
            if (!string.IsNullOrWhiteSpace(request.DeviceIdentifier) && !string.IsNullOrWhiteSpace(request.DeviceName))
            {
                var device = Device.Create(
                    request.DeviceIdentifier,
                    request.DeviceName,
                    request.Platform ?? string.Empty,
                    request.OperatingSystem ?? string.Empty,
                    request.OsVersion ?? string.Empty,
                    user.Id);
                
                user.AddDevice(device);
            }
            
            // 更新用户
            await _userRepository.UpdateAsync(user, cancellationToken);
            
            // 创建响应
            var response = new LoginResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email.Value,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenType = "Bearer",
                ExpiresIn = expiresIn,
                User = _mapper.Map<DTOs.UserDto>(user)
            };
            
            return Result<LoginResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            // 处理领域异常
            if (ex is Domain.Exceptions.DomainException domainException)
            {
                return Result<LoginResponseDto>.Failure(domainException.Message);
            }
            
            // 处理其他异常
            throw new ApplicationServiceException("登录时发生错误", ex);
        }
    }
    
    /// <summary>
    /// 根据用户名或电子邮件查找用户
    /// </summary>
    /// <param name="usernameOrEmail">用户名或电子邮件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户，如果不存在则返回null</returns>
    private async Task<User?> FindUserAsync(string usernameOrEmail, CancellationToken cancellationToken)
    {
        // 尝试作为用户名查找
        var user = await _userRepository.GetByUsernameAsync(usernameOrEmail, cancellationToken);
        
        if (user == null)
        {
            // 尝试作为电子邮件查找
            user = await _userRepository.GetByEmailAsync(usernameOrEmail, cancellationToken);
        }
        
        return user;
    }
}
