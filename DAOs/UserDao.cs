using SqlBatis.DataMapper;
using SurveyApp.API.DAOs.Interfaces;
using SurveyApp.API.Models;

namespace SurveyApp.API.DAOs;

public class UserDao : IUserDao
{
  private readonly ISqlMapper _mapper;

  public UserDao(ISqlMapper mapper)
  {
    _mapper = mapper;
  }

  public User? GetByUsername(string username)
  {
    return _mapper.QueryForObject<User>("Users.GetByUsername", username);
  }
}
