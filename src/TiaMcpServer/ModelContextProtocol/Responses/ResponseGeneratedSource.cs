namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseGeneratedSource : ResponseMessage
    {
        public string? Name { get; set; }

        public string? Path { get; set; }

        public string? File { get; set; }

        public string? Format { get; set; }

        public string? Language { get; set; }

        public long? Size { get; set; }
    }
}
