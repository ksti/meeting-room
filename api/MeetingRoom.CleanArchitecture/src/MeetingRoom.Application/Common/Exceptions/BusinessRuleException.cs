namespace MeetingRoom.Application.Common.Exceptions;

/// <summary>
/// 业务规则异常
/// 当业务规则被违反时抛出
/// </summary>
public class BusinessRuleException : ApplicationException
{
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    public BusinessRuleException(string message)
        : base(message)
    {
    }
    
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="message">异常消息</param>
    /// <param name="innerException">内部异常</param>
    public BusinessRuleException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
