using Dcis.Am.Mock.Icp.Responses;
using System.Collections.Generic;

namespace Dcis.Am.Mock.Icp.Services
{
    public class VerifyProtectedClientLinksScenario : ResponseBase
    {
        public AuthenticatedClient AuthenticatedClient { get; set; }

        public AuthenticatedAccountRole AuthenticatedAccountRole { get; set; }

        public List<Client> SubjectClientList { get; set; }

    }

    public class AuthenticatedClient
    {
        // ExternalClientId
        public string ClientIdentifierValueID { get; set; }

        // ExternalClientTypeCode
        public string ClientIdentifierTypeCode { get; set; }
    }

    public class AuthenticatedAccountRole
    {
        public string ClientInternalId { get; set; }

        // IntermediaryTypeCode
        public string ClientIdentifierTypeCode { get; set; }

        // IntermediaryId
        public string ClientIdentifierValueID { get; set; }

        public string ClientAccountId { get; set; }

        // RoleTypeCode
        public string ClientRoleTypeCode { get; set; }
    }

    public class Client
    {
        public List<ClientIdentifier> ClientIdentifiers { get; set; }

        public ClientAccount ClientAccount { get; set; }

        public List<ClientLink> ClientLinks { get; set; }

        public ClientSecurity ClientSecurity { get; set; }

    }

    public class ClientIdentifier
    {
        public string ClientInternalId { get; set; }

        public string SelectedClientTypeCode { get; set; }

        public string SelectedClientId { get; set; }
    }

    public class ClientAccount
    {
        public string ClientAccountId { get; set; }

        public string ClientAccountTypeCode { get; set; }
    }

    public class ClientLink
    {
        public string ClientLinkTypeCode { get; set; }
    }

    public class ClientSecurity
    {
        public string ClassificationCode { get; set; }
    }
}