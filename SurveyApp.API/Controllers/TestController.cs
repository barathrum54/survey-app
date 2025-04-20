
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

  public TestController(IUserDao userDao, IJwtTokenService jwtTokenService, ISurveyDao surveyDao)
  {
    _userDao = userDao;
    _jwtTokenService = jwtTokenService;
    _surveyDao = surveyDao;
    Console.WriteLine("🧪 SurveyDao injected: " + (surveyDao != null));
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
    var token = _jwtTokenService.GenerateToken(Guid.NewGuid(), "testuser");
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

}
