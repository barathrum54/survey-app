using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.API.DTOs;
using SurveyApp.API.Services.Interfaces;

namespace SurveyApp.API.Controllers
{
  [ApiController]
  [Route("[controller]")]
  [Authorize]
  public class SurveyController : ControllerBase
  {
    private readonly ISurveyService _surveyService;
    private readonly IVoteService _voteService;

    public SurveyController(ISurveyService surveyService, IVoteService voteService)
    {
      _surveyService = surveyService;
      _voteService = voteService;
    }

    // Create Survey
    [HttpPost]
    public IActionResult CreateSurvey([FromBody] CreateSurveyRequest request)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);

      try
      {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var survey = _surveyService.CreateSurvey(request, userId);
        return CreatedAtAction(nameof(GetSurvey), new { id = survey.Id }, survey);
      }
      catch (Exception ex)
      {
        Console.WriteLine("🔥 CREATE SURVEY ERROR: " + ex.Message);
        Console.WriteLine("🔥 STACK: " + ex.StackTrace);
        return StatusCode(500, ex.Message); // TEMP: surface actual problem
      }
    }

    // Get Survey by ID
    [HttpGet("{id}")]
    [AllowAnonymous]
    public IActionResult GetSurvey(int id)
    {
      var survey = _surveyService.GetSurveyById(id);
      if (survey == null) return NotFound();
      return Ok(survey);
    }

    // Get Surveys by UserId (Created by user)
    [HttpGet("me")]
    public IActionResult GetMySurveys()
    {
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      var surveys = _surveyService.GetSurveysByUserId(userId);
      return Ok(surveys);
    }

    // Delete Survey
    [HttpDelete("{id}")]
    public IActionResult DeleteSurvey(int id)
    {
      try
      {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        _surveyService.DeleteSurvey(id, userId);
        return NoContent();
      }
      catch (KeyNotFoundException)
      {
        return NotFound();
      }
      catch (UnauthorizedAccessException)
      {
        return Forbid();
      }
    }

    [HttpGet("{id}/results")]
    [AllowAnonymous]
    public IActionResult GetSurveyResults(int id)
    {
      var survey = _surveyService.GetSurveyById(id);
      if (survey == null)
      {
        return NotFound(new { message = "Survey not found." });
      }

      var votes = _voteService.GetVotesBySurveyId(id);
      var totalVotes = votes.Count();

      if (totalVotes == 0)
      {
        return Ok(new List<SurveyResult>());  // No votes, return an empty list
      }

      var results = votes
          .GroupBy(vote => vote.OptionId)
          .Select(group => new SurveyResult
          {
            OptionId = group.Key,
            VoteCount = group.Count(),
            Percentage = (double)group.Count() / totalVotes * 100
          })
          .ToList();

      return Ok(results);
    }
  }
}
