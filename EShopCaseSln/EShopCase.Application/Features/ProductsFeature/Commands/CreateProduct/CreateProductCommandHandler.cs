using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;

namespace EShopCase.Application.Features.ProductsFeature.Commands.CreateProduct;

public class CreateProductCommandHandler:BaseHandler,IRequestHandler<CreateProductCommandRequest,ResponseDto<CreateProductCommandResponse>>
{
    public CreateProductCommandHandler(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
    }

    public async Task<ResponseDto<CreateProductCommandResponse>> Handle(CreateProductCommandRequest request, CancellationToken cancellationToken)
    {
        var mapEntity = mapper.Map<Products,CreateProductCommandRequest >(request);

        await unitOfWork.OpenTransactionAsync(cancellationToken);
        
        await unitOfWork.GetWriteRepository<Products>().AddAsync(mapEntity, cancellationToken);

        await unitOfWork.SaveAsync(cancellationToken);
        
        await unitOfWork.CommitAsync(cancellationToken);

        return new ResponseDto<CreateProductCommandResponse>().Success();
    }
}