namespace TiaMcpServer.ModelContextProtocol
{
    /// <summary>One external source file written by a Generate* tool.</summary>
    public class ResponseGeneratedSourceItem
    {
        public string? Name { get; set; }

        /// <summary>Root-relative path of the object in the project.</summary>
        public string? Path { get; set; }

        /// <summary>Absolute path of the file on the machine running the server.</summary>
        public string? File { get; set; }

        /// <summary>The extension Openness demanded: '.db', '.awl', '.scl' or '.udt'.</summary>
        public string? Format { get; set; }

        /// <summary>Programming language of the block, or 'UDT' for a PLC data type.</summary>
        public string? Language { get; set; }

        public long? Size { get; set; }
    }
}
