using EShopCase.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EShopCase.Test.Context;

public class TestShopDbContext : DbContext
{
    public TestShopDbContext(DbContextOptions<TestShopDbContext> opt) : base(opt) { }

    public DbSet<Products> Products => Set<Products>();
    public DbSet<Category> Categories => Set<Category>();
}