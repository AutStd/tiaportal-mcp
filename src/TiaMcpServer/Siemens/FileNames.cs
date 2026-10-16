using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace TiaMcpServer.Siemens
{
    /// <summary>
    /// Turns the name of a TIA Portal object into a name that is valid for a file.
    ///
    /// TIA Portal allows names such as 'T_Data->HMI' or 'GTS_EQ_I/FLower'. Used as a file name,
    /// '>' makes Path.Combine throw on .NET Framework and '/' silently creates a subfolder.
    /// Callers: the export and scratch-export code in Portal. Only the object name is cleaned;
    /// folder parts come from group paths. Reads/writes no data files.
    /// </summary>
    public static class FileNames
    {
        private const char Replacement = '_';

        // Windows device names are not usable as a file name, with or without an extension.
        private static readonly Regex ReservedName =
            new Regex(@"^(CON|PRN|AUX|NUL|COM[1-9]|LPT[1-9])(\..*)?$", RegexOptions.IgnoreCase);

        private static readonly char[] Invalid = Path.GetInvalidFileNameChars();

        /// <summary>
        /// Replaces characters that are invalid in a file name with '_', replaces trailing dots
        /// and spaces (Windows trims them), and prefixes a reserved device name. A name that is
        /// already valid is returned unchanged; an empty name becomes '_'.
        /// </summary>
        public static string Safe(string? name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Replacement.ToString();
            }

            var chars = name!.Select(c => Array.IndexOf(Invalid, c) >= 0 ? Replacement : c).ToArray();

            for (var i = chars.Length - 1; i >= 0 && (chars[i] == '.' || chars[i] == ' '); i--)
            {
                chars[i] = Replacement;
            }

            var safe = new string(chars);

            return ReservedName.IsMatch(safe) ? Replacement + safe : safe;
        }
    }
}
