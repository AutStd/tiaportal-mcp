using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseExportTypesAsDocuments : ResponseMessage
    {
        public IEnumerable<ResponseTypeInfo>? Items { get; set; }

        public IEnumerable<ResponseDocumentFiles>? Documents { get; set; }

        /// <summary>Types skipped because TIA Portal does not export inconsistent objects.</summary>
        public IEnumerable<ResponseTypeInfo>? Inconsistent { get; set; }

        /// <summary>Types that failed to export, one entry per type, with the reason.</summary>
        public IEnumerable<string>? Failures { get; set; }
    }
}
