using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.CategoryFeature.Commands.DeleteCategory;

public abstract class DeleteCategoryCommandRequest(int id) : IRequest<ResponseDto<DeleteCategoryCommandResponse>>
{
    public int Id { get; set; } = id;
}