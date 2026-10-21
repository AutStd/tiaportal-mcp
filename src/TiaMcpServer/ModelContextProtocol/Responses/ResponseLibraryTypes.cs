using System.Collections.Generic;
using TiaMcpServer.Siemens;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseLibraryTypeInstances : ResponseMessage
    {
        public IEnumerable<LibraryTypeInstanceEntry>? Items { get; set; }
    }

    public class ResponseLibraryTypeVersion : ResponseMessage
    {
        public LibraryTypeVersionEntry? Version { get; set; }
    }

    public class ResponseLibraryTypeExport : ResponseLibraryTypeVersion
    {
        public IEnumerable<string>? Files { get; set; }
    }
}
