namespace EShopCase.Infrastructure.Concrete.Auth;

public class TokenSettings
{
    public string Audience { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public int TokenValidityInMinutus { get; set; }
}