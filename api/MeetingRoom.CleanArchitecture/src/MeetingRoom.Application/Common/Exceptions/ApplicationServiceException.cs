namespace MeetingRoom.Application.Common.Exceptions;

/// <summary>
/// 应用服务异常
/// 表示应用服务层发生的异常
/// </summary>
public class ApplicationServiceException : AppException
{
    public ApplicationServiceException(string message)
        : base(message)
    {
    }
    
    public ApplicationServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
