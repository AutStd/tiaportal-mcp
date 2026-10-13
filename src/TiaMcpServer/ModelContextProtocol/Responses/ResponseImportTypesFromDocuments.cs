using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseImportTypesFromDocuments : ResponseMessage
    {
        public IEnumerable<ResponseTypeInfo>? Items { get; set; }
    }
}
