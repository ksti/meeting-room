using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MeetingRoom.Application.Common.Behaviors;

namespace MeetingRoom.Application;

/// <summary>
/// 应用层依赖注入配置
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// 添加应用层服务
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 添加MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });
        
        // 添加AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        
        // 添加FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        return services;
    }
}
