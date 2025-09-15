using Asp.Versioning;
using EShopCase.Application.Bases;
using EShopCase.Application.Features.CategoryFeature.Commands.CreateCategory;
using EShopCase.Application.Features.CategoryFeature.Commands.DeleteCategory;
using EShopCase.Application.Features.CategoryFeature.Commands.UpdateCategory;
using EShopCase.Application.Features.CategoryFeature.Queries.GetAllCategory;
using EShopCase.Application.Features.CategoryFeature.Queries.GetByIdCategory;
using EShopCase.Application.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EShopCase.Api.Controllers.CategoryCont;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/category")]
public class CategoryController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    [HttpGet("{id:int}")]
    [SwaggerDescriptionAttirbute("Seçili Kategori")]
    public async Task<ResponseDto<GetByIdCategoryQueryResponse>> GetByIdAsync(int id)
    {
        return await _mediator.Send(new GetByIdCategoryQueryRequest(id));
    }
    [HttpPost("GetAll")]
    [SwaggerDescriptionAttirbute("Kategoriler")]
    public async Task<ResponseDto<PagedResult<GetAllCategoryQueryResponse>>> GetAllAsync(GetAllCategoryQueryRequest request)
    {
        return await _mediator.Send(request);
    }
    [HttpPost("Create")]
    [SwaggerDescriptionAttirbute("Yeni Kategori")]
    public async Task<ResponseDto<CreateCategoryCommandResponse>> CreateAsync(CreateCategoryCommandRequest request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut("Update")]
    [SwaggerDescriptionAttirbute("Kategori Güncelleme")]
    public async Task<ResponseDto<UpdateCategoryCommandResponse>> UpdateAsync(UpdateCategoryCommandRequest request)
    {
        return await _mediator.Send(request);
    }
    [HttpPut("Delete")]
    [SwaggerDescriptionAttirbute("Kategori Silme")]
    public async Task<ResponseDto<DeleteCategoryCommandResponse>> DeleteAsync(DeleteCategoryCommandRequest request)
    {
        return await _mediator.Send(request);
    }
}