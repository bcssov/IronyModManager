// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 02-24-2020
// ***********************************************************************
// <copyright file="DiskFileReader.cs" company="Mario">
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

namespace IronyModManager.IO
{
    /// <summary>
    /// Class DiskFileReader.
    /// Implements the <see cref="IronyModManager.IO.IFileReader" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.IFileReader" />
    [ExcludeFromCoverage("Shloud be covered by Unit tests from source project.")]
    public class DiskFileReader : IFileReader
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can read the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if this instance can read the specified path; otherwise, <c>false</c>.</returns>
        public bool CanRead(string path)
        {
            return Directory.Exists(path) && !path.EndsWith(Constants.ModDirectory, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IReadOnlyCollection&lt;IFileInfo&gt;.</returns>
        public IReadOnlyCollection<IFileInfo> Read(string path)
        {
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            if (files?.Count() > 0)
            {
                var result = new List<IFileInfo>();
                foreach (var file in files)
                {
                    var relativePath = file.Replace(path, string.Empty).Trim(Path.DirectorySeparatorChar);
                    if (!relativePath.Contains(Path.DirectorySeparatorChar))
                    {
                        continue;
                    }
                    var info = DIResolver.Get<IFileInfo>();
                    using var stream = File.OpenRead(file);
                    info.FileName = relativePath;
                    if (Shared.Constants.TextExtensions.Any(s => file.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
                    {
                        using var streamReader = new StreamReader(stream, true);
                        var text = streamReader.ReadToEnd();
                        streamReader.Close();
                        info.IsBinary = false;
                        info.Content = text.SplitOnNewLine();
                        info.ContentSHA = text.CalculateSHA();
                    }
                    else
                    {
                        info.IsBinary = true;
                        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                        info.ContentSHA = fs.CalculateSHA();
                    }
                    result.Add(info);
                }
                return result;
            }
            return null;
        }

        #endregion Methods
    }
}
