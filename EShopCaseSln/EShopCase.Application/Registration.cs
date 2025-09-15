using System.Globalization;
using System.Reflection;
using EShopCase.Application.Bases;
using EShopCase.Application.Filters;
using EShopCase.Application.Middleware.Exceptions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EShopCase.Application;

public static class Registration
{
    
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddTransient<ExceptionMiddleware>();
        
        services.AddValidatorsFromAssembly(assembly);

            
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });
        
        services.AddMvc(x =>
            {
                x.Filters.Add(typeof(ValidatorActionFilter));
            }).AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<ObjectResult>())
            .AddSessionStateTempDataProvider();
        
        var supportedCultures = new[]
        {
            new CultureInfo("tr-TR"),
            new CultureInfo("en-US")
        };
        
       services.AddLocalization(opt => opt.ResourcesPath = "Resources");

       services.Configure<RequestLocalizationOptions>(opt =>
       {
           opt.DefaultRequestCulture = new RequestCulture("tr-TR"); // varsayılan
           opt.SupportedCultures = supportedCultures;
           opt.SupportedUICultures = supportedCultures;

           
           opt.RequestCultureProviders = new IRequestCultureProvider[]
           {
               new QueryStringRequestCultureProvider(),  
               new CookieRequestCultureProvider(),       
               new AcceptLanguageHeaderRequestCultureProvider()
           };
       });
        return services;

    }
        
}