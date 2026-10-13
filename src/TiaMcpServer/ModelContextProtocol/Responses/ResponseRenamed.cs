namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseRenamed : ResponseMessage
    {
        public string? Kind { get; set; }
        public string? OldPath { get; set; }
        public string? NewName { get; set; }
        public string? NewPath { get; set; }
    }
}
