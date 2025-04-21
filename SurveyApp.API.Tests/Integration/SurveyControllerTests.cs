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
    public async Task GetSurveyResults_ShouldReturnMessage_WhenNoVotesExist()
    {
      var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
      _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var request = new CreateSurveyRequest
      {
        Title = "Empty Votes Survey",
        Options = new List<string> { "Option A", "Option B" }
      };

      var createSurveyResponse = await _client.PostAsJsonAsync("/survey", request);
      createSurveyResponse.EnsureSuccessStatusCode();

      var created = await createSurveyResponse.Content.ReadFromJsonAsync<SurveyWithOptionsResponse>();

      var resultsResponse = await _client.GetAsync($"/survey/{created!.Id}/results");
      resultsResponse.EnsureSuccessStatusCode();

      var result = await resultsResponse.Content.ReadFromJsonAsync<ApiResponse<List<SurveyResult>>>();
      Assert.NotNull(result);
      Assert.True(result!.Success);
      Assert.NotNull(result.Data);
      Assert.Empty(result.Data);
      Assert.Equal("No votes yet for this survey.", result.Message);
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

      var apiResponse = await resultsResponse.Content.ReadFromJsonAsync<ApiResponse<List<SurveyResult>>>();
      var results = apiResponse!.Data!; Assert.NotNull(results);
      Assert.NotNull(results);
      Assert.Contains(results, r => r.OptionId == 1 && r.VoteCount == 1 && Math.Abs(r.Percentage - 50.0) < 0.01);
      Assert.Contains(results, r => r.OptionId == 2 && r.VoteCount == 1 && Math.Abs(r.Percentage - 50.0) < 0.01);
    }
    [Fact]
    public async Task GetSurveyResults_ShouldCalculateCorrectPercentages_WithFreshUsers()
    {
      var ownerToken = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
      _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ownerToken);

      var surveyRequest = new CreateSurveyRequest
      {
        Title = "Percentage Test Survey",
        Options = new List<string> { "A", "B" }
      };

      var createResponse = await _client.PostAsJsonAsync("/survey", surveyRequest);
      createResponse.EnsureSuccessStatusCode();
      var survey = await createResponse.Content.ReadFromJsonAsync<Survey>();

      var voters = new[]
      {
    new { Vote = 1, Token = await _client.LoginAndGetTokenAsync($"user_{Guid.NewGuid():N}", "123456", autoRegister: true) },
    new { Vote = 1, Token = await _client.LoginAndGetTokenAsync($"user_{Guid.NewGuid():N}", "123456", autoRegister: true) },
    new { Vote = 2, Token = await _client.LoginAndGetTokenAsync($"user_{Guid.NewGuid():N}", "123456", autoRegister: true) }
  };

      foreach (var v in voters)
      {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", v.Token);
        await _client.PostAsJsonAsync("/vote", new VoteRequest { SurveyId = survey!.Id, OptionId = v.Vote });
      }

      var resultResponse = await _client.GetAsync($"/survey/{survey!.Id}/results");
      resultResponse.EnsureSuccessStatusCode();

      var apiResponse = await resultResponse.Content.ReadFromJsonAsync<ApiResponse<List<SurveyResult>>>();
      var results = apiResponse!.Data;
      Assert.NotNull(results);
      Assert.Contains(results, r => r.OptionId == 1 && r.VoteCount == 2 && Math.Abs(r.Percentage - 66.66) < 1);
      Assert.Contains(results, r => r.OptionId == 2 && r.VoteCount == 1 && Math.Abs(r.Percentage - 33.33) < 1);
    }
    [Fact]
    public async Task CreateSurvey_ShouldReturnBadRequest_WhenOptionsAreInvalid()
    {
      var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
      _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var request = new CreateSurveyRequest
      {
        Title = "Test Survey",
        Options = new List<string> { "Option A" }  // Only 1 option, should fail
      };

      var response = await _client.PostAsJsonAsync("/Survey", request);

      // Assert: Expecting BadRequest due to validation failure
      Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

      var content = await response.Content.ReadAsStringAsync();
      Assert.Contains("At least 2 options are required.", content);  // Check for the validation error message
    }
    [Fact]
    public async Task GetSurvey_ShouldReturnNotFound_WhenSurveyDoesNotExist()
    {
      var response = await _client.GetAsync("/survey/999999");

      Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
    [Fact]
    public async Task GetMySurveys_ShouldReturnEmptyList_WhenNoSurveysExist()
    {
      var newUser = new RegisterRequest
      {
        Username = $"new_{Guid.NewGuid():N}",
        Password = "TempPass123!",
        Email = $"{Guid.NewGuid():N}@mail.com"
      };
      await _client.PostAsJsonAsync("/auth/register", newUser);

      var token = await _client.LoginAndGetTokenAsync(newUser.Username, newUser.Password);
      _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var response = await _client.GetAsync("/survey/me");
      response.EnsureSuccessStatusCode();

      var result = await response.Content.ReadFromJsonAsync<List<Survey>>();
      Assert.NotNull(result);
      Assert.Empty(result!);
    }
    [Fact]
    public async Task CreateSurvey_ShouldAcceptEdgeLimits()
    {
      var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
      _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

      var request = new CreateSurveyRequest
      {
        Title = new string('A', 100),  // Max title length
        Options = new List<string> { "1", "2", "3", "4", "5" }  // Max options
      };

      var response = await _client.PostAsJsonAsync("/survey", request);
      response.EnsureSuccessStatusCode();

      var created = await response.Content.ReadFromJsonAsync<SurveyWithOptionsResponse>();
      Assert.Equal(100, created!.Title.Length);
      Assert.Equal(5, created.Options.Count);
    }
  }

}
