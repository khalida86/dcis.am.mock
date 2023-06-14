using Ato.EN.ApplicationServices.MQ;

namespace Dcis.Am.Mock.Icp.Services
{
    interface IService
    {
        public EaiMessage ProcessRequest(EaiMessage eaiRequest, IMessageGateway messageGateway, bool generateDefaultResponse);
    }
}
