using EShopCase.Application.Bases;
using MediatR;

namespace EShopCase.Application.Features.UserFeature.Commands.Register;

public class RegisterCommandRequest(string email, string password, string confirmPassword)
    : IRequest<ResponseDto<RegisterCommandResponse>>
{
    public string Email { get; } = email;
    public string Password { get; } = password;
    public string ConfirmPassword { get; } = confirmPassword;
}