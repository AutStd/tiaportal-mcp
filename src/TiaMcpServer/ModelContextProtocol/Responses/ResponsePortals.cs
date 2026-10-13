using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponsePortals : ResponseMessage
    {
        /// <summary>
        /// Process id of the instance this server is attached to - the one tools use when no
        /// 'portalId' is passed. Null when no instance or more than one is attached.
        /// </summary>
        public int? PortalId { get; set; }

        public IEnumerable<TiaMcpServer.Siemens.PortalInstance>? Portals { get; set; }
    }
}
