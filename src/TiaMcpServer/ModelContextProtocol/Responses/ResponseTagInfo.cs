using Siemens.Engineering;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseTagInfo : ResponseAttributes
    {
        public string? Path { get; set; }
        public string? Name { get; set; }
        public string? TableName { get; set; }
        public string? DataTypeName { get; set; }
        public string? LogicalAddress { get; set; }
        public string? Comment { get; set; }
        public bool? ExternalAccessible { get; set; }
        public bool? ExternalVisible { get; set; }
        public bool? ExternalWritable { get; set; }
        public bool? IsSafety { get; set; }
    }
}
