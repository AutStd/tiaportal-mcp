namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseBlocksWithHierarchy : ResponseMessage
    {
        public BlockGroupInfo? Root { get; set; }
    }
}
