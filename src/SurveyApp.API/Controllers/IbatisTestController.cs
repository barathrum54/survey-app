private readonly IIbatisService _ibatis;

public IbatisTestController(IIbatisService ibatis)
{
    _ibatis = ibatis;
}

[HttpGet("ping")]
public IActionResult Ping()
{
    try
    {
        var mapper = _ibatis.Mapper;
        return Ok("IBATIS initialized via service.");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"IBATIS error: {ex.Message}");
    }
}
