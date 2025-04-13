using AutoMapper;
using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.Common.Exceptions;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Users.Queries.GetUserById;

/// <summary>
/// 根据ID获取用户查询处理程序
/// </summary>
public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    
    public GetUserByIdQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    /// <summary>
    /// 处理根据ID获取用户查询
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>结果，包含用户DTO</returns>
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (user == null)
        {
            return Result<UserDto>.Failure($"ID为 {request.Id} 的用户不存在");
        }
        
        var userDto = _mapper.Map<UserDto>(user);
        
        return Result<UserDto>.Success(userDto);
    }
}
