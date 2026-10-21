using Microsoft.Extensions.Logging;
using Siemens.Engineering;
using Siemens.Engineering.Library;
using Siemens.Engineering.Library.Types;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TiaMcpServer.Siemens
{
    /// <summary>One block or PLC data type of a PLC software that is an instance of a library type.</summary>
    public class LibraryTypeInstanceEntry
    {
        /// <summary>"Block" or "Type".</summary>
        public string Kind { get; set; } = "";

        /// <summary>Path of the block or PLC data type in the PLC software, e.g. "Group/Name".</summary>
        public string ObjectPath { get; set; } = "";

        public string TypeName { get; set; } = "";

        /// <summary>Folder of the type in the project library, "" for the root.</summary>
        public string TypeFolderPath { get; set; } = "";

        public string? VersionNumber { get; set; }

        public string? State { get; set; }

        public bool IsDefault { get; set; }
    }

    /// <summary>One version of a library type, as reported after an edit, release or document import.</summary>
    public class LibraryTypeVersionEntry
    {
        public string TypeName { get; set; } = "";

        public string TypeFolderPath { get; set; } = "";

        public string? VersionNumber { get; set; }

        public string? State { get; set; }

        public bool IsDefault { get; set; }

        public string? Author { get; set; }

        /// <summary>Version comment per language, e.g. { "de-AT": "Ersterstellung" }.</summary>
        public Dictionary<string, string>? Comment { get; set; }
    }

    /// <summary>Exported version of a library type and the files written for it.</summary>
    public class LibraryTypeExportResult
    {
        public LibraryTypeVersionEntry Version { get; set; } = new LibraryTypeVersionEntry();

        /// <summary>Full paths of the written documents, including the library meta files.</summary>
        public List<string> Files { get; set; } = new List<string>();
    }

    public partial class Portal
    {
        #region library types

        /// <summary>
        /// Lists every block and PLC data type of the PLC software that is connected to a type of
        /// the project library. Objects without that connection are left out, so an object missing
        /// here is not typed (or lost its connection, e.g. through an XML import).
        /// </summary>
        public List<LibraryTypeInstanceEntry> GetLibraryTypeInstances(string softwarePath)
        {
            _logger?.LogInformation($"Getting library type instances in {softwarePath}...");

            return Operation.Run(_logger, nameof(GetLibraryTypeInstances), PortalErrorCode.NotFound,
                () =>
                {
                    var software = GetPlcSoftwareOrThrow(softwarePath);
                    var result = new List<LibraryTypeInstanceEntry>();

                    var blocks = new List<PlcBlock>();
                    GetBlocksRecursive(software.BlockGroup, blocks);
                    foreach (var block in blocks)
                    {
                        AddInstanceEntry(result, "Block", GetBlockPath(block), block.GetService<LibraryTypeInstanceInfo>());
                    }

                    var types = new List<PlcType>();
                    GetTypesRecursive(software.TypeGroup, types);
                    foreach (var type in types)
                    {
                        AddInstanceEntry(result, "Type", GetTypePath(type), type.GetService<LibraryTypeInstanceInfo>());
                    }

                    return result;
                });
        }

        /// <summary>
        /// Opens the library type of a typed block or PLC data type for editing: creates an
        /// in-work version and makes the given object its test instance.
        /// </summary>
        public LibraryTypeVersionEntry EditLibraryType(string softwarePath, string objectPath, string objectKind)
        {
            _logger?.LogInformation($"Editing library type of {objectKind} {objectPath}...");

            return Operation.Run(_logger, nameof(EditLibraryType), PortalErrorCode.InvalidState,
                () =>
                {
                    var info = GetInstanceInfoOrThrow(softwarePath, objectPath, objectKind);
                    var version = info.LibraryTypeVersion;
                    var inWork = version.Edit(info);

                    return ToVersionEntry(inWork);
                });
        }

        /// <summary>Releases the in-work version of a library type of the project library.</summary>
        public LibraryTypeVersionEntry ReleaseLibraryType(string typeName, string? versionNumber, string? author, string? comment, bool releaseDependencies)
        {
            _logger?.LogInformation($"Releasing library type {typeName}...");

            return Operation.Run(_logger, nameof(ReleaseLibraryType), PortalErrorCode.InvalidState,
                () =>
                {
                    var inWork = GetInWorkVersionOrThrow(typeName);

                    Version? number = null;
                    if (!string.IsNullOrWhiteSpace(versionNumber) && !Version.TryParse(versionNumber, out number))
                    {
                        throw new PortalException(PortalErrorCode.InvalidParams, $"'{versionNumber}' is not a valid version number, e.g. '1.0.1'");
                    }

                    var mode = releaseDependencies
                        ? CreateOrReleaseDependenciesMode.AutomaticallyCreateOrReleaseDependenciesIfRequired
                        : CreateOrReleaseDependenciesMode.DoNotAutomaticallyCreateOrReleaseDependencies;

                    inWork.Release(mode, number, author, comment);

                    return ToVersionEntry(inWork);
                });
        }

        /// <summary>
        /// Discards the in-work version of a library type; its instances return to the last
        /// released version. Returns that version, i.e. the state after discarding.
        /// </summary>
        public LibraryTypeVersionEntry DiscardLibraryType(string typeName)
        {
            _logger?.LogInformation($"Discarding in-work version of library type {typeName}...");

            return Operation.Run(_logger, nameof(DiscardLibraryType), PortalErrorCode.InvalidState,
                () =>
                {
                    var type = GetLibraryTypeOrThrow(typeName);
                    var inWork = GetInWorkVersionOrThrow(typeName);
                    inWork.Discard();

                    var current = type.Versions.FirstOrDefault(v => v.IsDefault) ?? type.Versions.LastOrDefault()
                        ?? throw new PortalException(PortalErrorCode.InvalidState, $"Library type '{typeName}' has no version left after discarding");

                    return ToVersionEntry(current);
                });
        }

        /// <summary>
        /// Exports one version of a library type as documents, together with the library meta
        /// file that CreateLibraryTypeVersionFromDocument needs to find the type again.
        /// </summary>
        public LibraryTypeExportResult ExportLibraryTypeVersion(string typeName, string? versionNumber, string exportPath, string? format)
        {
            _logger?.LogInformation($"Exporting library type {typeName} to {exportPath}...");

            return Operation.Run(_logger, nameof(ExportLibraryTypeVersion), PortalErrorCode.ExportFailed,
                () =>
                {
                    var type = GetLibraryTypeOrThrow(typeName);
                    var version = string.IsNullOrWhiteSpace(versionNumber)
                        ? type.Versions.FirstOrDefault(v => v.IsDefault) ?? type.Versions.Last()
                        : FindVersionOrThrow(type, versionNumber!);

                    var supported = type.GetSupportedExportFormats()?.ToList() ?? new List<string>();
                    var chosen = string.IsNullOrWhiteSpace(format)
                        ? supported.FirstOrDefault(f => f.IndexOf("SimaticML", StringComparison.OrdinalIgnoreCase) >= 0) ?? supported.FirstOrDefault()
                        : supported.FirstOrDefault(f => string.Equals(f, format, StringComparison.OrdinalIgnoreCase));

                    if (chosen == null)
                    {
                        throw new PortalException(PortalErrorCode.InvalidParams,
                            $"Format '{format}' is not supported for library type '{typeName}'. Supported: {string.Join(", ", supported)}");
                    }

                    // The DirectoryInfo returned by Directory.CreateDirectory only holds the last
                    // folder name as original path (.NET Framework), which Openness rejects as relative.
                    Directory.CreateDirectory(exportPath);
                    var directory = new DirectoryInfo(Path.GetFullPath(exportPath));
                    var result = version.ExportAsDocuments(directory, type.Name, chosen, LibraryExportOptions.WithLibraryVersionInfoFile);
                    if (result != null && result.TransferResultState != TransferResultState.Success && result.TransferResultState != TransferResultState.Warning)
                    {
                        throw new PortalException(PortalErrorCode.ExportFailed,
                            $"Export of library type '{typeName}' failed: {string.Join("; ", result.Messages.Select(m => m.Message))}");
                    }

                    return new LibraryTypeExportResult
                    {
                        Version = ToVersionEntry(version),
                        Files = result?.ExportedDocuments?.Select(f => f.FullName).ToList() ?? new List<string>()
                    };
                });
        }

        /// <summary>
        /// Creates (or overrides) the in-work version of a library type from documents exported
        /// with ExportLibraryTypeVersion. The instance is created or updated in the given group,
        /// so the connection to the type is kept. Release the version afterwards.
        /// </summary>
        public LibraryTypeVersionEntry CreateLibraryTypeVersionFromDocument(string typeName, string importPath, string fileName, string softwarePath, string groupPath, string objectKind)
        {
            _logger?.LogInformation($"Creating in-work version of library type {typeName} from {importPath}...");

            return Operation.Run(_logger, nameof(CreateLibraryTypeVersionFromDocument), PortalErrorCode.ImportFailed,
                () =>
                {
                    var type = GetLibraryTypeOrThrow(typeName);
                    var directory = new DirectoryInfo(Path.GetFullPath(importPath));
                    if (!directory.Exists)
                    {
                        throw new PortalException(PortalErrorCode.NotFound, $"Directory '{importPath}' does not exist");
                    }

                    IEngineeringObject target = IsBlockKind(objectKind)
                        ? GetPlcBlockGroupByPath(softwarePath, groupPath) as IEngineeringObject
                            ?? throw new PortalException(PortalErrorCode.NotFound, $"Block group '{groupPath}' not found. Use 'GetSoftwareTree' to list the groups")
                        : GetPlcTypeGroupByPath(softwarePath, groupPath) as IEngineeringObject
                            ?? throw new PortalException(PortalErrorCode.NotFound, $"Type group '{groupPath}' not found. Use 'GetSoftwareTree' to list the groups");

                    var result = type.Versions.CreateFromDocuments(directory, fileName, target, CreateOptions.Override, LibraryImportOptions.None);
                    if (result.TransferResultState != TransferResultState.Success && result.TransferResultState != TransferResultState.Warning || result.CreatedVersion == null)
                    {
                        throw new PortalException(PortalErrorCode.ImportFailed,
                            $"Creating a version of library type '{typeName}' failed: {string.Join("; ", result.Messages.Select(m => m.Message))}");
                    }

                    return ToVersionEntry(result.CreatedVersion);
                });
        }

        private void AddInstanceEntry(List<LibraryTypeInstanceEntry> result, string kind, string objectPath, LibraryTypeInstanceInfo? info)
        {
            var version = info?.LibraryTypeVersion;
            if (version == null)
            {
                return;
            }

            var libraryType = version.Parent as LibraryType;

            result.Add(new LibraryTypeInstanceEntry
            {
                Kind = kind,
                ObjectPath = objectPath,
                TypeName = libraryType?.Name ?? "",
                TypeFolderPath = GetLibraryTypeFolderPath(libraryType),
                VersionNumber = version.VersionNumber?.ToString(),
                State = version.State.ToString(),
                IsDefault = version.IsDefault
            });
        }

        private LibraryTypeInstanceInfo GetInstanceInfoOrThrow(string softwarePath, string objectPath, string objectKind)
        {
            IEngineeringServiceProvider? target = IsBlockKind(objectKind)
                ? GetBlock(softwarePath, objectPath)
                : GetType(softwarePath, objectPath);

            if (target == null)
            {
                throw new PortalException(PortalErrorCode.NotFound,
                    $"{objectKind} '{objectPath}' not found. Use 'GetBlocks' or 'GetTypes' to list valid paths");
            }

            return target.GetService<LibraryTypeInstanceInfo>()
                ?? throw new PortalException(PortalErrorCode.InvalidState,
                    $"{objectKind} '{objectPath}' is not connected to a library type. Use 'GetLibraryTypeInstances' to list typed objects");
        }

        private LibraryType GetLibraryTypeOrThrow(string typeName)
        {
            var project = _project ?? throw new PortalException(PortalErrorCode.InvalidState, "No project is open in TIA Portal");

            return FindLibraryType(project.ProjectLibrary.TypeFolder, typeName)
                ?? throw new PortalException(PortalErrorCode.NotFound,
                    $"Library type '{typeName}' not found in the project library. Use 'GetReferencedLibraries' to list the types");
        }

        private static LibraryType? FindLibraryType(LibraryTypeFolder folder, string typeName)
        {
            var type = folder.Types.FirstOrDefault(t => string.Equals(t.Name, typeName, StringComparison.Ordinal));
            if (type != null)
            {
                return type;
            }

            foreach (var subFolder in folder.Folders)
            {
                type = FindLibraryType(subFolder, typeName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private LibraryTypeVersion GetInWorkVersionOrThrow(string typeName)
        {
            var type = GetLibraryTypeOrThrow(typeName);

            return type.Versions.FirstOrDefault(v => v.State == LibraryTypeVersionState.InWork)
                ?? throw new PortalException(PortalErrorCode.InvalidState,
                    $"Library type '{typeName}' has no in-work version. Use 'EditLibraryType' first");
        }

        private static LibraryTypeVersion FindVersionOrThrow(LibraryType type, string versionNumber)
        {
            if (!Version.TryParse(versionNumber, out var number))
            {
                throw new PortalException(PortalErrorCode.InvalidParams, $"'{versionNumber}' is not a valid version number, e.g. '1.0.1'");
            }

            return type.Versions.Find(number)
                ?? throw new PortalException(PortalErrorCode.NotFound, $"Library type '{type.Name}' has no version '{versionNumber}'");
        }

        private static bool IsBlockKind(string objectKind)
        {
            if (string.Equals(objectKind, "Block", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (string.Equals(objectKind, "Type", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            throw new PortalException(PortalErrorCode.InvalidParams, $"objectKind must be 'Block' or 'Type', not '{objectKind}'");
        }

        private static string GetLibraryTypeFolderPath(LibraryType? type)
        {
            var names = new List<string>();
            var folder = type?.Parent as LibraryTypeFolder;

            // The system type folder of the library is the root and is not part of the path.
            while (folder != null && folder.Parent is LibraryTypeFolder parent)
            {
                names.Insert(0, folder.Name);
                folder = parent;
            }

            return string.Join("/", names);
        }

        private static LibraryTypeVersionEntry ToVersionEntry(LibraryTypeVersion version)
        {
            var type = version.Parent as LibraryType;

            return new LibraryTypeVersionEntry
            {
                TypeName = type?.Name ?? "",
                TypeFolderPath = GetLibraryTypeFolderPath(type),
                VersionNumber = version.VersionNumber?.ToString(),
                State = version.State.ToString(),
                IsDefault = version.IsDefault,
                Author = version.Author,
                Comment = GetTexts(version.Comment)
            };
        }

        /// <summary>Non-empty translations of a MultilingualText by culture name, or null when there is none.</summary>
        private static Dictionary<string, string>? GetTexts(MultilingualText? text)
        {
            var texts = new Dictionary<string, string>();
            foreach (var item in text?.Items ?? Enumerable.Empty<MultilingualTextItem>())
            {
                var culture = item.Language?.Culture?.Name;
                if (!string.IsNullOrEmpty(culture) && !string.IsNullOrEmpty(item.Text))
                {
                    texts[culture!] = item.Text;
                }
            }

            return texts.Count > 0 ? texts : null;
        }

        #endregion
    }
}
