using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.CategoryFeature.Commands.UpdateCategory;

public class UpdateCategoryCommandRequest : IRequest<ResponseDto<UpdateCategoryCommandResponse>>
{
    public int Id { get; set; }
    public string Name { get; set; }

    public UpdateCategoryCommandRequest(int id, string name)
    {
        this.Id = id;
        this.Name = name;
    }
}