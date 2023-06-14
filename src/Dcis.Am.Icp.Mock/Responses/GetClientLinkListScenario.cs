using Ato.EN.Security.Authorisation.AM.Messaging.ICP;
using System;
using System.Collections.Generic;
using System.Text;
using static Ato.EN.Security.Authorisation.AM.Messaging.ICP.GetClientLinkListReponseDocument;

namespace Dcis.Am.Mock.Icp.Responses
{
    public class GetClientLinkListScenario
    {
        public string ExternalId { get; set; }
        public int ExternalIdType { get; set; }
        public List<ClientLinksArrayDocument> ClientLinksArray { get; set; }
    }
}
