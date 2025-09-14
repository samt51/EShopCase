using System.Linq.Expressions;
using EShopCase.Application.Features.ProductsFeature.Commands.UpdateProduct;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.Repositories;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace EShopCase.Test.Tests.ProductTest;

public class UpdateProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_Product_And_Commit()
    {
     
        var mapper    = new Mock<IMapper>();
        var uow       = new Mock<IUnitOfWork>();
        var readRepo  = new Mock<IReadRepository<Products>>();
        var writeRepo = new Mock<IWriteRepository<Products>>();

        var req = new UpdateProductCommandRequest(
            id: 4,
            name: "Table v2",
            description: "updated",
            price: 150m,
            stock: 3,
            categoryId: 3
        );


        readRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Products, bool>>>(),
                It.IsAny<Func<IQueryable<Products>, IIncludableQueryable<Products, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync(new Products { Id = req.Id, IsDeleted = false }); 

        

        mapper.Setup(m => m.Map<Products, UpdateProductCommandRequest>(It.IsAny<UpdateProductCommandRequest>()))
              .Returns((UpdateProductCommandRequest r) => new Products
              {
                  Id = r.Id,
                  Name = r.Name,
                  Description = r.Description,
                  Price = r.Price,
                  Stock = r.Stock,
                  CategoryId = r.CategoryId
              });

        uow.Setup(u => u.GetReadRepository<Products>()).Returns(readRepo.Object);
        uow.Setup(u => u.GetWriteRepository<Products>()).Returns(writeRepo.Object);

        uow.Setup(u => u.OpenTransactionAsync(It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        writeRepo.Setup(r => r.UpdateAsync(It.IsAny<Products>(), It.IsAny<CancellationToken>()))
            .Returns((Products p, CancellationToken _) => Task.FromResult(p));

     
        uow.Setup(u => u.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var sut = new UpdateProductCommandHandler(mapper.Object, uow.Object);

 
        var resp = await sut.Handle(req, CancellationToken.None);

   
        resp.Should().NotBeNull();
        resp.IsSuccess.Should().BeTrue();

        readRepo.Verify(r => r.GetAsync(
            It.IsAny<Expression<Func<Products, bool>>>(),
            It.IsAny<Func<IQueryable<Products>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Products, object>>>(),
            It.IsAny<bool>()),
            Times.Once);

        mapper.Verify(m => m.Map<Products, UpdateProductCommandRequest>(It.Is<UpdateProductCommandRequest>(x => x == req)), Times.Once);

        writeRepo.Verify(w => w.UpdateAsync(It.Is<Products>(p =>
                p.Id == req.Id &&
                p.Name == req.Name &&
                p.Description == req.Description &&
                p.Price == req.Price &&
                p.Stock == req.Stock &&
                p.CategoryId == req.CategoryId
            ),It.IsAny<CancellationToken>()), Times.Once);

        uow.Verify(u => u.OpenTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.RollBackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_When_Save_Fails_Should_Rollback_And_Rethrow()
    {
        
        var mapper    = new Mock<IMapper>();
        var uow       = new Mock<IUnitOfWork>();
        var readRepo  = new Mock<IReadRepository<Products>>();
        var writeRepo = new Mock<IWriteRepository<Products>>();

        var req = new UpdateProductCommandRequest(
            id: 9,
            name: "Phone v2",
            description: "oops",
            price: 900m,
            stock: 5,
            categoryId: 2
        );

        readRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Products, bool>>>(),
                It.IsAny<Func<IQueryable<Products>, IIncludableQueryable<Products, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync(new Products { Id = req.Id, IsDeleted = false }); 

        mapper.Setup(m => m.Map<Products, UpdateProductCommandRequest>(It.IsAny<UpdateProductCommandRequest>()))
              .Returns(new Products { Id = req.Id, Name = req.Name, Description = req.Description, Price = req.Price, Stock = req.Stock, CategoryId = req.CategoryId });

        uow.Setup(u => u.GetReadRepository<Products>()).Returns(readRepo.Object);
        uow.Setup(u => u.GetWriteRepository<Products>()).Returns(writeRepo.Object);

        uow.Setup(u => u.OpenTransactionAsync(It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        writeRepo.Setup(r => r.UpdateAsync(It.IsAny<Products>(), It.IsAny<CancellationToken>()))
            .Returns((Products p, CancellationToken _) => Task.FromResult(p));

     
        uow.Setup(u => u.RollBackAsync(It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        uow.Setup(u => u.SaveAsync(It.IsAny<CancellationToken>()))
           .Callback<CancellationToken>(ct => uow.Object.RollBackAsync(ct))
           .ThrowsAsync(new DbUpdateException("save failed", new Exception("inner")));

    
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var sut = new UpdateProductCommandHandler(mapper.Object, uow.Object);

      
        Func<Task> act = () => sut.Handle(req, CancellationToken.None);

   
        await act.Should().ThrowAsync<DbUpdateException>();
        uow.Verify(u => u.OpenTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.RollBackAsync(It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}