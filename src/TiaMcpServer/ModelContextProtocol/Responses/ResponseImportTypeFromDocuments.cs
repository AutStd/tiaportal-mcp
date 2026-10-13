using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseImportTypeFromDocuments : ResponseMessage
    {
        public IEnumerable<ResponseTypeInfo>? Items { get; set; }
    }
}
