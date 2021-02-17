// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-17-2021
//
// Last Modified By : Mario
// Last Modified On : 02-17-2021
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
using BCnEncoder.Decoder;
using BCnEncoder.Shared;
using BCnEncoder.Shared.ImageFiles;
using IronyModManager.Shared;
using Microsoft.Toolkit.HighPerformance.Memory;
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
        /// BCNs the encoder to image.
        /// </summary>
        /// <param name="colors">The colors.</param>
        /// <returns>Image&lt;Rgba32&gt;.</returns>
        private Image<Rgba32> ColorMemoryToImage(Memory2D<ColorRgba32> colors)
        {
            // Taken from project due to a breaking change: https://github.com/Nominom/BCnEncoder.NET/blob/master/BCnEncoder.NET.ImageSharp/BCnDecoderExtensions.cs
            var output = new Image<Rgba32>(colors.Width, colors.Height);
            output.TryGetSinglePixelSpan(out var pixels);
            colors.Span.TryGetSpan(out var decodedPixels);

            for (var i = 0; i < pixels.Length; i++)
            {
                var c = decodedPixels[i];
                pixels[i] = new Rgba32(c.r, c.g, c.b, c.a);
            }
            return output;
        }

        /// <summary>
        /// Gets the DDS.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>MemoryStream.</returns>
        /// <exception cref="AggregateException"></exception>
        private async Task<MemoryStream> GetDDS(Stream stream)
        {
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            var exceptions = new List<Exception>();
            MemoryStream ms = null;
            try
            {
                ms = new MemoryStream();
                var file = DdsFile.Load(stream);
                var decoder = new BcDecoder();
                var bcnImage = await decoder.Decode2DAsync(file);
                var image = ColorMemoryToImage(bcnImage);
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
            // fallback
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
                        using var image = Image.LoadPixelData<Bgra32>(TightPfimData(pfimImage), pfimImage.Width, pfimImage.Height);
                        await image.SaveAsPngAsync(ms);
                    }
                    else if (pfimImage.Format == ImageFormat.Rgb24)
                    {
                        ms = new MemoryStream();
                        using var image = Image.LoadPixelData<Bgr24>(TightPfimData(pfimImage), pfimImage.Width, pfimImage.Height);
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
            // Fallback can result in memory stream being empty so throw aggregate exception only if both attempts failed
            if (ms == null && exceptions.Count == 2)
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

        /// <summary>
        /// Tights the pfim data.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>System.Byte[].</returns>
        private byte[] TightPfimData(Dds image)
        {
            // Code from this PR (MIT licensed): https://github.com/hguy/dds-reader/pull/1/commits/ba751f0af4fc1c725842dc86d12ecf69f0c70108
            var tightStride = image.Width * image.BitsPerPixel / 8;
            if (image.Stride == tightStride)
            {
                return image.Data;
            }

            byte[] newData = new byte[image.Height * tightStride];
            for (int i = 0; i < image.Height; i++)
            {
                Buffer.BlockCopy(image.Data, i * image.Stride, newData, i * tightStride, tightStride);
            }

            return newData;
        }

        #endregion Methods
    }
}
