using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;

namespace MeetingRoom.Api.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
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

        public static ApiResponse<T> BadRequest(string message = "Bad request", int code = 400)
        {
            return Error(message, code);
        }

        public static ApiResponse<T> BadRequest(ModelStateDictionary modelState, int code = 400)
        {
            var errors = string.Join("; ", modelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage));
            return BadRequest(errors);
        }

        public static ApiResponse<T> NotFound(string message = "Resource not found")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Code = (int)HttpStatusCode.NotFound
            };
        }

        public static ApiResponse<T> Unauthorized(string message = "Unauthorized access")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Code = (int)HttpStatusCode.Unauthorized
            };
        }

        public static ApiResponse<T> InternalError(string message = "An internal server error occurred")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Code = (int)HttpStatusCode.InternalServerError
            };
        }
    }
}
