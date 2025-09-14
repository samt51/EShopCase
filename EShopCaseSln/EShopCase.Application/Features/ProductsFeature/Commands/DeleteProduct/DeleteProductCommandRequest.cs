using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.ProductsFeature.Commands.DeleteProduct;

public class DeleteProductCommandRequest : IRequest<ResponseDto<DeleteProductCommandResponse>>
{
    public int Id { get; set; }

    public DeleteProductCommandRequest(int id)
    {
        this.Id = id;   
    }
    
}