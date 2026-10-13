namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseDeleted : ResponseMessage
    {
        public string? Kind { get; set; }
        public string? Path { get; set; }
    }
}
