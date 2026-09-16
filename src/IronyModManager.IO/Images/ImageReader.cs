// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-17-2021
//
// Last Modified By : Mario
// Last Modified On : 09-09-2026
// ***********************************************************************
// <copyright file="ImageReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ImageMagick;
using ImageMagick.Formats;
using IronyModManager.Shared;

namespace IronyModManager.IO.Images
{
    /// <summary>
    /// Class ImageReader.
    /// </summary>
    internal class ImageReader
    {
        #region Fields

        private const string DDSExtension = ".dds";
        private const string TGAExtension = ".tga";
        private readonly IDdsCompatibilityReader ddsCompatibilityReader = new DdsCompatibilityReader();
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
                var attemptedAsOther = false;
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
                        attemptedAsOther = true;
                        ms = await ParseOther(stream, file);
                    }

                    // Real mods contain misleading extensions, so the extension is a hint rather than authority.
                    if (ms == null)
                    {
                        if (!attemptedAsDds)
                        {
                            ms = await ParseDDS(stream);
                        }
                        else if (!attemptedAsOther)
                        {
                            ms = await ParseOther(stream, file);
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
        /// Gets a DDS image.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>MemoryStream.</returns>
        private async Task<MemoryStream> GetDDS(Stream stream)
        {
            var exceptions = new List<Exception>();

            try
            {
                Rewind(stream);
                using var images = new MagickImageCollection(stream);
                if (images.Count == 6)
                {
                    using var cubeMap = CreateCubeMap(images);
                    return await WritePng(cubeMap, true);
                }

                if (images.Count > 0)
                {
                    return await WritePng(images[0]);
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            try
            {
                Rewind(stream);
                using var compatibilityImage = ddsCompatibilityReader.TryRead(stream);
                if (compatibilityImage != null)
                {
                    return await WritePng(compatibilityImage);
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }

            return null;
        }

        /// <summary>
        /// Gets a non-DDS image.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="file">The file name.</param>
        /// <returns>MemoryStream.</returns>
        private async Task<MemoryStream> GetOther(Stream stream, string file)
        {
            var exceptions = new List<Exception>();

            if (file.EndsWith(TGAExtension, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    Rewind(stream);
                    var settings = new MagickReadSettings { Format = MagickFormat.Tga };
                    using var tgaImage = new MagickImage(stream, settings);
                    return await WritePng(tgaImage);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }

            try
            {
                Rewind(stream);
                using var image = new MagickImage(stream);
                return await WritePng(image);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            throw new AggregateException(exceptions);
        }

        /// <summary>
        /// Creates Irony's established transparent cubemap cross.
        /// </summary>
        /// <param name="images">The six faces in ImageMagick order.</param>
        /// <returns>The composed cubemap.</returns>
        private static MagickImage CreateCubeMap(MagickImageCollection images)
        {
            if (images.Count != 6)
            {
                throw new InvalidDataException("A DDS cubemap must contain exactly six faces.");
            }

            var width = images[0].Width;
            var height = images[0].Height;
            for (var i = 1; i < images.Count; i++)
            {
                if (images[i].Width != width || images[i].Height != height)
                {
                    throw new InvalidDataException("DDS cubemap faces must have matching dimensions.");
                }
            }

            var canvas = new MagickImage(MagickColors.Transparent, checked(width * 4), checked(height * 3));

            // ImageMagick face order: +X, -X, +Y, -Y, +Z, -Z.
            canvas.Composite(images[2], checked((int)width), 0, CompositeOperator.Over);
            canvas.Composite(images[1], 0, checked((int)height), CompositeOperator.Over);
            canvas.Composite(images[4], checked((int)width), checked((int)height), CompositeOperator.Over);
            canvas.Composite(images[0], checked((int)(width * 2)), checked((int)height), CompositeOperator.Over);
            canvas.Composite(images[5], checked((int)(width * 3)), checked((int)height), CompositeOperator.Over);
            canvas.Composite(images[3], checked((int)width), checked((int)(height * 2)), CompositeOperator.Over);
            return canvas;
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
        /// Parses a non-DDS image.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="file">The file name.</param>
        /// <returns>MemoryStream.</returns>
        private async Task<MemoryStream> ParseOther(Stream stream, string file)
        {
            MemoryStream ms = null;
            try
            {
                ms = await GetOther(stream, file);
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
        /// Rewinds a stream for another decoding attempt.
        /// </summary>
        /// <param name="stream">The stream.</param>
        private static void Rewind(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Writes an image to an in-memory PNG stream.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>The PNG stream.</returns>
        private static async Task<MemoryStream> WritePng(IMagickImage<byte> image, bool optimizeForPreviewLatency = false)
        {
            var ms = new MemoryStream();
            try
            {
                image.Format = MagickFormat.Png;
                if (optimizeForPreviewLatency)
                {
                    var defines = new PngWriteDefines
                    {
                        CompressionLevel = 1,
                        CompressionStrategy = PngCompressionStrategy.ZRLENoFilter
                    };
                    await image.WriteAsync(ms, defines);
                }
                else
                {
                    await image.WriteAsync(ms);
                }
                return ms;
            }
            catch
            {
                ms.Close();
                await ms.DisposeAsync();
                throw;
            }
        }

        #endregion Methods
    }
}
