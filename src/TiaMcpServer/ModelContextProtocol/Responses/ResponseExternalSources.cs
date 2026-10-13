using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseExternalSources : ResponseMessage
    {
        public IEnumerable<ResponseExternalSourceInfo>? Items { get; set; }
    }
}
