using System.Net;
using MeetingRoom.Api.Exceptions;
using MeetingRoom.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoom.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = new ApiResponse<object>();

            switch (exception)
            {
                case BusinessException businessException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = ApiResponse<object>.BadRequest(businessException.Message);
                    break;

                case KeyNotFoundException keyNotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = ApiResponse<object>.NotFound(keyNotFoundException.Message);
                    break;

                case DbUpdateException dbUpdateException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var message = dbUpdateException.InnerException?.Message ?? dbUpdateException.Message;
                    if (message.Contains("Duplicate entry"))
                    {
                        var regex = new System.Text.RegularExpressions.Regex(@"Duplicate entry ['""](.+?)['""] for key ['""](.*[_]+.*)['""]");
                        var match = regex.Match(message);
                        if (match.Success)
                        {
                            var duplicateValue = match.Groups[1].Value;
                            var keyName = match.Groups[2].Value;
                            var fieldName = keyName.Replace("IX_", "")
                                                 .Replace("_IsDeleted", "")
                                                 .Split('_')
                                                 .Last();
                            message = $"The value '{duplicateValue}' already exists for field '{fieldName}'.";
                        }
                        else
                        {
                            message = "A record with the same unique identifier already exists.";
                        }
                    }
                    response = ApiResponse<object>.BadRequest(message);
                    break;

                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response = ApiResponse<object>.BadRequest("An internal server error occurred.");
                    break;
            }

            await context.Response.WriteAsJsonAsync(response);
        }
    }

    // Extension method for easy middleware registration
    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
