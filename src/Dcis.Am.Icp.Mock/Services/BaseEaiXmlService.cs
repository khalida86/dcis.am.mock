using Ato.EN.ApplicationServices.MQ;
using Ato.EN.Security.Authorisation.AM.Messaging.ICP;
using Dcis.Am.Mock.Icp.Exceptions;
using Dcis.Am.Mock.Icp.Helpers;
using Dcis.Am.Mock.Icp.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Json;

namespace Dcis.Am.Mock.Icp.Services
{
    public abstract class BaseEaiXmlService<LogCategoryNameType, RequestType, ResponseType> : BaseService<LogCategoryNameType>
        where RequestType: IEAIRequest, new()
        where ResponseType: IEAIReply, new()
    {
        protected string RequestPayload;

        public BaseEaiXmlService(IOptionsMonitor<ResponseScenarios> scenarios, ILogger<LogCategoryNameType> logger) : base(scenarios, logger) { }
        
        public override EaiMessage ProcessRequest(EaiMessage eaiRequest, IMessageGateway messageGateway, bool generateDefaultResponse)
        {
            RequestPayload = eaiRequest.GetStringPayload();

            Logger.LogTrace($"{eaiRequest.PutTimestamp}: {RequestPayload}");
            var requestDocument = CreateRequestDocument();

            Logger.LogTrace($"Message received: {JsonSerializer.Serialize(requestDocument)}");

            var responseDocument = CreateResponseDocument(requestDocument, generateDefaultResponse);

            Logger.LogTrace($"Message returned: {JsonSerializer.Serialize(responseDocument)}");

            var eaiReponse = messageGateway.CreateResponse(eaiRequest, XmlHelper.SanitizeMessageDateAndGetXml(responseDocument));
            Logger.LogTrace($"Message sent: {eaiRequest.ServiceName} | {eaiRequest.Id} | {eaiRequest.PutTimestamp}");

            RequestPayload = null;

            return eaiReponse;
        }

        protected virtual ControlType CreateControlHeader(ControlType requestControl, bool generateId = false) => new ControlType
        {
            RequestedService = requestControl.RequestedService,
            MessageDatetime = DateTime.Now,
            MessageDatetimeSpecified = true,
            QualityOfServiceIndicator = requestControl.QualityOfServiceIndicator,
            TAFlags = requestControl.TAFlags,
            MessageSourceID = generateId ? NewGuId : requestControl.MessageSourceID,
            UserLoginText = generateId ? "AMService" : requestControl.UserLoginText,
            SessionID = generateId ? NewGuId : requestControl.SessionID,
            TransactionID = generateId ? NewGuId : requestControl.TransactionID,
        };

        protected virtual ProcessMessageType CreateProcessErrorMessage(int messageCode) => new ProcessMessageType
        {
            MessageCode = messageCode.ToString(),
            SeverityCode = ((int)ClientDataError.GetProcessMessage(messageCode).Severity).ToString(),
            DescriptionText = ClientDataError.GetErrorMessage(messageCode),
            LocationText = "ICDL1475" // not sure if this is always this value
        };

        protected virtual ControlType CreateControlErrorMessage(ControlType requestControl, int messageCode)
        {
            var responseControl = CreateControlHeader(requestControl);

            responseControl.ProcessErrorMsgs = new ProcessMessageType[] { CreateProcessErrorMessage(messageCode) };

            return responseControl;
        }

        protected virtual ResponseType CreateNotFoundResponse(RequestType request, int notFoundErrorCode) => new ResponseType
        {
            Control = CreateControlErrorMessage(request.Control, notFoundErrorCode)
        };

        protected string NewGuId => Guid.NewGuid().ToString("N");

        protected abstract RequestType CreateRequestDocument();

        protected abstract ResponseType CreateResponseDocument(RequestType request, bool generateDefaultResponse);
    }
}
