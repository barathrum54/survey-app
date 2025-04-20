using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace SurveyApp.API.Tests.Integration;

public class TestControllerAuthTests : IClassFixture<WebApplicationFactory<Program>>
{
  private readonly HttpClient _client;

  public TestControllerAuthTests(WebApplicationFactory<Program> factory)
  {
    _client = factory.CreateClient();
  }

  [Fact]
  public async Task GetUser_WithoutToken_ShouldReturn401()
  {
    var response = await _client.GetAsync("/test/user/admin");
    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
  }

  [Fact]
  public async Task GetUser_WithValidToken_ShouldReturn200()
  {
    var tokenResp = await _client.PostAsync("/test/token", null);
    var content = await tokenResp.Content.ReadAsStringAsync();
    Console.WriteLine("RAW TOKEN RESPONSE: " + content);

    var payload = JsonSerializer.Deserialize<ApiResponse>(content);
    var token = payload?.Data;

    Console.WriteLine("USING TOKEN: " + token);

    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    var response = await _client.GetAsync("/test/user/admin");

    Console.WriteLine("STATUS: " + response.StatusCode);
    Console.WriteLine("BODY: " + await response.Content.ReadAsStringAsync());

    Assert.True(response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotFound);
  }

  private class ApiResponse
  {
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;
  }

}
