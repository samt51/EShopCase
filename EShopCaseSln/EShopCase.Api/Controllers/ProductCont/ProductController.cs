using Asp.Versioning;
using EShopCase.Application.Bases;
using EShopCase.Application.Features.ProductsFeature.Commands.CreateProduct;
using EShopCase.Application.Features.ProductsFeature.Commands.DeleteProduct;
using EShopCase.Application.Features.ProductsFeature.Commands.UpdateProduct;
using EShopCase.Application.Features.ProductsFeature.Queries.GetAllProduct;
using EShopCase.Application.Features.ProductsFeature.Queries.GetByIdProduct;
using EShopCase.Application.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopCase.Api.Controllers.ProductCont;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductController : ControllerBase
{
    private readonly IMediator mediator;

    public ProductController(IMediator mediator)
    {
        this.mediator = mediator;
    }
    [HttpGet("{id}")]
    [SwaggerDescriptionAttirbute("Seçili Ürün")]
    public async Task<ResponseDto<GetByIdProductQueryResponse>> GetAllAsync(int id)
    {
        return await mediator.Send(new GetByIdProductQueryRequest(id));
    }
    [HttpPost]
    [SwaggerDescriptionAttirbute("Ürünler")]
    public async Task<ResponseDto<PagedResult<GetAllProductQueryResponse>>> GetAllAsync(GetAllProductQueryRequest request)
    {
        return await mediator.Send(request);
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [SwaggerDescriptionAttirbute("Yeni Ürün")]
    public async Task<ResponseDto<CreateProductCommandResponse>> CreateAsync(CreateProductCommandRequest request)
    {
        return await mediator.Send(request);
    }
    [Authorize(Roles = "Admin")]
    [HttpPut]
    [SwaggerDescriptionAttirbute("Ürün Güncelleme")]
    public async Task<ResponseDto<UpdateProductCommandResponse>> UpdateAsync(UpdateProductCommandRequest request)
    {
        return await mediator.Send(request);
    }
    [HttpPut]
    [SwaggerDescriptionAttirbute("Ürün Silme")]
    public async Task<ResponseDto<DeleteProductCommandResponse>> DeleteAsync(DeleteProductCommandRequest request)
    {
        return await mediator.Send(request);
    }
}