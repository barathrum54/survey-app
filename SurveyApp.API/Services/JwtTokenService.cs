using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveyApp.API.Configuration;
using SurveyApp.API.Services.Interfaces;

namespace SurveyApp.API.Services;

public class JwtTokenService : IJwtTokenService
{
  private readonly JwtSettings _settings;

  public JwtTokenService(IOptions<JwtSettings> options)
  {
    _settings = options.Value;
  }

  public string GenerateToken(int userId, string username)
  {
    var claims = new[]
    {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: _settings.Issuer,
        audience: _settings.Audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(_settings.ExpireMinutes),
        signingCredentials: creds);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}
