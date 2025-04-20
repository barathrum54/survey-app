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
  private readonly IJwtTokenService _jwtTokenService;

  public AuthController(IUserDao userDao, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
  {
    _userDao = userDao;
    _passwordHasher = passwordHasher;
    _jwtTokenService = jwtTokenService;
  }

  [HttpPost("login")]
  public IActionResult Login([FromBody] LoginRequest request)
  {
    var user = _userDao.GetByUsername(request.Username);
    if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
    {
      return Unauthorized(ApiResponse<string>.Fail("Invalid username or password"));
    }

    var token = _jwtTokenService.GenerateToken(Guid.NewGuid(), user.Username);
    return Ok(ApiResponse<string>.Ok(token, "Login successful"));
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