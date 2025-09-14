namespace EShopCase.Application.Dtos.LoginDto;

public  class LoginCommandResponse
{
    public string Token { get; set; } =string.Empty;
    public DateTime TokenExpireDate { get; set; }
}