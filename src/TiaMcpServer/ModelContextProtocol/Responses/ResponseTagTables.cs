using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseTagTables : ResponseMessage
    {
        public IEnumerable<ResponseTagTableInfo>? Items { get; set; }
    }
}
