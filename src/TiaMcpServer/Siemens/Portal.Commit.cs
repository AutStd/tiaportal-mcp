using Microsoft.Extensions.Logging;
using Siemens.Engineering.Multiuser;
using System;

namespace TiaMcpServer.Siemens
{
    public partial class Portal
    {
        #region commit

        private const string NoLocalSessionMessage =
            "Committing is possible only in a multiuser local session. Open a local session (.als) first.";

        /// <summary>True when the open local session has all changes of the server project.</summary>
        public bool CheckIsUpToDate()
        {
            _logger?.LogInformation("Checking whether the local session is up to date...");

            return Operation.Run(_logger, nameof(CheckIsUpToDate), PortalErrorCode.InvalidState,
                () =>
                {
                    var session = _session ?? throw new PortalException(PortalErrorCode.InvalidState, NoLocalSessionMessage);

                    try
                    {
                        return session.IsUptoDate();
                    }
                    catch (MultiuserException ex)
                    {
                        throw new PortalException(PortalErrorCode.InvalidState,
                            $"Could not determine whether the local session is up to date: {ex.Message}");
                    }
                });
        }

        /// <summary>
        /// Commits the changes of the open local session to the server project and returns the new
        /// revision number. Openness closes the session as part of the commit, so the session and
        /// project of this portal are cleared afterwards.
        /// </summary>
        public int CommitChanges(string comment)
        {
            _logger?.LogInformation("Committing local session changes to the server project...");

            return Operation.Run(_logger, nameof(CommitChanges), PortalErrorCode.CommitFailed,
                () =>
                {
                    if (string.IsNullOrWhiteSpace(comment))
                    {
                        throw new PortalException(PortalErrorCode.InvalidParams, "A comment for the commit is required.");
                    }

                    var session = _session ?? throw new PortalException(PortalErrorCode.InvalidState, NoLocalSessionMessage);

                    try
                    {
                        if (!session.IsUptoDate())
                        {
                            throw new PortalException(PortalErrorCode.InvalidState,
                                "The local session is not up to date with the server project. Update the session from the server in TIA Portal first; nothing was committed.");
                        }

                        var revision = session.CloseAndCommit(comment.Trim());

                        // CloseAndCommit closed the session, so it must not be closed or saved again.
                        _project = null;
                        _session = null;

                        return revision;
                    }
                    catch (MultiuserException ex)
                    {
                        throw new PortalException(PortalErrorCode.CommitFailed,
                            $"Committing the local session failed: {ex.Message}");
                    }
                });
        }

        #endregion
    }
}
