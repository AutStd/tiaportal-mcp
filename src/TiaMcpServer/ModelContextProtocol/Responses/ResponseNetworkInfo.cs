using System.Collections.Generic;
using TiaMcpServer.Siemens;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseNetworkInfo : ResponseMessage
    {
        public IEnumerable<NetworkNodeInfo>? Items { get; set; }
    }
}
