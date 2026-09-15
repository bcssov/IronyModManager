// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// ***********************************************************************

using System;

namespace IronyModManager.IO.Common.FileSystem
{
    /// <summary>
    /// Provides narrow filesystem trust checks for application workflows.
    /// </summary>
    public interface IFileSystemStateProbe
    {
        /// <summary>
        /// Checks that an expected directory exists and can be enumerated.
        /// </summary>
        FileSystemPathCheckResult CheckDirectory(string path);

        /// <summary>
        /// Determines whether an exception represents a filesystem access/trust failure.
        /// </summary>
        bool IsFileSystemAccessFailure(Exception exception);
    }
}
