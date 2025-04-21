using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using SurveyApp.API.DTOs;
using Xunit.Abstractions;

namespace SurveyApp.API.Tests.Auth;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
  private readonly HttpClient _client;
  private readonly ITestOutputHelper _output;

  public AuthControllerTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
  {
    _client = factory.CreateClient();
    _output = output;
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
  [Fact]
  public async Task Login_WithValidCredentials_ShouldReturnToken()
  {
    var payload = new LoginRequest
    {
      Username = "admin2",
      Password = "admin1234"
    };

    var response = await _client.PostAsJsonAsync("/auth/login", payload);
    var content = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();

    _output.WriteLine("TOKEN: " + content?.Data);

    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    Assert.True(!string.IsNullOrWhiteSpace(content?.Data));
  }
  [Fact]
  public async Task Login_ShouldReturnBadRequest_WhenUsernameIsEmpty()
  {
    var request = new LoginRequest
    {
      Username = "",  // Invalid empty username
      Password = "admin1234"
    };

    var response = await _client.PostAsJsonAsync("/auth/login", request);

    // Assert: Expecting BadRequest due to validation failure
    Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
  }
  [Fact]
  public async Task Register_ShouldReturnBadRequest_WhenEmailIsInvalid()
  {
    var request = new RegisterRequest
    {
      Username = "testuser",
      Password = "Password123!",
      Email = "invalid-email"  // Invalid email format
    };

    var response = await _client.PostAsJsonAsync("/auth/register", request);

    // Assert: Expecting BadRequest due to invalid email format
    Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
  }


}