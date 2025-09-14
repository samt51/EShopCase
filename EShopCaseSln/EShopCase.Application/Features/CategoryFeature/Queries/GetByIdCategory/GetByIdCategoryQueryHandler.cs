using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;

namespace EShopCase.Application.Features.CategoryFeature.Queries.GetByIdCategory;

public class GetByIdCategoryQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) : BaseHandler(mapper, unitOfWork),
    IRequestHandler<GetByIdCategoryQueryRequest, ResponseDto<GetByIdCategoryQueryResponse>>
{
    public async Task<ResponseDto<GetByIdCategoryQueryResponse>> Handle(GetByIdCategoryQueryRequest request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.GetReadRepository<Category>().GetAsync(x=>x.Id == request.Id&&!x.IsDeleted);

        var map = mapper.Map<GetByIdCategoryQueryResponse,Category>(data);

        return new ResponseDto<GetByIdCategoryQueryResponse>().Success(map);
    }
}