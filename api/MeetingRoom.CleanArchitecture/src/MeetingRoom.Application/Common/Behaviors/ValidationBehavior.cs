using FluentValidation;
using MediatR;

namespace MeetingRoom.Application.Common.Behaviors;

/// <summary>
/// 验证行为
/// 用于在处理命令/查询前进行验证
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }
        
        // 创建验证上下文
        var context = new ValidationContext<TRequest>(request);
        
        // 执行所有验证器
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        
        // 合并验证错误
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();
        
        // 如果有验证错误，抛出异常
        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }
        
        // 验证通过，继续处理请求
        return await next();
    }
}
