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

        [McpServerTool(Name = "GetLibraryTypeVersions", Title = "Get library type versions", ReadOnly = true, OpenWorld = false, UseStructuredContent = true), Description("List the versions of project library types with their instances in the PLC software, dependent type versions and master copies, and whether CleanUpLibraryTypes would remove them. Select one type, a library folder or all types")]
        public static ResponseLibraryTypeVersions GetLibraryTypeVersions(
            [Description("softwarePath: defines the path in the project structure to the PLC software whose instances are counted")] string softwarePath,
            [Description("typeName: optional, one type of the project library")] string typeName = "",
            [Description("folderPath: optional, a folder of the project library, e.g. 'Z-LIB PLC Types/MODE (Betriebsarten)'; empty with empty typeName lists all types")] string folderPath = "")
        {
            try
            {
                var items = Portal.GetLibraryTypeVersions(softwarePath, typeName, folderPath);

                return new ResponseLibraryTypeVersions
                {
                    Message = $"{items.Count} versions found, {items.Count(i => i.WouldBeCleanedUp == true)} unused besides the default version. Instances outside '{softwarePath}' are not counted; for {items.Count(i => !i.InstancesCounted)} versions of other type kinds (e.g. HMI) the use is unknown",
                    Items = items,
                    Meta = Ok(new JsonObject
                    {
                        ["totalVersions"] = items.Count,
                        ["unused"] = items.Count(i => i.IsUnused == true),
                        ["wouldBeCleanedUp"] = items.Count(i => i.WouldBeCleanedUp == true),
                        ["notCounted"] = items.Count(i => !i.InstancesCounted)
                    })
                };
            }
            catch (PortalException pex)
            {
                throw new McpException(pex.Message, pex);
            }
            catch (Exception ex) when (ex is not McpException)
            {
                throw new McpException($"Unexpected error retrieving library type versions: {ex.Message}", ex);
            }
        }

        [WriteTool]
        [McpServerTool(Name = "RenameLibraryType", Title = "Rename library type", Destructive = true, OpenWorld = false, UseStructuredContent = true),
         Description("Rename a type of the project library and harmonize the names of its instances in the PLC software, so they stay connected to the type")]
        public static ResponseLibraryTypeRename RenameLibraryType(
            [Description("typeName: current name of the type in the project library")] string typeName,
            [Description("newName: new name of the type")] string newName,
            [Description("softwarePath: defines the path in the project structure to the PLC software whose instances are renamed")] string softwarePath,
            [Description("harmonize: optional, false renames only the library type and leaves the instance names")] bool harmonize = true)
        {
            return Guarded(nameof(RenameLibraryType), () =>
            {
                var result = Portal.RenameLibraryType(typeName, newName, softwarePath, harmonize);

                return new ResponseLibraryTypeRename
                {
                    Message = $"Library type '{typeName}' renamed to '{result.NewName}', {result.InstancesAfter.Count} instances in '{softwarePath}'. Compile the PLC software. {SaveHint}",
                    Result = result,
                    Meta = OkMeta()
                };
            });
        }

        [WriteTool]
        [McpServerTool(Name = "CleanUpLibraryTypes", Title = "Clean up library types", Destructive = true, Idempotent = true, OpenWorld = false, UseStructuredContent = true),
         Description("Remove the unused versions of the selected project library types with the TIA Portal 'Clean up library' function; the default version of each type is always kept. Check first with GetLibraryTypeVersions")]
        public static ResponseLibraryCleanUp CleanUpLibraryTypes(
            [Description("typeNames: optional, comma separated types of the project library")] string typeNames = "",
            [Description("folderPath: optional, a folder of the project library whose types are cleaned up")] string folderPath = "")
        {
            return Guarded(nameof(CleanUpLibraryTypes), () =>
            {
                var result = Portal.CleanUpLibraryTypes(typeNames, folderPath);

                return new ResponseLibraryCleanUp
                {
                    Message = $"{result.Removed.Count} unused versions removed. {SaveHint}",
                    Removed = result.Removed,
                    Remaining = result.Remaining,
                    Meta = OkMeta()
                };
            });
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
