using FluentValidation.Results;

namespace MeetingRoom.Application.Common.Exceptions;

/// <summary>
/// 验证异常
/// 当输入验证失败时抛出
/// </summary>
public class ValidationException : ApplicationException
{
    /// <summary>
    /// 验证错误集合
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public ValidationException()
        : base("发生了一个或多个验证错误。")
    {
        Errors = new Dictionary<string, string[]>();
    }
    
    /// <summary>
    /// 从验证失败集合创建异常
    /// </summary>
    /// <param name="failures">验证失败集合</param>
    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }
}
