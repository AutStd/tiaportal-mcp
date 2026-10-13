namespace TiaMcpServer.ModelContextProtocol
{
    /// <summary>One object matched by ResolveObjectPath.</summary>
    public class ResponseObjectMatch
    {
        /// <summary>block, type, tag, tagTable, watchTable or source.</summary>
        public string? Kind { get; set; }

        public string? Name { get; set; }

        /// <summary>Root-relative path, ready to pass to the tools of that area.</summary>
        public string? Path { get; set; }
    }
}
