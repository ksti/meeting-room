using System.Linq.Expressions;
using AutoMapper;
using MediatR;
using MeetingRoom.Application.Common;
using MeetingRoom.Application.DTOs;
using MeetingRoom.Domain.Aggregates.UserAggregate;
using MeetingRoom.Domain.Enums;
using MeetingRoom.Domain.Interfaces.Repositories;

namespace MeetingRoom.Application.Features.Users.Queries.GetUsersList;

/// <summary>
/// 获取用户列表查询处理程序
/// </summary>
public class GetUsersListQueryHandler : IRequestHandler<GetUsersListQuery, Result<PaginatedList<UserDto>>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    
    public GetUsersListQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    /// <summary>
    /// 处理获取用户列表查询
    /// </summary>
    /// <param name="request">查询请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>结果，包含分页用户列表</returns>
    public async Task<Result<PaginatedList<UserDto>>> Handle(GetUsersListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // 构建查询条件
            Expression<Func<User, bool>>? predicate = null;
            
            // 搜索关键字
            if (!string.IsNullOrWhiteSpace(request.SearchKeyword))
            {
                var keyword = request.SearchKeyword.Trim().ToLower();
                predicate = user =>
                    user.Username.ToLower().Contains(keyword) ||
                    user.Email.Value.ToLower().Contains(keyword) ||
                    (user.Name != null && (
                        user.Name.FirstName.ToLower().Contains(keyword) ||
                        user.Name.LastName.ToLower().Contains(keyword)
                    ));
            }
            
            // 角色过滤
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                var role = EnumExtensions.GetEnumFromDisplayName<UserRole>(request.Role);
                Expression<Func<User, bool>> rolePredicate = user => user.Role == role;
                predicate = predicate == null
                    ? rolePredicate
                    : predicate.And(rolePredicate);
            }
            
            // 状态过滤
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                var status = EnumExtensions.GetEnumFromDisplayName<UserStatus>(request.Status);
                Expression<Func<User, bool>> statusPredicate = user => user.Status == status;
                predicate = predicate == null
                    ? statusPredicate
                    : predicate.And(statusPredicate);
            }
            
            // 获取用户列表
            var users = await _userRepository.GetListAsync(predicate, cancellationToken);
            
            // 分页
            var totalCount = users.Count;
            var pagedUsers = users
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();
            
            // 映射为DTO
            var userDtos = _mapper.Map<List<UserDto>>(pagedUsers);
            
            // 创建分页结果
            var paginatedList = new PaginatedList<UserDto>(
                userDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);
            
            return Result<PaginatedList<UserDto>>.Success(paginatedList);
        }
        catch (Exception ex)
        {
            return Result<PaginatedList<UserDto>>.Failure($"获取用户列表失败: {ex.Message}");
        }
    }
}

/// <summary>
/// 表达式扩展方法
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// 组合两个表达式，使用AND操作符
    /// </summary>
    /// <typeparam name="T">表达式参数类型</typeparam>
    /// <param name="expr1">表达式1</param>
    /// <param name="expr2">表达式2</param>
    /// <returns>组合后的表达式</returns>
    public static Expression<Func<T, bool>> And<T>(
        this Expression<Func<T, bool>> expr1,
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
