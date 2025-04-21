using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.API.DTOs;
using SurveyApp.API.Services.Interfaces;
using FluentValidation;
using SurveyApp.API.Models;

namespace SurveyApp.API.Controllers
{
  /// <summary>
  /// Handles voting operations for surveys.
  /// </summary>
  [ApiController]
  [Route("[controller]")]
  [Authorize]
  [Produces("application/json")]
  public class VoteController : ControllerBase
  {
    private readonly IVoteService _voteService;
    private readonly IValidator<VoteRequest> _voteRequestValidator;

    public VoteController(IVoteService voteService, IValidator<VoteRequest> voteRequestValidator)
    {
      _voteService = voteService;
      _voteRequestValidator = voteRequestValidator;
    }

    /// <summary>
    /// Cast a vote on a survey option.
    /// </summary>
    /// <remarks>
    /// Requires a valid JWT token and a valid VoteRequest.
    /// </remarks>
    /// <param name="voteRequest">The vote request containing SurveyId and OptionId.</param>
    /// <returns>The created vote object.</returns>
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Vote), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
  }
}
