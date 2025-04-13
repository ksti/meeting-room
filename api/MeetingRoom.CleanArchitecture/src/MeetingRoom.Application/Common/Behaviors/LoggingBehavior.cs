using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace MeetingRoom.Application.Common.Behaviors;

/// <summary>
/// 日志行为
/// 用于记录命令/查询的执行情况
/// </summary>
/// <typeparam name="TRequest">请求类型</typeparam>
/// <typeparam name="TResponse">响应类型</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestId = Guid.NewGuid().ToString();
        
        _logger.LogInformation("开始处理请求 {RequestName} {RequestId}", requestName, requestId);
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            
            stopwatch.Stop();
            _logger.LogInformation("成功处理请求 {RequestName} {RequestId} - 耗时: {ElapsedMilliseconds}ms",
                requestName, requestId, stopwatch.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "处理请求 {RequestName} {RequestId} 时发生错误 - 耗时: {ElapsedMilliseconds}ms",
                requestName, requestId, stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}
