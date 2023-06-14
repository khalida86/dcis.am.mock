using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dcis.Am.Ram.Mock
{
    public class KnownException : ApplicationException
    {
        public KnownException(string internalCode, string description)
        {
            InternalCode = internalCode;
            Description = description;
        }

        public string InternalCode { get; }
        public string Description { get; }

        public override string ToString()
        {
            return $"KnownException - InternalCode {InternalCode}, Description {Description}";
        }
    }
}
