using Asp.Versioning;
using EShopCase.Application.Bases;
using EShopCase.Application.Features.UserFeature.Commands.Login;
using EShopCase.Application.Features.UserFeature.Commands.Register;
using EShopCase.Application.Features.UserFeature.Queries.GetAllUser;
using EShopCase.Application.Features.UserFeature.Queries.GetByUser;
using EShopCase.Application.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EShopCase.Api.Controllers.UserCont;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
public class UserController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    [HttpPost("login")]
    [SwaggerDescriptionAttirbute("Login")]
    public async Task<ResponseDto<LoginCommandResponse>> LoginAsync(LoginCommandRequest request)
    {
        return await _mediator.Send(request);
    }
    [HttpPost("register")]
    [SwaggerDescriptionAttirbute("Register")]
    public async Task<ResponseDto<RegisterCommandResponse>> RegisterAsync(RegisterCommandRequest request)
    {
        return await _mediator.Send(request);
    }
    [HttpGet]
    [SwaggerDescriptionAttirbute("Tüm Kullancılar")]
    public async Task<ResponseDto<List<GetAllUserQueryResponse>>> GetAllAsync()
    {
        return await _mediator.Send(new GetAllUserQueryRequest());
    }
    [HttpGet("{userId:int}")]
    [SwaggerDescriptionAttirbute("Id göre Kullanıcı")]
    public async Task<ResponseDto<GetByUserQueryResponse>> GetByIdAsync(int userId)
    {
        return await _mediator.Send(new GetByUserQueryRequest(userId));
    }
}