namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseExportTypeAsDocuments : ResponseMessage
    {
        public ResponseTypeInfo? Item { get; set; }

        public ResponseDocumentFiles? Documents { get; set; }
    }
}
