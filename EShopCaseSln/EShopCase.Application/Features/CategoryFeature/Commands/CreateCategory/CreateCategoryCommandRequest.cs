using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.CategoryFeature.Commands.CreateCategory;

public class CreateCategoryCommandRequest : IRequest<ResponseDto<CreateCategoryCommandResponse>>
{
    public string Name { get; set; }

    public CreateCategoryCommandRequest(string name)
    {
            this.Name = name;
    }
}