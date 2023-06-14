using Ato.EN.Security.Authorisation.AM.Messaging.ICP;
using Dcis.Am.Mock.Icp.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Dcis.Am.Mock.Icp.Services
{
    public class GetAccountListSummaryV3 : BaseEaiCopyBookService<GetAccountListSummaryV3, GetAcctListSmry3RequestDocument, GetAcctListSmry3ResponseDocument>
    {
        public GetAccountListSummaryV3(IOptionsMonitor<ResponseScenarios> scenarios, ILogger<GetAccountListSummaryV3> logger) : base(scenarios, logger) { }

        protected override GetAcctListSmry3ResponseDocument CreateResponseDocument(GetAcctListSmry3RequestDocument request, bool generateDefaultResponse)
        {
            GetAcctListSmry3ResponseDocument response = CreateNewResponse(request);

            var matchedScenario = Scenarios.CurrentValue.GetAccountListSummariesV3?.FirstOrDefault(x => x.ClientInternalID == request.ClientInternalID);

            if (matchedScenario != default(GetAcctListSmry3ResponseDocument))
            {
                response.ClientInternalID = matchedScenario.ClientInternalID;
                response.accountSummaryCount = matchedScenario.accountSummaryCount;
                response.RecordStartNumber = request.RecordStartNumber;
                response.RecordTotalCount = matchedScenario.RecordTotalCount;
                response.PageSizeNumber = request.PageSizeNumber;
                response.AccountSummaryDetails = matchedScenario.AccountSummaryDetails;
            }
            // Test cases rely on no FIRB account being returned when matching scenario is not found in the mock.
            // Hence ignoring generateDefaultResponse parameter.
            else
                return CreateNotFoundResponse(request, 40199);

            return response;
        }
    }
}
