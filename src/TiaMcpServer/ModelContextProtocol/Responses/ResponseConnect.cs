namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseConnect : ResponseMessage
    {
        /// <summary>
        /// Process id of the attached TIA Portal instance. Pass it as 'portalId' to the other
        /// tools when more than one instance is attached.
        /// </summary>
        public int? PortalId { get; set; }
    }
}
