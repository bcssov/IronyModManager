// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 07-09-2020
// ***********************************************************************
// <copyright file="Reader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Shared;

namespace IronyModManager.IO.Readers
{
    /// <summary>
    /// Class Reader.
    /// Implements the <see cref="IronyModManager.IO.Common.Readers.IReader" />
    /// </summary>
    /// <seealso cref="IronyModManager.IO.Common.Readers.IReader" />
    public class Reader : IReader
    {
        #region Fields

        /// <summary>
        /// The readers
        /// </summary>
        private readonly IEnumerable<IFileReader> readers;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Reader" /> class.
        /// </summary>
        /// <param name="readers">The readers.</param>
        public Reader(IEnumerable<IFileReader> readers)
        {
            this.readers = readers;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the file information.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>IFileInfo.</returns>
        public IFileInfo GetFileInfo(string rootPath, string file)
        {
            using var stream = GetStream(rootPath, file);
            if (stream != null)
            {
                var info = DIResolver.Get<IFileInfo>();
                info.FileName = file;
                if (Constants.TextExtensions.Any(s => file.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
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
                    info.ContentSHA = stream.CalculateSHA();
                }
                return info;
            }
            return null;
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        public virtual IEnumerable<string> GetFiles(string path)
        {
            var reader = readers.FirstOrDefault(p => p.CanRead(path));
            if (reader != null)
            {
                return reader.GetFiles(path);
            }
            return null;
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>Stream.</returns>
        public Stream GetStream(string rootPath, string file)
        {
            var reader = readers.FirstOrDefault(r => r.CanRead(rootPath));
            if (reader != null)
            {
                return reader.GetStream(rootPath, file);
            }
            return null;
        }

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="allowedPaths">The allowed paths.</param>
        /// <returns>IEnumerable&lt;IFileInfo&gt;.</returns>
        public IEnumerable<IFileInfo> Read(string path, IEnumerable<string> allowedPaths = null)
        {
            var reader = readers.FirstOrDefault(r => r.CanRead(path));
            if (reader != null)
            {
                return reader.Read(path, allowedPaths);
            }
            return null;
        }

        #endregion Methods
    }
}
