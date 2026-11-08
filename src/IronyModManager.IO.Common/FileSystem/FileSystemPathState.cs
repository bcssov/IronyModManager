// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// ***********************************************************************

namespace IronyModManager.IO.Common.FileSystem
{
    /// <summary>
    /// Describes whether an expected filesystem source can be trusted.
    /// </summary>
    public enum FileSystemPathState
    {
        /// <summary>
        /// The path exists and can be enumerated.
        /// </summary>
        Available,

        /// <summary>
        /// The expected path does not exist.
        /// </summary>
        Missing,

        /// <summary>
        /// The path exists, but its contents cannot be read reliably.
        /// </summary>
        Unavailable
    }
}
