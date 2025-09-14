using EShopCase.Application.Features.ProductsFeature.Queries.GetAllProduct;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using EShopCase.Test.Context;
using Microsoft.EntityFrameworkCore;
using Moq;
using FluentAssertions;

namespace EShopCase.Test.Tests.ProductTest;

 public class GetAllProductQueryHandlerTests
    {
        private static (TestShopDbContext ctx, Mock<IUnitOfWork> uow, Mock<IMapper> mapper) SetupInMemory()
        {
            var opts = new DbContextOptionsBuilder<TestShopDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var ctx = new TestShopDbContext(opts);

            var cat1 = new Category { Id = 1, Name = "Fruit" };
            var cat2 = new Category { Id = 2, Name = "Electro" };
            var cat3 = new Category { Id = 3, Name = "Furniture" };

            ctx.Categories.AddRange(cat1, cat2, cat3);
            ctx.Products.AddRange(
                new Products { Id=1,  Name="Apple",   Price=10,  Stock=5,  IsDeleted=false, CategoryId=1, Category=cat1, CreatedDate=DateTime.UtcNow.AddDays(-10)},
                new Products { Id=2,  Name="Banana",  Price=5,   Stock=7,  IsDeleted=false, CategoryId=1, Category=cat1, CreatedDate=DateTime.UtcNow.AddDays(-9)},
                new Products { Id=3,  Name="TV",      Price=500, Stock=2,  IsDeleted=false, CategoryId=2, Category=cat2, CreatedDate=DateTime.UtcNow.AddDays(-8)},
                new Products { Id=4,  Name="Table",   Price=120, Stock=0,  IsDeleted=false, CategoryId=3, Category=cat3, CreatedDate=DateTime.UtcNow.AddDays(-7)},
                new Products { Id=5,  Name="Desk",    Price=90,  Stock=12, IsDeleted=false, CategoryId=3, Category=cat3, CreatedDate=DateTime.UtcNow.AddDays(-6)},
                new Products { Id=6,  Name="Mouse",   Price=25,  Stock=30, IsDeleted=true,  CategoryId=2, Category=cat2, CreatedDate=DateTime.UtcNow.AddDays(-5)}, // deleted
                new Products { Id=7,  Name="Avocado", Price=15,  Stock=3,  IsDeleted=false, CategoryId=1, Category=cat1, CreatedDate=DateTime.UtcNow.AddDays(-4)},
                new Products { Id=8,  Name="Sofa",    Price=700, Stock=1,  IsDeleted=false, CategoryId=3, Category=cat3, CreatedDate=DateTime.UtcNow.AddDays(-3)},
                new Products { Id=9,  Name="Phone",   Price=800, Stock=9,  IsDeleted=false, CategoryId=2, Category=cat2, CreatedDate=DateTime.UtcNow.AddDays(-2)},
                new Products { Id=10, Name="Pear",    Price=8,   Stock=4,  IsDeleted=false, CategoryId=1, Category=cat1, CreatedDate=DateTime.UtcNow.AddDays(-1)}
            );
            ctx.SaveChanges();

            var uow = new Mock<IUnitOfWork>();
            uow.Setup(u => u.GetReadRepository<Products>())
               .Returns(new FakeReadRepository<Products>(ctx)); // kendi FakeReadRepository’n

            var mapper = new Mock<IMapper>();
            mapper.Setup(m => m.Map<GetAllProductQueryResponse, Products>(It.IsAny<IQueryable<Products>>()))
                  .Returns(new System.Collections.Generic.List<GetAllProductQueryResponse>());

            return (ctx, uow, mapper);
        }

        [Fact]
        public async Task Filters_Deleted_And_Paginates_Defaults()
        {
            var (ctx, uow, mapper) = SetupInMemory();
            var sut = new GetAllProductQueryHandler(mapper.Object, uow.Object);

            var req = new GetAllProductQueryRequest { Page = 1, PageSize = 20 };
            var resp = await sut.Handle(req, CancellationToken.None);

            resp.IsSuccess.Should().BeTrue();
            resp.Data!.TotalCount.Should().Be(9);
            resp.Data.Items.Should().HaveCount(9);
            resp.Data.Items.Any(x => x.Id == 6).Should().BeFalse(); // deleted yok
        }

        [Fact]
        public async Task Applies_Category_And_Price_Filters()
        {
            var (ctx, uow, mapper) = SetupInMemory();
            var sut = new GetAllProductQueryHandler(mapper.Object, uow.Object);

            var req = new GetAllProductQueryRequest { CategoryId = 3, PriceMin = 100, PriceMax = 650 };
            var resp = await sut.Handle(req, CancellationToken.None);

            resp.IsSuccess.Should().BeTrue();
            resp.Data!.TotalCount.Should().Be(1);
            resp.Data.Items.Single().Name.Should().Be("Table");
        }

        [Fact]
        public async Task Sorts_By_Price_Desc_And_Takes_First3()
        {
            var (ctx, uow, mapper) = SetupInMemory();
            var sut = new GetAllProductQueryHandler(mapper.Object, uow.Object);

            var req = new GetAllProductQueryRequest { SortBy = "price", SortDir = "desc", Page = 1, PageSize = 3 };
            var resp = await sut.Handle(req, CancellationToken.None);

            resp.IsSuccess.Should().BeTrue();
            resp.Data!.Items.Select(i => i.Name).Should().ContainInOrder("Phone", "Sofa", "TV");
        }
    }
