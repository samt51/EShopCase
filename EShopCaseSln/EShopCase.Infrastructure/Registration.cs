using System.Text;
using EShopCase.Application.Interfaces.Aut.Jwt;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.Repositories;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Infrastructure.Concrete.Auth;
using EShopCase.Infrastructure.Concrete.Mapping;
using EShopCase.Infrastructure.Concrete.Repositories;
using EShopCase.Infrastructure.Concrete.UnitOfWorks;
using EShopCase.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace EShopCase.Infrastructure;

public static class Registration
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.Configure<TokenSettings>(configuration.GetSection("JWT"));
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

        services.AddSingleton<IMapper, Mapper>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        services.AddTransient<ITokenService, TokenService>();
        
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
        {
            opt.SaveToken = true;
            opt.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"])),
                ValidateLifetime = false,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidAudience = configuration["JWT:Audience"],
                ClockSkew = TimeSpan.Zero
            };
        });
        
    }
}