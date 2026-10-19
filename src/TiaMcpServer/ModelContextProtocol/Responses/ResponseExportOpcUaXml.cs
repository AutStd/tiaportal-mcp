namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseExportOpcUaXml : ResponseMessage
    {
        public string? Name { get; set; }
        public string? Path { get; set; }
    }
}
