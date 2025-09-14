using EShopCase.Application.Bases;
using EShopCase.Application.Dtos.LoginDto;
using EShopCase.Application.Helpers;
using EShopCase.Application.Interfaces.Aut.Jwt;
using EShopCase.Application.Interfaces.Mapper;
using EShopCase.Application.Interfaces.UnitOfWorks;
using EShopCase.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopCase.Application.Features.UserFeature.Commands.Login;

public class LoginCommandHandler(IMapper mapper, IUnitOfWork unitOfWork, ITokenService tokenService) : BaseHandler(mapper, unitOfWork),
    IRequestHandler<LoginCommandRequest, ResponseDto<LoginCommandResponse>>
{
    public async Task<ResponseDto<LoginCommandResponse>> Handle(LoginCommandRequest request, CancellationToken cancellationToken)
    {
        
        var user = await unitOfWork.GetReadRepository<Users>().GetAsync(x => x.Email == request.Email && x.Password == PasswordHash.HashPassword(request.Password) && !x.IsDeleted,
            y => y.Include(x => x.Role));
            
        var token = await tokenService.GenerateToken(new GenerateTokenRequest(user.Id, user.Email, user.Role.Name));

        var tkn = new LoginCommandResponse(token.Token,token.TokenExpireDate);
        
        return new ResponseDto<LoginCommandResponse>().Success(tkn);
    }
}