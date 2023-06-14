using System;
using System.Collections.Generic;
using System.Text;

namespace Dcis.Am.Mock.Icp.Exceptions
{
    class IcpException: Exception
    {
        public int ResultCode { get; }

        public IcpException(int code): base(ClientDataError.GetErrorMessage(code))
        {
            ResultCode = code;
        }
    }
}
