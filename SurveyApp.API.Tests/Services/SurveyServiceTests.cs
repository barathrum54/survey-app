using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.DTOs;
using SurveyApp.API.Models;
using SurveyApp.API.Services.Interfaces;
using SurveyApp.API.Tests.Fixtures;
using Xunit;

namespace SurveyApp.API.Tests.Services;

public class SurveyServiceTests : IClassFixture<DatabaseFixture>
{
  private readonly ISurveyService _surveyService;
  private readonly IOptionDao _optionDao;

  public SurveyServiceTests(DatabaseFixture fixture)
  {
    _surveyService = fixture.GetService<ISurveyService>();
    _optionDao = fixture.GetService<IOptionDao>();
  }

  [Fact]
  public void CreateSurvey_WithValidData_ShouldPersistSurveyAndOptions()
  {
    // Arrange
    var request = new CreateSurveyRequest
    {
      Title = "Unit Test Survey",
      Options = new List<string> { "Option 1", "Option 2" }
    };

    // Act
    var survey = _surveyService.CreateSurvey(request, userId: 1);

    // Assert
    Assert.NotNull(survey);
    Assert.True(survey.Id > 0);
    Assert.Equal("Unit Test Survey", survey.Title);
    Assert.Equal(1, survey.CreatedBy);

    var options = _optionDao.GetAll().Where(o => o.SurveyId == survey.Id).ToList();
    Assert.Equal(2, options.Count);
    Assert.Contains(options, o => o.Text == "Option 1");
    Assert.Contains(options, o => o.Text == "Option 2");
  }

  [Fact]
  public void CreateSurvey_WithInvalidOptionCount_ShouldThrow()
  {
    // Arrange
    var request = new CreateSurveyRequest
    {
      Title = "Bad Survey",
      Options = new List<string> { "Only One Option" } // invalid, less than 2
    };

    // Act & Assert
    var ex = Assert.Throws<ArgumentException>(() => _surveyService.CreateSurvey(request, userId: 1));
    Assert.Equal("Survey must contain between 2 and 5 options.", ex.Message);
  }
}
