// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 04-30-2021
//
// Last Modified By : Mario
// Last Modified On : 12-07-2025
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
using IronyModManager.DI;
using IronyModManager.Shared;
using IronyModManager.Shared.Configuration;

namespace IronyModManager.IO.Common
{
    /// <summary>
    /// Class DiskOperations.
    /// </summary>
    public static class DiskOperations
    {
        #region Methods

        /// <summary>
        /// Copies the file.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool CopyFile(string source, string destination)
        {
            if (File.Exists(source))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                File.Copy(source, destination);
                return true;
            }

            return false;
        }

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
            var fileInfo = new FileInfo(file) { Attributes = FileAttributes.Normal };
            fileInfo.Delete();
        }

        /// <summary>
        /// Resolves the storage path.
        /// </summary>
        /// <returns>System.String.</returns>
        public static string ResolveStoragePath()
        {
            var path = DIResolver.Get<IDomainConfiguration>().GetOptions().App.StoragePath;
            var segments = path.StandardizeDirectorySeparator().Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
            var expanded = Path.Combine(segments.Select(ResolveEnvironmentVariable).ToArray());
            var resolved = Path.GetFullPath(expanded, AppDomain.CurrentDomain.BaseDirectory).StandardizeDirectorySeparator();
            return resolved;
        }

        /// <summary>
        /// Resolves the environment variable.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns>System.String.</returns>
        private static string ResolveEnvironmentVariable(string variable)
        {
            if (string.IsNullOrEmpty(variable))
            {
                return variable;
            }

            if (variable.StartsWith('$'))
            {
                var name = variable.TrimStart('$');
                var value = Environment.GetEnvironmentVariable(name);
                return value ?? variable;
            }

            if (variable.StartsWith('%') && variable.EndsWith('%'))
            {
                var name = variable.Substring(1, variable.Length - 2);
                var value = Environment.GetEnvironmentVariable(name);
                return value ?? variable;
            }

            return variable;
        }

        #endregion Methods
    }
}
