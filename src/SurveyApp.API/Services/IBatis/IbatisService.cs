
namespace SurveyApp.API.Services.IBatis;

public class IbatisService : IIbatisService
{
    public ISqlMapper Mapper { get; }

    public IbatisService()
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "Data", "Mappers", "sqlmap-config.xml");
        var builder = new DomSqlMapBuilder();
        Mapper = builder.Configure(configPath);
    }
}
