namespace MeetingRoom.Api.Exceptions
{
    public class UnauthorizedException(string message) : BusinessException(message)
    {
    }
}
