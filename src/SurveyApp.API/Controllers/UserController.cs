using Microsoft.AspNetCore.Mvc;
using SurveyApp.API.Models;
using SurveyApp.API.Services.IBatis;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
  private readonly IIbatisService _ibatisService;

  public UserController(IIbatisService ibatisService)
  {
    _ibatisService = ibatisService;
  }

  [HttpGet("{id:int}")]
  public IActionResult GetUser(int id)
  {
    var user = _ibatisService.Mapper.QueryForObject<User>("User.GetUserById", id);
    return Ok(user);
  }
}
