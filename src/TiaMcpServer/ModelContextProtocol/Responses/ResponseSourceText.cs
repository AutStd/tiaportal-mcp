using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseSourceText : ResponseMessage
    {
        public string? Name { get; set; }

        public string? Path { get; set; }

        /// <summary>'document' or 'xml'. May differ from the request when the object has no source document.</summary>
        public string? Format { get; set; }

        public string? Text { get; set; }

        /// <summary>Length before truncation, so the caller knows how much was withheld.</summary>
        public int? TotalChars { get; set; }

        public bool? Truncated { get; set; }

        public IEnumerable<string>? FileNames { get; set; }
    }
}
