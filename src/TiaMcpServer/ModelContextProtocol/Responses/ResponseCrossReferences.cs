using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseCrossReferences : ResponseMessage
    {
        public IEnumerable<CrossRefSource>? Sources { get; set; }
        public int? SourceCount { get; set; }
        public int? ReferenceCount { get; set; }

        /// <summary>
        /// True when maxDepth cut the result short, so the caller knows to re-query a specific
        /// object with a higher depth rather than assuming the tree was complete.
        /// </summary>
        public bool? Truncated { get; set; }
    }
}
