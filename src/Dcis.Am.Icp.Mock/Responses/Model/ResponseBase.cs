
using System.Collections.Generic;

namespace Dcis.Am.Mock.Icp.Responses
{
    public abstract class ResponseBase
    {
        public IEnumerable<ProcessMsg> ProcessMessages { get; set; }
    }
}
