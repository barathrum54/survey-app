using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace SurveyApp.API.Tests.Fixtures;

public class DatabaseFixture : IDisposable
{
  private readonly WebApplicationFactory<Program> _factory;
  private readonly IServiceScope _scope;

  public IServiceProvider Services => _scope.ServiceProvider;

  public DatabaseFixture()
  {
    _factory = new WebApplicationFactory<Program>();
    _scope = _factory.Services.CreateScope(); // ✅ scoped properly now
  }

  public T GetService<T>() where T : notnull => Services.GetRequiredService<T>();
  public void Dispose() => _scope.Dispose();
}
