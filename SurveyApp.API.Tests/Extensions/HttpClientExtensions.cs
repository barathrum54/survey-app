
using System.Net.Http.Json;
using System.Text.Json;
using SurveyApp.API.DTOs;

namespace SurveyApp.API.Tests.Extensions;

public static class HttpClientExtensions
{
  public static async Task<string> LoginAndGetTokenAsync(this HttpClient client, string username, string password)
  {
    var loginRequest = new LoginRequest
    {
      Username = username,
      Password = password
    };

    var response = await client.PostAsJsonAsync("/auth/login", loginRequest);
    response.EnsureSuccessStatusCode();

    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<string>>(new JsonSerializerOptions
    {
      PropertyNameCaseInsensitive = true
    });

    if (apiResponse == null || string.IsNullOrWhiteSpace(apiResponse.Data))
      throw new Exception("Login failed: token not returned");

    Console.WriteLine($"Token for {username}: {apiResponse.Data}");

    return apiResponse.Data;
  }
}