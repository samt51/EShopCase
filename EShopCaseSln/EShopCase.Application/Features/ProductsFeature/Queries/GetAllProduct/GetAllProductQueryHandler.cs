using EShopCase.Application.Bases;
using EShopCase.Application.Dtos.CategoryDtos;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopCase.Application.Features.ProductsFeature.Queries.GetAllProduct;

public class GetAllProductQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) : BaseHandler(mapper, unitOfWork),
    IRequestHandler<GetAllProductQueryRequest, ResponseDto<PagedResult<GetAllProductQueryResponse>>>
{
    public async Task<ResponseDto<PagedResult<GetAllProductQueryResponse>>> Handle(GetAllProductQueryRequest request, CancellationToken cancellationToken)
    {
        
        var page     = request.Page    < 1   ? 1   : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;
        
        
        var query = await unitOfWork.GetReadRepository<Products>().GetAllQueryAsync(x => !x.IsDeleted, include:
            y => y.Include(c => c.Category));
        
        
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim();
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{name}%"));
        }

        if (request.CategoryId is { } cid)
            query = query.Where(p => p.CategoryId == cid);

        if (request.PriceMin is { } min)
            query = query.Where(p => p.Price >= min);

        if (request.PriceMax is { } max)
            query = query.Where(p => p.Price <= max);
        
   
        var sortBy  = (request.SortBy ?? "name").ToLowerInvariant();
        var sortDir = (request.SortDir ?? "asc").ToLowerInvariant();

        query = (sortBy, sortDir) switch
        {
            ("price",  "asc")  => query.OrderBy(p => p.Price).ThenBy(p => p.Id),
            ("price",  "desc") => query.OrderByDescending(p => p.Price).ThenBy(p => p.Id),
            ("created","asc")  => query.OrderBy(p => p.CreatedDate).ThenBy(p => p.Id),
            ("created","desc") => query.OrderByDescending(p => p.CreatedDate).ThenBy(p => p.Id),
            ("stock",  "asc")  => query.OrderBy(p => p.Stock).ThenBy(p => p.Id),
            ("stock",  "desc") => query.OrderByDescending(p => p.Stock).ThenBy(p => p.Id),
            ("name",   "desc") => query.OrderByDescending(p => p.Name).ThenBy(p => p.Id),
            _                  => query.OrderBy(p => p.Name).ThenBy(p => p.Id),
        };
        
        var total = await query.CountAsync(cancellationToken);


        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new GetAllProductQueryResponse
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                CategoryResponse = p.Category == null
                    ? null
                    : new CategoryResponseDto
                    {
                        Id   = p.Category.Id,
                        Name = p.Category.Name
                    }
            })
            .ToListAsync(cancellationToken);
        
        var result = new PagedResult<GetAllProductQueryResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return new ResponseDto<PagedResult<GetAllProductQueryResponse>>().Success(result);
    }
}