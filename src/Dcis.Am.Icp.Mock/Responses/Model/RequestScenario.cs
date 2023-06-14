using Dcis.Am.Mock.Icp.Matchers;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Dcis.Am.Mock.Icp.Responses
{
    public class RequestScenario
    {
        public IEnumerable<XPathMatcher> Matchers { get; set; }

        public bool IsMatch(XmlDocument request) => Matchers?.All(m => m.IsMatch(request)) ?? false;
    }
}
