namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseMarkedObjectInfo
    {
        /// <summary>Block, Type, TagTable, WatchTable, ForceTable, ExternalSource, or the Openness type name.</summary>
        public string? Kind { get; set; }

        public string? Name { get; set; }

        /// <summary>Path usable with the other tools; null when the object kind has no path helper.</summary>
        public string? Path { get; set; }

        public bool IsMarkedByMe { get; set; }

        public bool IsMarkedByOthers { get; set; }

        public bool IsUpToDate { get; set; }
    }
}
