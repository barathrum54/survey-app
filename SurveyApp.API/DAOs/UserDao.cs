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
  public User Insert(User user)
  {
    Console.WriteLine("📥 DAO Insert START for " + user.Username);
    try
    {
      _mapper.Insert("Users.Insert", user);
      Console.WriteLine("✅ DAO Insert DONE");
      return user;

    }
    catch (Exception ex)
    {
      Console.WriteLine("🔥 DAO ERROR: " + ex.Message);
      Console.WriteLine("🔥 STACK: " + ex.StackTrace);
      throw;
    }
  }
}
