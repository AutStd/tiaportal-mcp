using ModelContextProtocol.Server;
using System.ComponentModel;

namespace TiaMcpServer.ModelContextProtocol
{
    public static partial class McpPrompts
    {
        #region Device Templates

        [McpServerPrompt(Name = "GetDeviceInfo"), Description("Show details of one device")]
        public static string GetDeviceInfo(string devicePath)
        {
            return $@"Get info from a device from the current project/session.

Common parameter values:
- devicePath: defines the path in the project structure to the device

Use the GetDeviceInfo tool with these parameters:
- devicePath: {devicePath}";
        }

        [McpServerPrompt(Name = "GetDeviceItemInfo"), Description("Show details of one device item")]
        public static string GetDeviceItemInfo(string deviceItemPath)
        {
            return $@"Get info from a device item from the current project/session.

Common parameter values:
- deviceItemPath: defines the path in the project structure to the device item

Use the GetDeviceItemInfo tool with these parameters:
- deviceItemPath: {deviceItemPath}";
        }

        [McpServerPrompt(Name = "GetDevices"), Description("List the devices of the project or session")]
        public static string GetDevices()
        {
            return $@"Get a list of all devices in the project/session.

Use the GetDevices tool (it takes no parameters).";
        }

        #endregion

        #region Library Templates

        [McpServerPrompt(Name = "GetOpenLibraries"), Description("List the project library and the open global libraries")]
        public static string GetOpenLibraries()
        {
            return $@"Get the libraries that are open in TIA Portal: the project library and the global libraries, with name, path, state and attributes.

Use the GetOpenLibraries tool (it takes no parameters).";
        }

        [McpServerPrompt(Name = "GetReferencedLibraries"), Description("List the libraries whose types are used in the project")]
        public static string GetReferencedLibraries()
        {
            return $@"Get the libraries whose types are used in the current project, with the type versions taken from each library.

Use the GetReferencedLibraries tool (it takes no parameters).";
        }

        #endregion

        #region Multiuser Templates

        [McpServerPrompt(Name = "GetMarkedObjects"), Description("List the objects marked for check-in in a local session")]
        public static string GetMarkedObjects(
            [Description("filter: optional, 'all' (default), 'me' or 'others'")] string filter = "all")
        {
            return $@"Get the objects that are marked for check-in in the open multiuser local session, with their mark state.

Common parameter values:
- filter: 'all' lists every marked object, 'me' only those marked by me, 'others' only those marked by other users

Use the GetMarkedObjects tool with these parameters:
- filter: {filter}";
        }

        [McpServerPrompt(Name = "CheckIsUpToDate"), Description("Check whether a local session is up to date with the server")]
        public static string CheckIsUpToDate()
        {
            return @"Check whether the open multiuser local session is up to date with the server project.

Use the CheckIsUpToDate tool (no parameters). Run it before CommitChanges.";
        }

        [McpServerPrompt(Name = "CommitChanges"), Description("Commit the changes of a local session to the server (requires '--allow-write')")]
        public static string CommitChanges(
            [Description("comment: the comment stored with the new server revision")] string comment)
        {
            return $@"Commit the changes of the open multiuser local session to the server project (requires '--allow-write').

Notes:
- Run CheckIsUpToDate first; the commit fails when the session is not up to date
- The commit closes the local session; reopen it with OpenProject to continue

Use the CommitChanges tool with these parameters:
- comment: {comment}";
        }

        #endregion

        #region Software Templates

        [McpServerPrompt(Name = "GetSoftwareInfo"), Description("Show details of one PLC software")]
        public static string GetSoftwareInfo(string softwarePath)
        {
            return $@"Get PLC software info.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software

Use the GetSoftwareInfo tool with these parameters:
- softwarePath: {softwarePath}";
        }

        [McpServerPrompt(Name = "CompilePlcSoftware"), Description("Compile a whole PLC software and list every compiler message")]
        public static string CompilePlcSoftware(string softwarePath, string password = "")
        {
            return $@"Compile the whole PLC software and report every compiler message with the object it belongs to, so errors can be fixed without re-reading the whole PLC. Warnings are reported as a successful compile with detail; errors set meta.success to false and the call still returns every message.

Common parameter values:
- softwarePath: defines the path in the project structure to the software
- password: the password to access administration, default: no password

Use the CompilePlcSoftware tool with these parameters:
- softwarePath: {softwarePath}
- password: {password}";
        }

        [McpServerPrompt(Name = "CompileCodeBlock"), Description("Compile a single code block and list its compiler messages")]
        public static string CompileCodeBlock(string softwarePath, string blockPath, string password = "")
        {
            return $@"Compile a single code block (OB, FC, FB or DB) and report its compiler messages with the object each belongs to. Warnings are reported as a successful compile with detail; errors set meta.success to false and the call still returns every message.

Common parameter values:
- softwarePath: defines the path in the project structure to the software
- blockPath: defines the path in the project structure to the block, e.g. '0_OBs/Main_1'
- password: the password to access administration, default: no password

Use the CompileCodeBlock tool with these parameters:
- softwarePath: {softwarePath}
- blockPath: {blockPath}
- password: {password}";
        }

        [McpServerPrompt(Name = "ResolveObjectPath"), Description("Turn a partial object name into the full path other tools need")]
        public static string ResolveObjectPath(string softwarePath, string name, string kind = "any")
        {
            return $@"Turn a bare or partial object name into the root-relative path the other tools need, searching program blocks, data types, tags, tag tables, watch tables and external sources. Exact matches win; substring matches are only reported when nothing matches exactly.

Common parameter values:
- softwarePath: defines the path in the project structure to the software
- name: the object name to look for, e.g. 'FC_Block_1'. A full path may be passed; only its last segment is matched
- kind: restrict the search to 'block', 'type', 'tag', 'tagTable', 'watchTable' or 'source'. Default 'any' searches all of them

Use the ResolveObjectPath tool with these parameters:
- softwarePath: {softwarePath}
- name: {name}
- kind: {kind}";
        }

        [McpServerPrompt(Name = "FindInCode"), Description("Search block and type source text with a regular expression")]
        public static string FindInCode(string softwarePath, string pattern, string nameFilter = "", string maxResults = "200")
        {
            return $@"Search the actual source text of program blocks and PLC data types with a regular expression - every other filter in this server matches object names only. Returns the object path, line number and the matching line. Each call exports the candidate objects behind the scenes, so narrow a large PLC with 'nameFilter'.

Common parameter values:
- softwarePath: defines the path in the project structure to the software
- pattern: regular expression matched against each line, case-insensitive. Plain text works too
- nameFilter: optional regular expression on object names, to limit which objects are searched. Empty (default) searches all of them
- maxResults: stop after this many matching lines (default 200)

Use the FindInCode tool with these parameters:
- softwarePath: {softwarePath}
- pattern: {pattern}
- nameFilter: {nameFilter}
- maxResults: {maxResults}";
        }

        [McpServerPrompt(Name = "GetPlcSummary"), Description("Summarize one PLC software: counts, languages and health")]
        public static string GetPlcSummary(string softwarePath)
        {
            return $@"Counts, programming languages and health of one PLC software in a single call - replaces listing blocks, types, tags, tag tables and watch tables separately just to see what is there. Also names the inconsistent objects (which refuse to export until compiled) and the know-how protected ones (whose content cannot be read).

Common parameter values:
- softwarePath: defines the path in the project structure to the software

Use the GetPlcSummary tool with these parameters:
- softwarePath: {softwarePath}";
        }

        [McpServerPrompt(Name = "GetCrossReferences"), Description("List cross references of a PLC software or one object")]
        public static string GetCrossReferences(string softwarePath, string objectPath = "", string objectKind = "auto", string filter = "AllObjects", string maxDepth = "1")
        {
            return $@"Get cross references for a PLC software or for one block, type, tag table, tag or block group inside it. Watch tables, force tables and external sources have no cross references.

Common parameter values:
- softwarePath: defines the path in the project structure to the software
- objectPath: optional root-relative path of a block, type, tag table, tag or block group; empty targets the whole PLC software
- objectKind: 'auto' (default), 'block', 'type', 'tagTable', 'tag' or 'blockGroup'
- filter: 'AllObjects' (default), 'ObjectsWithReferences', 'ObjectsWithoutReferences' or 'UnusedObjects'
- maxDepth: 1 = sources and their references (default), 2 = also source children, 3 = also reference locations. Keeps large results manageable

Use the GetCrossReferences tool with these parameters:
- softwarePath: {softwarePath}
- objectPath: {objectPath}
- objectKind: {objectKind}
- filter: {filter}
- maxDepth: {maxDepth}";
        }

        [McpServerPrompt(Name = "WhereUsed"), Description("Find where a tag, block, type or tag table is used")]
        public static string WhereUsed(string softwarePath, string name, string kind = "any")
        {
            return $@"Answer 'what uses this?' for a tag, block, PLC data type or tag table by name. Resolves the name, picks the right object kind and flattens the cross-reference tree to a plain list of users. Use 'GetCrossReferences' instead when the full nested result or a specific filter is needed.

Common parameter values:
- softwarePath: defines the path in the project structure to the software
- name: the object to look up, by bare name or by full root-relative path
- kind: restrict resolution to 'block', 'type', 'tag' or 'tagTable'. Default 'any' picks the single match, and reports the candidates when the name is ambiguous

Use the WhereUsed tool with these parameters:
- softwarePath: {softwarePath}
- name: {name}
- kind: {kind}";
        }

        #endregion
    }
}
