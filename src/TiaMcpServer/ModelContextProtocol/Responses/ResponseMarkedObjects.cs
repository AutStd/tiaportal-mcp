using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseMarkedObjects : ResponseMessage
    {
        /// <summary>The names of the marked objects, in the same order as Items.</summary>
        public IEnumerable<string>? Names { get; set; }

        public IEnumerable<ResponseMarkedObjectInfo>? Items { get; set; }
    }
}
