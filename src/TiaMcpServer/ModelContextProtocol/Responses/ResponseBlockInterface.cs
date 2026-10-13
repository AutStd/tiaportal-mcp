using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseBlockInterface : ResponseMessage
    {
        public string? Path { get; set; }

        public IEnumerable<ResponseInterfaceMember>? Items { get; set; }
    }
}
