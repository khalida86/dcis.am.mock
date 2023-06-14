using Ato.EN.ApplicationServices.Documents;
using Ato.EN.Security.Authorisation.AM.Messaging.ICP;
using Dcis.Am.Mock.Icp.Constants;
using Dcis.Am.Mock.Icp.Helpers;
using Dcis.Am.Mock.Icp.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace Dcis.Am.Mock.Icp.Services
{
    public class GetIntermediaries : BaseEaiXmlService<GetIntermediaries, EAIGetIntermediariesRequest, EAIGetIntermediariesReply>
    {
        private static int maxRecords = 1000;

        public GetIntermediaries(IOptionsMonitor<ResponseScenarios> scenarios, ILogger<GetIntermediaries> logger) : base(scenarios, logger) { }

        protected override EAIGetIntermediariesRequest CreateRequestDocument()
        {
            var request = XmlHelper.DeserializeFromXml<EAIGetIntermediariesRequest>(RequestPayload);

            request.Control = CreateControlHeader(request.Control, true);
            request.ClientDataLevelIndicator = IcpHelper.DefaultClientDataLevelIndicator;

            return request;
        }

        protected override EAIGetIntermediariesReply CreateResponseDocument(EAIGetIntermediariesRequest request, bool generateDefaultResponse)
        {
            EAIGetIntermediariesReply reply = null;

            foreach (var s in Scenarios.CurrentValue.GetIntermediaries)
            {
                if (s.Client.ClientIdentifiers.Where(x => x.ClientIdentifierValueID == request.Client.ClientIdentifiers[0].ClientIdentifierValueID &&
                    x.ClientIdentifierTypeCode == request.Client.ClientIdentifiers[0].ClientIdentifierTypeCode).Count() == 1)
                {
                    reply = s;
                    break;
                }
            }

            if (reply == null)
                return GetNotFoundResponse(request);

            reply.Control = CreateControlHeader(request.Control);

            if (reply.Client.ClientIdentifiers.Any(x => x.ClientIdentifierValueID.Equals("162210001")))
                reply.ActiveRanDetails = GenerateArrayLargerThanMax();

            if (reply.ActiveRanDetails.Length > maxRecords)
            {
                reply.ClientIdentifierTaxAgentCount = maxRecords.ToString();
                reply.ActiveRanDetails = reply.ActiveRanDetails.Take(maxRecords).ToArray();
                reply.Control.ProcessErrorMsgs = new ProcessMessageType[] {
                    new ProcessMessageType()
                    {
                        //TODO confirm code and message
                        MessageCode = GetIntermiediariesResultCodes.MaxReturnExceeded.ToString(),
                        SeverityCode = ((int)ProcessMessageSeverity.Information).ToString(),
                        DescriptionText = "The request has returned the first 1000 records retrieved, there are more records available."
                    }
                };
            }
            return reply;
        }

        private EAIGetIntermediariesReply GetNotFoundResponse(EAIGetIntermediariesRequest request)
        {
            var response = base.CreateNotFoundResponse(request, 11020);

            response.ClientIdentifierTaxAgentCount = "0";

            return response;
        }

        private ActiveRanDetailType[] GenerateArrayLargerThanMax()
        {
            List<ActiveRanDetailType> list = new List<ActiveRanDetailType>();
            for (int i = 0; i < maxRecords + 1; i++)
            {
                var ran = new ActiveRanDetailType()
                {
                    Client = new ClientType()
                    {
                        ClientIdentifiers = new ClientIdentifierType[] {
                            new ClientIdentifierType()
                            {
                                ClientRoleTypeCode = "90",
                                ClientAccountID = $"Account-{i}",
                                ClientIdentifierValueID = i.ToString(),
                                ClientIdentifierTypeCode = "15"
                            }
                        },
                        ClientAccount = new ClientAccountType[]
                        {
                            new ClientAccountType() {
                                ClientAccountName = new ClientNameType[] {
                                    new ClientNameType()
                                    {
                                        UnstructuredName = new UnstructuredNameType()
                                        {
                                            UnstructuredFullName = $"John Citizen - {i}"
                                        }
                                    }
                                }
                            }
                        }

                    }
                };
                list.Add(ran);
            }
            return list.ToArray();

        }
    }
}
