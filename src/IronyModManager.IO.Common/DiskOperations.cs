// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-30-2021
//
// Last Modified By : Mario
// Last Modified On : 04-30-2021
// ***********************************************************************
// <copyright file="DiskOperations.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace IronyModManager.IO.Common
{
    /// <summary>
    /// Class DiskOperations.
    /// </summary>
    public static class DiskOperations
    {
        #region Methods

        /// <summary>
        /// Deletes the directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        public static void DeleteDirectory(string directory, bool recursive)
        {
            var dirInfo = new DirectoryInfo(directory) { Attributes = FileAttributes.Normal };
            foreach (var item in dirInfo.GetFileSystemInfos("*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
            {
                item.Attributes = FileAttributes.Normal;
            }
            dirInfo.Delete(recursive);
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="file">The file.</param>
        public static void DeleteFile(string file)
        {
            var fileInfo = new FileInfo(file)
            {
                Attributes = FileAttributes.Normal
            };
            fileInfo.Delete();
        }

        #endregion Methods
    }
}
