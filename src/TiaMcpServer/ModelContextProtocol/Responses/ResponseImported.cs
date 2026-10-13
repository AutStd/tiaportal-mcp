namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseImported : ResponseMessage
    {
        public string? Kind { get; set; }
        public string? GroupPath { get; set; }
        public string? ImportPath { get; set; }
    }
}
