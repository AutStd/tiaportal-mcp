namespace TiaMcpServer.ModelContextProtocol
{
    public class ResponseReferencedLibraryType
    {
        /// <summary>Folder of the type in the project library, empty for the root.</summary>
        public string? FolderPath { get; set; }

        public string? TypeName { get; set; }

        public string? VersionNumber { get; set; }

        public bool IsDefault { get; set; }

        public string? State { get; set; }
    }
}
