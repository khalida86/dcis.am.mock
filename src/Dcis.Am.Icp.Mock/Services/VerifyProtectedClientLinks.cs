using Ato.EN.Security.Authorisation.AM.Messaging.ICP;
using Dcis.Am.Mock.Icp.Helpers;
using Dcis.Am.Mock.Icp.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Dcis.Am.Mock.Icp.Services
{
    class VerifyProtectedClientLinks : BaseEaiXmlService<VerifyProtectedClientLinks, EAIVerifyProClntLinksRequest, EAIVerifyProClntLinksReply>
    {
        // Constants used for generic response, when enabled
        private const string ACCOUNT_ID = "000069004639";
        private const string TAX_AGENT_LINK_CODE = "90";
        private const string CLIENT_LINK_CODE = "100";
        private const string SECURITY_CLASSIFICATION_CODE = "60";

        public VerifyProtectedClientLinks(IOptionsMonitor<ResponseScenarios> scenarios, ILogger<VerifyProtectedClientLinks> logger) : base(scenarios, logger) { }

        protected override EAIVerifyProClntLinksRequest CreateRequestDocument()
        {
            var request = XmlHelper.DeserializeFromXml<EAIVerifyProClntLinksRequest>(RequestPayload);

            request.Control = CreateControlHeader(request.Control);

            return request;
        }

        protected override EAIVerifyProClntLinksReply CreateResponseDocument(EAIVerifyProClntLinksRequest request, bool generateDefaultResponse)
        {
            var requestDocument = XmlHelper.LoadWithoutNamespace(RequestPayload);
            var matchedScenario = Scenarios.CurrentValue.VerifyProtectedClientLinks?.FirstOrDefault(s => s.Request.IsMatch(requestDocument));

            EAIVerifyProClntLinksReply reply;
            if (matchedScenario is null)
            {
                Logger.LogTrace("Response scenario not found.");

                if (generateDefaultResponse)
                {
                    Logger.LogTrace("Returning generic response.");
                    reply = GenerateGenericResponse(request);
                }
                else
                {
                    reply = GenerateResponseWithProcessMessage(request, 11020);
                }
            }
            else
            {
                Logger.LogTrace($"Matched scenario: {matchedScenario.Title}");
                reply = MapResponseFromScenario(matchedScenario, request, requestDocument);
            }

            return reply;
        }

        private EAIVerifyProClntLinksReply GenerateResponseWithProcessMessage(EAIVerifyProClntLinksRequest request, int messageCode)
        {
            var response = new EAIVerifyProClntLinksReply();
            response.Control = GenerateControlHeader(request.Control, response);
            response.Control.ProcessErrorMsgs = new ProcessMessageType[]
            {
                CreateProcessErrorMessage(messageCode)
            };

            return response;
        }

        // Creates a default response based on the incoming request. 
        // - Caters for requests with (RA/RA-Client) and without (Individual) Authenticated Account Role.
        // - Caters for requests that provide a Client Internal Id rather than an ABN/TFN
        private EAIVerifyProClntLinksReply GenerateGenericResponse(EAIVerifyProClntLinksRequest request)
        {
            bool externalClientEqualsSelectedClient = request.AuthenticatedClient.ClientIdentifierValueID == request.SubjectClientList.Client?.SingleOrDefault().ClientIdentifiers.SingleOrDefault().ClientIdentifierValueID;

            // Create Authenticated Client
            var authenticatedClient = new ClientIdentifierType();
            string authenticatedClientValue = null;
            if (request.AuthenticatedClient != null)
            {
                authenticatedClientValue = request.AuthenticatedClient.ClientIdentifierValueID;
                authenticatedClient.ClientIdentifierValueID = authenticatedClientValue;
                authenticatedClient.ClientIdentifierTypeCode = request.AuthenticatedClient.ClientIdentifierTypeCode;
            }

            // If no client value provided, use client internal id instead
            if (authenticatedClientValue == null)
                authenticatedClientValue = request.AuthenticatedClient.ClientInternalID;

            // Create Authenticate Account Role
            var authenticatedAccountRole = new ClientIdentifierType();
            // Always return the Client Internal Id in Authenticated Account Role
            authenticatedAccountRole.ClientInternalID = GenerateClientInternalId(authenticatedClientValue);

            // Only return below values when a TAN is provided in the request
            if (request.AuthenticatedAccountRole != null && request.AuthenticatedAccountRole.ClientIdentifierValueID != null)
            {
                authenticatedAccountRole.ClientIdentifierValueID = request.AuthenticatedAccountRole.ClientIdentifierValueID;
                authenticatedAccountRole.ClientIdentifierTypeCode = request.AuthenticatedAccountRole.ClientIdentifierTypeCode;
                authenticatedAccountRole.ClientAccountID = ACCOUNT_ID;
                authenticatedAccountRole.ClientRoleTypeCode = TAX_AGENT_LINK_CODE;
            }

            // Create a Subject Client, if one was provided in the request
            ListOfClientsType subjectClientList = null;
            if (request.SubjectClientList != null && request.SubjectClientList.Client != null  && !externalClientEqualsSelectedClient)
            {
                ClientIdentifierType identifierType = new ClientIdentifierType()
                {
                    ClientIdentifierValueID = request.SubjectClientList.Client.SingleOrDefault().ClientIdentifiers.SingleOrDefault().ClientIdentifierValueID,
                    ClientIdentifierTypeCode = request.SubjectClientList.Client.SingleOrDefault().ClientIdentifiers.SingleOrDefault().ClientIdentifierTypeCode,
                    ClientInternalID = GenerateClientInternalId(request.SubjectClientList.Client.SingleOrDefault().ClientIdentifiers.SingleOrDefault().ClientIdentifierValueID)
                };

                ClientAccountType[] accountTypes = null;
                ListOfClientLinkType clientLinks = null;
                ClientSecurityDetailsType clientSecurity = null;

                // Only return below values if TAN provided in the request.
                if (request.AuthenticatedAccountRole != null && request.AuthenticatedAccountRole.ClientIdentifierValueID != null)
                {
                    ClientLinkType linkType = new ClientLinkType()
                    {
                        ClientLinkTypeCode = CLIENT_LINK_CODE
                    };

                    clientLinks = new ListOfClientLinkType()
                    {
                        ClientLink = new ClientLinkType[] { linkType }
                    };

                    clientSecurity = new ClientSecurityDetailsType()
                    {
                        ClientSecurityDetailSecurityClassificationCode = SECURITY_CLASSIFICATION_CODE
                    };
                }

                ClientType clientType = new ClientType()
                {
                    ClientIdentifiers = new ClientIdentifierType[] { identifierType },
                    ClientAccount = accountTypes,
                    ClientLinks = clientLinks,
                    ClientSecurity = clientSecurity
                };

                subjectClientList = new ListOfClientsType()
                {
                    Client = new ClientType[] { clientType }
                };
            }

            // Build response
            EAIVerifyProClntLinksReply response = new EAIVerifyProClntLinksReply();
            response.AuthenticatedClient = authenticatedClient;
            response.AuthenticatedAccountRole = authenticatedAccountRole;
            response.SubjectClientList = subjectClientList;
            response.RestartKey = new ClientType();
            response.SubjectClientCount = (externalClientEqualsSelectedClient) ? "1" : subjectClientList?.Client?.Length.ToString();
            response.RecordsCount = subjectClientList?.Client?.Length.ToString();
            response.Control = GenerateControlHeader(request.Control, response);

            return response;
        }

        private ControlType GenerateControlHeader(ControlType requestControl, EAIVerifyProClntLinksReply response, IEnumerable<ProcessMsg> processMessages = null)
        {

            var controlHeader = CreateControlHeader(requestControl);
            controlHeader.HasPayload = (response.AuthenticatedClient != null) ? "1" : "0";
            controlHeader.ProcessErrorMsgs = processMessages?.Select(x => CreateProcessErrorMessage(x.MessageCode)).ToArray();
            controlHeader.EAIHeader = new EAIHeaderType[] {
                    new EAIHeaderType
                    {
                        RequestOriginalMQData = "VrfyProCltLinks2^AM2CLOUD^AM.COMMON.REPLY.H^"
                    }
                };

            return controlHeader;
        }

        // Creates a Client Internal Id of 13 characters from a provided client value
        private string GenerateClientInternalId(string clientIdentifierValue)
        {
            return clientIdentifierValue.PadRight(13, '0').Substring(0, 13);
        }

        private EAIVerifyProClntLinksReply MapResponseFromScenario(Scenario<VerifyProtectedClientLinksScenario> matchedScenario, EAIVerifyProClntLinksRequest request, XmlDocument requestDocument)
        {
            var replyData = matchedScenario.GenerateResponse(requestDocument);

            int? uniqueSubjectClients = replyData.SubjectClientList?
                                        .Select(client => client?.ClientIdentifiers?.First().ClientInternalId)?
                                        .Distinct()?
                                        .Count() ?? 0;

            // Map selected clients
            ListOfClientsType subjectClientList = null;

            if (replyData.SubjectClientList != null)
            {
                List<ClientType> selectedClients = new List<ClientType>();

                foreach (var subjectClient in replyData.SubjectClientList)
                {
                    ClientIdentifierType identifierType = new ClientIdentifierType()
                    {
                        ClientIdentifierValueID = subjectClient.ClientIdentifiers?.First().SelectedClientId,
                        ClientIdentifierTypeCode = subjectClient.ClientIdentifiers?.First().SelectedClientTypeCode,
                        ClientInternalID = subjectClient.ClientIdentifiers?.First().ClientInternalId
                    };

                    ClientAccountType[] accountTypes = null;
                    if (subjectClient.ClientAccount != null)
                    {
                        ClientAccountType accountType = new ClientAccountType()
                        {
                            ClientAccountID = subjectClient.ClientAccount.ClientAccountId,
                            ClientAccountTypeCode = subjectClient.ClientAccount.ClientAccountTypeCode
                        };

                        accountTypes = new ClientAccountType[] { accountType };
                    }

                    ListOfClientLinkType clientLinks = null;
                    if (subjectClient.ClientLinks?.First() != null)
                    {
                        ClientLinkType linkType = new ClientLinkType()
                        {
                            ClientLinkTypeCode = subjectClient.ClientLinks.First().ClientLinkTypeCode
                        };

                        clientLinks = new ListOfClientLinkType()
                        {
                            ClientLink = new ClientLinkType[] { linkType }
                        };
                    }

                    ClientSecurityDetailsType clientSecurity = null;
                    if (subjectClient.ClientSecurity != null)
                    {
                        clientSecurity = new ClientSecurityDetailsType()
                        {
                            ClientSecurityDetailSecurityClassificationCode = subjectClient.ClientSecurity.ClassificationCode
                        };
                    }

                    ClientType clientType = new ClientType()
                    {
                        ClientIdentifiers = new ClientIdentifierType[] { identifierType },
                        ClientAccount = accountTypes,
                        ClientLinks = clientLinks,
                        ClientSecurity = clientSecurity
                    };

                    selectedClients.Add(clientType);
                }

                subjectClientList = new ListOfClientsType()
                {
                    Client = selectedClients.ToArray()
                };
            }

            string requestExternalClient = request.AuthenticatedClient.ClientIdentifierValueID;
            string requestSelectedClient = request.SubjectClientList?.Client?.First().ClientIdentifiers?.First().ClientIdentifierValueID;

            var authenticatedClient = new ClientIdentifierType();
            if (replyData.AuthenticatedClient != null)
            {
                authenticatedClient.ClientIdentifierValueID = replyData.AuthenticatedClient.ClientIdentifierValueID;
                authenticatedClient.ClientIdentifierTypeCode = replyData.AuthenticatedClient.ClientIdentifierTypeCode;
            }

            var authenticatedAccountRole = new ClientIdentifierType();
            if (replyData.AuthenticatedAccountRole != null)
            {
                authenticatedAccountRole.ClientInternalID = replyData.AuthenticatedAccountRole.ClientInternalId;
                authenticatedAccountRole.ClientIdentifierTypeCode = replyData.AuthenticatedAccountRole.ClientIdentifierTypeCode;
                authenticatedAccountRole.ClientIdentifierValueID = replyData.AuthenticatedAccountRole.ClientIdentifierValueID;
                authenticatedAccountRole.ClientAccountID = replyData.AuthenticatedAccountRole.ClientAccountId;
                authenticatedAccountRole.ClientRoleTypeCode = replyData.AuthenticatedAccountRole.ClientRoleTypeCode;
            }

            var response = new EAIVerifyProClntLinksReply()
            {
                AuthenticatedClient = authenticatedClient,
                AuthenticatedAccountRole = authenticatedAccountRole,
                RestartKey = new ClientType(),
                SubjectClientCount = (requestExternalClient == requestSelectedClient) ? "1" : uniqueSubjectClients?.ToString(),
                RecordsCount = replyData.SubjectClientList?.Count().ToString() ?? "0",
                SubjectClientList = subjectClientList
            };

            if (response.SubjectClientList?.Client.Count() == 0)
            {
                response.SubjectClientList = null;
            }

            response.Control = GenerateControlHeader(request.Control, response, replyData.ProcessMessages);

            return response;
        }
    }
}
