using Siemens.Engineering;
using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseWatchTableInfo : ResponseAttributes
    {
        /// <summary>Root-relative path, e.g. "WatchGroup1/WatchTable_1".</summary>
        public string? Path { get; set; }
        public string? Name { get; set; }

        /// <summary>"Watch" or "Force".</summary>
        public string? Kind { get; set; }
        public bool? IsConsistent { get; set; }
        public int? EntryCount { get; set; }

        /// <summary>Populated by GetWatchTableInfo / GetForceTable, not by the list tools.</summary>
        public IEnumerable<TableEntryInfo>? Entries { get; set; }
    }
}
