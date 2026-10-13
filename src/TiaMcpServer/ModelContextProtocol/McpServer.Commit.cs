using ModelContextProtocol;
using ModelContextProtocol.Server;
using System;
using System.ComponentModel;
using System.Text.Json.Nodes;
using TiaMcpServer.Siemens;

namespace TiaMcpServer.ModelContextProtocol
{
    public static partial class McpServer
    {
        #region commit

        [McpServerTool(Name = "CheckIsUpToDate", Title = "Check if session is up to date", ReadOnly = true, OpenWorld = false, UseStructuredContent = true), Description("Check whether the open multiuser local session is up to date with the server project. Run it before CommitChanges. Needs a local session (.als)")]
        public static ResponseCheckIsUpToDate CheckIsUpToDate()
        {
            try
            {
                var isUpToDate = Portal.CheckIsUpToDate();

                return new ResponseCheckIsUpToDate
                {
                    Message = isUpToDate
                        ? "The local session is up to date"
                        : "The local session is not up to date with the server project",
                    IsUpToDate = isUpToDate,
                    Meta = Ok(new JsonObject { ["isUpToDate"] = isUpToDate })
                };
            }
            catch (PortalException pex)
            {
                throw new McpException(pex.Message, pex);
            }
            catch (Exception ex) when (ex is not McpException)
            {
                throw new McpException($"Unexpected error checking the session: {ex.Message}", ex);
            }
        }

        [WriteTool]
        [McpServerTool(Name = "CommitChanges", Title = "Commit changes to server", Destructive = true, OpenWorld = false, UseStructuredContent = true),
         Description("Commit the changes of the open multiuser local session to the server project and create a new server revision. The session is closed by the commit; reopen it to continue. Fails if the session is not up to date (see CheckIsUpToDate)")]
        public static ResponseCommitChanges CommitChanges(
            [Description("comment: required, the comment stored with the new server revision")] string comment)
        {
            WritePolicy.EnsureEnabled(nameof(CommitChanges));

            try
            {
                var revision = Portal.CommitChanges(comment);

                return new ResponseCommitChanges
                {
                    Message = $"Changes committed as server revision {revision}. The local session is closed; reopen it to continue",
                    Revision = revision,
                    Comment = comment.Trim(),
                    Meta = Ok(new JsonObject { ["revision"] = revision, ["sessionClosed"] = true })
                };
            }
            catch (PortalException pex)
            {
                throw new McpException(pex.Message, pex);
            }
            catch (Exception ex) when (ex is not McpException)
            {
                throw new McpException($"Unexpected error committing changes: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
