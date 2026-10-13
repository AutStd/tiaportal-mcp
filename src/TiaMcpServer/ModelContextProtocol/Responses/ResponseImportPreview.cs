using System.Collections.Generic;

namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseImportPreview : ResponseMessage
    {
        public IEnumerable<ResponseImportPreviewItem>? Items { get; set; }

        public int? CreateCount { get; set; }

        public int? OverwriteCount { get; set; }

        public int? ConflictCount { get; set; }
    }
}
