using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseOpenTiaProject : ResponseMessage
    {
        public string? ProjectPath { get; set; }

        /// <summary>False when this call had to establish the TIA Portal connection itself.</summary>
        public bool? WasAlreadyConnected { get; set; }

        /// <summary>PLC software paths in the form the other tools accept.</summary>
        public IEnumerable<string>? SoftwarePaths { get; set; }

        /// <summary>The project tree, so no follow-up GetProjectTree call is needed.</summary>
        public string? Tree { get; set; }

        /// <summary>The TIA Portal instance the project was opened in; pass it on as 'portalId'.</summary>
        public int? PortalId { get; set; }
    }
}
