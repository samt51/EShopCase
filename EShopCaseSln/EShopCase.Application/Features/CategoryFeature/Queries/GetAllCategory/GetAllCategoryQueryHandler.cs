using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopCase.Application.Features.CategoryFeature.Queries.GetAllCategory;

public class GetAllCategoryQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) : BaseHandler(mapper, unitOfWork),
    IRequestHandler<GetAllCategoryQueryRequest, ResponseDto<PagedResult<GetAllCategoryQueryResponse>>>
{
    public async Task<ResponseDto<PagedResult<GetAllCategoryQueryResponse>>> Handle(GetAllCategoryQueryRequest request, CancellationToken cancellationToken)
    {
        var page     = request.Page    < 1   ? 1   : request.Page;
        var pageSize = request.PageSize is < 1 or > 100 ? 20 : request.PageSize;
        
        var query = await unitOfWork.GetReadRepository<Category>().GetAllQueryAsync(x => !x.IsDeleted, cancellationToken: cancellationToken);
        
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var name = request.Name.Trim();
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{name}%"));
        }
        var sortBy  = (request.SortBy ?? "name").ToLowerInvariant();
        var sortDir = (request.SortDir ?? "asc").ToLowerInvariant();
        
        query = (sortBy, sortDir) switch
        {
          
            ("created","asc")  => query.OrderBy(p => p.CreatedDate).ThenBy(p => p.Id),
            ("created","desc") => query.OrderByDescending(p => p.CreatedDate).ThenBy(p => p.Id),
            ("name",   "desc") => query.OrderByDescending(p => p.Name).ThenBy(p => p.Id),
            _                  => query.OrderBy(p => p.Name).ThenBy(p => p.Id),
        };
        
        var total = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new GetAllCategoryQueryResponse
            {
                Id = p.Id,
                Name = p.Name
            })
            .ToListAsync(cancellationToken);
        
        var result = new PagedResult<GetAllCategoryQueryResponse>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };

        return new ResponseDto<PagedResult<GetAllCategoryQueryResponse>>().Success(result);



    }
}