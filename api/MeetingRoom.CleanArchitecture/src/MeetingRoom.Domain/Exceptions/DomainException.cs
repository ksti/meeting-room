namespace MeetingRoom.Domain.Exceptions;

/// <summary>
/// 领域异常基类
/// 用于表示领域中的业务规则违反
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
    
    protected DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
