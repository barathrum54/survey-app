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
  public class VoteController : ControllerBase
  {
    private readonly IVoteService _voteService;

    public VoteController(IVoteService voteService)
    {
      _voteService = voteService;
    }

    [HttpPost]
    public IActionResult CreateVote([FromBody] VoteRequest voteRequest)
    {
      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      try
      {
        var vote = _voteService.AddVote(userId, voteRequest.SurveyId, voteRequest.OptionId);
        return CreatedAtAction(nameof(CreateVote), new { id = vote.Id }, vote);
      }
      catch (InvalidOperationException ex)
      {
        // Handle the case when a user has already voted on the survey
        return Conflict(new { message = ex.Message });  // Return 409 Conflict
      }
      catch (Exception ex)
      {
        return BadRequest(new { message = ex.Message });
      }
    }

    [HttpGet("{surveyId}/results")]
    [AllowAnonymous]
    public IActionResult GetSurveyResults(int surveyId)
    {
      var votes = _voteService.GetVotesBySurveyId(surveyId);
      return Ok(votes);
    }
  }
}

