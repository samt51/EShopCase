using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.UserFeature.Commands.Login;

public class LoginCommandRequest(string email, string password) : IRequest<ResponseDto<LoginCommandResponse>>
{
    public string Email { get; } = email;
    public string Password { get; } = password;
}