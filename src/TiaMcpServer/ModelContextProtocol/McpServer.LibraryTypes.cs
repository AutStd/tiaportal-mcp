using ModelContextProtocol;
using ModelContextProtocol.Server;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Nodes;
using TiaMcpServer.Siemens;

namespace TiaMcpServer.ModelContextProtocol
{
    public static partial class McpServer
    {
        #region library types

        [McpServerTool(Name = "GetLibraryTypeInstances", Title = "Get library type instances", ReadOnly = true, OpenWorld = false, UseStructuredContent = true), Description("List the blocks and PLC data types of the PLC software that are connected to a type of the project library, with type, folder, version and state. Objects that are not listed are not typed")]
        public static ResponseLibraryTypeInstances GetLibraryTypeInstances(
            [Description("softwarePath: defines the path in the project structure to the PLC software")] string softwarePath)
        {
            try
            {
                var items = Portal.GetLibraryTypeInstances(softwarePath);

                return new ResponseLibraryTypeInstances
                {
                    Message = $"{items.Count} typed objects found in '{softwarePath}'",
                    Items = items,
                    Meta = Ok(new JsonObject
                    {
                        ["totalInstances"] = items.Count,
                        ["inWork"] = items.Count(item => item.State == "InWork")
                    })
                };
            }
            catch (PortalException pex)
            {
                throw new McpException(pex.Message, pex);
            }
            catch (Exception ex) when (ex is not McpException)
            {
                throw new McpException($"Unexpected error retrieving library type instances: {ex.Message}", ex);
            }
        }

        [McpServerTool(Name = "ExportLibraryTypeVersion", Title = "Export library type version", ReadOnly = true, OpenWorld = false, UseStructuredContent = true), Description("Export one version of a project library type as documents with its library meta file, as input for CreateLibraryTypeVersionFromDocument")]
        public static ResponseLibraryTypeExport ExportLibraryTypeVersion(
            [Description("typeName: name of the type in the project library")] string typeName,
            [Description("exportPath: directory to write the documents to; created if missing")] string exportPath,
            [Description("versionNumber: optional, e.g. '1.0.2'; empty exports the default version")] string versionNumber = "",
            [Description("format: optional export format; empty prefers SimaticML")] string format = "")
        {
            try
            {
                var export = Portal.ExportLibraryTypeVersion(typeName, versionNumber, exportPath, format);

                return new ResponseLibraryTypeExport
                {
                    Message = $"Library type '{typeName}' version {export.Version.VersionNumber} exported to '{exportPath}' ({export.Files.Count} files)",
                    Version = export.Version,
                    Files = export.Files,
                    Meta = Ok()
                };
            }
            catch (PortalException pex)
            {
                throw new McpException(pex.Message, pex);
            }
            catch (Exception ex) when (ex is not McpException)
            {
                throw new McpException($"Unexpected error exporting library type '{typeName}': {ex.Message}", ex);
            }
        }

        [WriteTool]
        [McpServerTool(Name = "EditLibraryType", Title = "Edit library type", Destructive = true, OpenWorld = false, UseStructuredContent = true),
         Description("Create an in-work version of the library type a block or PLC data type is connected to, with that object as test instance. Changes made to it keep the type connection")]
        public static ResponseLibraryTypeVersion EditLibraryType(
            [Description("softwarePath: defines the path in the project structure to the PLC software")] string softwarePath,
            [Description("objectPath: path of the typed block or PLC data type, e.g. 'Group/Name'")] string objectPath,
            [Description("objectKind: 'Block' or 'Type'")] string objectKind)
        {
            return Guarded(nameof(EditLibraryType), () =>
            {
                var version = Portal.EditLibraryType(softwarePath, objectPath, objectKind);

                return new ResponseLibraryTypeVersion
                {
                    Message = $"Library type '{version.TypeName}' is in work with test instance '{objectPath}'. Release it with 'ReleaseLibraryType' or discard it with 'DiscardLibraryType'. {SaveHint}",
                    Version = version,
                    Meta = OkMeta()
                };
            });
        }

        [WriteTool]
        [McpServerTool(Name = "CreateLibraryTypeVersionFromDocument", Title = "Create type version from document", Destructive = true, Idempotent = true, OpenWorld = false, UseStructuredContent = true),
         Description("Create or override the in-work version of a project library type from documents with library meta file; the instance in the group stays connected to the type")]
        public static ResponseLibraryTypeVersion CreateLibraryTypeVersionFromDocument(
            [Description("typeName: name of the type in the project library")] string typeName,
            [Description("importPath: directory that contains the documents and the library meta file")] string importPath,
            [Description("fileName: document file name without extension")] string fileName,
            [Description("softwarePath: defines the path in the project structure to the PLC software")] string softwarePath,
            [Description("groupPath: block or type group of the instance, root-relative")] string groupPath,
            [Description("objectKind: 'Block' or 'Type'")] string objectKind)
        {
            return Guarded(nameof(CreateLibraryTypeVersionFromDocument), () =>
            {
                var version = Portal.CreateLibraryTypeVersionFromDocument(typeName, importPath, fileName, softwarePath, groupPath, objectKind);

                return new ResponseLibraryTypeVersion
                {
                    Message = $"In-work version of library type '{typeName}' created from '{importPath}'. Compile, then release it with 'ReleaseLibraryType'. {SaveHint}",
                    Version = version,
                    Meta = OkMeta()
                };
            });
        }

        [WriteTool]
        [McpServerTool(Name = "ReleaseLibraryType", Title = "Release library type", Destructive = true, OpenWorld = false, UseStructuredContent = true),
         Description("Release the in-work version of a project library type with version number, author and comment")]
        public static ResponseLibraryTypeVersion ReleaseLibraryType(
            [Description("typeName: name of the type in the project library")] string typeName,
            [Description("versionNumber: optional, e.g. '1.0.3'; empty keeps the number TIA Portal proposes")] string versionNumber = "",
            [Description("author: optional author of the version")] string author = "",
            [Description("comment: optional comment of the version")] string comment = "",
            [Description("releaseDependencies: true also creates or releases dependent types if required")] bool releaseDependencies = false)
        {
            return Guarded(nameof(ReleaseLibraryType), () =>
            {
                var version = Portal.ReleaseLibraryType(
                    typeName,
                    versionNumber,
                    string.IsNullOrWhiteSpace(author) ? null : author,
                    string.IsNullOrWhiteSpace(comment) ? null : comment,
                    releaseDependencies);

                return new ResponseLibraryTypeVersion
                {
                    Message = $"Library type '{typeName}' released as version {version.VersionNumber}. {SaveHint}",
                    Version = version,
                    Meta = OkMeta()
                };
            });
        }

        [WriteTool]
        [McpServerTool(Name = "DiscardLibraryType", Title = "Discard library type version", Destructive = true, Idempotent = true, OpenWorld = false, UseStructuredContent = true),
         Description("Discard the in-work version of a project library type; its instances return to the last released version")]
        public static ResponseLibraryTypeVersion DiscardLibraryType(
            [Description("typeName: name of the type in the project library")] string typeName)
        {
            return Guarded(nameof(DiscardLibraryType), () =>
            {
                var version = Portal.DiscardLibraryType(typeName);

                return new ResponseLibraryTypeVersion
                {
                    Message = $"In-work version of library type '{typeName}' discarded, instances use version {version.VersionNumber} again. {SaveHint}",
                    Version = version,
                    Meta = OkMeta()
                };
            });
        }

        #endregion
    }
}
