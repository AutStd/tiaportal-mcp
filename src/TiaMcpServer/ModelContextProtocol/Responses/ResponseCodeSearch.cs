using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseCodeSearch : ResponseMessage
    {
        public string? Pattern { get; set; }

        public IEnumerable<ResponseCodeMatch>? Items { get; set; }

        public int? ObjectsSearched { get; set; }

        /// <summary>Objects that could not be read, with the reason.</summary>
        public IEnumerable<string>? Unsearchable { get; set; }

        public bool? Truncated { get; set; }
    }
}
