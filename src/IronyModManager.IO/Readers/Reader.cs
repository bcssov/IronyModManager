// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 05-30-2021
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
using System.Threading.Tasks;
using IronyModManager.DI;
using IronyModManager.IO.Common.Readers;
using IronyModManager.IO.Images;
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
        /// The logger
        /// </summary>
        private readonly ImageReader imageReader;

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
        /// <param name="logger">The logger.</param>
        public Reader(IEnumerable<IFileReader> readers, ILogger logger)
        {
            this.readers = readers;
            imageReader = new ImageReader(logger);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the file information.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>IFileInfo.</returns>
        public virtual IFileInfo GetFileInfo(string rootPath, string file)
        {
            var fileInfo = GetStreamInternal(rootPath, file);
            using var stream = fileInfo.Item1;
            if (stream != null)
            {
                var info = DIResolver.Get<IFileInfo>();
                info.FileName = file;
                info.IsReadOnly = fileInfo.Item2;
                info.Size = stream.Length;
                if (Constants.TextExtensions.Any(s => file.EndsWith(s, StringComparison.OrdinalIgnoreCase)))
                {
                    using var streamReader = new StreamReader(stream, true);
                    var text = streamReader.ReadToEnd();
                    streamReader.Close();
                    streamReader.Dispose();
                    info.IsBinary = false;
                    info.Content = text.SplitOnNewLine(false);
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
            path ??= string.Empty;
            var reader = readers.FirstOrDefault(p => p.CanRead(path) && p.CanListFiles(path));
            if (reader != null)
            {
                return reader.GetFiles(path);
            }
            return null;
        }

        /// <summary>
        /// Gets the image stream asynchronous.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>Task&lt;MemoryStream&gt;.</returns>
        public virtual Task<MemoryStream> GetImageStreamAsync(string rootPath, string file)
        {
            if (Constants.ImageExtensions.Any(p => file.EndsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                var stream = GetStream(rootPath, file);
                return imageReader.Parse(stream, file);
            }
            return Task.FromResult((MemoryStream)null);
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>Stream.</returns>
        public virtual Stream GetStream(string rootPath, string file)
        {
            return GetStreamInternal(rootPath, file).Item1;
        }

        /// <summary>
        /// Gets the total size.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>System.Int64.</returns>
        public virtual long GetTotalSize(string path)
        {
            path ??= string.Empty;
            var reader = readers.FirstOrDefault(p => p.CanRead(path) && p.CanListFiles(path));
            if (reader != null)
            {
                return reader.GetTotalSize(path);
            }
            return 0;
        }

        /// <summary>
        /// Reads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="allowedPaths">The allowed paths.</param>
        /// <param name="searchSubFolders">if set to <c>true</c> [search sub folders].</param>
        /// <returns>IEnumerable&lt;IFileInfo&gt;.</returns>
        public virtual IEnumerable<IFileInfo> Read(string path, IEnumerable<string> allowedPaths = null, bool searchSubFolders = true)
        {
            path ??= string.Empty;
            var reader = readers.FirstOrDefault(r => r.CanRead(path, searchSubFolders));
            if (reader != null)
            {
                return reader.Read(path, allowedPaths, searchSubFolders);
            }
            return null;
        }

        /// <summary>
        /// Gets the stream internal.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        /// <param name="file">The file.</param>
        /// <returns>(System.IO.Stream, bool).</returns>
        private (Stream, bool) GetStreamInternal(string rootPath, string file)
        {
            rootPath ??= string.Empty;
            var reader = readers.FirstOrDefault(r => r.CanRead(rootPath) && r.CanReadStream(rootPath));
            if (reader != null)
            {
                return reader.GetStream(rootPath, file);
            }
            return (null, false);
        }

        #endregion Methods
    }
}
