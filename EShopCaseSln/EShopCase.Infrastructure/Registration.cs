using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.Repositories;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Infrastructure.Concrete.Mapping;
using EShopCase.Infrastructure.Concrete.Repositories;
using EShopCase.Infrastructure.Concrete.UnitOfWorks;
using EShopCase.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EShopCase.Infrastructure;

public static class Registration
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));

        services.AddSingleton<IMapper, Mapper>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();


    }
}