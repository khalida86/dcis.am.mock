using Ato.EN.ApplicationServices.Documents;
using Ato.EN.Security.Authorisation.AM.Messaging.ICP;
using Dcis.Am.Mock.Icp.Constants;
using Dcis.Am.Mock.Icp.Exceptions;
using Dcis.Am.Mock.Icp.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using static Ato.EN.Security.Authorisation.AM.Messaging.ICP.GetClientLinkListReponseDocument;

namespace Dcis.Am.Mock.Icp.Services
{
    class GetClientLinkList : BaseEaiCopyBookService<GetClientLinkList, GetClientLinkListRequestDocument, GetClientLinkListReponseDocument>
    {
        private const int MaximumAllowedReturn = 1000;

        public GetClientLinkList(IOptionsMonitor<ResponseScenarios> scenarios, ILogger<GetClientLinkList> logger) : base(scenarios, logger) { }

        protected override GetClientLinkListReponseDocument CreateResponseDocument(GetClientLinkListRequestDocument request, bool generateDefaultResponse)
        {
            var response = CreateEmptyGetClientLinkListReponseDocument(request);

            try
            {
                List<ClientLinksArrayDocument> links = MatchResponse(request, Scenarios.CurrentValue.GetClientLinkList);

                if (links == null)
                {
                    response = CreateEmptyGetClientLinkListReponseDocument(request);
                    ProcessMessage message = ClientDataError.GetProcessMessage(GetClientLinkListResultCodes.LinkDoesNotExist);
                    response.Header.ProcessMessages.Add(message);
                }
                else
                {
                    var pageSize = request.Paging.MaxRecordsFetch;
                    if (links.Count > MaximumAllowedReturn && pageSize > MaximumAllowedReturn)
                    {
                        ProcessMessage message = ClientDataError.GetProcessMessage(GetClientLinkListResultCodes.MoreRecordsExist);
                        response.Header.ProcessMessages.Add(message);
                        pageSize = MaximumAllowedReturn;
                    }

                    var index = (request.Paging.PageIndex - 1) * pageSize;

                    if (index > links.Count)
                    {
                        ProcessMessage message = ClientDataError.GetProcessMessage(GetClientLinkListResultCodes.InvalidNextKey);
                        response.Header.ProcessMessages.Add(message);

                        response.ClientLinksArray = null;
                        response.ClientLinksKeyFields.LinkTotalCount = links.Count;
                        response.PagingOut.PageIndex = request.Paging.PageIndex;
                        response.PagingOut.RecordsReturned = 0;
                        response.PagingOut.TotalRecordsReturned = links.Count;
                    }
                    else
                    {
                        var linksPage = links.GetRange(index, Math.Min(request.Paging.MaxRecordsFetch, links.Count - index));
                        response.ClientLinksArray = linksPage;
                        response.ClientLinksKeyFields.LinkTotalCount = links.Count;

                        response.PagingOut.PageIndex = request.Paging.PageIndex;
                        response.PagingOut.RecordsReturned = linksPage.Count;
                        response.PagingOut.TotalRecordsReturned = links.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                // Problems serializing the Tech Exceptions, added as ProcessMessage
                // response.Header.SetTechnicalException(new TechnicalException("An unkown error occured", ex));
                response = CreateEmptyGetClientLinkListReponseDocument(request);
                ProcessMessage message = ClientDataError.GetUnkownErrorMessage();
                message.Message = ex.Message;
                response.Header.ProcessMessages.Add(message);
            }

            return response;

        }

        private GetClientLinkListReponseDocument CreateEmptyGetClientLinkListReponseDocument(GetClientLinkListRequestDocument request)
        {
            var response = CreateNewResponse(request);

            response.ClientLinksKeyFields = new ClientLinksKeyFieldsDocument();
            response.ClientLinksArray = new List<ClientLinksArrayDocument>();
            response.PagingOut = new PagingOutDocument();

            return response;
        }

        private static List<ClientLinksArrayDocument> MatchResponse(GetClientLinkListRequestDocument request, List<GetClientLinkListScenario> clientList)
        {
            return clientList.Where(x => x.ExternalId.Equals(request.ClientLinkKeyFields.ClientExternalID)
                                    && x.ExternalIdType.Equals(request.ClientLinkKeyFields.ClientIDType))
                             .Select(x => x.ClientLinksArray)
                             .FirstOrDefault();
        }
    }
}
