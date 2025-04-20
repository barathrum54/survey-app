
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.API.Common;
using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.Services.Interfaces;

namespace SurveyApp.API.Controllers;

[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
  private readonly IUserDao _userDao;
  private readonly IJwtTokenService _jwtTokenService;

  public TestController(IUserDao userDao, IJwtTokenService jwtTokenService)
  {
    _userDao = userDao;
    _jwtTokenService = jwtTokenService;
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
}
