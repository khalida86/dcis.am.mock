using Ato.EN.Security.Authorisation.AM.Messaging.ICP;
using Dcis.Am.Mock.Icp.Responses.Model;
using Dcis.Am.Mock.Icp.Services;
using System.Collections.Generic;

namespace Dcis.Am.Mock.Icp.Responses
{
    public class ResponseScenarios
    {
        public List<ClientDetailScenario> GetClientDetailsV2 { get; set; }
        public List<IcpErrorMapping> GetIcpErrorMappings { get; set; }
        public List<GetClientLinkListScenario> GetClientLinkList { get; set; }
        public List<EAIGetIntermediariesReply> GetIntermediaries { get; set; }
        public List<Scenario<VerifyProtectedClientLinksScenario>> VerifyProtectedClientLinks { get; set; }
        public List<GetAcctListSmry3ResponseDocument> GetAccountListSummariesV3 { get; set; }
        public List<CheckClientSecretsScenario> CheckClientSecrets { get; set; }
        
    }
}
