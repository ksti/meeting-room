using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Domain.Aggregates.MeetingAggregate;
using MeetingRoom.Domain.Enums;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Meetings.Queries.GetMeetingsList;

/// <summary>
/// 获取会议列表查询处理程序
/// </summary>
public class GetMeetingsListQueryHandler : IRequestHandler<GetMeetingsListQuery, Result<PaginatedList<MeetingDto>>>
{
    private readonly IMeetingRepository _meetingRepository;
    private readonly IMapper _mapper;
    
    public GetMeetingsListQueryHandler(IMeetingRepository meetingRepository, IMapper mapper)
    {
        _meetingRepository = meetingRepository;
        _mapper = mapper;
    }
    
    /// <summary>
    /// 处理获取会议列表查询
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>结果，包含分页会议列表</returns>
    public async Task<Result<PaginatedList<MeetingDto>>> Handle(GetMeetingsListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 构建查询条件
            Expression<Func<Meeting, bool>>? predicate = null;
            
            // 搜索关键字
            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                var keyword = request.SearchKeyword.Trim().ToLower();
                predicate = meeting =>
                    meeting.Title.ToLower().Contains(keyword) ||
                    meeting.Description.ToLower().Contains(keyword);
            }
            
            // 会议室ID过滤
            if (!string.IsNullOrWhiteSpace(request.RoomId))
            {
                Expression<Func<Meeting, bool>> roomPredicate = meeting => meeting.RoomId == request.RoomId;
                predicate = predicate == null
                    ? roomPredicate
                    : CombineExpressions(predicate, roomPredicate);
            }
            
            // 组织者ID过滤
            if (!string.IsNullOrWhiteSpace(request.OrganizerId))
            {
                Expression<Func<Meeting, bool>> organizerPredicate = meeting => meeting.OrganizerId == request.OrganizerId;
                predicate = predicate == null
                    ? organizerPredicate
                    : CombineExpressions(predicate, organizerPredicate);
            }
            
            // 参与者ID过滤
            if (!string.IsNullOrWhiteSpace(request.ParticipantId))
            {
                Expression<Func<Meeting, bool>> participantPredicate = meeting => 
                    meeting.ParticipantIds.Contains(request.ParticipantId);
                predicate = predicate == null
                    ? participantPredicate
                    : CombineExpressions(predicate, participantPredicate);
            }
            
            // 日期范围过滤
            if (request.StartDate.HasValue)
            {
                var startDate = request.StartDate.Value.Date;
                Expression<Func<Meeting, bool>> startDatePredicate = meeting => meeting.TimeRange.End >= startDate;
                predicate = predicate == null
                    ? startDatePredicate
                    : CombineExpressions(predicate, startDatePredicate);
            }
            
            if (request.EndDate.HasValue)
            {
                var endDate = request.EndDate.Value.Date.AddDays(1); // 包含结束日期当天
                Expression<Func<Meeting, bool>> endDatePredicate = meeting => meeting.TimeRange.Start < endDate;
                predicate = predicate == null
                    ? endDatePredicate
                    : CombineExpressions(predicate, endDatePredicate);
            }
            
            // 状态过滤
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                var status = EnumExtensions.GetEnumFromDisplayName<MeetingStatus>(request.Status);
                Expression<Func<Meeting, bool>> statusPredicate = meeting => meeting.Status == status;
                predicate = predicate == null
                    ? statusPredicate
                    : CombineExpressions(predicate, statusPredicate);
            }
            
            // 获取会议列表
            var meetings = await _meetingRepository.GetListAsync(predicate, cancellationToken);
            
            // 分页
            var totalCount = meetings.Count;
            var pagedMeetings = meetings
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            // 映射为DTO
            var meetingDtos = _mapper.Map<List<MeetingDto>>(pagedMeetings);
            
            // 创建分页结果
            var paginatedList = new PaginatedList<MeetingDto>(
                meetingDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);
            
            return Result<PaginatedList<MeetingDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<MeetingDto>>.Failure($"获取会议列表失败: {ex.Message}");
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
