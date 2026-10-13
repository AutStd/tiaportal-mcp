using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseReferencedLibraries : ResponseMessage
    {
        public IEnumerable<ResponseReferencedLibrary>? Items { get; set; }
    }
}
