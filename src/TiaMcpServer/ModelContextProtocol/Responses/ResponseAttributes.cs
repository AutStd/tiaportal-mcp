using Siemens.Engineering;
using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseAttributes : ResponseMessage
    {
        public IEnumerable<Attribute>? Attributes { get; set; }
    }
}
