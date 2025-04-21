
using System.Net.Http.Json;
using System.Text.Json;
using SurveyApp.API.DTOs;

namespace SurveyApp.API.Tests.Extensions;

public static class HttpClientExtensions
{
  public static async Task<string> LoginAndGetTokenAsync(this HttpClient client, string username, string password, bool autoRegister = false)
  {
    if (autoRegister)
    {
      var registerPayload = new RegisterRequest
      {
        Username = username,
        Password = password,
        Email = $"{username}@test.com"
      };
      await client.PostAsJsonAsync("/auth/register", registerPayload);
    }

    var loginPayload = new LoginRequest
    {
      Username = username,
      Password = password
    };

    var loginResponse = await client.PostAsJsonAsync("/auth/login", loginPayload);
    loginResponse.EnsureSuccessStatusCode();

    var result = await loginResponse.Content.ReadFromJsonAsync<ApiResponse<string>>();
    return result!.Data!;
  }
}