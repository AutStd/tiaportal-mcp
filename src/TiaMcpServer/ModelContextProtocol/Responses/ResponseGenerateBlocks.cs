using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseGenerateBlocks : ResponseMessage
    {
        public IEnumerable<string>? GeneratedNames { get; set; }
        public int? Count { get; set; }
    }
}
