using Siemens.Engineering;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseProjectInfo : ResponseAttributes
    {
        /// <summary>Full path of the project or session file on the server machine.</summary>
        public string? Path { get; set; }
        public string? Name { get; set; }
    }
}
