using Siemens.Engineering;
using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseInterfaceMember
    {
        public string? Name { get; set; }

        public string? DataTypeName { get; set; }

        /// <summary>Every attribute Openness reports for the member, stringified.</summary>
        public IDictionary<string, string>? Attributes { get; set; }
    }
}
