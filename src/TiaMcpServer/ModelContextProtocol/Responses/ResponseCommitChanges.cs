namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseCommitChanges : ResponseMessage
    {
        /// <summary>The revision number the commit created on the server.</summary>
        public int Revision { get; set; }

        public string? Comment { get; set; }
    }
}
