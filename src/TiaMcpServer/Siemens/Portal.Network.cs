using Microsoft.Extensions.Logging;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TiaMcpServer.Siemens
{
    /// <summary>
    /// One network node (interface address) of a device item, with the IO-system data of the
    /// interface and the type data of the module that carries the interface.
    /// </summary>
    public class NetworkNodeInfo
    {
        public string? Device { get; set; }
        public string? DeviceItem { get; set; }
        public string? Node { get; set; }
        public string? IpAddress { get; set; }
        public string? SubnetMask { get; set; }
        public string? PnDeviceName { get; set; }
        public bool? PnDeviceNameAutoGeneration { get; set; }
        public string? Subnet { get; set; }
        public string? IoSystem { get; set; }
        public int? PnDeviceNumber { get; set; }
        public string? TypeName { get; set; }
        public string? OrderNumber { get; set; }
        public string? FirmwareVersion { get; set; }
        public string? GsdName { get; set; }
    }

    public partial class Portal
    {
        #region network

        /// <summary>
        /// Network nodes of all devices, optionally only of devices whose name matches regexName.
        /// IP address and PROFINET device name are not attributes of Device or DeviceItem; Openness
        /// provides them only through the NetworkInterface service of the interface device item.
        /// </summary>
        public List<NetworkNodeInfo> GetNetworkNodes(string regexName = "")
        {
            _logger?.LogInformation("Getting network nodes...");

            var list = new List<NetworkNodeInfo>();

            if (IsProjectNull())
            {
                return list;
            }

            foreach (var device in EnumerateAllDevices().Where(device => MatchesRegex(device, regexName)))
            {
                CollectNetworkNodes(device.Name, device.Name, device.DeviceItems, list);
            }

            return list;
        }

        private static void CollectNetworkNodes(string deviceName, string parentPath, DeviceItemComposition items, List<NetworkNodeInfo> list)
        {
            foreach (var item in items)
            {
                var path = $"{parentPath}/{item.Name}";
                var networkInterface = item.GetService<NetworkInterface>();

                if (networkInterface != null)
                {
                    // the module carrying the interface holds order number, firmware and GSD file
                    var module = item.Parent as DeviceItem ?? item;
                    var ioConnector = networkInterface.IoConnectors.FirstOrDefault();

                    foreach (var node in networkInterface.Nodes)
                    {
                        list.Add(new NetworkNodeInfo
                        {
                            Device = deviceName,
                            DeviceItem = path,
                            Node = node.Name,
                            IpAddress = ReadText(node, "Address"),
                            SubnetMask = ReadText(node, "SubnetMask"),
                            PnDeviceName = ReadText(node, "PnDeviceName"),
                            PnDeviceNameAutoGeneration = ReadValue(node, "PnDeviceNameAutoGeneration") as bool?,
                            Subnet = node.ConnectedSubnet?.Name,
                            IoSystem = ioConnector?.ConnectedToIoSystem?.Name,
                            PnDeviceNumber = ioConnector == null ? null : ReadValue(ioConnector, "PnDeviceNumber") as int?,
                            TypeName = ReadText(module, "TypeName"),
                            OrderNumber = ReadText(module, "OrderNumber"),
                            FirmwareVersion = ReadText(module, "FirmwareVersion"),
                            GsdName = ReadText(module, "GsdName")
                        });
                    }
                }

                CollectNetworkNodes(deviceName, path, item.DeviceItems, list);
            }
        }

        // Not every object offers every attribute (e.g. GsdName only on GSD devices)
        private static object? ReadValue(IEngineeringObject obj, string name)
        {
            try
            {
                return obj.GetAttribute(name);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string? ReadText(IEngineeringObject obj, string name)
        {
            var value = ReadValue(obj, name)?.ToString();
            return string.IsNullOrEmpty(value) ? null : value;
        }

        #endregion
    }
}
