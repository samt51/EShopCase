using System.Linq.Expressions;
using EShopCase.Application.Dtos.CategoryDtos;
using EShopCase.Application.Features.ProductsFeature.Queries.GetByIdProduct;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.Repositories;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace EShopCase.Test.Tests.ProductTest;

public class GetByIdProductQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_Return_Mapped_Product_When_Exists()
    {
 
        var mapper   = new Mock<IMapper>();
        var uow      = new Mock<IUnitOfWork>();
        var readRepo = new Mock<IReadRepository<Products>>();

        var req = new GetByIdProductQueryRequest(id: 9);


        var entity = new Products
        {
            Id = req.Id,
            Name = "Phone",
            Description = "desc",
            Price = 800m,
            Stock = 9,
            IsDeleted = true,           
            CategoryId = 2,
            Category = new Category { Id = 2, Name = "Electro" }
        };

        readRepo.Setup(r => r.GetAsync(
                It.Is<Expression<Func<Products, bool>>>(expr =>
                    expr.Compile().Invoke(new Products { Id = req.Id, IsDeleted = true })
                ),
                It.IsAny<Func<IQueryable<Products>, IIncludableQueryable<Products, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync(entity);

        uow.Setup(u => u.GetReadRepository<Products>()).Returns(readRepo.Object);

        var mapped = new GetByIdProductQueryResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Stock = entity.Stock,
            CategoryResponse = new CategoryResponseDto
            {
                Id = entity.Category.Id,
                Name = entity.Category.Name,
            }
        };

        mapper.Setup(m => m.Map<GetByIdProductQueryResponse, Products>(It.IsAny<Products>()))
              .Returns(mapped);

        var sut = new GetByIdProductQueryHandler(mapper.Object, uow.Object);

       
        var resp = await sut.Handle(req, CancellationToken.None);

   
        resp.IsSuccess.Should().BeTrue();
        resp.Data.Should().NotBeNull();
        resp.Data.Id.Should().Be(req.Id);
        resp.Data.Name.Should().Be("Phone");
        resp.Data.CategoryResponse!.Name.Should().Be("Electro");
        resp.Data.CategoryResponse.Id.Should().Be(2); 
        readRepo.Verify(r => r.GetAsync(
            It.IsAny<Expression<Func<Products, bool>>>(),
            It.IsNotNull<Func<IQueryable<Products>, IIncludableQueryable<Products, object>>>(),
            It.IsAny<bool>()),
            Times.Once);

        mapper.Verify(m => m.Map<GetByIdProductQueryResponse, Products>(entity), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_With_Null_When_NotFound()
    {
        
        var mapper   = new Mock<IMapper>();
        var uow      = new Mock<IUnitOfWork>();
        var readRepo = new Mock<IReadRepository<Products>>();

        var req = new GetByIdProductQueryRequest(id: 123);

        readRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Products, bool>>>(),
                It.IsAny<Func<IQueryable<Products>, IIncludableQueryable<Products, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync((Products?)null);

        uow.Setup(u => u.GetReadRepository<Products>()).Returns(readRepo.Object);

        mapper.Setup(m => m.Map<GetByIdProductQueryResponse, Products>(It.IsAny<Products?>()))
            .Returns((GetByIdProductQueryResponse?)null);


        var sut = new GetByIdProductQueryHandler(mapper.Object, uow.Object);

    
        var resp = await sut.Handle(req, CancellationToken.None);


        resp.IsSuccess.Should().BeTrue();
        resp.Data.Should().BeNull();

        readRepo.Verify(r => r.GetAsync(
            It.IsAny<Expression<Func<Products, bool>>>(),
            It.IsAny<Func<IQueryable<Products>, IIncludableQueryable<Products, object>>>(),
            It.IsAny<bool>()),
            Times.Once);

        mapper.Verify(m => m.Map<GetByIdProductQueryResponse, Products>((Products)null!), Times.Once);
        mapper.Verify(m => m.Map<GetByIdProductQueryResponse, Products>(
            It.Is<Products>(p => p == null)), Times.Once);
        mapper.Verify(m => m.Map<GetByIdProductQueryResponse, Products>(
            It.IsAny<IEnumerable<Products>>()), Times.Never);
    }
}