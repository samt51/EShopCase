using EShopCase.Application.Helpers;
using EShopCase.Domain.Entities;
using EShopCase.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EShopCase.Infrastructure.Extensions;

public static class HostingExtensions
{
    public static async Task MigrateDevAndSeedAsync<TContext>(
        this IHost host,
        Func<TContext, IServiceProvider, Task>? devSeed = null)
        where TContext : DbContext
    {
        using var scope = host.Services.CreateScope();
        var sp  = scope.ServiceProvider;
        var env = sp.GetRequiredService<IHostEnvironment>();
        if (!env.IsDevelopment()) return;

        var db     = sp.GetRequiredService<TContext>();
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("EF.Migration");

        await db.Database.MigrateAsync();
        if (devSeed is not null) await devSeed(db, sp);
        logger.LogInformation("Development migrate & seed completed.");
    }
        
    public static class DevSeeder
    {
        public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
        {
            if (!await db.Category.AnyAsync(ct))
            {
                await db.Category.AddRangeAsync(
                    new Category(1,"Pantolon"),
                    new Category(2,"T-Shirt"));
                await db.SaveChangesAsync(ct);
            }
            if (!await db.Product.AnyAsync(ct))
            {
                await db.Product.AddRangeAsync(
                    new Products { Id = 1, Name = "Kot",Description = "Kot Pantalon", Price = 12.50m, Stock = 100, CategoryId = 1 },
                    new Products { Id = 2, Name = "Baskılı T-Shirt", Description  = "Baskılı T-Shirt",Price = 14.90m, Stock =  80, CategoryId = 2 });
                await db.SaveChangesAsync(ct);
            }
            if (!await db.Roles.AnyAsync(ct))
            {
                await db.Roles.AddRangeAsync(
                    new Roles { Id = 1, Name = "Admin" },
                    new Roles { Id = 2, Name = "User" });
                await db.SaveChangesAsync(ct);
            }
            if (!await db.Users.AnyAsync(ct))
            {
                await db.Users.AddRangeAsync(
                    new Users { Id = 1, Email = "samt51.m@icloud.com",Password = PasswordHash.HashPassword("123456"),RoleId = 1},
                    new Users { Id = 2, Email = "samt51.m@hotmail.com",Password = PasswordHash.HashPassword("123456"),RoleId = 2});
                await db.SaveChangesAsync(ct);
            }
        }
    }
        
}