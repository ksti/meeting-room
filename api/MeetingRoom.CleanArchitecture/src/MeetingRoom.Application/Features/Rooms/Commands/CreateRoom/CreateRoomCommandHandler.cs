using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.Common.Exceptions;
using MeetingRoom.Application.Interfaces;
using MeetingRoom.Domain.Aggregates.RoomAggregate;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Rooms.Commands.CreateRoom;

/// <summary>
/// 创建会议室命令处理程序
/// </summary>
public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, Result<string>>
{
    private readonly IRoomRepository _roomRepository;
    private readonly ICurrentUserService _currentUserService;
    
    public CreateRoomCommandHandler(IRoomRepository roomRepository, ICurrentUserService currentUserService)
    {
        _roomRepository = roomRepository;
        _currentUserService = currentUserService;
    }
    
    /// <summary>
    /// 处理创建会议室命令
    /// </summary>
    /// <param name="request">命令请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>结果，包含会议室ID</returns>
    public async Task<Result<string>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 检查当前用户是否已认证
            if (!_currentUserService.IsAuthenticated)
            {
                return Result<string>.Failure("未授权的操作");
            }
            
            // 检查会议室名称是否已存在
            if (await _roomRepository.IsNameExistsAsync(request.Name, null, cancellationToken))
            {
                return Result<string>.Failure($"名称为 '{request.Name}' 的会议室已存在");
            }
            
            // 创建会议室
            var room = Room.Create(
                request.Name,
                request.Description ?? string.Empty,
                request.Capacity);
            
            // 设置审计信息
            var currentUserId = _currentUserService.UserId ?? "system";
            room.SetCreated(currentUserId);
            
            // 保存会议室
            await _roomRepository.AddAsync(room, cancellationToken);
            
            return Result<string>.Success(room.Id);
        }
        catch (Exception ex)
        {
            // 处理领域异常
            if (ex is Domain.Exceptions.DomainException domainException)
            {
                return Result<string>.Failure(domainException.Message);
            }
            
            // 处理其他异常
            throw new ApplicationServiceException("创建会议室时发生错误", ex);
        }
    }
}
