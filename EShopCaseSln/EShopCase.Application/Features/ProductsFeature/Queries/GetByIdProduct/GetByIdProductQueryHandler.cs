using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopCase.Application.Features.ProductsFeature.Queries.GetByIdProduct;

public class GetByIdProductQueryHandler :  BaseHandler, IRequestHandler<GetByIdProductQueryRequest, ResponseDto<GetByIdProductQueryResponse>>
{
    public GetByIdProductQueryHandler(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
    }

    public async Task<ResponseDto<GetByIdProductQueryResponse>> Handle(GetByIdProductQueryRequest request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.GetReadRepository<Products>().GetAsync(x=>x!.IsDeleted&&x.Id==request.Id,include:
                x=>x.Include(y=>y.Category));

        var map = mapper.Map< GetByIdProductQueryResponse,Products>(data);

        return new ResponseDto<GetByIdProductQueryResponse>().Success(map);
    }
}