using Microsoft.Extensions.Logging;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.HW.Features;
using Siemens.Engineering.HmiUnified;
using Siemens.Engineering.Multiuser;
using Siemens.Engineering.SW;
using Siemens.Engineering.SW.Blocks;
using Siemens.Engineering.SW.Types;
using Siemens.Engineering.Safety;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;

namespace TiaMcpServer.Siemens
{
    public partial class Portal
    {
        #region devices

        public List<Device> GetDevices(string regexName = "")
        {
            _logger?.LogInformation("Getting devices...");

            if (IsProjectNull())
            {
                return [];
            }

            return EnumerateAllDevices()
                .Where(device => MatchesRegex(device, regexName))
                .ToList();
        }

        public Device? GetDevice(string devicePath)
        {
            _logger?.LogInformation($"Getting device by path: {devicePath}");

            if (IsProjectNull())
            {
                return null;
            }

            // Retrieve the device by its path
            return GetDeviceByPath(devicePath);
        }

        public DeviceItem? GetDeviceItem(string deviceItemPath)
        {
            return GetDeviceItem(deviceItemPath, out _);
        }

        /// <summary>
        /// Like GetDeviceItem, with reason naming the first path segment that was not found.
        /// </summary>
        public DeviceItem? GetDeviceItem(string deviceItemPath, out string? reason)
        {
            _logger?.LogInformation($"Getting device item by path: {deviceItemPath}");

            reason = null;

            if (IsProjectNull())
            {
                reason = "no project open";
                return null;
            }

            return GetDeviceItemByPath(deviceItemPath, out reason);
        }

        #endregion

        #region get...by path

        private Device? GetDeviceByPath(string devicePath)
        {
            if (_project?.Devices == null || string.IsNullOrWhiteSpace(devicePath))
                return null;

            // A device name can itself contain '/' (e.g. 'S7-1500/ET200MP station_1'), so try the
            // whole input as a name before splitting it into group/device segments.
            var rootDevices = _project.Devices.Concat(EnumerateUngroupedDevices()).ToList();
            var exactMatch = FindDeviceByName(rootDevices, devicePath.Trim('/'));
            if (exactMatch != null)
            {
                return exactMatch;
            }

            var pathSegments = devicePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathSegments.Length == 0)
            {
                return null;
            }

            // Try top-level device first
            if (pathSegments.Length == 1)
            {
                return null;
            }

            // Traverse device groups
            DeviceUserGroupComposition? groups = _project.DeviceGroups;
            DeviceUserGroup? group = groups?.FirstOrDefault(g => g.Name.Equals(pathSegments[0], StringComparison.OrdinalIgnoreCase));

            if (group is null)
            {
                return null;
            }

            for (int i = 1; i < pathSegments.Length; i++)
            {
                // Try to find device in current group; the remaining segments may form one name containing '/'
                var device = FindDeviceByName(group.Devices, string.Join("/", pathSegments.Skip(i)))
                    ?? FindDeviceByName(group.Devices, pathSegments[i]);
                if (device != null)
                {
                    return device;
                }

                // Try to find subgroup
                group = group.Groups.FirstOrDefault(g => g.Name.Equals(pathSegments[i], StringComparison.OrdinalIgnoreCase));
                if (group is null)
                {
                    break;
                }
            }

            return null;
        }

        private static Device? FindDeviceByName(IEnumerable<Device> devices, string name)
        {
            return devices.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Result of one attempt to resolve a device item path from a device.
        /// Matched counts the path segments that were found, Error says why the rest was not.
        /// </summary>
        private sealed class DeviceItemLookup
        {
            public DeviceItem? Item { get; set; }
            public int Matched { get; set; }
            public string? Error { get; set; }
        }

        /// <summary>
        /// Resolves '<device>/<item>/<subitem>/...' as GetNetworkInfo lists it, optionally preceded by
        /// the device groups ('Group1/Group1.1/<device>/...'). The device name can itself contain '/'.
        /// A hardware PLC has a device name like 'S7-1500/ET200MP station_1' that the TIA Portal IDE
        /// does not show, so the device name may also be left out ('PLC_1/...' or 'Group1/PLC_1/...').
        /// Every segment below the device must name an existing device item, at any depth; otherwise
        /// the result is null and reason names the first segment that was not found.
        /// </summary>
        private DeviceItem? GetDeviceItemByPath(string deviceItemPath, out string? reason)
        {
            reason = null;

            if (_project is null || _project.Devices == null || string.IsNullOrWhiteSpace(deviceItemPath))
            {
                reason = "no project open or empty path";
                return null;
            }

            var pathSegments = deviceItemPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var attempts = new List<DeviceItemLookup>();

            foreach (var device in EnumerateAllDevices())
            {
                var groupPath = GetDeviceGroupPath(device);
                var starts = new List<int> { 0 };
                if (groupPath.Count > 0 && StartsWithSegments(pathSegments, groupPath))
                {
                    starts.Add(groupPath.Count);
                }

                foreach (var start in starts)
                {
                    // path with the device name
                    var deviceSegments = device.Name.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                    if (MatchesSegments(pathSegments, start, deviceSegments))
                    {
                        var itemStart = start + deviceSegments.Length;
                        if (itemStart == pathSegments.Length)
                        {
                            attempts.Add(new DeviceItemLookup { Matched = itemStart, Error = $"'{device.Name}' is a device, not a device item; use GetDeviceInfo or add the device item name" });
                        }
                        else
                        {
                            attempts.Add(ResolveDeviceItems(device.DeviceItems, pathSegments, itemStart, device.Name));
                        }
                    }

                    // path without the device name, starting at a top-level device item
                    if (start < pathSegments.Length)
                    {
                        attempts.Add(ResolveDeviceItems(device.DeviceItems, pathSegments, start, device.Name));
                    }
                }

                var found = attempts.FirstOrDefault(a => a.Item != null);
                if (found != null)
                {
                    return found.Item;
                }
            }

            var best = attempts.Where(a => a.Matched > 0).OrderByDescending(a => a.Matched).FirstOrDefault();
            reason = best?.Error ?? $"no device and no top-level device item named '{pathSegments.FirstOrDefault()}'";
            return null;
        }

        /// <summary>
        /// Walks down the device items segment by segment. A device item name may contain '/',
        /// so the longest joined run of segments that names a child wins.
        /// </summary>
        private static DeviceItemLookup ResolveDeviceItems(DeviceItemComposition items, string[] pathSegments, int index, string parentPath)
        {
            DeviceItem? current = null;
            var children = items;

            while (index < pathSegments.Length)
            {
                DeviceItem? next = null;
                int used = 0;

                for (int count = pathSegments.Length - index; count > 0 && next is null; count--)
                {
                    var name = string.Join("/", pathSegments, index, count);
                    next = children.FirstOrDefault(di => di.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    used = count;
                }

                if (next is null)
                {
                    var available = children.Select(di => $"'{di.Name}'").ToList();
                    return new DeviceItemLookup
                    {
                        Matched = index,
                        Error = available.Count == 0
                            ? $"'{pathSegments[index]}' not found, '{parentPath}' has no device items"
                            : $"'{pathSegments[index]}' not found below '{parentPath}'; available: {string.Join(", ", available)}"
                    };
                }

                current = next;
                parentPath = $"{parentPath}/{next.Name}";
                index += used;
                children = next.DeviceItems;
            }

            return new DeviceItemLookup { Item = current, Matched = index };
        }

        // Names of the user groups that contain the device, outermost first; empty for top-level and ungrouped devices
        private static List<string> GetDeviceGroupPath(Device device)
        {
            var names = new List<string>();
            var parent = device.Parent;

            while (parent is DeviceUserGroup group)
            {
                names.Insert(0, group.Name);
                parent = group.Parent;
            }

            return names;
        }

        private static bool StartsWithSegments(string[] pathSegments, List<string> prefix)
        {
            return MatchesSegments(pathSegments, 0, prefix.ToArray());
        }

        private static bool MatchesSegments(string[] pathSegments, int start, string[] expected)
        {
            if (expected.Length == 0 || start + expected.Length > pathSegments.Length)
            {
                return false;
            }

            for (int i = 0; i < expected.Length; i++)
            {
                if (!pathSegments[start + i].Equals(expected[i], StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        #region get recursive ...

        /// <summary>
        /// All devices of the open project: top level, every user group (recursively) and the
        /// "Ungrouped devices" system group, which is a DeviceSystemGroup and not part of DeviceGroups.
        /// </summary>
        private IEnumerable<Device> EnumerateAllDevices()
        {
            if (_project is null)
            {
                yield break;
            }

            if (_project.Devices != null)
            {
                foreach (var device in _project.Devices)
                {
                    yield return device;
                }
            }

            if (_project.DeviceGroups != null)
            {
                foreach (var group in _project.DeviceGroups)
                {
                    foreach (var device in EnumerateGroupDevices(group))
                    {
                        yield return device;
                    }
                }
            }

            foreach (var device in EnumerateUngroupedDevices())
            {
                yield return device;
            }
        }

        private IEnumerable<Device> EnumerateUngroupedDevices()
        {
            var devices = _project?.UngroupedDevicesGroup?.Devices;
            if (devices is null)
            {
                return [];
            }

            return devices;
        }

        private static IEnumerable<Device> EnumerateGroupDevices(DeviceUserGroup group)
        {
            foreach (var device in group.Devices)
            {
                yield return device;
            }

            foreach (var subgroup in group.Groups)
            {
                foreach (var device in EnumerateGroupDevices(subgroup))
                {
                    yield return device;
                }
            }
        }

        private static bool MatchesRegex(Device device, string regexName)
        {
            if (string.IsNullOrEmpty(regexName))
            {
                return true;
            }

            try
            {
                return Regex.IsMatch(device.Name, regexName, RegexOptions.IgnoreCase);
            }
            catch (ArgumentException)
            {
                // Invalid regex pattern, skip this device
                return false;
            }
        }

        #endregion
    }
}
