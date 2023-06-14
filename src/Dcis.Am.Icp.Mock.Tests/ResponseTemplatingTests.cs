using Dcis.Am.Icp.Mock.Tests.Support;
using Dcis.Am.Mock.Icp.Responses;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Xml;

namespace Dcis.Am.Icp.Mock.Tests
{
    /// <summary>
    /// If you don't want to hardcode values in the response you can specify an XPath to copy a value from the request to the response.
    /// </summary>
    public class ResponseTemplatingTests
    {
        [Test]
        public void Can_Use_Single_XPath_To_Set_Single_Request_Value_In_Response()
        {
            var scenario = Arrange.CreateScenario(
                requestMatchers: Arrange.CreateMatchers((@"./Person/LastName", "Jonson")),
                response: Arrange.CreatePerson(givenName: @"<./Person/GivenName>", lastName: "Jonson", age: "88"));
            var requestDocument = Arrange.CreateXmlDocument(givenName: "Foo", lastName: "Jonson", age: "88");

            var expectedResponse = new Person
            {
                GivenName = "Foo",
                LastName = "Jonson",
                Age = "88"
            };

            var actualResponse = GenerateResponse(scenario, requestDocument);

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }        

        [Test]
        public void Can_Use_Multiple_XPath_To_Set_Multiple_Request_Values_In_Response()
        {
            var scenario = Arrange.CreateScenario(
                requestMatchers: Arrange.CreateMatchers((@"./Person/LastName", "Jonson")),
                response: Arrange.CreatePerson(givenName: @"<./Person/GivenName>", lastName: "Jonson", age: @"<./Person/Age>"));
            var requestDocument = Arrange.CreateXmlDocument(givenName: "Foo", lastName: "Jonson", age: "42");

            var expectedResponse = new Person
            {
                GivenName = "Foo",
                LastName = "Jonson",
                Age = "42"
            };

            var actualResponse = GenerateResponse(scenario, requestDocument);

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void Can_Use_Single_XPath_To_Set_Multiple_Request_Values_In_Response()
        {
            var scenario = Arrange.CreateScenario(
                requestMatchers: Arrange.CreateMatchers((@"./Person/Age", "42")),
                response: Arrange.CreatePerson(givenName: @"<./Person/GivenName>", lastName: @"<./Person/GivenName>", age: "42"));
            var requestDocument = Arrange.CreateXmlDocument(givenName: "Bob", lastName: "Jonson", age: "42");

            var expectedResponse = new Person
            {
                GivenName = "Bob",
                LastName = "Bob",
                Age = "42"
            };

            var actualResponse = GenerateResponse(scenario, requestDocument);

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void Can_Use_XPath_Template_Within_Templated_Value()
        {
            var scenario = Arrange.CreateScenario(
                requestMatchers: Arrange.CreateMatchers((@"./Person/Age", "88")),
                response: Arrange.CreatePerson(givenName: @"<./Person/GivenName>", lastName: @"<./Person/GivenName>LAST", age: "88"));
            var requestDocument = Arrange.CreateXmlDocument(givenName: "Foo", lastName: "Jonson", age: "88");

            var expectedResponse = new Person
            {
                GivenName = "Foo",
                LastName = "FooLAST",
                Age = "88"
            };

            var actualResponse = GenerateResponse(scenario, requestDocument);

            actualResponse.Should().BeEquivalentTo(expectedResponse);
        }

        [Test]
        public void Exception_When_XPath_Does_Not_Select_Node()
        {
            var scenario = Arrange.CreateScenario(
                requestMatchers: Arrange.CreateMatchers((@"./Person/LastName", "Jonson")),
                response: new Person { GivenName = @"<./Person/Invalid>", LastName = "Jonson", Age = "20" }
                );
            var requestDocument = Arrange.CreateXmlDocument(givenName: "Foo", lastName: "Jonson", age: "20");

            FluentActions
                .Invoking(() => scenario.GenerateResponse(requestDocument))
                .Should().Throw<Exception>().WithMessage("Error trying to generate response for scenario [Title]")
                        .WithInnerException<Exception>().WithMessage("Could not select node with XPath [./Person/Invalid]");
        }



        private Person GenerateResponse(Scenario<Person> scenario, XmlDocument requestDocument)
        {
            var person = scenario.GenerateResponse(requestDocument);

            Log.WriteLine($"Generated response: {Environment.NewLine}{JsonConvert.SerializeObject(person, Newtonsoft.Json.Formatting.Indented)}{Environment.NewLine}");

            return person;
        }
    }
}