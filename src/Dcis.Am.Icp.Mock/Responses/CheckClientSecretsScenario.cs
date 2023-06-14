using Ato.EN.Security.Authorisation.AM.Messaging.ICP;

namespace Dcis.Am.Mock.Icp.Responses
{
    public class CheckClientSecretsScenario
    {
        public string ExternalId { get; set; }
        public int ExternalIdType { get; set; }
        public CheckClientSecretsResponseDocument CheckClientSecretsResponse { get; set; }
    }
}
