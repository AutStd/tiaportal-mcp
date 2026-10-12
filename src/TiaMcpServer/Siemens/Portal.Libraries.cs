using Microsoft.Extensions.Logging;
using Siemens.Engineering;
using Siemens.Engineering.Library;
using Siemens.Engineering.Library.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TiaMcpServer.Siemens
{
    /// <summary>One library that is open in the current project or portal instance.</summary>
    public class OpenLibraryEntry
    {
        /// <summary>Project, UserGlobal, SystemGlobal, CorporateGlobal or Global (unknown subtype).</summary>
        public string Kind { get; set; } = "";

        public string Name { get; set; } = "";

        /// <summary>File path of a global library; null for the project library.</summary>
        public string? Path { get; set; }

        public bool? IsReadOnly { get; set; }

        public bool? IsModified { get; set; }

        /// <summary>The Openness object, kept so the caller can read its attribute bag.</summary>
        public IEngineeringObject? Source { get; set; }
    }

    /// <summary>One library type version found in the project library, with the library it came from.</summary>
    public class LibraryTypeUsage
    {
        /// <summary>Name of the project or library the type version was generated from.</summary>
        public string OriginalLibrary { get; set; } = "";

        /// <summary>Folder of the type in the project library, "" for the root.</summary>
        public string FolderPath { get; set; } = "";

        public string TypeName { get; set; } = "";

        public string? VersionNumber { get; set; }

        public bool IsDefault { get; set; }

        public string? State { get; set; }
    }

    public partial class Portal
    {
        #region libraries

        /// <summary>The name reported for the one library every project has.</summary>
        public const string ProjectLibraryName = "Project library";

        public List<OpenLibraryEntry> GetOpenLibraries()
        {
            _logger?.LogInformation("Getting open libraries...");

            return Operation.Run(_logger, nameof(GetOpenLibraries), PortalErrorCode.NotFound,
                () =>
                {
                    var project = _project ?? throw new PortalException(PortalErrorCode.InvalidState, "No project is open in TIA Portal");

                    var result = new List<OpenLibraryEntry>
                    {
                        new OpenLibraryEntry
                        {
                            Kind = "Project",
                            Name = ProjectLibraryName,
                            Source = project.ProjectLibrary
                        }
                    };

                    if (_portal?.GlobalLibraries != null)
                    {
                        foreach (var library in _portal.GlobalLibraries)
                        {
                            result.Add(new OpenLibraryEntry
                            {
                                Kind = GetGlobalLibraryKind(library),
                                Name = library.Name,
                                Path = library.Path?.FullName,
                                IsReadOnly = library.IsReadOnly,
                                IsModified = library.IsModified,
                                Source = library
                            });
                        }
                    }

                    return result;
                });
        }

        public List<LibraryTypeUsage> GetReferencedLibraryTypes()
        {
            _logger?.LogInformation("Getting referenced library types...");

            return Operation.Run(_logger, nameof(GetReferencedLibraryTypes), PortalErrorCode.NotFound,
                () =>
                {
                    var project = _project ?? throw new PortalException(PortalErrorCode.InvalidState, "No project is open in TIA Portal");

                    var usages = new List<LibraryTypeUsage>();
                    CollectTypeUsages(project.ProjectLibrary.TypeFolder, "", usages);

                    return usages;
                });
        }

        private static string GetGlobalLibraryKind(GlobalLibrary library)
        {
            return library switch
            {
                UserGlobalLibrary => "UserGlobal",
                SystemGlobalLibrary => "SystemGlobal",
                CorporateGlobalLibrary => "CorporateGlobal",
                _ => "Global"
            };
        }

        private static void CollectTypeUsages(LibraryTypeFolder folder, string folderPath, List<LibraryTypeUsage> usages)
        {
            foreach (var type in folder.Types)
            {
                foreach (var version in type.Versions)
                {
                    usages.Add(new LibraryTypeUsage
                    {
                        OriginalLibrary = version.OriginalLibrary ?? "",
                        FolderPath = folderPath,
                        TypeName = type.Name,
                        VersionNumber = version.VersionNumber?.ToString(),
                        IsDefault = version.IsDefault,
                        State = version.State.ToString()
                    });
                }
            }

            foreach (var subFolder in folder.Folders)
            {
                var subPath = folderPath.Length == 0 ? subFolder.Name : $"{folderPath}/{subFolder.Name}";
                CollectTypeUsages(subFolder, subPath, usages);
            }
        }

        #endregion
    }
}
