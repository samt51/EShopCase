using System.Diagnostics;
using System.Threading.RateLimiting;
using EShopCase.Application;
using EShopCase.Application.Filters;
using EShopCase.Application.Middleware.Exceptions;
using EShopCase.Infrastructure;
using EShopCase.Infrastructure.Context;
using EShopCase.Infrastructure.Extensions;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;
using NpgsqlTypes;
using Asp.Versioning;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using EShopCase.Api;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;


var builder = WebApplication.CreateBuilder(args);


builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var columns = new Dictionary<string, ColumnWriterBase>
{
    ["timestamp"]        = new TimestampColumnWriter(),
    ["level"]            = new LevelColumnWriter(renderAsText: true, NpgsqlDbType.Varchar),
    ["message"]          = new RenderedMessageColumnWriter(),
    ["message_template"] = new MessageTemplateColumnWriter(),
    ["exception"]        = new ExceptionColumnWriter(),
    ["properties"]       = new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb),


    ["source_context"]   = new SinglePropertyColumnWriter("SourceContext", PropertyWriteMethod.ToString, NpgsqlDbType.Text),
    ["trace_id"]         = new SinglePropertyColumnWriter("TraceId",      PropertyWriteMethod.ToString, NpgsqlDbType.Text),
    ["user_id"]          = new SinglePropertyColumnWriter("UserId",       PropertyWriteMethod.ToString, NpgsqlDbType.Text),
};


builder.Host.UseSerilog((ctx, sp, cfg) =>
{
    cfg.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
        .WriteTo.Console()
        .WriteTo.File("logs/log.txt");

    var logsCs = ctx.Configuration.GetConnectionString("LogsDb");
    if (!string.IsNullOrWhiteSpace(logsCs))
    {
        cfg.WriteTo.Async(a => a.PostgreSQL(
            connectionString: logsCs,
            tableName: "logs",
            columnOptions: columns,
            needAutoCreateTable: true
        ));
    }
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy<string>("RoleBased", httpContext =>
    {
        var user = httpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            return RateLimitPartition.GetTokenBucketLimiter($"anon:{ip}", _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 30,
                TokensPerPeriod = 30,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
        }

        if (user.IsInRole("Admin"))
            return RateLimitPartition.GetNoLimiter("admins");

        var userId = user.FindFirst("sub")?.Value
                     ?? user.FindFirst("userid")?.Value
                     ?? user.Identity!.Name
                     ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter($"user:{userId}", _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 60,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
        });
    });
});

builder.Services.AddHealthChecks()

    .AddCheck("self", () => HealthCheckResult.Healthy("OK"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<DescriptionOperationFilter>();

   
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
    });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.CustomSchemaIds(t => t.FullName);

});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
});

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version")
        );
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";   // v1, v1.0
        options.SubstituteApiVersionInUrl = true;
    });


builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();


var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,   
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    },
    ResponseWriter = async (ctx, report) =>
    {
        var status = report.Status.ToString(); 
        ctx.Response.ContentType = "text/plain; charset=utf-8";
        await ctx.Response.WriteAsync(status);
    }
});

await app.MigrateDevAndSeedAsync<AppDbContext>(async (db, sp) =>
{
    await HostingExtensions.DevSeeder.SeedAsync(db);
});


app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (diag, http) =>
    {
        var traceId = Activity.Current?.Id ?? http.TraceIdentifier;
        diag.Set("TraceId", traceId);
        var userId = http.User?.FindFirst("Id")?.Value;
        if (!string.IsNullOrWhiteSpace(userId)) diag.Set("UserId", userId);
    };
});



if (app.Environment.IsDevelopment())
{app.UseDeveloperExceptionPage(); 
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var desc in provider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json",
                desc.GroupName.ToUpperInvariant());
        }
    });

}
app.UseRateLimiter();
app.UseHttpsRedirection();
app.ConfigureExceptionHandlingMiddleware();
app.UseAuthentication();
app.UseAuthorization();
app.Use(async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identities.Select(x => x.FindFirst("Id"))?.FirstOrDefault() : null;
    if (username is not null)
    {
        LogContext.PushProperty("UserId", username.Value.ToString());
    }
    await next();
});
app.MapControllers();
app.Run();

