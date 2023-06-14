using Ato.EN.ApplicationServices.MQ;
using Dcis.Am.Mock.Icp.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dcis.Am.Mock.Icp.Services
{
    public abstract class BaseService<LogCategoryNameType> : IService
    {
        protected readonly ILogger<LogCategoryNameType> Logger;
        protected readonly IOptionsMonitor<ResponseScenarios> Scenarios;

        public BaseService(IOptionsMonitor<ResponseScenarios> scenarios, ILogger<LogCategoryNameType> logger)
        {
            Logger = logger;
            Scenarios = scenarios;
        }

        public abstract EaiMessage ProcessRequest(EaiMessage eaiRequest, IMessageGateway messageGateway, bool generateDefaultResponse);
    }
}
