namespace TiaMcpServer.ModelContextProtocol
{
    /// <summary>One line of source text that matched a code search.</summary>
    public class ResponseCodeMatch
    {
        /// <summary>Root-relative path, ready to pass to the GetXml*, GetSource* and GetDocuments* tools.</summary>
        public string? ObjectPath { get; set; }

        /// <summary>'document' or 'xml' - which representation was searched.</summary>
        public string? Format { get; set; }

        /// <summary>1-based line number within that representation.</summary>
        public int? Line { get; set; }

        public string? Text { get; set; }
    }
}
