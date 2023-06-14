using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;

namespace Dcis.Am.Mock.Icp.Responses
{
    /// <summary>
    /// Represents a scenario - a specific request/response pair.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public class Scenario<TResponse>
    {
        public string Title { get; set; }
        public RequestScenario Request { get; set; }
        public TResponse ResponseTemplate { get; set; }

        // Given input 'I <like> foo <bar>', the regex will match '<like>' & '<bar>' 
        private static readonly Regex _xpathToken = new Regex(@"<.*?>");

        private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Do not escape characters such as <, >, &, and '
        };

        /// <summary>
        /// Response scenarios can use XPath to copy values from the request to the response.
        /// <para>
        /// The XPath in the response template must be surrounded by diamond braces.
        /// </para>
        /// </summary>
        public TResponse GenerateResponse(XmlDocument requestDocument)
        {
            try
            {
                var responseTemplate = JsonSerializer.Serialize(ResponseTemplate, options: _serializerOptions);

                var matches = _xpathToken.Matches(responseTemplate);
                if (matches.Count > 0)
                {
                    var distinctMatches = matches
                        .GroupBy(m => m.Value)
                        .Select(g => g.First());

                    var finalisedResponse = distinctMatches.Aggregate(
                        responseTemplate, 
                        (current, match) => ReplaceResponseValue(current, match, requestDocument));

                    return JsonSerializer.Deserialize<TResponse>(finalisedResponse, options: _serializerOptions);
                }

                return ResponseTemplate;

            }            
            catch (Exception e)
            {
                throw new Exception($"Error trying to generate response for scenario [{Title}]", e);
            }
        }

        private string ReplaceResponseValue(string responseObject, Match match, XmlDocument requestDocument)
        {
            var replacementText = match.Value;
            var xpath = replacementText.Trim('<', '>');
            var selectedNode = requestDocument.SelectSingleNode(xpath);
            
            if (selectedNode is null)
            {
                throw new Exception($"Could not select node with XPath [{xpath}]");
            }

            return responseObject.Replace(replacementText, selectedNode.InnerText);
        }
    }
}
