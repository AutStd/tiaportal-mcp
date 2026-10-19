using ModelContextProtocol;
using ModelContextProtocol.Server;
using System;
using System.ComponentModel;
using System.Text.Json.Nodes;

namespace TiaMcpServer.ModelContextProtocol
{
    public static partial class McpServer
    {
        #region network

        [McpServerTool(Name = "GetNetworkInfo", Title = "Get network info", ReadOnly = true, OpenWorld = false, UseStructuredContent = true), Description("List the network nodes of the devices: IP address, subnet mask, PROFINET device name, subnet, IO system, device number and the type, order number, firmware and GSD file of the module carrying the interface")]
        public static ResponseNetworkInfo GetNetworkInfo(
            [Description("regexName: optional regular expression to filter the device names; empty string (default) for all devices")] string regexName = "")
        {
            try
            {
                var list = Portal.GetNetworkNodes(regexName);

                return new ResponseNetworkInfo
                {
                    Message = $"{list.Count} network node(s) retrieved",
                    Items = list,
                    Meta = new JsonObject
                    {
                        ["timestamp"] = DateTime.Now,
                        ["success"] = true
                    }
                };
            }
            catch (Exception ex) when (ex is not McpException)
            {
                throw new McpException($"Unexpected error retrieving network info: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
