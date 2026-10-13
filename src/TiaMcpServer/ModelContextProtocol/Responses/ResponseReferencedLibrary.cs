using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseReferencedLibrary
    {
        /// <summary>Name of the project or library the types were generated from.</summary>
        public string? Name { get; set; }

        /// <summary>
        /// Kind of the open library with this name (UserGlobal, SystemGlobal, CorporateGlobal), or
        /// null when no library of that name is open, which also holds for the project itself.
        /// </summary>
        public string? OpenLibraryKind { get; set; }

        /// <summary>File path of the open global library with this name, when there is one.</summary>
        public string? OpenLibraryPath { get; set; }

        public int TypeVersionCount { get; set; }

        public IEnumerable<ResponseReferencedLibraryType>? Types { get; set; }
    }
}
