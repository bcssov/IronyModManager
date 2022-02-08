// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-17-2021
//
// Last Modified By : Mario
// Last Modified On : 02-08-2022
// ***********************************************************************
// <copyright file="ImageReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BCnEncoder.Shared.ImageFiles;
using IronyModManager.Shared;
using Pfim;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace IronyModManager.IO.Images
{
    /// <summary>
    /// Class ImageReader.
    /// </summary>
    internal class ImageReader
    {
        #region Fields

        /// <summary>
        /// The DDS extension
        /// </summary>
        private const string DDSExtension = ".dds";

        /// <summary>
        /// The DDS decoder
        /// </summary>
        private readonly DDSDecoder ddsDecoder;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageReader" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ImageReader(ILogger logger)
        {
            this.logger = logger;
            ddsDecoder = new DDSDecoder();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="file">The file.</param>
        /// <returns>MemoryStream.</returns>
        public async Task<MemoryStream> Parse(Stream stream, string file)
        {
            if (stream != null)
            {
                var attemptedAsDds = false;
                var attemptedAsPng = false;
                MemoryStream ms = null;
                try
                {
                    if (file.EndsWith(DDSExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        attemptedAsDds = true;
                        ms = await ParseDDS(stream);
                    }
                    else
                    {
                        attemptedAsPng = true;
                        ms = await ParseOther(stream);
                    }
                    if (ms == null)
                    {
                        if (!attemptedAsDds)
                        {
                            ms = await ParseDDS(stream);
                        }
                        else if (!attemptedAsPng)
                        {
                            ms = await ParseOther(stream);
                        }
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
                    if (ms != null)
                    {
                        ms.Close();
                        await ms.DisposeAsync();
                    }
                }
                finally
                {
                    stream.Close();
                    await stream.DisposeAsync();
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the DDS.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>MemoryStream.</returns>
        /// <exception cref="System.AggregateException"></exception>
        private async Task<MemoryStream> GetDDS(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            var exceptions = new List<Exception>();
            MemoryStream ms = null;
            // Default provider (SixLabors.Textures)
            try
            {
                var image = await ddsDecoder.DecodeStreamToImageAsync(stream);
                ms = new MemoryStream();
                await image.SaveAsPngAsync(ms);
            }
            catch (Exception ex)
            {
                if (ms != null)
                {
                    ms.Close();
                    await ms.DisposeAsync();
                }
                ms = null;
                exceptions.Add(ex);
            }
            // fallback #1 (BCnEncoder.NET)
            if (ms == null)
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                try
                {
                    var file = DdsFile.Load(stream);
                    var image = await ddsDecoder.DecodeToImageAsync(file);
                    ms = new MemoryStream();
                    await image.SaveAsPngAsync(ms);
                }
                catch (Exception ex)
                {
                    if (ms != null)
                    {
                        ms.Close();
                        await ms.DisposeAsync();
                    }
                    ms = null;
                    exceptions.Add(ex);
                }
            }
            // fallback #2 (pfim)
            if (ms == null)
            {
                if (stream.CanSeek)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }
                try
                {
                    using var pfimImage = Dds.Create(stream, new PfimConfig());
                    if (pfimImage.Compressed)
                    {
                        pfimImage.Decompress();
                    }
                    if (pfimImage.Format == ImageFormat.Rgba32)
                    {
                        ms = new MemoryStream();
                        using var image = Image.LoadPixelData<Bgra32>(ddsDecoder.TightData(pfimImage), pfimImage.Width, pfimImage.Height);
                        await image.SaveAsPngAsync(ms);
                    }
                    else if (pfimImage.Format == ImageFormat.Rgb24)
                    {
                        ms = new MemoryStream();
                        using var image = Image.LoadPixelData<Bgr24>(ddsDecoder.TightData(pfimImage), pfimImage.Width, pfimImage.Height);
                        await image.SaveAsPngAsync(ms);
                    }
                }
                catch (Exception ex)
                {
                    if (ms != null)
                    {
                        ms.Close();
                        await ms.DisposeAsync();
                    }
                    ms = null;
                    exceptions.Add(ex);
                }
            }
            // Fallback can result in memory stream being empty so throw aggregate exception only if all attempts failed
            if (ms == null && exceptions.Count == 3)
            {
                throw new AggregateException(exceptions);
            }
            return ms;
        }

        /// <summary>
        /// Gets the other.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>MemoryStream.</returns>
        private async Task<MemoryStream> GetOther(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            var ms = new MemoryStream();
            try
            {
                using var image = await Image.LoadAsync(stream);
                await image.SaveAsPngAsync(ms);
                return ms;
            }
            catch
            {
                ms.Close();
                await ms.DisposeAsync();
                throw;
            }
        }

        /// <summary>
        /// Parses the DDS.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>MemoryStream.</returns>
        private async Task<MemoryStream> ParseDDS(Stream stream)
        {
            MemoryStream ms = null;
            try
            {
                ms = await GetDDS(stream);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (ms != null)
                {
                    ms.Close();
                    await ms.DisposeAsync();
                }
                ms = null;
            }
            return ms;
        }

        /// <summary>
        /// Parses the other.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>MemoryStream.</returns>
        private async Task<MemoryStream> ParseOther(Stream stream)
        {
            MemoryStream ms = null;
            try
            {
                ms = await GetOther(stream);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                if (ms != null)
                {
                    ms.Close();
                    await ms.DisposeAsync();
                }
                ms = null;
            }
            return ms;
        }

        #endregion Methods
    }
}
