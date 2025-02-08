namespace MeetingRoom.Api.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int Code { get; set; }

        public static ApiResponse<T> Ok(T data, string message = "Success")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Code = 200
            };
        }

        public static ApiResponse<T> Error(string message, int code = 400)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Code = code
            };
        }
    }
}
