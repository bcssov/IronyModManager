// ***********************************************************************
// Assembly         : IronyModManager.IO
// ***********************************************************************

using System;
using System.IO;
using System.Linq;
using System.Security;
using IronyModManager.IO.Common.FileSystem;

namespace IronyModManager.IO.FileSystem
{
    /// <summary>
    /// Checks filesystem sources without exposing physical IO to higher layers.
    /// </summary>
    public class FileSystemStateProbe : IFileSystemStateProbe
    {
        /// <inheritdoc />
        public virtual FileSystemPathCheckResult CheckDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            {
                return new FileSystemPathCheckResult { Path = path, State = FileSystemPathState.Missing };
            }

            try
            {
                _ = Directory.EnumerateFileSystemEntries(path).Take(1).ToList();
                return new FileSystemPathCheckResult { Path = path, State = FileSystemPathState.Available };
            }
            catch (Exception exception) when (IsFileSystemAccessFailure(exception))
            {
                return new FileSystemPathCheckResult
                {
                    ExceptionType = exception.GetType().FullName,
                    Path = path,
                    State = FileSystemPathState.Unavailable
                };
            }
        }

        /// <inheritdoc />
        public virtual bool IsFileSystemAccessFailure(Exception exception)
        {
            if (exception == null)
            {
                return false;
            }

            if (exception is IOException or UnauthorizedAccessException or SecurityException)
            {
                return true;
            }

            if (exception is AggregateException aggregateException)
            {
                return aggregateException.Flatten().InnerExceptions.Any(IsFileSystemAccessFailure);
            }

            return exception.InnerException != null && IsFileSystemAccessFailure(exception.InnerException);
        }
    }
}
