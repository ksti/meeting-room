using AutoMapper;
using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Rooms.Queries.GetRoomById;

/// <summary>
/// 根据ID获取会议室查询处理程序
/// </summary>
public class GetRoomByIdQueryHandler : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;
    
    public GetRoomByIdQueryHandler(IRoomRepository roomRepository, IMapper mapper)
    {
        _roomRepository = roomRepository;
        _mapper = mapper;
    }
    
    /// <summary>
    /// 处理根据ID获取会议室查询
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>结果，包含会议室DTO</returns>
    public async Task<Result<RoomDto>> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (room == null)
        {
            return Result<RoomDto>.Failure($"ID为 {request.Id} 的会议室不存在");
        }
        
        var roomDto = _mapper.Map<RoomDto>(room);
        
        return Result<RoomDto>.Success(roomDto);
    }
}
