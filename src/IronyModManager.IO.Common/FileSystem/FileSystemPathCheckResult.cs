// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// ***********************************************************************

namespace IronyModManager.IO.Common.FileSystem
{
    /// <summary>
    /// Result of checking an expected filesystem source.
    /// </summary>
    public sealed class FileSystemPathCheckResult
    {
        /// <summary>
        /// Gets or sets the exception type when access failed.
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the checked path.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the path state.
        /// </summary>
        public FileSystemPathState State { get; set; }
    }
}
