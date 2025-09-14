namespace EShopCase.Application.Dtos.LoginDto;

public class GenerateTokenRequest(int id, string email, string role)
{
    public int Id { get; set; } = id;
    public string Email { get; set; } = email;
    public string Role { get; set; } = role;
}