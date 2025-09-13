using EShopCase.Application.Interfaces.Repositories;
using EShopCase.Domain.Common;

namespace EShopCase.Application.Interfaces.UnitOfWorks;

public interface IUnitOfWork : IAsyncDisposable
{
    IReadRepository<T> GetReadRepository<T>() where T : class, IEntityBase, new();
    IWriteRepository<T> GetWriteRepository<T>() where T : class, IEntityBase, new();
    Task OpenTransactionAsync(CancellationToken? cancellationToken = null);
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
    Task CommitAsync(CancellationToken cancellationToken = default);
    Task RollBackAsync(CancellationToken cancellationToken = default);
}