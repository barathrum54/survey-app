using System.Xml;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;

namespace SurveyApp.API.Services.IBatis;

public class IbatisService : IIbatisService
{
    public ISqlMapper Mapper { get; }

    public IbatisService()
    {
        var configPath = Path.Combine(AppContext.BaseDirectory, "Data", "Mappers", "sqlmap-config.xml");

        var xmlDoc = new XmlDocument();
        xmlDoc.Load(configPath);

        Console.WriteLine(AppContext.BaseDirectory);
        Console.WriteLine(configPath);
        var builder = new DomSqlMapBuilder();
        var configStream = File.OpenRead(configPath);
        Mapper = builder.Configure(configStream);
    }
}
