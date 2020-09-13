// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-23-2020
//
// Last Modified By : Mario
// Last Modified On : 09-13-2020
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
using IronyModManager.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
        /// The DDS extension
        /// </summary>
        private const string DDSExtension = ".dds";

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

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
            this.logger = logger;
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
        /// Gets the image stream asynchronous.
        /// </summary>
        /// <param name="rootPat">The root pat.</param>
        /// <param name="file">The file.</param>
        /// <returns>MemoryStream.</returns>
        public async Task<MemoryStream> GetImageStreamAsync(string rootPat, string file)
        {
            if (Constants.ImageExtensions.Any(p => file.EndsWith(p, StringComparison.OrdinalIgnoreCase)))
            {
                var stream = GetStream(rootPat, file);
                if (stream != null)
                {
                    MemoryStream ms = null;
                    try
                    {
                        if (file.EndsWith(DDSExtension, StringComparison.OrdinalIgnoreCase))
                        {
                            var pfimImage = Pfim.Dds.Create(stream, new Pfim.PfimConfig());
                            if (pfimImage.Compressed)
                            {
                                pfimImage.Decompress();
                            }
                            if (pfimImage.Format == Pfim.ImageFormat.Rgba32)
                            {
                                ms = new MemoryStream();
                                var image = Image.LoadPixelData<Bgra32>(pfimImage.Data, pfimImage.Width, pfimImage.Height);
                                image.SaveAsBmp(ms);
                            }
                            else if (pfimImage.Format == Pfim.ImageFormat.Rgb24)
                            {
                                ms = new MemoryStream();
                                var image = Image.LoadPixelData<Bgr24>(pfimImage.Data, pfimImage.Width, pfimImage.Height);
                                image.SaveAsBmp(ms);
                            }
                        }
                        else
                        {
                            ms = new MemoryStream();
                            var image = await Image.LoadAsync(stream);
                            await image.SaveAsBmpAsync(ms);
                        }
                        if (ms != null && ms.CanSeek)
                        {
                            ms.Seek(0, SeekOrigin.Begin);
                        }
                        return ms;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        ms.Close();
                        await ms.DisposeAsync();
                    }
                    finally
                    {
                        stream.Close();
                        await stream.DisposeAsync();
                    }
                }
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
