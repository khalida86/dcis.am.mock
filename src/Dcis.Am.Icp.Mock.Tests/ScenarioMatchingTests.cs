using Dcis.Am.Icp.Mock.Tests.Support;
using FluentAssertions;
using NUnit.Framework;

namespace Dcis.Am.Icp.Mock.Tests
{
    public class ScenarioMatchingTests
    {
        [Test]
        public void Matcher_Is_Satisfied_When_Element_Exists_With_Specified_Value()
        {
            var elementSelector = @"./Person/GivenName";
            var elementValue = "Jon";

            var scenario = Arrange.CreateScenario(
                requestMatchers: Arrange.CreateMatchers((elementSelector, elementValue)),
                response: default(Person));
            var requestDocument = Arrange.CreateXmlDocument(givenName: elementValue);

            var isMatch = scenario.Request.IsMatch(requestDocument);

            isMatch.Should().BeTrue();
        }

        [Test]
        public void Matcher_Is_Not_Satisfied_When_Element_Exists_But_Not_With_Specified_Value()
        {
            var elementSelector = @"./Person/GivenName";
            var elementValue = "Jon";

            var scenario = Arrange.CreateScenario(
                requestMatchers: Arrange.CreateMatchers((elementSelector, elementValue)),
                response: default(Person));
            var requestDocument = Arrange.CreateXmlDocument(givenName: "FooBar");

            var isMatch = scenario.Request.IsMatch(requestDocument);

            isMatch.Should().BeFalse();
        }

        [Test]
        public void Matcher_Is_Not_Satisfied_When_Element_Does_Not_Exist()
        {
            var elementSelector = @"./Person/Foo";
            var elementValue = "Jon";

            var scenario = Arrange.CreateScenario(
                requestMatchers: Arrange.CreateMatchers((elementSelector, elementValue)),
                response: default(Person));
            var requestDocument = Arrange.CreateXmlDocument(givenName: elementValue);

            var isMatch = scenario.Request.IsMatch(requestDocument);

            isMatch.Should().BeFalse();
        }

        [Test]
        public void Matcher_Ignores_Invalid_XPath()
        {
            var elementSelector = @"ThisIsNotXPath";
            var elementValue = "Jon";

            var scenario = Arrange.CreateScenario(
                requestMatchers: Arrange.CreateMatchers((elementSelector, elementValue)),
                response: default(Person));
            var requestDocument = Arrange.CreateXmlDocument(givenName: elementValue);

            var isMatch = scenario.Request.IsMatch(requestDocument);

            isMatch.Should().BeFalse();
        }

        [Test]
        public void Is_Match_When_All_Matchers_Are_Satisfied()
        {
            var scenario = Arrange.CreateScenario(
                requestMatchers: Arrange.CreateMatchers((@"./Person/GivenName", "Jon"), (@"./Person/LastName", "Jonson")),
                response: default(Person));
            var requestDocument = Arrange.CreateXmlDocument(givenName: "Jon", lastName: "Jonson");

            var isMatch = scenario.Request.IsMatch(requestDocument);

            isMatch.Should().BeTrue();
        }

        [Test]
        public void Is_Not_Match_When_A_Matcher_Is_Not_Satisfied()
        {
            var scenario = Arrange.CreateScenario(
                requestMatchers: Arrange.CreateMatchers((@"./Person/GivenName", "Jon"), (@"./Person/LastName", "Bobson")),
                response: default(Person));
            var requestDocument = Arrange.CreateXmlDocument(givenName: "Jon", lastName: "Jonson");

            var isMatch = scenario.Request.IsMatch(requestDocument);

            isMatch.Should().BeFalse();
        }        
    }
}
