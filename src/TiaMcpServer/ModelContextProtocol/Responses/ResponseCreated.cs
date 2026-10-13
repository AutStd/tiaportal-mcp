namespace TiaMcpServer.ModelContextProtocol
{
    /// <summary>
    /// Shared response shapes for the project-mutating tools in McpServer (the [WriteTool] methods).
    ///
    /// Callers: every [WriteTool] tool in McpServer. Affected API: additive only - new DTOs, no
    /// existing response changed. Data: serialized into MCP structuredContent; all fields are
    /// nullable strings or string collections, no dates. No file I/O.
    ///
    /// Three shapes cover roughly thirty tools deliberately: minting one DTO per operation would
    /// triple the output schema surface for no gain to the caller.
    /// </summary>
    public class ResponseCreated : ResponseMessage
    {
        /// <summary>What was created, e.g. "Tag table", "Block group", "FB".</summary>
        public string? Kind { get; set; }
        public string? Name { get; set; }

        /// <summary>Root-relative path of the new object, where one applies.</summary>
        public string? Path { get; set; }
    }
}
