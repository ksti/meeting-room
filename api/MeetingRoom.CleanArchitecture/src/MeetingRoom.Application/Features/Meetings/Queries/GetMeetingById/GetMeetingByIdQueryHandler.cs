using AutoMapper;
using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Meetings.Queries.GetMeetingById;

/// <summary>
/// 根据ID获取会议查询处理程序
/// </summary>
public class GetMeetingByIdQueryHandler : IRequestHandler<GetMeetingByIdQuery, Result<MeetingDetailDto>>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    
    public GetMeetingByIdQueryHandler(
        IMeetingRepository meetingRepository,
        IRoomRepository roomRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _meetingRepository = meetingRepository;
        _roomRepository = roomRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    /// <summary>
    /// 处理根据ID获取会议查询
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>结果，包含会议详情DTO</returns>
    public async Task<Result<MeetingDetailDto>> Handle(GetMeetingByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 获取会议
            var meeting = await _meetingRepository.GetByIdWithParticipantsAsync(request.Id, cancellationToken);
            
            if (meeting == null)
            {
                return Result<MeetingDetailDto>.Failure($"ID为 {request.Id} 的会议不存在");
            }
            
            // 获取会议室
            var room = await _roomRepository.GetByIdAsync(meeting.RoomId, cancellationToken);
            
            // 获取组织者
            var organizer = await _userRepository.GetByIdAsync(meeting.OrganizerId, cancellationToken);
            
            // 创建会议详情DTO
            var meetingDetailDto = _mapper.Map<MeetingDetailDto>(meeting);
            
            // 设置会议室信息
            if (room != null)
            {
                meetingDetailDto.Room = _mapper.Map<RoomDto>(room);
            }
            
            // 设置组织者信息
            if (organizer != null)
            {
                meetingDetailDto.Organizer = _mapper.Map<UserDto>(organizer);
            }
            
            // 设置参与者信息
            var participantIds = meeting.ParticipantIds.ToList();
            var participants = await _userRepository.GetByIdsAsync(participantIds, cancellationToken);
            
            meetingDetailDto.Participants = _mapper.Map<List<UserDto>>(participants);
            
            return Result<MeetingDetailDto>.Success(meetingDetailDto);
        }
        catch (Exception ex)
        {
            return Result<MeetingDetailDto>.Failure($"获取会议详情失败: {ex.Message}");
        }
    }
}
