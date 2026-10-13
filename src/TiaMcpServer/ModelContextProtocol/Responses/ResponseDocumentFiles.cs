using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    /// <summary>
    /// Files one source document export produced. The list comes from TIA Portal rather than
    /// from an assumed '.s7dcl' name, so the caller learns what is actually on disk.
    /// </summary>
    public class ResponseDocumentFiles
    {
        public string? Name { get; set; }

        public string? Directory { get; set; }

        public IEnumerable<string>? Files { get; set; }

        /// <summary>Openness messages, present when the export was not a plain success.</summary>
        public IEnumerable<string>? Messages { get; set; }

        /// <summary>Success, PartialSuccess or Failure.</summary>
        public string? State { get; set; }
    }
}
