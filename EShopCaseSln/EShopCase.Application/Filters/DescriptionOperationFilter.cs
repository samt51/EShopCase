using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace EShopCase.Application.Filters;

public class DescriptionOperationFilter : IOperationFilter
{

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var methodInfo = context.MethodInfo;
        var descriptionAttribute = methodInfo.GetCustomAttribute<SwaggerDescriptionAttirbute>();

        if (descriptionAttribute != null)
        {
            operation.Summary = descriptionAttribute.Summary;
        }
    }
}