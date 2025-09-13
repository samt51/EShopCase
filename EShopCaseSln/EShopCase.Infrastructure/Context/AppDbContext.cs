using EShopCase.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShopCase.Infrastructure.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder b)
    {
        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public DbSet<Products> Product { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem>OrderItems { get; set; }
    public DbSet<Roles> Roles { get; set; }
    public DbSet<Users> Users { get; set; }
}