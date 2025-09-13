using EShopCase.Application.Bases;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EShopCase.Application.Middleware.Exceptions;

public class ExceptionModel
{
    public ResponseDto<ExceptionModel> Response { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
public class LogDetailConsume
{
    private readonly ILogger<ExceptionMiddleware> _logger;
    public LogDetailConsume(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }
}

