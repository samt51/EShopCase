using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;

namespace EShopCase.Application.Features.CategoryFeature.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : BaseHandler,IRequestHandler<UpdateCategoryCommandRequest,ResponseDto<UpdateCategoryCommandResponse>>
{
    public UpdateCategoryCommandHandler(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
    }

    public async Task<ResponseDto<UpdateCategoryCommandResponse>> Handle(UpdateCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        var data = await unitOfWork.GetReadRepository<Category>().GetAsync(x=>x.Id == request.Id&&x!.IsDeleted);
        
        data.Name = request.Name;

        await unitOfWork.OpenTransactionAsync(cancellationToken);

        await unitOfWork.GetWriteRepository<Category>().UpdateAsync(data,cancellationToken);

        await unitOfWork.SaveAsync(cancellationToken);
        
        await unitOfWork.CommitAsync(cancellationToken);

        return new ResponseDto<UpdateCategoryCommandResponse>().Success();

    }
}