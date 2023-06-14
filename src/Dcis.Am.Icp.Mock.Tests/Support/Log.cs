using NUnit.Framework;

namespace Dcis.Am.Icp.Mock.Tests.Support
{
    internal static class Log
    {
        public static void WriteLine(string message)
        {
            TestContext.WriteLine(message);
        }
    }
}
