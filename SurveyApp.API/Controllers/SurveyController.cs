using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.API.DTOs;
using SurveyApp.API.Services.Interfaces;

namespace SurveyApp.API.Controllers;

[ApiController]
[Route("survey")]
[Authorize]
[Produces("application/json")]
public class SurveyController : ControllerBase
{
  private readonly ISurveyService _surveyService;
  private readonly IVoteService _voteService;

  public SurveyController(ISurveyService surveyService, IVoteService voteService)
  {
    _surveyService = surveyService;
    _voteService = voteService;
  }

  /// <summary>
  /// Creates a new survey with options.
  /// </summary>
  /// <param name="request">Survey title and options</param>
  /// <returns>Created survey details</returns>
  [HttpPost]
  [ProducesResponseType(typeof(SurveyWithOptionsResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(StatusCodes.Status400BadRequest)]
  [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
      return StatusCode(500, ex.Message);
    }
  }

  /// <summary>
  /// Retrieves a specific survey by ID.
  /// </summary>
  [HttpGet("{id}")]
  [AllowAnonymous]
  [ProducesResponseType(typeof(SurveyWithOptionsResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public IActionResult GetSurvey(int id)
  {
    var survey = _surveyService.GetSurveyById(id);
    if (survey == null) return NotFound();
    return Ok(survey);
  }

  /// <summary>
  /// Gets all surveys created by the current user.
  /// </summary>
  [HttpGet("me")]
  [ProducesResponseType(typeof(IEnumerable<SurveyWithOptionsResponse>), StatusCodes.Status200OK)]
  public IActionResult GetMySurveys()
  {
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    var surveys = _surveyService.GetSurveysByUserId(userId);
    return Ok(surveys);
  }

  /// <summary>
  /// Deletes a survey owned by the current user.
  /// </summary>
  [HttpDelete("{id}")]
  [ProducesResponseType(StatusCodes.Status204NoContent)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesResponseType(StatusCodes.Status403Forbidden)]
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

  /// <summary>
  /// Returns the vote results for a given survey.
  /// </summary>
  [HttpGet("{id}/results")]
  [AllowAnonymous]
  [ProducesResponseType(typeof(List<SurveyResult>), StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  public IActionResult GetSurveyResults(int id)
  {
    var survey = _surveyService.GetSurveyById(id);
    if (survey == null)
      return NotFound(new { message = "Survey not found." });

    var votes = _voteService.GetVotesBySurveyId(id);
    var totalVotes = votes.Count();

    if (totalVotes == 0)
    {
      return Ok(ApiResponse<List<SurveyResult>>.Ok([], "No votes yet for this survey."));
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

    return Ok(ApiResponse<List<SurveyResult>>.Ok(results, "Survey vote results calculated successfully."));
  }
}
