using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.Common.Exceptions;
using MeetingRoom.Application.Interfaces;
using MeetingRoom.Domain.Aggregates.MeetingAggregate;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Meetings.Commands.CreateMeeting;

/// <summary>
/// 创建会议命令处理程序
/// </summary>
public class CreateMeetingCommandHandler : IRequestHandler<CreateMeetingCommand, Result<string>>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;
    
    public CreateMeetingCommandHandler(
        IMeetingRepository meetingRepository,
        IRoomRepository roomRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _meetingRepository = meetingRepository;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }
    
    /// <summary>
    /// 处理创建会议命令
    /// </summary>
    /// <param name="request">命令请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>结果，包含会议ID</returns>
    public async Task<Result<string>> Handle(CreateMeetingCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // 检查当前用户是否已认证
            if (!_currentUserService.IsAuthenticated)
            {
                return Result<string>.Failure("未授权的操作");
            }
            
            // 获取当前用户ID
            var currentUserId = _currentUserService.UserId;
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Result<string>.Failure("无法获取当前用户信息");
            }
            
            // 检查会议室是否存在
            var room = await _roomRepository.GetByIdAsync(request.RoomId, cancellationToken);
            if (room == null)
            {
                return Result<string>.Failure($"ID为 {request.RoomId} 的会议室不存在");
            }
            
            // 检查会议室是否可用
            var isRoomAvailable = await _meetingRepository.IsRoomAvailableAsync(
                request.RoomId,
                request.StartTime,
                request.EndTime,
                null,
                cancellationToken);
                
            if (!isRoomAvailable)
            {
                return Result<string>.Failure("所选时间段内会议室已被预订");
            }
            
            // 创建会议
            var meeting = Meeting.Create(
                request.Title,
                request.Description,
                request.Capacity,
                request.StartTime,
                request.EndTime,
                currentUserId,
                request.RoomId);
            
            // 设置审计信息
            meeting.SetCreated(currentUserId);
            
            // 添加参与者
            foreach (var participantId in request.ParticipantIds.Distinct())
            {
                // 检查参与者是否存在
                var participant = await _userRepository.GetByIdAsync(participantId, cancellationToken);
                if (participant == null)
                {
                    return Result<string>.Failure($"ID为 {participantId} 的参与者不存在");
                }
                
                // 添加参与者
                meeting.AddParticipant(participantId);
            }
            
            // 保存会议
            await _meetingRepository.AddAsync(meeting, cancellationToken);
            
            return Result<string>.Success(meeting.Id);
        }
        catch (Exception ex)
        {
            // 处理领域异常
            if (ex is Domain.Exceptions.DomainException domainException)
            {
                return Result<string>.Failure(domainException.Message);
            }
            
            // 处理其他异常
            throw new ApplicationServiceException("创建会议时发生错误", ex);
        }
    }
}
