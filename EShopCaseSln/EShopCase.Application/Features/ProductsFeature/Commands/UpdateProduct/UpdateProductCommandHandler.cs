using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;

namespace EShopCase.Application.Features.ProductsFeature.Commands.UpdateProduct;

public class UpdateProductCommandHandler : BaseHandler,IRequestHandler<UpdateProductCommandRequest,ResponseDto<UpdateProductCommandResponse>>
{
    public UpdateProductCommandHandler(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
    }

    public async Task<ResponseDto<UpdateProductCommandResponse>> Handle(UpdateProductCommandRequest request, CancellationToken cancellationToken)
    {
        await unitOfWork.GetReadRepository<Products>().GetAsync(x=>x.Id == request.Id&&x!.IsDeleted);
        
        var map = mapper.Map<Products, UpdateProductCommandRequest>(request);

        await unitOfWork.OpenTransactionAsync(cancellationToken);
        
        await unitOfWork.GetWriteRepository<Products>().UpdateAsync(map, cancellationToken);

        await unitOfWork.SaveAsync(cancellationToken);
        
        await unitOfWork.CommitAsync(cancellationToken);

        return new ResponseDto<UpdateProductCommandResponse>().Success();


    }
}