using Dcis.Am.Mock.Icp;
using Dcis.Am.Mock.Icp.Configuration.Model;
using Dcis.Am.Mock.Icp.Responses;
using Dcis.Am.Mock.Icp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Linq;

namespace Dcis.Am.Icp.Mock
{
    public class Program
    {
        private const string ICP_MOC_ENV_VAR = "ICP_MOCK_ENVIRONMENT";

        private static IConfigurationRoot Configuration;

        public static void Main(string[] args)
        {
            var environment = GetEnvironmentName(args);
            Console.WriteLine($"Using environment: {environment}");
            
            Configuration = LoadConfiguration(environment);
            CreateHostBuilder(args).Build().Run();
        }

        public static string GetEnvironmentName(string[] args)
        {
            var envName = (args.Length > 0)
                ? args[0]
                : Environment.GetEnvironmentVariable(ICP_MOC_ENV_VAR);

            return (string.IsNullOrEmpty(envName))
                ? "Development"
                : envName;
        }

        public static IConfigurationRoot LoadConfiguration(string environment) =>
            new ConfigurationBuilder()
                .AddJsonFile(@"Scenarios\GetClientDetailsV2Config.json", optional: false, reloadOnChange: true)
                .AddJsonFile(@"Scenarios\GetClientLinkListConfig.json", optional: false, reloadOnChange: true)
                .AddJsonFile(@"Scenarios\GetIntermediariesConfig.json", optional: false, reloadOnChange: true)
                .AddJsonFile(@"Scenarios\VerifyProtectedClientLinksConfig.json", optional: false, reloadOnChange: true)
                .AddJsonFile(@"Scenarios\GetIcpErrorConfig.ErrorCodes.json", optional: false, reloadOnChange: true)
                .AddJsonFile(@"Scenarios\GetAccountListSummaryV3Config.json", optional: false, reloadOnChange: true)
                .AddJsonFile(@"Scenarios\CheckClientSecretsConfig.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddNLog(Configuration);
            })
            .ConfigureServices((hostContext, services) =>
            {
                // Register Services
                services.AddSingleton<GetClientDetailsV2>();
                services.AddSingleton<GetClientLinkList>();
                services.AddSingleton<VerifyProtectedClientLinks>();
                services.AddSingleton<GetIntermediaries>();
                services.AddSingleton<GetAccountListSummaryV3>();

                // Register Configuration
                services.AddScoped<IConfiguration>(_ => Configuration);

                // Register MQ
                services.AddMessageGateway(Configuration.GetSection("MQ"));

                // Register Generate Response Config Option - allows for hot reload
                services.Configure<ApplicationConfiguration>(Configuration);

                // Register Response Scenarios
                services.Configure<ResponseScenarios>(Configuration.GetSection("Scenarios"));

                // Register Service Names
                services.AddSingleton(Configuration.GetSection("MQ").GetSection("Services").GetChildren().Select(x => x.Key).ToList());

                // Register ICP Mock Service as a Hosted Background Service
                services.AddHostedService<IcpMockService>();
            })
            .UseWindowsService();
    }
}
