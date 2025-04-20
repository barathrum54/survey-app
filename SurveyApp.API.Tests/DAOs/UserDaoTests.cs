using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlBatis.DataMapper.DependencyInjection;
using SurveyApp.API.DAOs;
using SurveyApp.API.DAOs.Interfaces;

namespace SurveyApp.API.Tests.DAOs;

public class UserDaoTests
{
  private readonly IUserDao _userDao;

  public UserDaoTests()
  {
    var host = Host.CreateDefaultBuilder()
        .ConfigureServices((context, services) =>
        {
          services.AddSqlMapper(options =>
                  context.Configuration.GetSection("DB").Bind(options));
          services.AddScoped<IUserDao, UserDao>();
        })
        .Build();

    _userDao = host.Services.GetRequiredService<IUserDao>();
  }

  [Fact]
  public void GetByUsername_ShouldReturn_AdminUser()
  {
    // Act
    var user = _userDao.GetByUsername("admin");

    // Assert
    Assert.NotNull(user);
    Assert.Equal("admin", user.Username);
    Assert.Equal("rJaJ4ickJwheNbnT4+i+2IyzQ0gotDuG/AWWytTG4nA=", user.PasswordHash);
    Assert.True(user.Id > 0);
  }
}