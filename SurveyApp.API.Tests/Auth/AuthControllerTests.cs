using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SurveyApp.API.DTOs;

namespace SurveyApp.API.Tests.Auth;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
  private readonly HttpClient _client;

  public AuthControllerTests(WebApplicationFactory<Program> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task Register_NewUser_ShouldReturn200()
  {
    var payload = new RegisterRequest
    {
      Username = $"user_{Guid.NewGuid():N}",
      Password = "secure123",
      Email = $"user_{Guid.NewGuid():N}@example.com"
    };

    var response = await _client.PostAsJsonAsync("/auth/register", payload);
    var content = await response.Content.ReadAsStringAsync();

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.Contains("User registered successfully", content);
  }
}