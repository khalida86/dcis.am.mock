namespace Dcis.Am.Mock.Icp
{
    public class MQService
    {
        public string Name { get; set; }
        public Queue InboundQueue { get; set; }
        public Queue OutboundQueue { get; set; }

        public string Queues => $"InboundQueue:{InboundQueue?.Name}|OutboundQueue:{OutboundQueue?.Name}";

        public class Queue
        {
            public string Name { get; set; }
        }
    }
}
