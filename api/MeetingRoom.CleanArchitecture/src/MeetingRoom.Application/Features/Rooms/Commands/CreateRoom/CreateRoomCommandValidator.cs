using FluentValidation;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Rooms.Commands.CreateRoom;

/// <summary>
/// 创建会议室命令验证器
/// </summary>
public class CreateRoomCommandValidator : AbstractValidator<CreateRoomCommand>
{
    private readonly IRoomRepository _roomRepository;
    
    public CreateRoomCommandValidator(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
        
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("会议室名称不能为空")
            .MaximumLength(100).WithMessage("会议室名称长度不能超过100个字符")
            .MustAsync(BeUniqueName).WithMessage("会议室名称已存在");
        
        RuleFor(v => v.Description)
            .MaximumLength(500).WithMessage("会议室描述长度不能超过500个字符");
        
        RuleFor(v => v.Capacity)
            .GreaterThan(0).WithMessage("会议室容量必须大于0")
            .LessThanOrEqualTo(1000).WithMessage("会议室容量不能超过1000人");
    }
    
    /// <summary>
    /// 验证会议室名称是否唯一
    /// </summary>
    /// <param name="name">会议室名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否唯一</returns>
    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        return !await _roomRepository.IsNameExistsAsync(name, null, cancellationToken);
    }
}
