using Ato.EN.ApplicationServices.Documents;
using Ato.EN.ApplicationServices.MQ;
using Ato.EN.ApplicationServices.Serialization;
using Dcis.Am.Mock.Icp.Helpers;
using System.IO;

namespace Dcis.Am.Mock.Icp.Services
{
    public static class IcpHelper
    {
        public static string DefaultClientDataLevelIndicator { get; internal set; }

        public static EaiMessage CreateCopybookResponse<T>(InteractionDocument responseDocument, IMessageGateway mg, EaiMessage request)
        {
            CopyBookSerializer serialiser = new CopyBookSerializer(typeof(T));

            using StringWriter w = new StringWriter();

            serialiser.Serialize(w, responseDocument);

            EaiMessage response = mg.CreateResponse(request, w.ToString());

            return response;
        }

        public static EaiMessage CreateXMLResponse<T>(T responseDocument, IMessageGateway mg, EaiMessage request)
        {
            string responsePayload = XmlHelper.SanitizeMessageDateAndGetXml(responseDocument);

            EaiMessage response = mg.CreateResponse(request, responsePayload);

            return response;
        }
    }
}
