using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveyApp.API.Configuration;
using SurveyApp.API.Services;
using Xunit;

namespace SurveyApp.API.Tests.Services
{
  public class JwtTokenServiceTests
  {
    private readonly JwtSettings _testSettings;
    private readonly JwtTokenService _tokenService;

    public JwtTokenServiceTests()
    {
      _testSettings = new JwtSettings
      {
        Key = "ThisIsASuperSecretTestKey123456789!",
        Issuer = "TestIssuer",
        Audience = "TestAudience",
        ExpireMinutes = 60
      };

      var options = Options.Create(_testSettings);
      _tokenService = new JwtTokenService(options);
    }

    [Fact]
    public void GenerateToken_ShouldReturn_ValidJwtToken()
    {
      // Arrange
      var userId = Guid.NewGuid();
      var username = "testuser";

      // Act
      var token = _tokenService.GenerateToken(userId, username);

      // Assert
      Assert.False(string.IsNullOrWhiteSpace(token));

      var tokenHandler = new JwtSecurityTokenHandler();
      var validationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = _testSettings.Issuer,
        ValidAudience = _testSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_testSettings.Key)),
        ClockSkew = TimeSpan.Zero
      };

      tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
      Assert.NotNull(validatedToken);
    }
  }
}