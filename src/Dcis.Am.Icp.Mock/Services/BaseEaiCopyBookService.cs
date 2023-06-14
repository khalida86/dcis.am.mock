using Ato.EN.ApplicationServices.Documents;
using Ato.EN.ApplicationServices.MQ;
using Ato.EN.ApplicationServices.Serialization;
using Dcis.Am.Mock.Icp.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text.Json;

namespace Dcis.Am.Mock.Icp.Services
{
    public abstract class BaseEaiCopyBookService<LogCategoryNameType, RequestType, ResponseType> : BaseService<LogCategoryNameType>
        where RequestType : InteractionDocument, new()
        where ResponseType : InteractionDocument, new()
    {
        public BaseEaiCopyBookService(IOptionsMonitor<ResponseScenarios> scenarios, ILogger<LogCategoryNameType> logger) : base(scenarios, logger) { }

        public override EaiMessage ProcessRequest(EaiMessage eaiRequest, IMessageGateway messageGateway, bool generateDefaultResponse)
        {
            Logger.LogTrace($"{eaiRequest.PutTimestamp}: {eaiRequest.GetStringPayload()}");

            var requestDocument = CreateRequestDocument(eaiRequest);

            if (requestDocument == null)
            {
                Logger.LogTrace("Could not parse request");
                return IcpHelper.CreateCopybookResponse<ResponseType>(new InteractionDocument(), messageGateway, eaiRequest);
            }

            Logger.LogTrace($"Message received: {JsonSerializer.Serialize(requestDocument)}");

            var responseDocument = CreateResponseDocument(requestDocument, generateDefaultResponse);

            Logger.LogTrace($"Message returned: {JsonSerializer.Serialize(responseDocument)}");

            var eaiReponse = IcpHelper.CreateCopybookResponse<ResponseType>(responseDocument, messageGateway, eaiRequest);
            Logger.LogTrace($"Message sent: {eaiRequest.ServiceName} | {eaiRequest.Id} | {eaiRequest.PutTimestamp}");

            return eaiReponse;
        }

        private RequestType CreateRequestDocument(EaiMessage request) {
            string requestPayload = request.GetStringPayload();

            try
            {
                using var reader = new StringReader(requestPayload);
                var serialiser = new CopyBookSerializer(typeof(RequestType));
                return (RequestType)serialiser.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Invalid message");
            }

            return default;
        }

        protected virtual ResponseType CreateNewResponse(RequestType request) => new ResponseType
        {
            Header =
            {
                SessionId = request.Header.SessionId,
                TransactionId = request.Header.TransactionId,
                ApplicationId = 123,
                UserId = "Dcis.Am.Mock.Icp",
                ServiceName = request.Header.ServiceName,
                QualityOfService = 1
            }
        };

        protected abstract ResponseType CreateResponseDocument(RequestType request, bool generateDefaultResponse);

        protected virtual ProcessMessage CreateProcessErrorMessage(int messageCode, ProcessMessageSeverity severity) => new ProcessMessage
        {
            Message = messageCode.ToString(),
            Severity = severity,
            ModuleId = "ICDL1475" // not sure if this is always this value
        };

        protected virtual ResponseType CreateNotFoundResponse(RequestType request, int notFoundErrorCode)
        {
            var response = CreateNewResponse(request);
            response.Header.ProcessMessages.Add(CreateProcessErrorMessage(notFoundErrorCode, ProcessMessageSeverity.Error));
            return response;
        }
    }
}
