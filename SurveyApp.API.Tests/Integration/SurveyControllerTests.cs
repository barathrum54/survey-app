using System.Net.Http.Headers;
using System.Net.Http.Json;
using SurveyApp.API.DTOs;
using SurveyApp.API.Models;
using SurveyApp.API.Tests.Extensions;
using SurveyApp.API.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace SurveyApp.API.Tests.Integration
{
  public class SurveyControllerTests : IClassFixture<DatabaseFixture>
  {
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public SurveyControllerTests(DatabaseFixture fixture, ITestOutputHelper output)
    {
      _client = fixture._factory.CreateClient();
      _output = output;
    }

    // Test for creating a valid survey
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

    // Test for invalid survey creation (invalid options)
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

    // Test for retrieving the surveys created by the user
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

    // Test for deleting a survey by the owner
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

    // Test for deleting a survey by someone who is not the owner
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

    [Fact]
    public async Task GetSurveyResults_ShouldReturnAggregatedResults()
    {
      var token1 = await _client.LoginAndGetTokenAsync("admin2", "admin1234"); // First user
      _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token1);

      // Create a survey
      var createSurveyRequest = new CreateSurveyRequest
      {
        Title = "Test Survey for Results",
        Options = new List<string> { "Option A", "Option B" }
      };

      var createSurveyResponse = await _client.PostAsJsonAsync("/Survey", createSurveyRequest);
      createSurveyResponse.EnsureSuccessStatusCode();
      var createdSurvey = await createSurveyResponse.Content.ReadFromJsonAsync<Survey>();

      // First user votes
      var voteRequest1 = new VoteRequest
      {
        SurveyId = createdSurvey!.Id,
        OptionId = 1 // Voting for Option A
      };

      await _client.PostAsJsonAsync("/vote", voteRequest1);

      // Second user votes
      var token2 = await _client.LoginAndGetTokenAsync("admin", "admin1234"); // Second user
      _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token2);

      var voteRequest2 = new VoteRequest
      {
        SurveyId = createdSurvey!.Id,
        OptionId = 2 // Voting for Option B
      };

      await _client.PostAsJsonAsync("/vote", voteRequest2);

      // Retrieve survey results
      var resultsResponse = await _client.GetAsync($"/survey/{createdSurvey.Id}/results");
      resultsResponse.EnsureSuccessStatusCode();

      // Deserialize the response into a list of SurveyResult
      var results = await resultsResponse.Content.ReadFromJsonAsync<List<SurveyResult>>();
      Assert.NotNull(results);
      Assert.Contains(results, r => r.OptionId == 1 && r.VoteCount == 1); // Option A should have 1 vote
      Assert.Contains(results, r => r.OptionId == 2 && r.VoteCount == 1); // Option B should have 1 vote
    }
  }
}
