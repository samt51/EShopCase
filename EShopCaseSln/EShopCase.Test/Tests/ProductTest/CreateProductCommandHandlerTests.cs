using EShopCase.Application.Features.ProductsFeature.Commands.CreateProduct;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.Repositories;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EShopCase.Test.Tests.ProductTest;

public class CreateProductCommandHandlerTests
{

    [Fact]
    public async Task Handle_Should_Create_Product_And_Commit_Transaction()
    {

        var mapper = new Mock<IMapper>();
        var uow = new Mock<IUnitOfWork>();
        var writeRepo = new Mock<IWriteRepository<Products>>();

        var req = new CreateProductCommandRequest(name: "New Phone", description: "Great one", price: 123.45m,
            stock: 10, categoryId: 1);


        var mappedEntity = new Products
        {
            Name = req.Name,
            Description = req.Description,
            Price = req.Price,
            Stock = req.Stock,
            CategoryId = req.CategoryId
        };

        mapper.Setup(m => m.Map<Products, CreateProductCommandRequest>(req))
            .Returns(mappedEntity);

        uow.Setup(u => u.GetWriteRepository<Products>())
            .Returns(writeRepo.Object);


        var sut = new CreateProductCommandHandler(mapper.Object, uow.Object);
        var resp = await sut.Handle(req, CancellationToken.None);


        resp.Should().NotBeNull();
        resp.IsSuccess.Should().BeTrue();


        mapper.Verify(
            m => m.Map<Products, CreateProductCommandRequest>(It.Is<CreateProductCommandRequest>(x => x == req)),
            Times.Once);

        uow.Verify(u => u.OpenTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        writeRepo.Verify(r => r.AddAsync(It.Is<Products>(p =>
            p.Name == req.Name &&
            p.Description == req.Description &&
            p.Price == req.Price &&
            p.Stock == req.Stock &&
            p.CategoryId == req.CategoryId
        ), It.IsAny<CancellationToken>()), Times.Once);

        uow.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.RollBackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
public async Task Handle_When_Save_Fails_Should_Rollback_And_Rethrow()
{

    var mapper    = new Mock<IMapper>();
    var uow       = new Mock<IUnitOfWork>();
    var writeRepo = new Mock<IWriteRepository<Products>>();

    var req = new CreateProductCommandRequest(
        name: "Faulty", description: "", price: 10, stock: 1, categoryId: 1);

    mapper.Setup(m => m.Map<Products, CreateProductCommandRequest>(
                    It.IsAny<CreateProductCommandRequest>()))
          .Returns((CreateProductCommandRequest r) => new Products
          {
              Name = r.Name,
              Description = r.Description,
              Price = r.Price,
              Stock = r.Stock,
              CategoryId = r.CategoryId
          });

    uow.Setup(u => u.GetWriteRepository<Products>()).Returns(writeRepo.Object);
    uow.Setup(u => u.OpenTransactionAsync(It.IsAny<CancellationToken>()))
       .Returns(Task.CompletedTask);

    // IWriteRepository<T>.AddAsync => Task<T> döndürüyor
    writeRepo.Setup(r => r.AddAsync(It.IsAny<Products>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Products p, CancellationToken _) => p);

    // Save sırasında hata olsun ve rollback tetiklensin
    uow.Setup(u => u.RollBackAsync(It.IsAny<CancellationToken>()))
       .Returns(Task.CompletedTask);

    uow.Setup(u => u.SaveAsync(It.IsAny<CancellationToken>()))
       .Callback<CancellationToken>(ct => uow.Object.RollBackAsync(ct))
       .ThrowsAsync(new DbUpdateException("save failed", new Exception("inner")));


    uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>()))
       .Returns(Task.CompletedTask);

    var sut = new CreateProductCommandHandler(mapper.Object, uow.Object);


    Func<Task> act = () => sut.Handle(req, CancellationToken.None);

  
    await act.Should().ThrowAsync<DbUpdateException>();
    uow.Verify(u => u.OpenTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
    uow.Verify(u => u.RollBackAsync(It.IsAny<CancellationToken>()), Times.Once);  // İSİM: RollBackAsync!
    uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
}
}