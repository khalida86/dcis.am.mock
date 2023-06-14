using Ato.EN.ApplicationServices.MQ;
using Dcis.Am.Mock.Icp.Configuration.Model;
using Dcis.Am.Mock.Icp.Constants;
using Dcis.Am.Mock.Icp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dcis.Am.Mock.Icp.Extensions;

namespace Dcis.Am.Mock.Icp
{
    public class IcpMockService : BackgroundService
    {
        /* 1. Load the configuration into a dictionary item
         * 2. Enter a loop that looks at the queue
         *      2a. If there is a file, parse it into the request object
         *      2b. Get the contract then get the identifier from the contract
         *      2c. Map the data id from 2b to the key for the dictionary item
         *      2d. Form a response object and place it on the queue
         * 3. Sleep for x amount of time
         * 4. Goto 2.
         *     
        */

        private const string ClientDetailsServiceName = "GetClientDetailsV2";
        private const string VerifyProtectedClientLinksServiceName = "VerifyProtectedClientLinks";
        private const string GetAccountListSummaryV3ServiceName = "GetAccountListSummaryV3";

        private readonly ILogger<IcpMockService> _logger;
        private readonly int _timeout;
        private readonly IMessageGatewayFactory _messageGatewayFactory;
        private readonly IEnumerable<string> _services;
        private readonly IOptionsMonitor<ApplicationConfiguration> _applicationConfig;
        private readonly IServiceProvider _serviceProvider;

        public IcpMockService(
            IMessageGatewayFactory messageGatewayFactory, 
            ILogger<IcpMockService> logger, 
            IConfiguration config,
            IOptionsMonitor<ApplicationConfiguration> applicationConfig,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _messageGatewayFactory = messageGatewayFactory;
            // we will only get the unique queues to avoid multiple listener to that queue            
            _services = config.GetSection("MQ:Services").Get<IEnumerable<MQService>>().DistinctBy(x => x.Queues).Select(x => x.Name).ToList();
            _timeout = config.GetValue<int>("Timeout");
            _applicationConfig = applicationConfig;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Task> taskList = new List<Task>();
            foreach (string serviceName in _services)
            {
                taskList.Add(Task.Run(() => ProcessMessagesForService(serviceName, stoppingToken)));
            }

            await Task.WhenAll(taskList);
        }

        private void ProcessMessagesForService(string serviceName, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                using (IMessageGateway eaiGateway = _messageGatewayFactory.Create(serviceName))
                {
                    EaiMessage eaiMessage;

                    try
                    {
                        eaiMessage = eaiGateway.ReceiveRequest(_timeout);
                        
                        if (IsValidRequest(eaiMessage))
                        {
                            _logger.LogTrace($"New Message from {serviceName}");
                          
                            IService service = CreateService(eaiMessage.GetStringPayload());

                            EaiMessage response = service.ProcessRequest(eaiMessage, eaiGateway, _applicationConfig.CurrentValue.GenerateDefaultResponses);

                            eaiGateway.SendResponse(response);
                        }
                    }
                    catch (TimeoutException)
                    {  }
                    catch (Exception exception)
                    {
                        _logger.LogError(exception, $"Error occurred in {serviceName}: ");
                    }
                }
            }
        }

        private bool IsValidRequest(EaiMessage eaiMessage)
        {
            if (string.IsNullOrWhiteSpace(eaiMessage?.GetStringPayload()))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// create service based on payload
        /// </summary>
        /// <param name="request">The service request</param>
        /// <returns></returns>
        private IService CreateService(string request) {
            string serviceName;
            string serviceNameType;

            try
            {
                // eai xml
                var xmlDoc = Helpers.XmlHelper.LoadWithoutNamespace(request);

                serviceName = xmlDoc.DocumentElement.SelectSingleNode("//Control/RequestedService")?.InnerText;
            }
            catch (Exception)
            {
                // copy book
                serviceName = request.Split(' ')?.FirstOrDefault();
            }

            serviceNameType = serviceName;

            switch (serviceName)
            {
                case ServiceConstants.GetClientDtls2:
                    serviceNameType = ClientDetailsServiceName;
                    break;
                case ServiceConstants.VrfyProCltLinks2:
                    serviceNameType = VerifyProtectedClientLinksServiceName;
                    break;
                case ServiceConstants.GetAccountListSummaryV3:
                    serviceNameType = GetAccountListSummaryV3ServiceName;
                    break;
            }

            return _serviceProvider.GetService(Type.GetType($"Dcis.Am.Mock.Icp.Services.{serviceNameType}, Dcis.Am.Mock.Icp")) as IService;
        }


    }
}


