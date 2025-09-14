using System.Reflection;
using EShopCase.Application.Middleware.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EShopCase.Application;

public static class Registration
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddTransient<ExceptionMiddleware>();
            
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        
        
        return services;

    }
        
}