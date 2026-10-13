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
        #region marking

        [McpServerTool(Name = "GetMarkedObjects", Title = "Get marked objects", ReadOnly = true, OpenWorld = false, UseStructuredContent = true), Description("List the objects that are marked for check-in in the open multiuser local session, with their mark state (marked by me, marked by others, up to date). Needs a local session (.als); a plain project has no marks")]
        public static ResponseMarkedObjects GetMarkedObjects(
            [Description("filter: optional, 'all' (default), 'me' for objects marked by me, or 'others' for objects marked by other users")] string filter = "all")
        {
            try
            {
                var items = Portal.GetMarkedObjects(filter)
                    .Select(entry => new ResponseMarkedObjectInfo
                    {
                        Kind = entry.Kind,
                        Name = entry.Name,
                        Path = entry.Path,
                        IsMarkedByMe = entry.IsMarkedByMe,
                        IsMarkedByOthers = entry.IsMarkedByOthers,
                        IsUpToDate = entry.IsUpToDate
                    })
                    .ToList();

                return new ResponseMarkedObjects
                {
                    Message = "Marked objects retrieved",
                    Names = items.Select(item => item.Name ?? "").ToList(),
                    Items = items,
                    Meta = Ok(new JsonObject { ["totalMarked"] = items.Count })
                };
            }
            catch (PortalException pex)
            {
                throw new McpException(pex.Message, pex);
            }
            catch (Exception ex) when (ex is not McpException)
            {
                throw new McpException($"Unexpected error retrieving marked objects: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
