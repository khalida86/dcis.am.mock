namespace Dcis.Am.Mock.Icp.Responses.Model
{
    public class IcpErrorMapping
    {
        public int IdentifierType { get; set; }
        public string IdentifierValue { get; set; }
        public int IcpCode { get; set; }

        public override string ToString() => $"{IdentifierType}/{IdentifierValue} [{IcpCode}]";
    }
}