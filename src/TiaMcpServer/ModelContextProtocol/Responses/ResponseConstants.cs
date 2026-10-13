using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseConstants : ResponseMessage
    {
        public IEnumerable<ResponseConstantInfo>? Items { get; set; }
    }
}
