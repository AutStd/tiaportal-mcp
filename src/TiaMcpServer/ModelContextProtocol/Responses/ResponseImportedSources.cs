using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseImportedSources : ResponseMessage
    {
        public string? Directory { get; set; }

        /// <summary>Objects generated per area: blocks, types.</summary>
        public IDictionary<string, int>? Written { get; set; }

        public IEnumerable<ResponseImportedSourceItem>? Items { get; set; }

        /// <summary>Files that could not be imported, with the reason.</summary>
        public IEnumerable<string>? Failures { get; set; }
    }
}
