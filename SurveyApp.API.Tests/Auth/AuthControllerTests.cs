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
  [Fact]
  public async Task Register_ShouldReturnConflict_WhenUsernameExists()
  {
    var request = new RegisterRequest
    {
      Username = "admin2", // already exists
      Password = "SomePass123!",
      Email = "admin2@example.com"
    };

    var response = await _client.PostAsJsonAsync("/auth/register", request);

    Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
  }
  [Fact]
  public async Task Login_ShouldReturnUnauthorized_WhenPasswordIncorrect()
  {
    var request = new LoginRequest
    {
      Username = "admin2",
      Password = "wrong-password"
    };

    var response = await _client.PostAsJsonAsync("/auth/login", request);

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }
  [Fact]
  public async Task Register_ShouldReturnBadRequest_WhenMissingFields()
  {
    var request = new RegisterRequest
    {
      Username = "",
      Password = "",
      Email = ""
    };

    var response = await _client.PostAsJsonAsync("/auth/register", request);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }
  [Fact]
  public async Task Login_ShouldReturnBadRequest_WhenFieldsAreEmpty()
  {
    var request = new LoginRequest
    {
      Username = "",
      Password = ""
    };

    var response = await _client.PostAsJsonAsync("/auth/login", request);

    Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
  }
  [Fact]
  public async Task Register_ThenLogin_ShouldSucceed()
  {
    var username = $"user_{Guid.NewGuid():N}";
    var password = "strongPass123!";
    var email = $"{username}@example.com";

    var registerPayload = new RegisterRequest
    {
      Username = username,
      Password = password,
      Email = email
    };

    var registerResponse = await _client.PostAsJsonAsync("/auth/register", registerPayload);
    Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

    var loginPayload = new LoginRequest
    {
      Username = username,
      Password = password
    };

    var loginResponse = await _client.PostAsJsonAsync("/auth/login", loginPayload);
    var loginContent = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();

    Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
    Assert.False(string.IsNullOrWhiteSpace(loginContent?.Data));
    _output.WriteLine("TOKEN: " + loginContent?.Data);
  }

}