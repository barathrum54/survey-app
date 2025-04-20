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
  [Fact]
  public async Task DeleteSurvey_ShouldReturnNoContent_WhenUserIsOwner()
  {
    var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    var createRequest = new CreateSurveyRequest
    {
      Title = "Survey To Delete",
      Options = new List<string> { "Option 1", "Option 2" }
    };

    var createResponse = await _client.PostAsJsonAsync("/Survey", createRequest);
    createResponse.EnsureSuccessStatusCode();
    var created = await createResponse.Content.ReadFromJsonAsync<Survey>();

    var deleteResponse = await _client.DeleteAsync($"/Survey/{created!.Id}");
    Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);
  }

  [Fact]
  public async Task DeleteSurvey_ShouldReturnForbidden_WhenUserIsNotOwner()
  {
    // Login as the owner
    var ownerToken = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ownerToken);

    // Create a survey owned by the owner
    var request = new CreateSurveyRequest
    {
      Title = "Other's Survey",
      Options = new List<string> { "A", "B" }
    };
    var createResponse = await _client.PostAsJsonAsync("/Survey", request);
    createResponse.EnsureSuccessStatusCode();
    var created = await createResponse.Content.ReadFromJsonAsync<Survey>();

    var attackerToken = await _client.LoginAndGetTokenAsync("admin", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", attackerToken);

    var response = await _client.DeleteAsync($"/Survey/{created!.Id}");

    Assert.Equal(System.Net.HttpStatusCode.Forbidden, response.StatusCode);
  }
}
