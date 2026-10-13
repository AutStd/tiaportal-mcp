using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseLibraries : ResponseMessage
    {
        public IEnumerable<ResponseLibraryInfo>? Items { get; set; }
    }
}
