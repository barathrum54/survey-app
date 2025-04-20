using SurveyApp.API.DAOs;
using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.Models;
using SurveyApp.API.Tests.Fixtures;
using Xunit;

namespace SurveyApp.API.Tests.DAOs;

public class OptionDaoTests : IClassFixture<DatabaseFixture>
{
  private readonly IOptionDao _optionDao;

  public OptionDaoTests(DatabaseFixture fixture)
  {
    _optionDao = fixture.GetService<IOptionDao>(); // no cast
  }
  [Fact]
  public void Insert_And_GetById_ShouldWorkCorrectly()
  {
    var option = new Option
    {
      SurveyId = 1,
      Text = "Test Option",
      CreatedAt = DateTime.UtcNow
    };

    _optionDao.Insert(option);
    var result = _optionDao.GetById(option.Id);

    Assert.NotNull(result);
    Assert.Equal(option.Text, result?.Text);
    Assert.Equal(option.SurveyId, result?.SurveyId);
  }
}
