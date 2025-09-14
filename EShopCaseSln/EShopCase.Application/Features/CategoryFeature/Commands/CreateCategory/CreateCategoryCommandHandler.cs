using EShopCase.Application.Bases;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;

namespace EShopCase.Application.Features.CategoryFeature.Commands.CreateCategory;

public class CreateCategoryCommandHandler:BaseHandler, IRequestHandler<CreateCategoryCommandRequest, ResponseDto<CreateCategoryCommandResponse>>
{
    public CreateCategoryCommandHandler(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
    {
    }

    public async Task<ResponseDto<CreateCategoryCommandResponse>> Handle(CreateCategoryCommandRequest request, CancellationToken cancellationToken)
    {
        var map = mapper.Map<Category, CreateCategoryCommandRequest>(request);

        await unitOfWork.OpenTransactionAsync(cancellationToken);
        
        await unitOfWork.GetWriteRepository<Category>().AddAsync(map, cancellationToken);

        await unitOfWork.SaveAsync(cancellationToken);
        
        await unitOfWork.CommitAsync(cancellationToken);

        return new ResponseDto<CreateCategoryCommandResponse>().Success();
    }
}