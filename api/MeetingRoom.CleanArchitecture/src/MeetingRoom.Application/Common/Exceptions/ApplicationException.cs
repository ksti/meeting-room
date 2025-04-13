namespace MeetingRoom.Application.Common.Exceptions;

/// <summary>
/// 应用层异常基类
/// </summary>
public abstract class AppException : Exception
{
    protected AppException(string message)
        : base(message)
    {
    }
    
    protected AppException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
