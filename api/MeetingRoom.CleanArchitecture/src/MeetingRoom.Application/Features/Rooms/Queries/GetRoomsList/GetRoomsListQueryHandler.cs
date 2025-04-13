using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Domain.Aggregates.RoomAggregate;
using MeetingRoom.Domain.Enums;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Rooms.Queries.GetRoomsList;

/// <summary>
/// 获取会议室列表查询处理程序
/// </summary>
public class GetRoomsListQueryHandler : IRequestHandler<GetRoomsListQuery, Result<PaginatedList<RoomDto>>>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IMapper _mapper;
    
    public GetRoomsListQueryHandler(IRoomRepository roomRepository, IMapper mapper)
    {
        _roomRepository = roomRepository;
        _mapper = mapper;
    }
    
    /// <summary>
    /// 处理获取会议室列表查询
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>结果，包含分页会议室列表</returns>
    public async Task<Result<PaginatedList<RoomDto>>> Handle(GetRoomsListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 构建查询条件
            Expression<Func<Room, bool>>? predicate = null;
            
            // 搜索关键字
            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                var keyword = request.SearchKeyword.Trim().ToLower();
                predicate = room =>
                    room.Name.ToLower().Contains(keyword) ||
                    room.Description.ToLower().Contains(keyword);
            }
            
            // 状态过滤
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                var status = EnumExtensions.GetEnumFromDisplayName<RoomStatus>(request.Status);
                Expression<Func<Room, bool>> statusPredicate = room => room.Status == status;
                predicate = predicate == null
                    ? statusPredicate
                    : CombineExpressions(predicate, statusPredicate);
            }
            
            // 容量过滤
            if (request.MinCapacity.HasValue)
            {
                Expression<Func<Room, bool>> minCapacityPredicate = room => room.Capacity >= request.MinCapacity.Value;
                predicate = predicate == null
                    ? minCapacityPredicate
                    : CombineExpressions(predicate, minCapacityPredicate);
            }
            
            if (request.MaxCapacity.HasValue)
            {
                Expression<Func<Room, bool>> maxCapacityPredicate = room => room.Capacity <= request.MaxCapacity.Value;
                predicate = predicate == null
                    ? maxCapacityPredicate
                    : CombineExpressions(predicate, maxCapacityPredicate);
            }
            
            // 只显示可用的会议室
            if (request.OnlyAvailable == true)
            {
                Expression<Func<Room, bool>> availablePredicate = room => room.Status == RoomStatus.Idle;
                predicate = predicate == null
                    ? availablePredicate
                    : CombineExpressions(predicate, availablePredicate);
            }
            
            // 获取会议室列表
            var rooms = await _roomRepository.GetListAsync(predicate, cancellationToken);
            
            // 分页
            var totalCount = rooms.Count;
            var pagedRooms = rooms
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            // 映射为DTO
            var roomDtos = _mapper.Map<List<RoomDto>>(pagedRooms);
            
            // 创建分页结果
            var paginatedList = new PaginatedList<RoomDto>(
                roomDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);
            
            return Result<PaginatedList<RoomDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<RoomDto>>.Failure($"获取会议室列表失败: {ex.Message}");
        }
    }
    
    /// <summary>
    /// 组合两个表达式，使用AND操作符
    /// </summary>
    /// <typeparam name="T">表达式参数类型</typeparam>
    /// <param name="expr1">表达式1</param>
    /// <param name="expr2">表达式2</param>
    /// <returns>组合后的表达式</returns>
    private Expression<Func<T, bool>> CombineExpressions<T>(
        Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));
        
        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);
        
        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);
        
        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(left!, right!),
            parameter);
    }
    
    /// <summary>
    /// 表达式参数替换访问器
    /// </summary>
    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;
        
        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }
        
        public override Expression? Visit(Expression? node)
        {
            if (node == _oldValue)
            {
                return _newValue;
            }
            
            return base.Visit(node);
        }
    }
}
