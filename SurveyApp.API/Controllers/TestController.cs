
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.API.DTOs;
using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.Services.Interfaces;
using SurveyApp.API.Models;

namespace SurveyApp.API.Controllers;

[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
  private readonly IUserDao _userDao;
  private readonly IJwtTokenService _jwtTokenService;
  private readonly ISurveyDao _surveyDao;
  private readonly IOptionDao _optionDao;

  public TestController(IUserDao userDao, IJwtTokenService jwtTokenService, ISurveyDao surveyDao, IOptionDao optionDao)
  {
    _optionDao = optionDao;
    _userDao = userDao;
    _jwtTokenService = jwtTokenService;
    _surveyDao = surveyDao;
    Console.WriteLine("🧪 OptionDao injected: " + (optionDao != null));
  }

  [Authorize]
  [HttpGet("user/{username}")]
  public IActionResult GetUser(string username)
  {
    var user = _userDao.GetByUsername(username);
    if (user == null)
      return NotFound(ApiResponse<string>.Fail("User not found"));

    return Ok(ApiResponse<object>.Ok(user));
  }

  [AllowAnonymous]
  [HttpPost("token")]
  public IActionResult GetToken()
  {
    var token = _jwtTokenService.GenerateToken(1, "testuser"); // Replace Guid.NewGuid() with an integer value
    return Ok(ApiResponse<string>.Ok(token));
  }
  [HttpPost("survey")]
  public IActionResult TestInsertSurvey()
  {
    var survey = new Survey
    {
      Title = "Manual Survey",
      CreatedBy = 1,
      CreatedAt = DateTime.UtcNow
    };

    _surveyDao.Insert(survey);
    var result = _surveyDao.GetById(survey.Id);

    return Ok(result);
  }
  [HttpPost("option")]
  public IActionResult TestInsertOption()
  {
    var option = new Option
    {
      Text = "Manual Survey",
      SurveyId = 1,
      CreatedAt = DateTime.UtcNow
    };

    _optionDao.Insert(option);
    var result = _optionDao.GetById(option.Id);

    return Ok(result);
  }
  [HttpGet("trigger-error")]
  public IActionResult TriggerError()
  {
    // This will simulate an unhandled exception
    throw new Exception("This is a test exception for global error handling.");
  }

}
