using Microsoft.Extensions.Logging;
using Siemens.Engineering;
using Siemens.Engineering.Multiuser;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.ExternalSources;
using Siemens.Engineering.SW.Tags;
using Siemens.Engineering.SW.Types;
using Siemens.Engineering.SW.WatchAndForceTables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TiaMcpServer.Siemens
{
    /// <summary>One object that is marked for check-in in the open local session.</summary>
    public class MarkedObjectEntry
    {
        /// <summary>Block, Type, TagTable, WatchTable, ForceTable, ExternalSource, or the Openness type name for other objects.</summary>
        public string Kind { get; set; } = "";

        public string Name { get; set; } = "";

        /// <summary>Path usable with the other tools; null when the object kind has no path helper.</summary>
        public string? Path { get; set; }

        public bool IsMarkedByMe { get; set; }

        public bool IsMarkedByOthers { get; set; }

        public bool IsUpToDate { get; set; }
    }

    public partial class Portal
    {
        #region marking

        /// <summary>Accepted values for the 'filter' parameter of GetMarkedObjects.</summary>
        private static readonly string[] MarkedObjectFilters = ["all", "me", "others"];

        public List<MarkedObjectEntry> GetMarkedObjects(string filter = "all")
        {
            _logger?.LogInformation("Getting marked objects...");

            return Operation.Run(_logger, nameof(GetMarkedObjects), PortalErrorCode.NotFound,
                () =>
                {
                    var normalizedFilter = (filter ?? "all").Trim().ToLowerInvariant();
                    if (!MarkedObjectFilters.Contains(normalizedFilter))
                    {
                        throw new PortalException(PortalErrorCode.InvalidParams,
                            $"Unknown filter '{filter}'. Use one of: {string.Join(", ", MarkedObjectFilters)}.");
                    }

                    var session = _session ?? throw new PortalException(PortalErrorCode.InvalidState,
                        "Marked objects exist only in a multiuser local session. Open a local session (.als) first.");

                    var result = new List<MarkedObjectEntry>();

                    foreach (var marking in session.MarkingService.GetMarkings().AllMarkings)
                    {
                        var state = marking.MarkState;
                        var byMe = state.HasFlag(MarkState.IsMarkedByMe);
                        var byOthers = state.HasFlag(MarkState.IsMarkedByOthers);

                        if ((normalizedFilter == "me" && !byMe) || (normalizedFilter == "others" && !byOthers))
                        {
                            continue;
                        }

                        var entry = DescribeMarkedObject(marking.MarkedObject);
                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            entry.Name = entry.Path?.Split('/').LastOrDefault() ?? "";
                        }

                        entry.IsMarkedByMe = byMe;
                        entry.IsMarkedByOthers = byOthers;
                        entry.IsUpToDate = state.HasFlag(MarkState.IsUptoDate);
                        result.Add(entry);
                    }

                    return result;
                });
        }

        private MarkedObjectEntry DescribeMarkedObject(IEngineeringObject? obj)
        {
            try
            {
                return DescribeKnownObject(obj);
            }
            catch (Exception ex) when (ex is EngineeringException or PortalException)
            {
                // One object whose parent chain cannot be resolved must not hide the other marks.
                _logger?.LogWarning(ex, "Could not build the path of a marked object");
                return new MarkedObjectEntry { Kind = obj?.GetType().Name ?? "Unknown", Name = obj is null ? "" : ReadName(obj) };
            }
        }

        private MarkedObjectEntry DescribeKnownObject(IEngineeringObject? obj)
        {
            return obj switch
            {
                PlcBlock block => new MarkedObjectEntry { Kind = "Block", Name = block.Name, Path = GetBlockPath(block) },
                PlcType type => new MarkedObjectEntry { Kind = "Type", Name = type.Name, Path = GetTypePath(type) },
                PlcTagTable table => new MarkedObjectEntry { Kind = "TagTable", Name = table.Name, Path = GetTagTablePath(table) },
                PlcWatchTable table => new MarkedObjectEntry { Kind = "WatchTable", Name = table.Name, Path = GetWatchTablePath(table) },
                PlcForceTable table => new MarkedObjectEntry { Kind = "ForceTable", Name = table.Name, Path = GetForceTablePath(table) },
                PlcExternalSource source => new MarkedObjectEntry { Kind = "ExternalSource", Name = source.Name, Path = GetExternalSourcePath(source) },
                null => new MarkedObjectEntry { Kind = "Unknown" },
                _ => new MarkedObjectEntry { Kind = obj.GetType().Name, Name = ReadName(obj) }
            };
        }

        private static string ReadName(IEngineeringObject obj)
        {
            try
            {
                return obj.GetAttribute("Name")?.ToString() ?? "";
            }
            catch (EngineeringException)
            {
                return "";
            }
        }

        #endregion
    }
}
