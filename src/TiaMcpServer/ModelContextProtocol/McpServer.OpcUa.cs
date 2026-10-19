using ModelContextProtocol;
using ModelContextProtocol.Server;
using System;
using System.ComponentModel;

namespace TiaMcpServer.ModelContextProtocol
{
    public static partial class McpServer
    {
        // OPC UA XML export of one PLC.
        //
        // Callers: registered through Program.BuildTools() and invoked directly by the tests.
        // Data: returns ResponseExportOpcUaXml as MCP structuredContent. Writes one .xml file below
        // exportPath; it does not modify the project, so it is not a [WriteTool].

        [McpServerTool(Name = "ExportOpcUaXml", Title = "Export OPC UA data as XML", Destructive = true, Idempotent = true, OpenWorld = false, UseStructuredContent = true),
         Description("Export the OPC UA data of one PLC as an XML file named after the PLC into the 'OPC UA' folder below <exportPath>/<softwarePath>")]
        public static ResponseExportOpcUaXml ExportOpcUaXml(
            [Description("softwarePath: defines the path in the project structure to the PLC software")] string softwarePath,
            [Description("exportPath: directory on this machine that receives the XML file; the file is written to <exportPath>/<softwarePath>/OPC UA/")] string exportPath)
        {
            try
            {
                var (plc, filePath) = Portal.ExportOpcUaXml(softwarePath, exportPath);

                return new ResponseExportOpcUaXml
                {
                    Message = $"OPC UA data of '{softwarePath}' exported to '{filePath}'",
                    Name = plc.Name,
                    Path = filePath,
                    Meta = Ok()
                };
            }
            catch (TiaMcpServer.Siemens.PortalException pex)
            {
                throw new McpException(pex.Message, pex);
            }
            catch (Exception ex) when (ex is not McpException)
            {
                throw new McpException($"Unexpected error exporting OPC UA data of '{softwarePath}': {ex.Message}", ex);
            }
        }
    }
}
