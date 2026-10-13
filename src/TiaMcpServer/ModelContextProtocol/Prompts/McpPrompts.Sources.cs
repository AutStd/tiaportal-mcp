using ModelContextProtocol.Server;
using System.ComponentModel;

namespace TiaMcpServer.ModelContextProtocol
{
    public static partial class McpPrompts
    {
        #region Document Templates

        [McpServerPrompt(Name = "ExportTypeAsDocuments"), Description("Export one PLC data type as documents (.s7dcl/.s7res)")]
        public static string ExportTypeAsDocuments(string softwarePath, string typePath, string exportPath, string preservePath = "false")
        {
            return $@"Export type as a SIMATIC Source Document set (.s7dcl plus an optional .s7res) instead of XML. The response lists the files TIA Portal actually wrote. Requires TIA Portal V21 or newer.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- typePath: root-relative path of the PLC data type, e.g. 'Common/BtnTyp_X'. Use 'GetTypes' to list them
- exportPath: directory on this machine that receives the document files
- preservePath: recreate the group structure below <exportPath>/<softwarePath>, inside the 'PLC data types' system folder as TIA Portal names it in the current interface language

Use the ExportTypeAsDocuments tool with these parameters:
- softwarePath: {softwarePath}
- typePath: {typePath}
- exportPath: {exportPath}
- preservePath: {NormalizeBool(preservePath)}";
        }

        [McpServerPrompt(Name = "ImportTypeFromDocuments"), Description("Import one PLC data type from documents (.s7dcl/.s7res)")]
        public static string ImportTypeFromDocuments(string softwarePath, string groupPath, string importPath, string fileNameWithoutExtension, string importOption = "Override")
        {
            return $@"Import one type from a SIMATIC Source Document set (.s7dcl plus an optional .s7res) on the file system of the machine running this server. A type name is unique across the whole PLC, so importing an existing name into a different group fails even with importOption 'Override' - target the group the type already lives in. Requires TIA Portal V21 or newer (requires the server started with '--allow-write').

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- groupPath: root-relative type group that receives the type; empty uses the PLC data types root. A leading 'PLC data types' segment, as written by preservePath exports, is accepted and ignored
- importPath: directory containing the document files
- fileNameWithoutExtension: base name of the document set, e.g. 'BtnTyp_X'
- importOption: ImportDocumentOptions value (None, Override, SkipInactiveCultures, ActivateInactiveCultures)

Use the ImportTypeFromDocuments tool with these parameters:
- softwarePath: {softwarePath}
- groupPath: {groupPath}
- importPath: {importPath}
- fileNameWithoutExtension: {fileNameWithoutExtension}
- importOption: {importOption}";
        }

        [McpServerPrompt(Name = "ExportAsDocuments"), Description("Export one block as documents (.s7dcl/.s7res)")]
        public static string ExportAsDocuments(string softwarePath, string blockPath, string exportPath, string preservePath = "false")
        {
            return $@"Export as documents (.s7dcl/.s7res) from a block in the PLC software to path.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- blockPath: defines the path in the project structure to the block
- exportPath: defines the path where to export the documents
- preservePath: preserves the path/structure of the PLC software

Use the ExportAsDocuments tool with these parameters:
- softwarePath: {softwarePath}
- blockPath: {blockPath}
- exportPath: {exportPath}
- preservePath: {NormalizeBool(preservePath)}";
        }

        #endregion

        #region Source File Templates

        [McpServerPrompt(Name = "GetXmlBlock"), Description("Show one block as SimaticML XML without exporting a file")]
        public static string GetXmlBlock(string softwarePath, string blockPath, string maxChars = "40000")
        {
            return $@"Return one program block as SimaticML XML text directly, instead of exporting a file and reading it back. Works for every consistent block, whatever its language.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- blockPath: root-relative path of the block, e.g. '0_OBs/Main'. Use 'ResolveObjectPath' if you only know the name
- maxChars: truncate the text at this many characters, on a line boundary (default 40000)

Use the GetXmlBlock tool with these parameters:
- softwarePath: {softwarePath}
- blockPath: {blockPath}
- maxChars: {maxChars}";
        }

        [McpServerPrompt(Name = "GetSourceBlock"), Description("Show one block as external source text (.scl/.db/.awl) without exporting a file")]
        public static string GetSourceBlock(string softwarePath, string blockPath, string withDependencies = "false", string maxChars = "40000")
        {
            return $@"Return one program block as TIA Portal external source text ('.scl' for SCL, '.db' for data blocks, '.awl' for STL) directly, in the format the compiler reads back. LAD, FBD and GRAPH blocks have no source form and are rejected - use GetXmlBlock or GetDocumentsBlock for those.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- blockPath: root-relative path of the block, e.g. '0_OBs/Main'. Use 'ResolveObjectPath' if you only know the name
- withDependencies: also include every object the block uses, so the text compiles on its own. Default false
- maxChars: truncate the text at this many characters, on a line boundary (default 40000)

Use the GetSourceBlock tool with these parameters:
- softwarePath: {softwarePath}
- blockPath: {blockPath}
- withDependencies: {NormalizeBool(withDependencies)}
- maxChars: {maxChars}";
        }

        [McpServerPrompt(Name = "GetDocumentsBlock"), Description("Show one block as SIMATIC Source Document text without exporting a file")]
        public static string GetDocumentsBlock(string softwarePath, string blockPath, string maxChars = "40000")
        {
            return $@"Return one program block as readable SIMATIC Source Document text (.s7dcl/.s7res, TIA Portal V20+) directly. STL and mixed-language blocks have no source document and are rejected - use GetXmlBlock or GetSourceBlock for those.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- blockPath: root-relative path of the block, e.g. '0_OBs/Main'. Use 'ResolveObjectPath' if you only know the name
- maxChars: truncate the text at this many characters, on a line boundary (default 40000)

Use the GetDocumentsBlock tool with these parameters:
- softwarePath: {softwarePath}
- blockPath: {blockPath}
- maxChars: {maxChars}";
        }

        [McpServerPrompt(Name = "GetXmlType"), Description("Show one PLC data type as SimaticML XML without exporting a file")]
        public static string GetXmlType(string softwarePath, string typePath, string maxChars = "40000")
        {
            return $@"Return one PLC data type as SimaticML XML text directly, instead of exporting a file and reading it back.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- typePath: root-relative path of the PLC data type, e.g. 'Common/BtnTyp_X'. Use 'ResolveObjectPath' if you only know the name
- maxChars: truncate the text at this many characters, on a line boundary (default 40000)

Use the GetXmlType tool with these parameters:
- softwarePath: {softwarePath}
- typePath: {typePath}
- maxChars: {maxChars}";
        }

        [McpServerPrompt(Name = "GetSourceType"), Description("Show one PLC data type as external source text (.udt) without exporting a file")]
        public static string GetSourceType(string softwarePath, string typePath, string withDependencies = "false", string maxChars = "40000")
        {
            return $@"Return one PLC data type as TIA Portal external source text ('.udt', the TYPE ... END_TYPE declaration) directly, in the format the compiler reads back. Needs no TIA Portal V21.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- typePath: root-relative path of the PLC data type, e.g. 'Common/BtnTyp_X'. Use 'ResolveObjectPath' if you only know the name
- withDependencies: also include every data type this one uses. Default false
- maxChars: truncate the text at this many characters, on a line boundary (default 40000)

Use the GetSourceType tool with these parameters:
- softwarePath: {softwarePath}
- typePath: {typePath}
- withDependencies: {NormalizeBool(withDependencies)}
- maxChars: {maxChars}";
        }

        [McpServerPrompt(Name = "GetDocumentsType"), Description("Show one PLC data type as SIMATIC Source Document text without exporting a file")]
        public static string GetDocumentsType(string softwarePath, string typePath, string maxChars = "40000")
        {
            return $@"Return one PLC data type as readable SIMATIC Source Document text (.s7dcl/.s7res) directly. Requires TIA Portal V21; on older versions use GetSourceType or GetXmlType.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- typePath: root-relative path of the PLC data type, e.g. 'Common/BtnTyp_X'. Use 'ResolveObjectPath' if you only know the name
- maxChars: truncate the text at this many characters, on a line boundary (default 40000)

Use the GetDocumentsType tool with these parameters:
- softwarePath: {softwarePath}
- typePath: {typePath}
- maxChars: {maxChars}";
        }

        [McpServerPrompt(Name = "ExportPlcAsDocuments"), Description("Export a whole PLC software as one folder tree of documents")]
        public static string ExportPlcAsDocuments(string softwarePath, string exportPath)
        {
            return $@"Write a whole PLC software to one folder tree that mirrors the project, ready to commit: program blocks and PLC data types as readable SIMATIC Source Documents where TIA Portal supports them, tag tables and watch tables as XML, each below its localised system folder. Replaces running the four bulk exports separately. Objects that cannot be exported are reported instead of failing the snapshot.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- exportPath: directory on this machine that receives the tree; existing files of the same name are overwritten

Use the ExportPlcAsDocuments tool with these parameters:
- softwarePath: {softwarePath}
- exportPath: {exportPath}";
        }

        [McpServerPrompt(Name = "ExportSourceBlocks"), Description("Export blocks as source files, optionally filtered by name regex (no types)")]
        public static string ExportSourceBlocks(string softwarePath, string groupPath, string exportPath, string withDependencies = "false", string preservePath = "false", string regexName = "")
        {
            return $@"Write every program block below a block group, including all subgroups, as TIA Portal external source files. Blocks with no source form (LAD, FBD, GRAPH), inconsistent blocks and know-how protected ones are reported in 'Skipped' instead of failing the run.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- groupPath: root-relative path of the block group to export recursively, e.g. '0_OBs'. Empty means all blocks below 'Program blocks'
- exportPath: directory on this machine that receives the files; existing files of the same name are overwritten
- withDependencies: also write every object each block uses into its file. Default false
- preservePath: mirror the project groups below '<exportPath>/<softwarePath>/Program blocks'. Default false, which writes straight into <exportPath>/<softwarePath>
- regexName: optional regular expression, exports only blocks whose name matches; filtered during the export. Empty means all

Use the ExportSourceBlocks tool with these parameters:
- softwarePath: {softwarePath}
- groupPath: {groupPath}
- exportPath: {exportPath}
- withDependencies: {withDependencies}
- preservePath: {preservePath}
- regexName: {regexName}";
        }

        [McpServerPrompt(Name = "ExportSourceTypes"), Description("Export PLC data types as .udt source files, optionally filtered by name regex")]
        public static string ExportSourceTypes(string softwarePath, string groupPath, string exportPath, string withDependencies = "false", string preservePath = "false", string regexName = "")
        {
            return $@"Write every PLC data type below a type group, including all subgroups, as '*.udt' external source files. Inconsistent and know-how protected types are reported in 'Skipped' instead of failing the run.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- groupPath: root-relative path of the type group to export recursively, e.g. 'Common'. Empty means all types below 'PLC data types'
- exportPath: directory on this machine that receives the files; existing files of the same name are overwritten
- withDependencies: also write every data type each one uses into its file. Default false
- preservePath: mirror the project groups below '<exportPath>/<softwarePath>/PLC data types'. Default false, which writes straight into <exportPath>/<softwarePath>
- regexName: optional regular expression, exports only types whose name matches; filtered during the export. Empty means all

Use the ExportSourceTypes tool with these parameters:
- softwarePath: {softwarePath}
- groupPath: {groupPath}
- exportPath: {exportPath}
- withDependencies: {withDependencies}
- preservePath: {preservePath}
- regexName: {regexName}";
        }

        [McpServerPrompt(Name = "ExportSources"), Description("Export all blocks and PLC data types as source files")]
        public static string ExportSources(string softwarePath, string exportPath, string regexName = "", string withDependencies = "false")
        {
            return $@"Write every block and PLC data type of one PLC software as external source files into a folder tree that mirrors the project groups: '<exportPath>/<softwarePath>/Program blocks/...' and '<exportPath>/<softwarePath>/PLC data types/...', one file per object. The whole-PLC counterpart to 'ExportSourceBlocks' and 'ExportSourceTypes' and the compilable counterpart to 'ExportPlcAsDocuments'; 'ImportSources' reads such a tree back. Objects with no source form (LAD, FBD, GRAPH), inconsistent objects and know-how protected ones are reported in 'Skipped' instead of failing the run.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- exportPath: directory on this machine that receives the tree; existing files of the same name are overwritten
- regexName: optional regular expression, exports only objects whose name matches. Empty means all
- withDependencies: also write every object each one uses into its file. Default false, which keeps one object per file

Use the ExportSources tool with these parameters:
- softwarePath: {softwarePath}
- exportPath: {exportPath}
- regexName: {regexName}
- withDependencies: {NormalizeBool(withDependencies)}";
        }

        [McpServerPrompt(Name = "GetExternalSources"), Description("List the external source files of a PLC software")]
        public static string GetExternalSources(string softwarePath, string regexName = "")
        {
            return $@"List the external source files of a PLC software, optionally filtered by a regular expression on the source name.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- regexName: optional regular expression to filter the external source names

Use the GetExternalSources tool with these parameters:
- softwarePath: {softwarePath}
- regexName: {regexName}";
        }

        [McpServerPrompt(Name = "GetExternalSourceInfo"), Description("Show details of one external source file")]
        public static string GetExternalSourceInfo(string softwarePath, string sourcePath)
        {
            return $@"Get a single external source file. Beyond its name, all metadata is returned in the generic Attributes list.

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- sourcePath: root-relative path of the external source, e.g. 'SourceGroup1/Source_1'

Use the GetExternalSourceInfo tool with these parameters:
- softwarePath: {softwarePath}
- sourcePath: {sourcePath}";
        }

        [McpServerPrompt(Name = "CreateExternalSourceFromFile"), Description("Add a source file to the external source files of a PLC software")]
        public static string CreateExternalSourceFromFile(string softwarePath, string groupPath, string name, string filePath)
        {
            return $@"Add a source file (for example an SCL file) from the file system into the external source files of the PLC software (requires the server started with '--allow-write').

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- groupPath: root-relative external source group; empty uses the External source files root
- name: name the source gets in the project, without a slash
- filePath: full path of the source file on the machine running this server

Use the CreateExternalSourceFromFile tool with these parameters:
- softwarePath: {softwarePath}
- groupPath: {groupPath}
- name: {name}
- filePath: {filePath}";
        }

        [McpServerPrompt(Name = "DeleteExternalSource"), Description("Delete an external source file")]
        public static string DeleteExternalSource(string softwarePath, string sourcePath)
        {
            return $@"Remove an external source file from the PLC software (requires the server started with '--allow-write').

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- sourcePath: root-relative path of the external source, e.g. SourceGroup1/Source_1

Use the DeleteExternalSource tool with these parameters:
- softwarePath: {softwarePath}
- sourcePath: {sourcePath}";
        }

        [McpServerPrompt(Name = "CreateExternalSourceGroup"), Description("Create an external source group")]
        public static string CreateExternalSourceGroup(string softwarePath, string parentGroupPath, string name)
        {
            return $@"Create a group below the External source files root of the PLC software (requires the server started with '--allow-write').

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- parentGroupPath: root-relative path of the parent group; empty creates directly below the root
- name: name of the new group, without a slash

Use the CreateExternalSourceGroup tool with these parameters:
- softwarePath: {softwarePath}
- parentGroupPath: {parentGroupPath}
- name: {name}";
        }

        [McpServerPrompt(Name = "DeleteExternalSourceGroup"), Description("Delete an external source group and everything inside it")]
        public static string DeleteExternalSourceGroup(string softwarePath, string groupPath)
        {
            return $@"Delete an external source group and everything inside it. The External source files system group itself cannot be deleted (requires the server started with '--allow-write').

Common parameter values:
- softwarePath: defines the path in the project structure to the PLC software
- groupPath: root-relative path of the group to delete

Use the DeleteExternalSourceGroup tool with these parameters:
- softwarePath: {softwarePath}
- groupPath: {groupPath}";
        }

        #endregion
    }
}
