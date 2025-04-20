using System.Net.Http.Headers;
using System.Net.Http.Json;
using SurveyApp.API.DTOs;
using SurveyApp.API.Models;
using SurveyApp.API.Tests.Extensions;
using SurveyApp.API.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace SurveyApp.API.Tests.Integration;

public class SurveyControllerTests : IClassFixture<DatabaseFixture>
{
  private readonly HttpClient _client;
  private readonly ITestOutputHelper _output;

  public SurveyControllerTests(DatabaseFixture fixture, ITestOutputHelper output)
  {
    _client = fixture._factory.CreateClient();
    _output = output;
  }

  [Fact]
  public async Task CreateSurvey_ShouldReturnCreated_WhenRequestIsValid()
  {
    var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    var request = new CreateSurveyRequest
    {
      Title = "Integration Test Survey",
      Options = new List<string> { "Option A", "Option B" }
    };

    var response = await _client.PostAsJsonAsync("/Survey", request);
    var content = await response.Content.ReadAsStringAsync();

    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<Survey>();
    Assert.NotNull(result);
    Assert.Equal(request.Title, result!.Title);
  }

  [Fact]
  public async Task CreateSurvey_ShouldReturnBadRequest_WhenOptionsInvalid()
  {
    var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    var request = new CreateSurveyRequest
    {
      Title = "Bad Survey",
      Options = new List<string> { "Only One Option" }
    };

    var response = await _client.PostAsJsonAsync("/Survey", request);

    Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
  }

  [Fact]
  public async Task GetMySurveys_ShouldReturnOnlyOwnedSurveys()
  {
    var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    var response = await _client.GetAsync("/Survey/me");
    var content = await response.Content.ReadAsStringAsync();
    _output.WriteLine("🔴 Raw Response:\n" + content);

    response.EnsureSuccessStatusCode();

    var result = await response.Content.ReadFromJsonAsync<List<Survey>>();
    Assert.NotNull(result);
    Assert.All(result!, s => Assert.Equal(5, s.CreatedBy));
  }
}
