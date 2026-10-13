using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseWatchTables : ResponseMessage
    {
        public IEnumerable<ResponseWatchTableInfo>? Items { get; set; }
    }
}
