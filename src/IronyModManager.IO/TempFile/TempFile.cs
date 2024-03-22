// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 12-07-2020
//
// Last Modified By : Mario
// Last Modified On : 03-22-2024
// ***********************************************************************
// <copyright file="TempFile.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.IO.Common;
using IronyModManager.Shared;

namespace IronyModManager.IO.TempFile
{
    /// <summary>
    /// The temp file.
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.ITempFile" />
    public class TempFile(ILogger logger) : ITempFile
    {
        #region Fields

        /// <summary>
        /// The temporary extension
        /// </summary>
        private const string TempExtension = "tmp";

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger = logger;

        /// <summary>
        /// The disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// The temporary directory
        /// </summary>
        private string tempDirectory = string.Empty;

        #endregion Fields

        #region Destructors

        /// <summary>
        /// Finalizes an instance of the <see cref="TempFile" /> class.
        /// </summary>
        ~TempFile()
        {
            Delete();
        }

        #endregion Destructors

        #region Properties

        /// <summary>
        /// Gets the file.
        /// </summary>
        /// <value>The file.</value>
        public string File { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the temporary directory.
        /// </summary>
        /// <value>The temporary directory.</value>
        public string TempDirectory
        {
            get
            {
                return string.IsNullOrWhiteSpace(tempDirectory) ? Path.GetTempPath() : tempDirectory;
            }
            set
            {
                tempDirectory = value;
            }
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return System.IO.File.Exists(File) ? System.IO.File.ReadAllText(File) : string.Empty;
            }
            set
            {
                if (System.IO.File.Exists(File))
                {
                    System.IO.File.AppendAllText(File, value ?? string.Empty);
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates the specified path.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>System.String.</returns>
        public string Create(string fileName = Shared.Constants.EmptyParam)
        {
            if (string.IsNullOrWhiteSpace(File))
            {
                File = Path.Combine(TempDirectory, string.IsNullOrWhiteSpace(fileName) ? Path.ChangeExtension(Path.GetRandomFileName(), TempExtension) : fileName);
                var fs = System.IO.File.Create(File);
                fs.Dispose();
            }

            return File;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Delete()
        {
            try
            {
                if (System.IO.File.Exists(File))
                {
                    DiskOperations.DeleteFile(File);
                    return true;
                }
            }
            catch (Exception ex)
            {
                logger?.Error(ex);
            }

            return false;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                GC.SuppressFinalize(this);
                Delete();
                disposed = true;
            }
        }

        /// <summary>
        /// Gets the name of the temporary file.
        /// </summary>
        /// <param name="desiredFilename">The desired filename.</param>
        /// <returns>System.String.</returns>
        public string GetTempFileName(string desiredFilename)
        {
            return $"{desiredFilename}.{TempExtension}".GenerateValidFileName(false);
        }

        #endregion Methods
    }
}
