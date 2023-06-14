using Dcis.Am.Mock.Icp.Helpers;
using Dcis.Am.Mock.Icp.Matchers;
using Dcis.Am.Mock.Icp.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Dcis.Am.Icp.Mock.Tests.Support
{
    /// <summary>
    /// For creating objects during the arrange phase.
    /// </summary>
    internal static class Arrange
    {
        public static Scenario<TResponse> CreateScenario<TResponse>(
            IEnumerable<XPathMatcher> requestMatchers,
            TResponse response,
            string title = "Title")
        {
            return CreateScenario(new RequestScenario { Matchers = requestMatchers }, response, title);
        }

        public static Scenario<TResponse> CreateScenario<TResponse>(
            RequestScenario request,
            TResponse response,
            string title = "Title")
        {
            var scenario = new Scenario<TResponse>
            {
                Title = title,
                Request = request,
                ResponseTemplate = response
            };

            Log.WriteLine($"Given scenario: {Environment.NewLine}{JsonConvert.SerializeObject(scenario, Newtonsoft.Json.Formatting.Indented)}{Environment.NewLine}");

            return scenario;
        }

        public static Person CreatePerson(string givenName = "givenname", string lastName = "lastname", string age = "42")
        {
            return new Person
            {
                GivenName = givenName,
                LastName = lastName,
                Age = age
            };
        }

        public static IEnumerable<XPathMatcher> CreateMatchers(params (string, string)[] matchers)
        {
            return matchers.Select(m => new XPathMatcher { Path = m.Item1, Value = m.Item2 });
        }

        public static XmlDocument CreateXmlDocument(string givenName = "givenname", string lastName ="lastname", string age = "42")
        {
            string xml = $@"
<Person>
    <GivenName>{givenName}</GivenName>
    <LastName>{lastName}</LastName>
    <Age>{age}</Age>
</Person>
";
            Log.WriteLine($"Given request document: {xml}{Environment.NewLine}");

            return XmlHelper.LoadWithoutNamespace(xml);
        }
    }
}
