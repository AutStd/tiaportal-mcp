using Siemens.Engineering;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseLibraryInfo : ResponseAttributes
    {
        /// <summary>Project, UserGlobal, SystemGlobal, CorporateGlobal or Global.</summary>
        public string? Kind { get; set; }

        public string? Name { get; set; }

        /// <summary>File path of a global library; null for the project library.</summary>
        public string? Path { get; set; }

        public bool? IsReadOnly { get; set; }

        public bool? IsModified { get; set; }
    }
}
