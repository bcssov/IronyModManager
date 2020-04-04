// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 04-04-2020
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
using IronyModManager.IO.Common.Readers;

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
        /// <returns>IEnumerable&lt;IFileInfo&gt;.</returns>
        public IEnumerable<IFileInfo> Read(string path)
        {
            var reader = readers.FirstOrDefault(r => r.CanRead(path));
            if (reader != null)
            {
                return reader.Read(path);
            }
            return null;
        }

        #endregion Methods
    }
}
