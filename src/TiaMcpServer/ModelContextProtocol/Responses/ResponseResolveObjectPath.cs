using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseResolveObjectPath : ResponseMessage
    {
        public IEnumerable<ResponseObjectMatch>? Items { get; set; }
    }
}
