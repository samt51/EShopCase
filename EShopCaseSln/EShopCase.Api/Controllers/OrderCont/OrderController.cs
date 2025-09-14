using Asp.Versioning;
using EShopCase.Application.Bases;
using EShopCase.Application.Features.OrderFeature.Commands.CreateOrder;
using EShopCase.Application.Features.OrderFeature.Queries.GetAllOrder;
using EShopCase.Application.Features.OrderFeature.Queries.GetByOrder;
using EShopCase.Application.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EShopCase.Api.Controllers.OrderCont;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/order")]
public class OrderController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    [HttpPost]
    [SwaggerDescriptionAttirbute("Yeni Sipariş")]
    public async Task<ResponseDto<CreateOrderCommandResponse>> CreateAsync(CreateOrderCommandRequest request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [SwaggerDescriptionAttirbute("Tüm Siparişler")]
    public async Task<ResponseDto<List<GetAllOrderQueryResponse>>> GetAllOrderAsync()
    {
        return await _mediator.Send(new GetAllOrderQueryRequest());
    }
    [HttpGet("{orderId}")]
    [SwaggerDescriptionAttirbute("Id göre Sipariş")]
    public async Task<ResponseDto<GetByOrderQueryResponse>> GetByIdOrderAsync(int orderId)
    {
        return await _mediator.Send(new GetByOrderQueryRequest{ OrderId = orderId });
    }
}