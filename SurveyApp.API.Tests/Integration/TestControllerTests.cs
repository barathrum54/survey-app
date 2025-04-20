using System.Net.Http.Headers;
using System.Net.Http.Json;
using SurveyApp.API.Middleware;
using SurveyApp.API.Tests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace SurveyApp.API.Tests.Integration
{
  public class TestControllerTests : IClassFixture<DatabaseFixture>
  {
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public TestControllerTests(DatabaseFixture fixture, ITestOutputHelper output)
    {
      _client = fixture._factory.CreateClient();
      _output = output;
    }

    [Fact]
    public async Task GlobalExceptionHandler_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
      // Act: Call the endpoint that triggers an error
      var response = await _client.GetAsync("/test/trigger-error");

      // Assert: Check that the status code is 500 (Internal Server Error)
      Assert.Equal(System.Net.HttpStatusCode.InternalServerError, response.StatusCode);

      // Read the response content
      var errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();

      // Assert: Check that the error message and detail are included
      Assert.NotNull(errorResponse);
      Assert.Equal("An unexpected error occurred.", errorResponse.Message);
      Assert.Contains("This is a test exception for global error handling.", errorResponse.Detail);
    }
  }
}
