using ModelContextProtocol;
using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json.Nodes;
using TiaMcpServer.Siemens;

namespace TiaMcpServer.ModelContextProtocol
{
    public static partial class McpServer
    {
        #region libraries

        [McpServerTool(Name = "GetOpenLibraries", Title = "Get open libraries", ReadOnly = true, OpenWorld = false, UseStructuredContent = true), Description("List the project library and the global libraries that are open in TIA Portal, with name, path, state and all attributes. Global libraries belong to the TIA Portal instance, not to the project")]
        public static ResponseLibraries GetOpenLibraries()
        {
            try
            {
                var libraries = Portal.GetOpenLibraries();
                var items = libraries.Select(ToLibraryInfo).ToList();

                return new ResponseLibraries
                {
                    Message = "Open libraries retrieved",
                    Items = items,
                    Meta = Ok(new JsonObject { ["totalLibraries"] = items.Count })
                };
            }
            catch (PortalException pex)
            {
                throw new McpException(pex.Message, pex);
            }
            catch (Exception ex) when (ex is not McpException)
            {
                throw new McpException($"Unexpected error retrieving open libraries: {ex.Message}", ex);
            }
        }

        [McpServerTool(Name = "GetReferencedLibraries", Title = "Get referenced libraries", ReadOnly = true, OpenWorld = false, UseStructuredContent = true), Description("List the libraries whose types are used in the current project, grouped by the library each type version came from, with the type versions per library. Reads the project library, so it also reports libraries that are no longer open. The project itself is listed when it has types of its own")]
        public static ResponseReferencedLibraries GetReferencedLibraries()
        {
            try
            {
                var usages = Portal.GetReferencedLibraryTypes();
                var open = Portal.GetOpenLibraries()
                    .Where(library => library.Kind != "Project")
                    .GroupBy(library => library.Name)
                    .ToDictionary(group => group.Key, group => group.First());

                var items = usages
                    .GroupBy(usage => usage.OriginalLibrary)
                    .Select(group =>
                    {
                        open.TryGetValue(group.Key, out var match);

                        return new ResponseReferencedLibrary
                        {
                            Name = group.Key.Length == 0 ? null : group.Key,
                            OpenLibraryKind = match?.Kind,
                            OpenLibraryPath = match?.Path,
                            TypeVersionCount = group.Count(),
                            Types = group.Select(usage => new ResponseReferencedLibraryType
                            {
                                FolderPath = usage.FolderPath,
                                TypeName = usage.TypeName,
                                VersionNumber = usage.VersionNumber,
                                IsDefault = usage.IsDefault,
                                State = usage.State
                            }).ToList()
                        };
                    })
                    .OrderBy(library => library.Name, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return new ResponseReferencedLibraries
                {
                    Message = "Referenced libraries retrieved",
                    Items = items,
                    Meta = Ok(new JsonObject
                    {
                        ["totalLibraries"] = items.Count,
                        ["totalTypeVersions"] = usages.Count
                    })
                };
            }
            catch (PortalException pex)
            {
                throw new McpException(pex.Message, pex);
            }
            catch (Exception ex) when (ex is not McpException)
            {
                throw new McpException($"Unexpected error retrieving referenced libraries: {ex.Message}", ex);
            }
        }

        private static ResponseLibraryInfo ToLibraryInfo(OpenLibraryEntry library)
        {
            return new ResponseLibraryInfo
            {
                Kind = library.Kind,
                Name = library.Name,
                Path = library.Path,
                IsReadOnly = library.IsReadOnly,
                IsModified = library.IsModified,
                Attributes = Helper.GetAttributeList(library.Source!)
            };
        }

        #endregion
    }
}
