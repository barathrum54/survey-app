using Microsoft.AspNetCore.Mvc;
using SurveyApp.API.DAOs.Interfaces;

namespace SurveyApp.API.Controllers;

[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
  private readonly IUserDao _userDao;

  public TestController(IUserDao userDao)
  {
    _userDao = userDao;
  }

  [HttpGet("user/{username}")]
  public IActionResult GetUser(string username)
  {
    var user = _userDao.GetByUsername(username);
    return user == null ? NotFound() : Ok(user);
  }
}
