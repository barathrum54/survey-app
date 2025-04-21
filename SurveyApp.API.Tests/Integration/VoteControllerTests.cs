using System.Net.Http.Headers;
using System.Net.Http.Json;
using SurveyApp.API.DTOs;
using SurveyApp.API.Models;
using SurveyApp.API.Tests.Extensions;
using SurveyApp.API.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace SurveyApp.API.Tests.Integration;

public class VoteControllerTests : IClassFixture<DatabaseFixture>
{
  private readonly HttpClient _client;
  private readonly ITestOutputHelper _output;

  public VoteControllerTests(DatabaseFixture fixture, ITestOutputHelper output)
  {
    _client = fixture._factory.CreateClient();
    _output = output;
  }

  // Test for casting a valid vote
  [Fact]
  public async Task Vote_CastingVote_ShouldReturnCreated_WhenVoteIsValid()
  {
    var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    // First create a survey
    var createSurveyRequest = new CreateSurveyRequest
    {
      Title = "Test Survey for Voting",
      Options = new List<string> { "Option A", "Option B" }
    };

    var createSurveyResponse = await _client.PostAsJsonAsync("/Survey", createSurveyRequest);
    createSurveyResponse.EnsureSuccessStatusCode();
    var createdSurvey = await createSurveyResponse.Content.ReadFromJsonAsync<Survey>();

    // Now cast a vote for the survey
    var voteRequest = new VoteRequest
    {
      SurveyId = createdSurvey!.Id,
      OptionId = 1 // Voting for Option A
    };

    var voteResponse = await _client.PostAsJsonAsync("/Vote", voteRequest);
    var content = await voteResponse.Content.ReadAsStringAsync();

    voteResponse.EnsureSuccessStatusCode();
    _output.WriteLine("Raw Response: " + content);

    // Assert vote was created successfully
    var result = await voteResponse.Content.ReadFromJsonAsync<Vote>();
    Assert.NotNull(result);
    Assert.Equal(voteRequest.SurveyId, result!.SurveyId);
    Assert.Equal(voteRequest.OptionId, result.OptionId);
  }

  // Test for casting multiple votes for the same survey
  [Fact]
  public async Task Vote_CastingMultipleVotesForSameSurvey_ShouldReturnConflict()
  {
    var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    // First create a survey
    var createSurveyRequest = new CreateSurveyRequest
    {
      Title = "Test Survey for Voting",
      Options = new List<string> { "Option A", "Option B" }
    };

    var createSurveyResponse = await _client.PostAsJsonAsync("/Survey", createSurveyRequest);
    createSurveyResponse.EnsureSuccessStatusCode();
    var createdSurvey = await createSurveyResponse.Content.ReadFromJsonAsync<Survey>();

    // First vote for the survey
    var voteRequest1 = new VoteRequest
    {
      SurveyId = createdSurvey!.Id,
      OptionId = 1
    };

    var voteResponse1 = await _client.PostAsJsonAsync("/vote", voteRequest1);
    voteResponse1.EnsureSuccessStatusCode();

    // Try to cast another vote for the same survey
    var voteRequest2 = new VoteRequest
    {
      SurveyId = createdSurvey!.Id,
      OptionId = 2 // Trying to vote again on the same survey
    };

    var voteResponse2 = await _client.PostAsJsonAsync("/vote", voteRequest2);

    // Assert the second vote is rejected (Conflict)
    Assert.Equal(System.Net.HttpStatusCode.Conflict, voteResponse2.StatusCode);
  }

  // Test for attempting to vote without being authenticated
  [Fact]
  public async Task Vote_CastingVote_ShouldReturnForbidden_WhenUserIsNotAuthenticated()
  {
    // Attempt to cast a vote without a token
    var voteRequest = new VoteRequest
    {
      SurveyId = 1,
      OptionId = 1
    };

    var voteResponse = await _client.PostAsJsonAsync("/vote", voteRequest);

    // Assert the response is Forbidden (401 Unauthorized or 403 Forbidden)
    Assert.Equal(System.Net.HttpStatusCode.Unauthorized, voteResponse.StatusCode);
  }

  [Fact]
  public async Task Vote_GetVotesBySurvey_ShouldReturnAllVotes()
  {
    var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    // First create a survey
    var createSurveyRequest = new CreateSurveyRequest
    {
      Title = "Test Survey for Voting",
      Options = new List<string> { "Option A", "Option B" }
    };

    var createSurveyResponse = await _client.PostAsJsonAsync("/survey", createSurveyRequest);
    createSurveyResponse.EnsureSuccessStatusCode();
    var createdSurvey = await createSurveyResponse.Content.ReadFromJsonAsync<Survey>();

    // Cast a vote
    var voteRequest = new VoteRequest
    {
      SurveyId = createdSurvey!.Id,
      OptionId = 1
    };

    // POST /vote instead of /votes
    await _client.PostAsJsonAsync("/vote", voteRequest);  // Change here

    // Retrieve votes for the survey
    var voteResponse = await _client.GetAsync($"/vote/{createdSurvey!.Id}/results");  // Change here
    voteResponse.EnsureSuccessStatusCode();

    var votes = await voteResponse.Content.ReadFromJsonAsync<List<Vote>>();
    Assert.NotNull(votes);
    Assert.NotEmpty(votes);
    Assert.All(votes, v => Assert.Equal(createdSurvey.Id, v.SurveyId));
  }
  [Fact]
  public async Task Vote_ShouldReturnBadRequest_WhenSurveyIdIsInvalid()
  {
    var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234"); // Get a valid token
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    var request = new VoteRequest
    {
      SurveyId = 0,  // Invalid SurveyId
      OptionId = 1
    };

    var response = await _client.PostAsJsonAsync("/vote", request);

    // Assert: Expecting BadRequest due to validation failure
    Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
  }
  [Fact]
  public async Task Vote_ShouldReturnBadRequest_WhenOptionIdIsInvalid()
  {
    var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    var request = new VoteRequest
    {
      SurveyId = 1,
      OptionId = 0  // Invalid OptionId
    };

    var response = await _client.PostAsJsonAsync("/vote", request);

    Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
  }
  [Fact]
  public async Task Vote_GetSurveyResults_ShouldReturnEmpty_WhenNoVotesExist()
  {
    var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    var surveyRequest = new CreateSurveyRequest
    {
      Title = "No Votes Survey",
      Options = new List<string> { "Option A", "Option B" }
    };

    var createResponse = await _client.PostAsJsonAsync("/survey", surveyRequest);
    var created = await createResponse.Content.ReadFromJsonAsync<Survey>();

    var resultResponse = await _client.GetAsync($"/vote/{created!.Id}/results");

    resultResponse.EnsureSuccessStatusCode();
    var results = await resultResponse.Content.ReadFromJsonAsync<List<Vote>>();

    Assert.NotNull(results);
    Assert.Empty(results);
  }
  [Fact]
  public async Task Vote_ShouldReturnBadRequest_WhenOptionDoesNotBelongToSurvey()
  {
    var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    var createRequest = new CreateSurveyRequest
    {
      Title = "Survey Mismatch Test",
      Options = new List<string> { "A", "B" }
    };

    var surveyResponse = await _client.PostAsJsonAsync("/survey", createRequest);
    var createdSurvey = await surveyResponse.Content.ReadFromJsonAsync<Survey>();

    var request = new VoteRequest
    {
      SurveyId = createdSurvey!.Id,
      OptionId = 9999  // likely doesn't exist
    };

    var voteResponse = await _client.PostAsJsonAsync("/vote", request);

    Assert.Equal(System.Net.HttpStatusCode.BadRequest, voteResponse.StatusCode);
  }
  [Fact]
  public async Task Vote_ShouldReturnBadRequest_WhenSurveyDoesNotExist()
  {
    var token = await _client.LoginAndGetTokenAsync("admin2", "admin1234");
    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    var request = new VoteRequest
    {
      SurveyId = 9999,  // non-existent
      OptionId = 1
    };

    var voteResponse = await _client.PostAsJsonAsync("/vote", request);

    Assert.Equal(System.Net.HttpStatusCode.BadRequest, voteResponse.StatusCode);
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

    var results = await resultResponse.Content.ReadFromJsonAsync<List<SurveyResult>>();
    Assert.NotNull(results);
    Assert.Contains(results, r => r.OptionId == 1 && r.VoteCount == 2 && Math.Abs(r.Percentage - 66.66) < 1);
    Assert.Contains(results, r => r.OptionId == 2 && r.VoteCount == 1 && Math.Abs(r.Percentage - 33.33) < 1);
  }
  [Fact]
  public async Task CreateVote_ShouldReturnUnauthorized_IfUserNotAuthenticated()
  {
    var voteRequest = new VoteRequest
    {
      SurveyId = 1,
      OptionId = 1
    };

    var response = await _client.PostAsJsonAsync("/vote", voteRequest);

    Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
  }



}
