namespace TiaMcpServer.ModelContextProtocol
{
    public class CompileMessage
    {
        /// <summary>Object the message belongs to, as TIA Portal reports it.</summary>
        public string? Path { get; set; }

        /// <summary>Success, Information, Warning or Error.</summary>
        public string? State { get; set; }

        public string? Description { get; set; }

        public int? ErrorCount { get; set; }

        public int? WarningCount { get; set; }

        /// <summary>Nesting level in the original message tree; 0 is a direct child of the result.</summary>
        public int? Depth { get; set; }
    }
}
