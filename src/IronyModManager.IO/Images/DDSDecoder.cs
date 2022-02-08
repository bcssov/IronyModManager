// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 02-17-2021
//
// Last Modified By : Mario
// Last Modified On : 02-08-2022
// ***********************************************************************
// <copyright file="DDSDecoder.cs" company="Mario">
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
using BCnEncoder.Decoder.Options;
using BCnEncoder.Shared;
using BCnEncoder.Shared.ImageFiles;
using Microsoft.Toolkit.HighPerformance;
using Pfim;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Textures.TextureFormats;
using DxgiFormat = BCnEncoder.Shared.ImageFiles.DxgiFormat;

namespace IronyModManager.IO.Images
{
    /// <summary>
    /// Class DDSDecoder.
    /// </summary>
    internal class DDSDecoder
    {
        #region Methods

        /// <summary>
        /// Decodes the stream to image asynchronous.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Task&lt;Image&gt;.</returns>
        public Task<Image> DecodeStreamToImageAsync(Stream stream)
        {
            var ddsDecoder = new SixLabors.ImageSharp.Textures.Formats.Dds.DdsDecoder();
            var texture = ddsDecoder.DecodeTexture(SixLabors.ImageSharp.Textures.Configuration.Default, stream);
            if (texture is CubemapTexture cubemapTexture)
            {
                var right = cubemapTexture.PositiveX.MipMaps.FirstOrDefault().GetImage();
                var left = cubemapTexture.NegativeX.MipMaps.FirstOrDefault().GetImage();
                var top = cubemapTexture.PositiveY.MipMaps.FirstOrDefault().GetImage();
                var bottom = cubemapTexture.NegativeY.MipMaps.FirstOrDefault().GetImage();
                var front = cubemapTexture.PositiveZ.MipMaps.FirstOrDefault().GetImage();
                var back = cubemapTexture.NegativeZ.MipMaps.FirstOrDefault().GetImage();
                var image = GetCubeMap(right, left, top, bottom, front, back);
                if (image != null)
                {
                    return Task.FromResult(image);
                }
            }
            else if (texture is FlatTexture flatTexture)
            {
                var image = flatTexture.MipMaps.FirstOrDefault().GetImage();
                if (image != null)
                {
                    return Task.FromResult(image);
                }
            }

            return Task.FromResult((Image)null);
        }

        /// <summary>
        /// Gets the cube map.
        /// </summary>
        /// <param name="right">The right.</param>
        /// <param name="left">The left.</param>
        /// <param name="top">The top.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="front">The front.</param>
        /// <param name="back">The back.</param>
        /// <returns>Image.</returns>
        private Image GetCubeMap(Image right, Image left, Image top, Image bottom, Image front, Image back)
        {
            var width = left.Width + front.Width + right.Width + back.Width;
            var height = top.Height + front.Height + bottom.Height;
            var image = new Image<Rgba32>(width, height);
            image.Mutate(i =>
            {
                i.DrawImage(top, new Point(left.Width, 0), 1f);
                i.DrawImage(left, new Point(0, top.Height), 1f);
                i.DrawImage(front, new Point(left.Width, top.Height), 1f);
                i.DrawImage(right, new Point(left.Width + front.Width, top.Height), 1f);
                i.DrawImage(back, new Point(left.Width + front.Width + right.Width, top.Height), 1f);
                i.DrawImage(bottom, new Point(left.Width, top.Height + front.Height), 1f);
            });
            return image;
        }

        /// <summary>
        /// decode to image as an asynchronous operation.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>Image&lt;Rgba32&gt;[].</returns>
        public async Task<Image> DecodeToImageAsync(DdsFile file)
        {
            var decoder = new BcDecoder();
            Image<Rgba32> image;
            if (file.Faces.Count == 6)
            {
                // Cubemap
                var right = ColorMemoryToImage(await decoder.DecodeRaw2DAsync(file.Faces[0].MipMaps[0].Data, Convert.ToInt32(file.Faces[0].Width), Convert.ToInt32(file.Faces[0].Height), GetCompressionFormat(file, decoder.InputOptions)));
                var left = ColorMemoryToImage(await decoder.DecodeRaw2DAsync(file.Faces[1].MipMaps[0].Data, Convert.ToInt32(file.Faces[1].Width), Convert.ToInt32(file.Faces[1].Height), GetCompressionFormat(file, decoder.InputOptions)));
                var top = ColorMemoryToImage(await decoder.DecodeRaw2DAsync(file.Faces[2].MipMaps[0].Data, Convert.ToInt32(file.Faces[2].Width), Convert.ToInt32(file.Faces[2].Height), GetCompressionFormat(file, decoder.InputOptions)));
                var bottom = ColorMemoryToImage(await decoder.DecodeRaw2DAsync(file.Faces[3].MipMaps[0].Data, Convert.ToInt32(file.Faces[3].Width), Convert.ToInt32(file.Faces[3].Height), GetCompressionFormat(file, decoder.InputOptions)));
                var front = ColorMemoryToImage(await decoder.DecodeRaw2DAsync(file.Faces[4].MipMaps[0].Data, Convert.ToInt32(file.Faces[4].Width), Convert.ToInt32(file.Faces[4].Height), GetCompressionFormat(file, decoder.InputOptions)));
                var back = ColorMemoryToImage(await decoder.DecodeRaw2DAsync(file.Faces[5].MipMaps[0].Data, Convert.ToInt32(file.Faces[5].Width), Convert.ToInt32(file.Faces[5].Height), GetCompressionFormat(file, decoder.InputOptions)));
                return GetCubeMap(right, left, top, bottom, front, back);
            }
            else
            {
                var bcnImage = await decoder.Decode2DAsync(file);
                image = ColorMemoryToImage(bcnImage);
            }
            return image;
        }

        /// <summary>
        /// Tights the data.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns>System.Byte[].</returns>
        public byte[] TightData(Dds image)
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

        /// <summary>
        /// Colors the memory to image.
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
        /// Gets the compression format.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="decoderInputOptions">The decoder input options.</param>
        /// <returns>CompressionFormat.</returns>
        /// <exception cref="ArgumentOutOfRangeException">format - null</exception>
        private CompressionFormat GetCompressionFormat(DdsFile file, DecoderInputOptions decoderInputOptions)
        {
            // Copied from BCnEncoder due to protection level
            var format = file.header.ddsPixelFormat.IsDxt10Format ? file.dx10Header.dxgiFormat : file.header.ddsPixelFormat.DxgiFormat;

            switch (format)
            {
                case DxgiFormat.DxgiFormatR8Unorm:
                    return CompressionFormat.R;

                case DxgiFormat.DxgiFormatR8G8Unorm:
                    return CompressionFormat.Rg;

                // HINT: R8G8B8 has no DxgiFormat to convert from

                case DxgiFormat.DxgiFormatR8G8B8A8Unorm:
                    return CompressionFormat.Rgba;

                case DxgiFormat.DxgiFormatB8G8R8A8Unorm:
                    return CompressionFormat.Bgra;

                case DxgiFormat.DxgiFormatBc1Unorm:
                case DxgiFormat.DxgiFormatBc1UnormSrgb:
                case DxgiFormat.DxgiFormatBc1Typeless:
                    if (file.header.ddsPixelFormat.dwFlags.HasFlag(PixelFormatFlags.DdpfAlphaPixels))
                    {
                        return CompressionFormat.Bc1WithAlpha;
                    }

                    if (decoderInputOptions.DdsBc1ExpectAlpha)
                    {
                        return CompressionFormat.Bc1WithAlpha;
                    }
                    return CompressionFormat.Bc1;

                case DxgiFormat.DxgiFormatBc2Unorm:
                case DxgiFormat.DxgiFormatBc2UnormSrgb:
                case DxgiFormat.DxgiFormatBc2Typeless:
                    return CompressionFormat.Bc2;

                case DxgiFormat.DxgiFormatBc3Unorm:
                case DxgiFormat.DxgiFormatBc3UnormSrgb:
                case DxgiFormat.DxgiFormatBc3Typeless:
                    return CompressionFormat.Bc3;

                case DxgiFormat.DxgiFormatBc4Unorm:
                case DxgiFormat.DxgiFormatBc4Snorm:
                case DxgiFormat.DxgiFormatBc4Typeless:
                    return CompressionFormat.Bc4;

                case DxgiFormat.DxgiFormatBc5Unorm:
                case DxgiFormat.DxgiFormatBc5Snorm:
                case DxgiFormat.DxgiFormatBc5Typeless:
                    return CompressionFormat.Bc5;

                case DxgiFormat.DxgiFormatBc6HTypeless:
                case DxgiFormat.DxgiFormatBc6HUf16:
                    return CompressionFormat.Bc6U;

                case DxgiFormat.DxgiFormatBc6HSf16:
                    return CompressionFormat.Bc6S;

                case DxgiFormat.DxgiFormatBc7Unorm:
                case DxgiFormat.DxgiFormatBc7UnormSrgb:
                case DxgiFormat.DxgiFormatBc7Typeless:
                    return CompressionFormat.Bc7;

                case DxgiFormat.DxgiFormatAtcExt:
                    return CompressionFormat.Atc;

                case DxgiFormat.DxgiFormatAtcExplicitAlphaExt:
                    return CompressionFormat.AtcExplicitAlpha;

                case DxgiFormat.DxgiFormatAtcInterpolatedAlphaExt:
                    return CompressionFormat.AtcInterpolatedAlpha;

                default:
                    return CompressionFormat.Unknown;
            }
        }
    }

    #endregion Methods
}
