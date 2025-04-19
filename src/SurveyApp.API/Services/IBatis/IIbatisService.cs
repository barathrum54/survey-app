using IBatisNet.DataMapper;

namespace SurveyApp.API.Services.IBatis;

public interface IIbatisService
{
    ISqlMapper Mapper { get; }
}
