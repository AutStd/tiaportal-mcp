using System;
using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponsePlcSummary : ResponseMessage
    {
        public string? Name { get; set; }

        public string? SoftwarePath { get; set; }

        public int? BlockCount { get; set; }

        public int? TypeCount { get; set; }

        public int? TagTableCount { get; set; }

        public int? TagCount { get; set; }

        public int? UserConstantCount { get; set; }

        public int? WatchTableCount { get; set; }

        public int? ExternalSourceCount { get; set; }

        /// <summary>Blocks per concrete kind: OB, FB, FC, InstanceDB, GlobalDB.</summary>
        public IDictionary<string, int>? BlocksByKind { get; set; }

        /// <summary>Blocks per programming language: LAD, SCL, STL, FBD, ...</summary>
        public IDictionary<string, int>? BlocksByLanguage { get; set; }

        /// <summary>Objects that refuse to export until the software is compiled.</summary>
        public IEnumerable<string>? InconsistentObjects { get; set; }

        /// <summary>Objects whose content is hidden, so source and interface reads will fail.</summary>
        public IEnumerable<string>? KnowHowProtectedObjects { get; set; }

        public DateTime? LastModified { get; set; }
    }
}
