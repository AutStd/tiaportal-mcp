using ModelContextProtocol.Server;
using System.ComponentModel;

namespace TiaMcpServer.ModelContextProtocol
{
    public static partial class McpPrompts
    {
        #region Library Type Templates

        [McpServerPrompt(Name = "GetLibraryTypeInstances"), Description("List the typed blocks and PLC data types of a PLC software")]
        public static string GetLibraryTypeInstances(
            [Description("softwarePath: defines the path in the project structure to the PLC software")] string softwarePath)
        {
            return $@"List the blocks and PLC data types that are connected to a type of the project library. Run it before any write: typed objects must not be changed by XML import.

Use the GetLibraryTypeInstances tool with these parameters:
- softwarePath: {softwarePath}";
        }

        [McpServerPrompt(Name = "ExportLibraryTypeVersion"), Description("Export a library type version as documents with meta file")]
        public static string ExportLibraryTypeVersion(
            [Description("typeName: name of the type in the project library")] string typeName,
            [Description("exportPath: directory to write the documents to")] string exportPath,
            [Description("versionNumber: optional, empty exports the default version")] string versionNumber = "")
        {
            return $@"Export one version of a project library type as documents together with its library meta file.

Use the ExportLibraryTypeVersion tool with these parameters:
- typeName: {typeName}
- exportPath: {exportPath}
- versionNumber: {versionNumber}";
        }

        [McpServerPrompt(Name = "EditLibraryType"), Description("Put the library type of a typed object in work (requires '--allow-write')")]
        public static string EditLibraryType(
            [Description("softwarePath: defines the path in the project structure to the PLC software")] string softwarePath,
            [Description("objectPath: path of the typed block or PLC data type")] string objectPath,
            [Description("objectKind: 'Block' or 'Type'")] string objectKind)
        {
            return $@"Create an in-work version of the library type the object is connected to, with the object as test instance (requires the server started with '--allow-write').

Notes:
- Only one version of a type can be in work at a time
- Finish with ReleaseLibraryType or DiscardLibraryType

Use the EditLibraryType tool with these parameters:
- softwarePath: {softwarePath}
- objectPath: {objectPath}
- objectKind: {objectKind}";
        }

        [McpServerPrompt(Name = "CreateLibraryTypeVersionFromDocument"), Description("Create an in-work type version from documents (requires '--allow-write')")]
        public static string CreateLibraryTypeVersionFromDocument(
            [Description("typeName: name of the type in the project library")] string typeName,
            [Description("importPath: directory with the documents and the library meta file")] string importPath,
            [Description("fileName: document file name without extension")] string fileName,
            [Description("softwarePath: defines the path in the project structure to the PLC software")] string softwarePath,
            [Description("groupPath: block or type group of the instance")] string groupPath,
            [Description("objectKind: 'Block' or 'Type'")] string objectKind)
        {
            return $@"Create or override the in-work version of a project library type from documents exported with ExportLibraryTypeVersion (requires the server started with '--allow-write').

Notes:
- The library meta file of the export must lie next to the document
- Compile the PLC software, then release the version with ReleaseLibraryType

Use the CreateLibraryTypeVersionFromDocument tool with these parameters:
- typeName: {typeName}
- importPath: {importPath}
- fileName: {fileName}
- softwarePath: {softwarePath}
- groupPath: {groupPath}
- objectKind: {objectKind}";
        }

        [McpServerPrompt(Name = "ReleaseLibraryType"), Description("Release the in-work version of a library type (requires '--allow-write')")]
        public static string ReleaseLibraryType(
            [Description("typeName: name of the type in the project library")] string typeName,
            [Description("versionNumber: optional, e.g. '1.0.3'")] string versionNumber = "",
            [Description("author: optional author of the version")] string author = "",
            [Description("comment: optional comment of the version")] string comment = "")
        {
            return $@"Release the in-work version of a project library type (requires the server started with '--allow-write').

Use the ReleaseLibraryType tool with these parameters:
- typeName: {typeName}
- versionNumber: {versionNumber}
- author: {author}
- comment: {comment}";
        }

        [McpServerPrompt(Name = "DiscardLibraryType"), Description("Discard the in-work version of a library type (requires '--allow-write')")]
        public static string DiscardLibraryType(
            [Description("typeName: name of the type in the project library")] string typeName)
        {
            return $@"Discard the in-work version of a project library type; its instances return to the last released version (requires the server started with '--allow-write').

Use the DiscardLibraryType tool with these parameters:
- typeName: {typeName}";
        }

        #endregion
    }
}
