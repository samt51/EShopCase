using System.Linq.Expressions;
using EShopCase.Application.Interfaces.Repositories;
using EShopCase.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace EShopCase.Test;

public class FakeReadRepository<T> : IReadRepository<T> where T : class, IEntityBase, new()
{
    private readonly DbSet<T> _set;

    public FakeReadRepository(DbContext ctx) => _set = ctx.Set<T>();
    public Task<IQueryable<T>> GetAllQueryAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, T>>? selector = null,
        bool enableTracking = false,
        CancellationToken cancellationToken = default)
    {
        IQueryable<T> q = enableTracking ? _set : _set.AsNoTracking();

        if (predicate != null) q = q.Where(predicate);
        if (include   != null) q = include(q);
        if (orderBy   != null) q = orderBy(q);
        if (selector  != null) q = q.Select(selector);

        return Task.FromResult(q);
    }
    public Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Expression<Func<T, T>>? selector = null,
        bool enableTracking = false, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<IList<T>> GetAllByPagingAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool enableTracking = false, int currentPage = 1, int pageSize = 3)
    {
        throw new NotImplementedException();
    }

    public Task<T> GetAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool enableTracking = false)
    {
        throw new NotImplementedException();
    }

    public Task<T> FindAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool enableTracking = false)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        throw new NotImplementedException();
    }
}