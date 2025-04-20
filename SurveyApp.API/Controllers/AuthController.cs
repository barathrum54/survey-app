using Microsoft.AspNetCore.Mvc;
using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.Models;
using SurveyApp.API.Services.Interfaces;
using SurveyApp.API.DTOs;

namespace SurveyApp.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
  private readonly IUserDao _userDao;
  private readonly IPasswordHasher _passwordHasher;

  public AuthController(IUserDao userDao, IPasswordHasher passwordHasher)
  {
    _userDao = userDao;
    _passwordHasher = passwordHasher;
  }

  [HttpPost("register")]
  public IActionResult Register([FromBody] RegisterRequest request)
  {
    try
    {
      var existing = _userDao.GetByUsername(request.Username);
      if (existing != null)
      {
        return Conflict(ApiResponse<string>.Fail("Username already exists"));
      }

      var hashedPassword = _passwordHasher.Hash(request.Password);
      var user = new User
      {
        Username = request.Username,
        PasswordHash = hashedPassword,
        Email = request.Email
      };

      _userDao.Insert(user);
      return Ok(ApiResponse<object?>.Ok(null, "User registered successfully"));
    }
    catch
    {
      return StatusCode(500, ApiResponse<string>.Fail("Internal server error"));
    }
  }
}