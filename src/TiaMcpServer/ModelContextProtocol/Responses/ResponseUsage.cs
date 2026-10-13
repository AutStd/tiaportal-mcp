namespace TiaMcpServer.ModelContextProtocol
{
    /// <summary>One object that uses the object WhereUsed was asked about.</summary>
    public class ResponseUsage
    {
        public string? UsedBy { get; set; }

        public string? Path { get; set; }

        public string? Address { get; set; }

        public string? TypeName { get; set; }
    }
}
