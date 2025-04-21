using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SurveyApp.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
  [HttpGet("/healthz")]
  [AllowAnonymous]
  public IActionResult GetHealth()
  {
    return Ok(new
    {
      status = "ok",
      timestamp = DateTime.UtcNow.ToString("o")
    });
  }
}
