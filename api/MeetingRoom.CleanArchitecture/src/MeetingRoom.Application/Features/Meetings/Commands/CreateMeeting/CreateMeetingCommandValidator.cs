using FluentValidation;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Meetings.Commands.CreateMeeting;

/// <summary>
/// 创建会议命令验证器
/// </summary>
public class CreateMeetingCommandValidator : AbstractValidator<CreateMeetingCommand>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IMeetingRepository _meetingRepository;
    private readonly IUserRepository _userRepository;
    
    public CreateMeetingCommandValidator(
        IRoomRepository roomRepository,
        IMeetingRepository meetingRepository,
        IUserRepository userRepository)
    {
        _roomRepository = roomRepository;
        _meetingRepository = meetingRepository;
        _userRepository = userRepository;
        
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("会议标题不能为空")
            .MaximumLength(100).WithMessage("会议标题长度不能超过100个字符");
        
        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("会议描述长度不能超过500个字符");
        
        RuleFor(v => v.RoomId)
            .NotEmpty().WithMessage("会议室ID不能为空")
            .MustAsync(RoomExistsAsync).WithMessage("指定的会议室不存在");
        
        RuleFor(v => v.StartTime)
            .NotEmpty().WithMessage("开始时间不能为空")
            .Must(BeValidStartTime).WithMessage("开始时间必须大于当前时间");
        
        RuleFor(v => v.EndTime)
            .NotEmpty().WithMessage("结束时间不能为空")
            .GreaterThan(v => v.StartTime).WithMessage("结束时间必须大于开始时间");
        
        RuleFor(v => v)
            .MustAsync(BeRoomAvailableAsync).WithMessage("所选时间段内会议室已被预订");
        
        RuleForEach(v => v.ParticipantIds)
            .MustAsync(UserExistsAsync).WithMessage("参与者不存在");
    }
    
    /// <summary>
    /// 验证会议室是否存在
    /// </summary>
    /// <param name="roomId">会议室ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    private async Task<bool> RoomExistsAsync(string roomId, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(roomId, cancellationToken);
        return room != null;
    }
    
    /// <summary>
    /// 验证开始时间是否有效
    /// </summary>
    /// <param name="startTime">开始时间</param>
    /// <returns>是否有效</returns>
    private bool BeValidStartTime(DateTime startTime)
    {
        return startTime > DateTime.UtcNow;
    }
    
    /// <summary>
    /// 验证会议室在指定时间段内是否可用
    /// </summary>
    /// <param name="command">创建会议命令</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否可用</returns>
    private async Task<bool> BeRoomAvailableAsync(CreateMeetingCommand command, CancellationToken cancellationToken)
    {
        return await _meetingRepository.IsRoomAvailableAsync(
            command.RoomId,
            command.StartTime,
            command.EndTime,
            null,
            cancellationToken);
    }
    
    /// <summary>
    /// 验证用户是否存在
    /// </summary>
    /// <param name="userId">用户ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    private async Task<bool> UserExistsAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        return user != null;
    }
}
