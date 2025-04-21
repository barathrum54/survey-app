using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.API.DTOs;
using SurveyApp.API.Services.Interfaces;
using FluentValidation;

namespace SurveyApp.API.Controllers
{
  [ApiController]
  [Route("[controller]")]
  [Authorize]
  public class VoteController : ControllerBase
  {
    private readonly IVoteService _voteService;
    private readonly IValidator<VoteRequest> _voteRequestValidator;

    public VoteController(IVoteService voteService, IValidator<VoteRequest> voteRequestValidator)
    {
      _voteService = voteService;
      _voteRequestValidator = voteRequestValidator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateVote([FromBody] VoteRequest voteRequest)
    {
      var validationResult = await _voteRequestValidator.ValidateAsync(voteRequest);
      if (!validationResult.IsValid)
        return BadRequest(validationResult.Errors);

      var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
      try
      {
        var vote = _voteService.AddVote(userId, voteRequest.SurveyId, voteRequest.OptionId);
        return CreatedAtAction(nameof(CreateVote), new { id = vote.Id }, vote);
      }
      catch (InvalidOperationException ex)
      {
        return Conflict(new { message = ex.Message });
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
