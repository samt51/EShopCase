namespace EShopCase.Application.Features.UserFeature.Commands.Login;

public class LoginCommandResponse(string token,DateTime expires)
{
    public string Token { get; set; } = token;
    public DateTime TokenExpireDate { get; set; } = expires;
}