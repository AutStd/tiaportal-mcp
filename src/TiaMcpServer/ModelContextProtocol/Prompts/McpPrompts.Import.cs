using ModelContextProtocol.Server;
using System.ComponentModel;

namespace TiaMcpServer.ModelContextProtocol
{
    public static partial class McpPrompts
    {
        #region Import From Documents Templates

        [McpServerPrompt(Name = "ImportFromDocuments"), Description("Import one block from documents (.s7dcl/.s7res, V20+)")]
        public static string ImportFromDocuments(string softwarePath, string importPath, string fileNameWithoutExtension, string groupPath = "", string importOption = "Override")
        {
            return $@"Import a program block from documents into PLC software (requires TIA Portal V20 or newer).

Common parameter values:
- softwarePath: e.g. 'PLC_1' for hardware PLC
- groupPath: optional, e.g. 'Program blocks/FBs'
- importPath: folder containing .s7dcl/.s7res files; read from <importPath>/<softwarePath>/
- fileNameWithoutExtension: e.g. 'FC_DateTime'
- importOption: 'Override' (default), 'None', 'SkipInactiveCultures', 'ActivateInactiveCultures'

Note: As of 2025-09-02, importing Ladder (LAD) blocks requires the companion .s7res to contain en-US tags for all items; otherwise import may fail.

Use the ImportFromDocuments tool with these parameters:
- softwarePath: {softwarePath}
- groupPath: {groupPath}
- importPath: {importPath}
- fileNameWithoutExtension: {fileNameWithoutExtension}
- importOption: {importOption}";
        }

        [McpServerPrompt(Name = "ImportTypesFromDocuments"), Description("Import PLC data types from documents (.s7dcl/.s7res, V21+)")]
        public static string ImportTypesFromDocuments(string softwarePath, string importPath, string groupPath = "", string regexName = "", string importOption = "Override")
        {
            return $@"Import types (UDTs) from documents into PLC software (requires TIA Portal V21 or newer and the server started with '--allow-write').

Common parameter values:
- softwarePath: e.g. 'PLC_1' for hardware PLC
- groupPath: optional target group, empty for the 'PLC data types' root. A leading 'PLC data types' segment from a preservePath export is accepted
- importPath: folder holding the document files; read from <importPath>/<softwarePath>/
- regexName: empty for all, or e.g. 'UDT_.*'
- importOption: 'Override' (default), 'None', 'SkipInactiveCultures', 'ActivateInactiveCultures'

Use ImportTypeFromDocuments instead when you want a single named document set.

Use the ImportTypesFromDocuments tool with these parameters:
- softwarePath: {softwarePath}
- groupPath: {groupPath}
- importPath: {importPath}
- regexName: {regexName}
- importOption: {importOption}";
        }

        [McpServerPrompt(Name = "ImportBlocksFromDocuments"), Description("Import blocks from documents (.s7dcl/.s7res, V20+)")]
        public static string ImportBlocksFromDocuments(string softwarePath, string importPath, string groupPath = "", string regexName = "", string importOption = "Override")
        {
            return $@"Import multiple program blocks from SIMATIC SD documents into PLC software (requires TIA Portal V20 or newer).

Common parameter values:
- softwarePath: e.g. 'PLC_1' for hardware PLC
- groupPath: optional target group path, empty for root
- importPath: folder containing .s7dcl/.s7res files; read from <importPath>/<softwarePath>/
- regexName: empty for all, or e.g. 'FB_.*'
- importOption: 'Override' (default), 'None', 'SkipInactiveCultures', 'ActivateInactiveCultures'

Note: As of 2025-09-02, importing Ladder (LAD) blocks requires the companion .s7res to contain en-US tags for all items; otherwise import may fail.

Use the ImportBlocksFromDocuments tool with these parameters:
- softwarePath: {softwarePath}
- groupPath: {groupPath}
- importPath: {importPath}
- regexName: {regexName}
- importOption: {importOption}";
        }

        #endregion

        #region Import From Sources Templates

        [McpServerPrompt(Name = "ImportSources"), Description("Import a folder tree of source files (.db, .scl, .awl, .udt)")]
        public static string ImportSources(string softwarePath, string importPath, string regexName = "", string keepOnError = "false")
        {
            return $@"Import every block and PLC data type source file (.db, .awl, .scl, .udt) under a folder tree into PLC software (requires the server started with '--allow-write').

Common parameter values:
- softwarePath: e.g. 'PLC_1' for hardware PLC
- importPath: folder to walk recursively, typically the tree the ExportSources tool wrote. Each file goes into the block or PLC data type group its folder path implies; the group must already exist; read from <importPath>/<softwarePath>/
- regexName: empty for all, or e.g. 'FB_.*' to select files by base name (use one file name to import a single object)
- keepOnError: Use false (default) to roll back a whole file on any error, true to keep the objects of a file that were generated successfully

Use the ImportSources tool with these parameters:
- softwarePath: {softwarePath}
- importPath: {importPath}
- regexName: {regexName}
- keepOnError: {NormalizeBool(keepOnError)}";
        }

        [McpServerPrompt(Name = "ImportSourceBlock"), Description("Import one block from a source file (.db, .scl or .awl)")]
        public static string ImportSourceBlock(string softwarePath, string groupPath, string importPath, string keepOnError = "false")
        {
            return $@"Compile one block source file into a block group of PLC software (requires the server started with '--allow-write').

Common parameter values:
- softwarePath: e.g. 'PLC_1' for hardware PLC
- groupPath: block user group that receives the blocks, e.g. '0_OBs'. Empty for the source's default location; the 'Program blocks' root itself is not allowed
- importPath: full path of the .db, .scl or .awl file, typically one the ExportSourceBlock tool wrote
- keepOnError: Use false (default) to roll back the whole file on any error, true to keep the objects that were generated successfully

Use the ImportSourceBlock tool with these parameters:
- softwarePath: {softwarePath}
- groupPath: {groupPath}
- importPath: {importPath}
- keepOnError: {NormalizeBool(keepOnError)}";
        }

        [McpServerPrompt(Name = "ImportSourceType"), Description("Import one PLC data type from a .udt source file")]
        public static string ImportSourceType(string softwarePath, string groupPath, string importPath, string keepOnError = "false")
        {
            return $@"Compile one '*.udt' source file into a PLC data type group of PLC software (requires the server started with '--allow-write').

Common parameter values:
- softwarePath: e.g. 'PLC_1' for hardware PLC
- groupPath: PLC data type user group that receives the types, e.g. 'Common'. Empty for the source's default location; the 'PLC data types' root itself is not allowed
- importPath: full path of the .udt file, typically one the ExportSourceType tool wrote
- keepOnError: Use false (default) to roll back the whole file on any error, true to keep the objects that were generated successfully

Use the ImportSourceType tool with these parameters:
- softwarePath: {softwarePath}
- groupPath: {groupPath}
- importPath: {importPath}
- keepOnError: {NormalizeBool(keepOnError)}";
        }

        [McpServerPrompt(Name = "ImportSourceBlocks"), Description("Import all block source files of a folder into a group")]
        public static string ImportSourceBlocks(string softwarePath, string groupPath, string importPath, string regexName = "", string preservePath = "false", string keepOnError = "false")
        {
            return $@"Compile every block source file (.db, .scl, .awl) of a folder into a block group of PLC software (requires the server started with '--allow-write'). A file that fails is reported and the rest are still imported.

Common parameter values:
- softwarePath: e.g. 'PLC_1' for hardware PLC
- groupPath: block user group that receives the blocks, e.g. '0_OBs'. Empty for each source's default location; the 'Program blocks' root itself is not allowed
- importPath: folder with the source files, typically the one the ExportSourceBlocks tool wrote; read from <importPath>/<softwarePath>/
- regexName: empty for all, or e.g. 'FB_.*' to select files by base name
- preservePath: Use false (default) to read only the files directly in importPath, true to also read its subfolders into the matching subgroups of groupPath (the groups must already exist)
- keepOnError: Use false (default) to roll back a whole file on any error, true to keep the objects of a file that were generated successfully

Use the ImportSources tool instead to import blocks and PLC data types in one go.

Use the ImportSourceBlocks tool with these parameters:
- softwarePath: {softwarePath}
- groupPath: {groupPath}
- importPath: {importPath}
- regexName: {regexName}
- preservePath: {NormalizeBool(preservePath)}
- keepOnError: {NormalizeBool(keepOnError)}";
        }

        [McpServerPrompt(Name = "ImportSourceTypes"), Description("Import all .udt source files of a folder into a group")]
        public static string ImportSourceTypes(string softwarePath, string groupPath, string importPath, string regexName = "", string preservePath = "false", string keepOnError = "false")
        {
            return $@"Compile every '*.udt' source file of a folder into a PLC data type group of PLC software (requires the server started with '--allow-write'). A file that fails is reported and the rest are still imported.

Common parameter values:
- softwarePath: e.g. 'PLC_1' for hardware PLC
- groupPath: PLC data type user group that receives the types, e.g. 'Common'. Empty for each source's default location; the 'PLC data types' root itself is not allowed
- importPath: folder with the .udt files, typically the one the ExportSourceTypes tool wrote; read from <importPath>/<softwarePath>/
- regexName: empty for all, or e.g. 'UDT_.*' to select files by base name
- preservePath: Use false (default) to read only the files directly in importPath, true to also read its subfolders into the matching subgroups of groupPath (the groups must already exist)
- keepOnError: Use false (default) to roll back a whole file on any error, true to keep the objects of a file that were generated successfully

Use the ImportSources tool instead to import blocks and PLC data types in one go.

Use the ImportSourceTypes tool with these parameters:
- softwarePath: {softwarePath}
- groupPath: {groupPath}
- importPath: {importPath}
- regexName: {regexName}
- preservePath: {NormalizeBool(preservePath)}
- keepOnError: {NormalizeBool(keepOnError)}";
        }

        [McpServerPrompt(Name = "ImportExternalSource"), Description("Compile an external source already in the project into blocks")]
        public static string ImportExternalSource(string softwarePath, string sourcePath, string targetGroupPath = "", string keepOnError = "false")
        {
            return $@"Compile/import an external source that is already registered in PLC software into program blocks and PLC data types (requires the server started with '--allow-write').

Common parameter values:
- softwarePath: e.g. 'PLC_1' for hardware PLC
- sourcePath: root-relative path of the external source, e.g. 'SourceGroup1/Source_1'. Use the GetExternalSources tool to list them
- targetGroupPath: optional block user group that receives the blocks, empty for the source's default location. The 'Program blocks' root itself is not allowed
- keepOnError: Use false (default) to generate nothing when any object fails, true to keep the objects that were generated successfully

Use the ImportSourceBlock or ImportSourceType tool instead to import a source file from disk, or ImportSources for a whole folder tree.

Use the ImportExternalSource tool with these parameters:
- softwarePath: {softwarePath}
- sourcePath: {sourcePath}
- targetGroupPath: {targetGroupPath}
- keepOnError: {NormalizeBool(keepOnError)}";
        }

        #endregion

        #region Convenience Import Templates

        [McpServerPrompt(Name = "ImportAllSources"), Description("Import all source files of a folder tree")]
        public static string ImportAllSources(string softwarePath, string importPath)
        {
            return ImportSources(softwarePath, importPath, "", "false");
        }

        [McpServerPrompt(Name = "ImportAllSourcesKeepOnError"), Description("Import all sources; keep the objects generated successfully on error")]
        public static string ImportAllSourcesKeepOnError(string softwarePath, string importPath)
        {
            return ImportSources(softwarePath, importPath, "", "true");
        }

        #endregion
    }
}
