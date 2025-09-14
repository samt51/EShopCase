using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EShopCase.Application.Dtos.LoginDto;
using EShopCase.Application.Interfaces.Aut.Jwt;
using EShopCase.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EShopCase.Infrastructure.Concrete.Auth;

    public class TokenService : ITokenService
    {

        private readonly TokenSettings tokenSettings;

        readonly IConfiguration configuration;
        public TokenService(IOptions<TokenSettings> options, IConfiguration configuration)
        {
            tokenSettings = options.Value;
            this.configuration = configuration;
        }
        public async Task<JwtSecurityToken> CreateToken(Users user, IList<string> roles)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                 new Claim(ClaimTypes.Name, user.Email.ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret));

            var token = new JwtSecurityToken(
                issuer: tokenSettings.Issuer,
                audience: tokenSettings.Audience,
                expires: DateTime.Now.AddMinutes(tokenSettings.TokenValidityInMinutus),
                claims: claims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            return token;

        }


        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

    
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            TokenValidationParameters tokenValidationParamaters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Secret)),
                ValidateLifetime = false
            };

            JwtSecurityTokenHandler tokenHandler = new();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParamaters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Token bulunamadı.");

            return principal;

        }
        public Task<LoginCommandResponse> GenerateToken(GenerateTokenRequest roleRequest)
        {

            SymmetricSecurityKey symmetricSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["JWT:Secret"]));

            var notBefore = DateTime.Now;
            var expiresTime = DateTime.Now.AddDays(1);


            JwtSecurityToken jwt = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                claims: new List<Claim> {
                    new Claim("role", roleRequest.Role),
                    new Claim(ClaimTypes.Email,roleRequest.Email),
                    new Claim("Id",roleRequest.Id.ToString()),
                    new Claim(ClaimTypes.Name,roleRequest.Email.ToString())

                },
                expires: expiresTime,
                notBefore: notBefore,
                signingCredentials: new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
            );

            return Task.FromResult(new LoginCommandResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwt),
                TokenExpireDate = notBefore
            });
        }
    }