namespace MeetingRoom.Api.Exceptions
{
    public class NotFoundException(string message = "Not found") : BusinessException(message)
    {
    }
}
