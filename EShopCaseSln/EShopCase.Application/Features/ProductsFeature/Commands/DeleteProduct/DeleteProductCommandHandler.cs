using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;

namespace EShopCase.Application.Features.ProductsFeature.Commands.DeleteProduct;

public class DeleteProductCommandHandler : BaseHandler,IRequestHandler<DeleteProductCommandRequest,ResponseDto<DeleteProductCommandResponse>>
{
    public DeleteProductCommandHandler(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
    }

    public async Task<ResponseDto<DeleteProductCommandResponse>> Handle(DeleteProductCommandRequest request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.GetReadRepository<Products>().GetAsync(x=>x.Id == request.Id&&x!.IsDeleted);
        
        data.IsDeleted = true;
        data.IsActive = false;

        await unitOfWork.OpenTransactionAsync(cancellationToken);

        await unitOfWork.GetWriteRepository<Products>().UpdateAsync(data,cancellationToken);

        await unitOfWork.SaveAsync(cancellationToken);
        
        await unitOfWork.CommitAsync(cancellationToken);

        return new ResponseDto<DeleteProductCommandResponse>().Success();


    }
}