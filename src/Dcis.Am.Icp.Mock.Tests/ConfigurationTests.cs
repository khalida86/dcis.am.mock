using FluentAssertions;
using NUnit.Framework;

namespace Dcis.Am.Icp.Mock.Tests
{
    public class ConfigurationTests
    {
        [Test]
        public void Can_Load_Configuration()
        {
            // LoadConfiguration will throw an exception if any of the configuration files (including the scenarios files)
            // are not valid JSON.

            FluentActions
                .Invoking(() => Program.LoadConfiguration(null))
                .Should().NotThrow();
        }
    }
}
