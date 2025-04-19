using Microsoft.AspNetCore.Mvc;
using SurveyApp.API.Services.IBatis;
using SurveyApp.API.Models;

namespace SurveyApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IbatisTestController : ControllerBase
    {
        private readonly IIbatisService _ibatis;

        public IbatisTestController(IIbatisService ibatis)
        {
            _ibatis = ibatis;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            var mapper = _ibatis.Mapper;
            return Ok("IBATIS initialized via service.");
        }

        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            var users = _ibatis.Mapper.QueryForList<User>("User.SelectAll", null);
            return Ok(users);
        }
    }
}
