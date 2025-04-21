using Microsoft.AspNetCore.Mvc;
using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.Models;
using SurveyApp.API.Services.Interfaces;
using SurveyApp.API.DTOs;
using FluentValidation;

namespace SurveyApp.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
  private readonly IUserDao _userDao;
  private readonly IPasswordHasher _passwordHasher;
  private readonly IJwtTokenService _jwtTokenService;
  private readonly IValidator<LoginRequest> _loginRequestValidator;
  private readonly IValidator<RegisterRequest> _registerRequestValidator;

  public AuthController(
      IUserDao userDao,
      IPasswordHasher passwordHasher,
      IJwtTokenService jwtTokenService,
      IValidator<LoginRequest> loginRequestValidator,
      IValidator<RegisterRequest> registerRequestValidator)
  {
    _userDao = userDao;
    _passwordHasher = passwordHasher;
    _jwtTokenService = jwtTokenService;
    _loginRequestValidator = loginRequestValidator;
    _registerRequestValidator = registerRequestValidator;

  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest request)
  {
    var validationResult = await _loginRequestValidator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
      return BadRequest(validationResult.Errors);
    }

    var user = _userDao.GetByUsername(request.Username);
    if (user == null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
    {
      return Unauthorized(ApiResponse<string>.Fail("Invalid username or password"));
    }

    var token = _jwtTokenService.GenerateToken(user.Id, user.Username);
    return Ok(ApiResponse<string>.Ok(token, "Login successful"));
  }


  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterRequest request)
  {
    var validationResult = await _registerRequestValidator.ValidateAsync(request);
    if (!validationResult.IsValid)
      return BadRequest(validationResult.Errors);

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
