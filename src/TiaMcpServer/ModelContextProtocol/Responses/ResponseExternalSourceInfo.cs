using Siemens.Engineering;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseExternalSourceInfo : ResponseAttributes
    {
        /// <summary>Root-relative path, e.g. "SourceGroup1/Source_1".</summary>
        public string? Path { get; set; }
        public string? Name { get; set; }
    }
}
