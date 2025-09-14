using System.Linq.Expressions;
using EShopCase.Application.Features.ProductsFeature.Commands.DeleteProduct;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.Repositories;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace EShopCase.Test.Tests.ProductTest;

public class DeleteProductCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_SoftDelete_And_Commit()
    {
        var mapper    = new Mock<IMapper>();
        var uow       = new Mock<IUnitOfWork>();
        var readRepo  = new Mock<IReadRepository<Products>>();
        var writeRepo = new Mock<IWriteRepository<Products>>();

        var req = new DeleteProductCommandRequest(id: 42);

 
        var existing = new Products { Id = req.Id, IsDeleted = false, IsActive = true };

        readRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Products, bool>>>(),
                It.IsAny<Func<IQueryable<Products>, IIncludableQueryable<Products, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync(existing);

        uow.Setup(u => u.GetReadRepository<Products>()).Returns(readRepo.Object);
        uow.Setup(u => u.GetWriteRepository<Products>()).Returns(writeRepo.Object);

        uow.Setup(u => u.OpenTransactionAsync(It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);


        writeRepo.Setup(r => r.UpdateAsync(It.IsAny<Products>(), It.IsAny<CancellationToken>()))
            .Returns((Products p, CancellationToken _) => Task.FromResult(p));


        uow.Setup(u => u.SaveAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var sut = new DeleteProductCommandHandler(mapper.Object, uow.Object);

        var resp = await sut.Handle(req, CancellationToken.None);

        resp.IsSuccess.Should().BeTrue();

        writeRepo.Verify(w => w.UpdateAsync(
            It.Is<Products>(p => p.Id == req.Id && p.IsDeleted == true && p.IsActive == false),
            It.IsAny<CancellationToken>()),
            Times.Once);

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

        var req = new DeleteProductCommandRequest(id: 7);
        var existing = new Products { Id = req.Id, IsDeleted = false, IsActive = true };

        readRepo.Setup(r => r.GetAsync(
                It.IsAny<Expression<Func<Products, bool>>>(),
                It.IsAny<Func<IQueryable<Products>, IIncludableQueryable<Products, object>>>(),
                It.IsAny<bool>()))
            .ReturnsAsync(existing);

        uow.Setup(u => u.GetReadRepository<Products>()).Returns(readRepo.Object);
        uow.Setup(u => u.GetWriteRepository<Products>()).Returns(writeRepo.Object);

        uow.Setup(u => u.OpenTransactionAsync(It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        writeRepo.Setup(r => r.UpdateAsync(It.IsAny<Products>(), It.IsAny<CancellationToken>()))
            .Returns((Products p, CancellationToken _) => Task.FromResult(p));



        uow.Setup(u => u.RollBackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        uow.Setup(u => u.SaveAsync(It.IsAny<CancellationToken>()))
           .Callback<CancellationToken>(ct => uow.Object.RollBackAsync(ct))
           .ThrowsAsync(new DbUpdateException("save failed", new Exception("inner")));

        uow.Setup(u => u.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var sut = new DeleteProductCommandHandler(mapper.Object, uow.Object);


        Func<Task> act = () => sut.Handle(req, CancellationToken.None);

 
        await act.Should().ThrowAsync<DbUpdateException>();


        writeRepo.Verify(w => w.UpdateAsync(
            It.Is<Products>(p => p.Id == req.Id && p.IsDeleted && !p.IsActive),
            It.IsAny<CancellationToken>()),
            Times.Once);

        uow.Verify(u => u.OpenTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.RollBackAsync(It.IsAny<CancellationToken>()), Times.Once);
        uow.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}