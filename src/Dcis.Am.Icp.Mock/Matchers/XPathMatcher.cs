using System;
using System.Text.RegularExpressions;
using System.Xml;

namespace Dcis.Am.Mock.Icp.Matchers
{
    /// <summary>
    /// Defines a criteria to match against an XML document.
    /// </summary>
    public class XPathMatcher
    {
        /// <summary>
        /// The XPath statement for selecting a single node within XML.
        /// </summary>
        /// <remarks>
        /// Only handles simple location paths. Does not handle complex predicates or any XPath functions.
        /// </remarks>
        public string Path { get; set; }
        
        /// <summary>
        /// The required value of the node selected by <see cref="Path"/>.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Returns <see langword="true"/> if and only if the first element selected by `Path` in <paramref name="request"/> has inner text equal to `Value`. 
        /// </summary>
        public bool IsMatch(XmlDocument request)
        {
            var selectedToken = request.SelectSingleNode(Path);

            if (selectedToken is null)
                return false;

            var val = (string)Value;

            if (val.StartsWith('<'))
            {
                Regex matchPattern = new Regex(@$"{val.TrimStart('<').TrimEnd('>')}", RegexOptions.Compiled, TimeSpan.FromSeconds(1));
                return matchPattern.IsMatch(selectedToken.InnerText);
            }
            else
            {
                return selectedToken.InnerText.Equals(Value);
            }
        }
    }
}
