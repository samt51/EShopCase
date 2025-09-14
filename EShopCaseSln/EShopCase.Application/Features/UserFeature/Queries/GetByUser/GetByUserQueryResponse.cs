namespace EShopCase.Application.Features.UserFeature.Queries.GetByUser;

public class GetByUserQueryResponse(int id, string fullName, string email)
{
    public int Id { get; set; } = id;
    public string FullName { get; set; } = fullName;
    public string Email { get; set; } = email;
}