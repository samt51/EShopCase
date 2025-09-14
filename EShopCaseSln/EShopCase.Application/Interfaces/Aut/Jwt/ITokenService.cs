using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EShopCase.Domain.Entities;

namespace EShopCase.Application.Interfaces.Aut.Jwt;


public interface ITokenService
{
    Task<JwtSecurityToken> CreateToken(Users user, IList<string> roles);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
}