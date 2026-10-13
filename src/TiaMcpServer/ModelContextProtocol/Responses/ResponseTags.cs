using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseTags : ResponseMessage
    {
        public IEnumerable<ResponseTagInfo>? Items { get; set; }
    }
}
