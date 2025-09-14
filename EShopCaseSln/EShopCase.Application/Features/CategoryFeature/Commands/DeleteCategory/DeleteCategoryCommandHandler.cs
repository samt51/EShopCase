using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;

namespace EShopCase.Application.Features.CategoryFeature.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler(IMapper mapper, IUnitOfWork unitOfWork) : BaseHandler(mapper, unitOfWork),
    IRequestHandler<DeleteCategoryCommandRequest, ResponseDto<DeleteCategoryCommandResponse>>
{
    public async Task<ResponseDto<DeleteCategoryCommandResponse>> Handle(DeleteCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.GetReadRepository<Category>().GetAsync(x=>x.Id == request.Id&&x!.IsDeleted);
        
        data.IsDeleted = true;
        data.IsActive = false;

        await unitOfWork.OpenTransactionAsync(cancellationToken);

        await unitOfWork.GetWriteRepository<Category>().UpdateAsync(data, cancellationToken);

        await unitOfWork.SaveAsync(cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken);

        return new ResponseDto<DeleteCategoryCommandResponse>().Success();
    }
}