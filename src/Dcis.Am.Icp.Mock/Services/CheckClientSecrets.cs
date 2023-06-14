using System;
using System.Collections.Generic;
using System.Linq;
using Ato.EN.ApplicationServices.Documents;
using Ato.EN.Security.Authorisation.AM.Messaging.ICP;
using Dcis.Am.Mock.Icp.Constants;
using Dcis.Am.Mock.Icp.Exceptions;
using Dcis.Am.Mock.Icp.Responses;
using Dcis.Am.Mock.Icp.Responses.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dcis.Am.Mock.Icp.Services
{
    class CheckClientSecrets : BaseEaiCopyBookService<CheckClientSecrets, CheckClientSecretsRequestDocument, CheckClientSecretsResponseDocument>
    {
        private const int MaximumAllowedReturn = 1000;

        public CheckClientSecrets(IOptionsMonitor<ResponseScenarios> scenarios, ILogger<CheckClientSecrets> logger) : base(scenarios, logger) { }

        protected override CheckClientSecretsResponseDocument CreateResponseDocument(CheckClientSecretsRequestDocument request, bool generateDefaultResponse)
        {
            int? icpCode = null;
            var response = Scenarios.CurrentValue.CheckClientSecrets?.FirstOrDefault(x => x.ExternalId == request.ClientExternalId && x.ExternalIdType == request.ClientExternalIdType)?.CheckClientSecretsResponse;
            if(response == null)
            {
                if(ShouldReturnErrorResultCode(Scenarios.CurrentValue.GetIcpErrorMappings, request, out icpCode))
                {
                    AddErrorResponse(response,  icpCode.Value);
                    return response;
                }
                 response = CreateNewResponse(request);
                ProcessMessage message = ClientDataError.GetUnkownErrorMessage();
                response.Header.ProcessMessages.Add(message);
            }
          

            return response;

        }
   
        private static void AddErrorResponse(CheckClientSecretsResponseDocument responseListDocument,  int resultCode)
        {
            ProcessMessage message = ClientDataError.GetProcessMessage(resultCode);
            responseListDocument.Header.ProcessMessages.Add(message);
        }

        private static bool ShouldReturnErrorResultCode(List<IcpErrorMapping> getClientDetailsErrorMappings, CheckClientSecretsRequestDocument clientSecretsRequest, out int? resultCode)
        {
            foreach (var mapping in getClientDetailsErrorMappings)
            {
                if (clientSecretsRequest.ClientExternalId.Equals(mapping.IdentifierValue) && clientSecretsRequest.ClientExternalIdType == mapping.IdentifierType)
                {
                    resultCode = mapping.IcpCode;
                    return true;
                }
            }

            resultCode = default;
            return false;
        }

    }
}
