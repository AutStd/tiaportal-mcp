using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseWhereUsed : ResponseMessage
    {
        /// <summary>The resolved path of the object that was looked up.</summary>
        public string? Path { get; set; }

        public string? Kind { get; set; }

        public IEnumerable<ResponseUsage>? Items { get; set; }
    }
}
