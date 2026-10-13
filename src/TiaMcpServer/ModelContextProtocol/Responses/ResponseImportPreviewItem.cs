namespace TiaMcpServer.ModelContextProtocol
{
    /// <summary>What importing one file would do.</summary>
    public class ResponseImportPreviewItem
    {
        public string? Name { get; set; }

        /// <summary>'create', 'overwrite', 'conflict' or 'overwrite-elsewhere'.</summary>
        public string? Effect { get; set; }

        /// <summary>Where the import would put it.</summary>
        public string? TargetPath { get; set; }

        /// <summary>Where an object of that name already lives, when one does.</summary>
        public string? ExistingPath { get; set; }

        public string? Note { get; set; }
    }
}
