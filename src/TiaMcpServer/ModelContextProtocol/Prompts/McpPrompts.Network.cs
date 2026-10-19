using ModelContextProtocol.Server;
using System.ComponentModel;

namespace TiaMcpServer.ModelContextProtocol
{
    public static partial class McpPrompts
    {
        #region Network Templates

        [McpServerPrompt(Name = "GetNetworkInfo"), Description("List IP addresses, PROFINET device names and IO systems of the devices")]
        public static string GetNetworkInfo(string regexName = "")
        {
            return $@"Get the network nodes of the devices in the current project/session: IP address, subnet mask, PROFINET device name, subnet, IO system and device number, plus type, order number, firmware and GSD file of the module carrying the interface.

Common parameter values:
- regexName: optional regular expression to filter the device names. Empty (default) lists all devices

Use the GetNetworkInfo tool with these parameters:
- regexName: {regexName}";
        }

        #endregion
    }
}
