// ***********************************************************************
// Assembly         : IronyModManager.IO.Tests
// Author           : Mario
// Created          : 09-09-2026
//
// Last Modified By : Mario
// Last Modified On : 09-09-2026
// ***********************************************************************
// <copyright file="ImageReaderCompatibilityTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AwesomeAssertions;
using ImageMagick;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Readers;
using IronyModManager.IO.Readers;
using IronyModManager.Shared;
using Moq;
using Xunit;

namespace IronyModManager.IO.Tests
{
    /// <summary>
    /// Characterizes image behavior while the historical provider stack is consolidated onto Magick.NET.
    /// </summary>
    public sealed class ImageReaderCompatibilityTests
    {
        /// <summary>
        /// Verifies ordinary formats remain readable through the public IO contract.
        /// </summary>
        /// <param name="format">The source format.</param>
        /// <param name="extension">The file extension.</param>
        [Theory]
        [InlineData(MagickFormat.Png, ".png")]
        [InlineData(MagickFormat.Jpeg, ".jpg")]
        [InlineData(MagickFormat.Bmp, ".bmp")]
        [InlineData(MagickFormat.Gif, ".gif")]
        [InlineData(MagickFormat.Tiff, ".tiff")]
        public async Task Should_decode_ordinary_image_formats(MagickFormat format, string extension)
        {
            var source = CreateOrdinaryImage(format, new MagickColor(236, 231, 191));

            using var result = await Decode(source, $"image{extension}");

            result.Should().NotBeNull();
            using var image = new MagickImage(result);
            image.Width.Should().Be(3);
            image.Height.Should().Be(2);
        }

        /// <summary>
        /// Verifies TGA is decoded with its explicit stream hint.
        /// </summary>
        [Fact]
        public async Task Should_decode_tga_with_explicit_format_hint()
        {
            var source = CreateOrdinaryImage(MagickFormat.Tga, MagickColors.Red);

            using var result = await Decode(source, "image.tga");

            GetPixel(result, 0, 0).Should().BeEquivalentTo(new byte[] { 255, 0, 0, 255 });
        }

        /// <summary>
        /// Verifies a misleading TGA extension falls back to content detection after the hinted attempt.
        /// </summary>
        [Fact]
        public async Task Should_retry_content_detection_for_misleading_tga_extension()
        {
            var source = CreateOrdinaryImage(MagickFormat.Png, MagickColors.Blue);

            using var result = await Decode(source, "misleading.tga");

            GetPixel(result, 0, 0).Should().BeEquivalentTo(new byte[] { 0, 0, 255, 255 });
        }

        /// <summary>
        /// Verifies a misleading DDS extension still reaches ordinary content detection.
        /// </summary>
        [Fact]
        public async Task Should_retry_ordinary_decode_for_misleading_dds_extension()
        {
            var source = CreateOrdinaryImage(MagickFormat.Png, MagickColors.Green);

            using var result = await Decode(source, "misleading.dds");

            GetPixel(result, 0, 0).Should().BeEquivalentTo(new byte[] { 0, 128, 0, 255 });
        }

        /// <summary>
        /// Verifies the flat block-compressed DDS formats observed in the corpus remain readable.
        /// </summary>
        /// <param name="fourCc">The DDS FourCC.</param>
        [Theory]
        [InlineData("DXT1")]
        [InlineData("DXT3")]
        [InlineData("DXT5")]
        public async Task Should_decode_flat_dxt_dds(string fourCc)
        {
            var source = CreateDxtDds(fourCc, 0xF800);

            using var result = await Decode(source, "image.dds");

            GetPixel(result, 0, 0).Should().BeEquivalentTo(new byte[] { 255, 0, 0, 255 });
        }

        /// <summary>
        /// Verifies DXGI 98 / BC7 reaches the ordinary Magick DDS path.
        /// </summary>
        [Fact]
        public async Task Should_decode_dxgi_98_bc7_dds()
        {
            // Synthetic BC7 mode-six block with zero endpoints and indices.
            var source = CreateDx10Dds(98, new byte[] { 0x40, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 });

            using var result = await Decode(source, "image.dds");

            result.Should().NotBeNull();
            using var image = new MagickImage(result);
            image.Width.Should().Be(4);
            image.Height.Should().Be(4);
        }

        /// <summary>
        /// Verifies exact RGB555 decoding, first-mip selection, rounded expansion, and opaque alpha.
        /// </summary>
        [Fact]
        public async Task Should_decode_rgb555_first_mip_with_rounded_channels_and_opaque_alpha()
        {
            // R=3, G=17, B=29 proves rounded conversion rather than bit replication.
            var firstMipPixel = (ushort)((3 << 10) | (17 << 5) | 29);
            var source = CreateRgb555Dds(1, 1, new[] { firstMipPixel }, new ushort[] { 0x7C00 });

            using var result = await Decode(source, "leader.dds");

            GetPixel(result, 0, 0).Should().BeEquivalentTo(new byte[] { 25, 140, 239, 255 });
        }

        /// <summary>
        /// Verifies the owner-approved standards-correct DXGI 29 channel order.
        /// </summary>
        [Fact]
        public async Task Should_normalize_dxgi_29_to_standards_correct_rgba_order()
        {
            var source = CreateDx10Dds(29, new byte[] { 236, 231, 191, 255 });

            using var result = await Decode(source, "colored.dds");

            GetPixel(result, 0, 0).Should().BeEquivalentTo(new byte[] { 236, 231, 191, 255 });
        }

        /// <summary>
        /// Verifies DXGI 91 is normalized to its byte-compatible non-sRGB BGRA format.
        /// </summary>
        [Fact]
        public async Task Should_normalize_dxgi_91_to_bgra_format()
        {
            var source = CreateDx10Dds(91, new byte[] { 191, 231, 236, 255 });

            using var result = await Decode(source, "colored.dds");

            GetPixel(result, 0, 0).Should().BeEquivalentTo(new byte[] { 236, 231, 191, 255 });
        }

        /// <summary>
        /// Verifies the established cubemap face order, top mip, dimensions, and transparent cells.
        /// </summary>
        [Fact]
        public async Task Should_compose_cubemap_as_transparent_four_by_three_cross()
        {
            var source = CreateCubemapDds();

            using var result = await Decode(source, "cube.dds");
            using var image = new MagickImage(result);

            image.Width.Should().Be(16);
            image.Height.Should().Be(12);
            GetPixel(image, 4, 0).Should().BeEquivalentTo(new byte[] { 0, 0, 255, 255 });       // +Y
            GetPixel(image, 0, 4).Should().BeEquivalentTo(new byte[] { 0, 255, 0, 255 });       // -X
            GetPixel(image, 4, 4).Should().BeEquivalentTo(new byte[] { 255, 255, 0, 255 });     // +Z
            GetPixel(image, 8, 4).Should().BeEquivalentTo(new byte[] { 255, 0, 0, 255 });       // +X
            GetPixel(image, 12, 4).Should().BeEquivalentTo(new byte[] { 0, 255, 255, 255 });    // -Z
            GetPixel(image, 4, 8).Should().BeEquivalentTo(new byte[] { 255, 255, 255, 255 });   // -Y
            GetPixel(image, 0, 0).Should().BeEquivalentTo(new byte[] { 0, 0, 0, 0 });
            GetPixel(image, 0, 8).Should().BeEquivalentTo(new byte[] { 0, 0, 0, 0 });
        }

        /// <summary>
        /// Verifies malformed compatibility headers and payloads preserve logged-null failure semantics.
        /// </summary>
        /// <param name="failure">The malformed case.</param>
        [Theory]
        [InlineData("header")]
        [InlineData("dx10")]
        [InlineData("payload")]
        [InlineData("dimensions")]
        [InlineData("overflow")]
        public async Task Should_reject_malformed_dds_with_existing_failure_semantics(string failure)
        {
            var source = failure switch
            {
                "header" => new byte[64],
                "dx10" => CreateTruncatedDx10Dds(),
                "payload" => CreateRgb555Dds(2, 2, new ushort[] { 0x7C00 }, null),
                "dimensions" => CreateInvalidRgb555Dds(0, 1),
                "overflow" => CreateInvalidRgb555Dds(uint.MaxValue, uint.MaxValue),
                _ => throw new InvalidOperationException()
            };
            var logger = new Mock<ILogger>();

            using var result = await Decode(source, "invalid.dds", logger.Object);

            result.Should().BeNull();
            logger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        /// <summary>
        /// Verifies the input is owned and disposed while the returned PNG starts at zero.
        /// </summary>
        [Fact]
        public async Task Should_dispose_input_and_return_png_positioned_at_zero()
        {
            var input = new TrackingMemoryStream(CreateOrdinaryImage(MagickFormat.Png, MagickColors.Red));
            var reader = new Reader(new[] { new StreamFileReader(input) }, Mock.Of<ILogger>());

            using var result = await reader.GetImageStreamAsync("root", "image.png");

            input.Disposed.Should().BeTrue();
            result.Should().NotBeNull();
            result.Position.Should().Be(0);
            using var decoded = new MagickImage(result);
            decoded.Format.Should().Be(MagickFormat.Png);
        }

        private static async Task<MemoryStream> Decode(byte[] data, string file, ILogger logger = null)
        {
            var input = new TrackingMemoryStream(data);
            var reader = new Reader(new[] { new StreamFileReader(input) }, logger ?? Mock.Of<ILogger>());
            return await reader.GetImageStreamAsync("root", file);
        }

        private static byte[] CreateOrdinaryImage(MagickFormat format, MagickColor color)
        {
            using var image = new MagickImage(color, 3, 2) { Format = format };
            return image.ToByteArray();
        }

        private static byte[] CreateDxtDds(string fourCc, ushort color)
        {
            byte[] block;
            if (fourCc == "DXT1")
            {
                block = CreateDxtColorBlock(color);
            }
            else if (fourCc == "DXT3")
            {
                block = new byte[16];
                Array.Fill(block, byte.MaxValue, 0, 8);
                CreateDxtColorBlock(color).CopyTo(block, 8);
            }
            else
            {
                block = new byte[16];
                block[0] = byte.MaxValue;
                CreateDxtColorBlock(color).CopyTo(block, 8);
            }

            var header = CreateDdsHeader(4, 4, (uint)block.Length, FourCc(fourCc), 0x4, 0x1000, 0, 1);
            return Combine(header, block);
        }

        private static byte[] CreateDx10Dds(uint format, byte[] payload)
        {
            var dimension = format == 98 ? 4u : 1u;
            var header = CreateDdsHeader(dimension, dimension, (uint)payload.Length, FourCc("DX10"), 0x4, 0x1000, 0, 1);
            if (format is 29 or 91)
            {
                Write(header, 8, 0x2100F);
                Write(header, 20, dimension * 4);
                Write(header, 24, 1);
            }

            var dx10 = new byte[20];
            Write(dx10, 0, format);
            Write(dx10, 4, 3);
            Write(dx10, 8, 0);
            Write(dx10, 12, 1);
            return Combine(header, dx10, payload);
        }

        private static byte[] CreateRgb555Dds(int width, int height, ushort[] firstMip, ushort[] secondMip)
        {
            var header = CreateDdsHeader((uint)width, (uint)height, checked((uint)(width * height * 2)), 0, 0x40, 0x401008, 0, secondMip == null ? 1u : 2u);
            Write(header, 88, 16);
            Write(header, 92, 0x7C00);
            Write(header, 96, 0x03E0);
            Write(header, 100, 0x001F);
            Write(header, 104, 0);
            return Combine(header, ToBytes(firstMip), ToBytes(secondMip));
        }

        private static byte[] CreateInvalidRgb555Dds(uint width, uint height)
        {
            var header = CreateDdsHeader(width, height, uint.MaxValue, 0, 0x40, 0x1000, 0, 1);
            Write(header, 88, 16);
            Write(header, 92, 0x7C00);
            Write(header, 96, 0x03E0);
            Write(header, 100, 0x001F);
            return header;
        }

        private static byte[] CreateTruncatedDx10Dds()
        {
            var header = CreateDdsHeader(1, 1, 4, FourCc("DX10"), 0x4, 0x1000, 0, 1);
            return Combine(header, new byte[8]);
        }

        private static byte[] CreateCubemapDds()
        {
            var header = CreateDdsHeader(4, 4, 8, FourCc("DXT1"), 0x4, 0x401008, 0xFE00, 2);
            var payloads = new List<byte[]>();
            foreach (var color in new ushort[] { 0xF800, 0x07E0, 0x001F, 0xFFFF, 0xFFE0, 0x07FF })
            {
                payloads.Add(CreateDxtColorBlock(color));
                payloads.Add(CreateDxtColorBlock(0));
            }

            var parts = new List<byte[]> { header };
            parts.AddRange(payloads);
            return Combine(parts.ToArray());
        }

        private static byte[] CreateDdsHeader(uint width, uint height, uint linearSize, uint fourCc, uint pixelFlags, uint caps, uint caps2, uint mipCount)
        {
            var header = new byte[128];
            Write(header, 0, 0x20534444);
            Write(header, 4, 124);
            Write(header, 8, 0xA1007);
            Write(header, 12, height);
            Write(header, 16, width);
            Write(header, 20, linearSize);
            Write(header, 28, mipCount);
            Write(header, 76, 32);
            Write(header, 80, pixelFlags);
            Write(header, 84, fourCc);
            Write(header, 108, caps);
            Write(header, 112, caps2);
            return header;
        }

        private static byte[] CreateDxtColorBlock(ushort color)
        {
            var block = new byte[8];
            BinaryPrimitives.WriteUInt16LittleEndian(block.AsSpan(0, 2), color);
            BinaryPrimitives.WriteUInt16LittleEndian(block.AsSpan(2, 2), 0);
            return block;
        }

        private static uint FourCc(string value)
        {
            return BinaryPrimitives.ReadUInt32LittleEndian(System.Text.Encoding.ASCII.GetBytes(value));
        }

        private static void Write(byte[] bytes, int offset, uint value)
        {
            BinaryPrimitives.WriteUInt32LittleEndian(bytes.AsSpan(offset, 4), value);
        }

        private static byte[] ToBytes(ushort[] values)
        {
            if (values == null)
            {
                return Array.Empty<byte>();
            }

            var bytes = new byte[values.Length * 2];
            for (var i = 0; i < values.Length; i++)
            {
                BinaryPrimitives.WriteUInt16LittleEndian(bytes.AsSpan(i * 2, 2), values[i]);
            }

            return bytes;
        }

        private static byte[] Combine(params byte[][] arrays)
        {
            var length = 0;
            foreach (var array in arrays)
            {
                length = checked(length + array.Length);
            }

            var result = new byte[length];
            var offset = 0;
            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, result, offset, array.Length);
                offset += array.Length;
            }

            return result;
        }

        private static byte[] GetPixel(Stream stream, int x, int y)
        {
            stream.Position = 0;
            using var image = new MagickImage(stream);
            return GetPixel(image, x, y);
        }

        private static byte[] GetPixel(MagickImage image, int x, int y)
        {
            using var pixels = image.GetPixels();
            var color = pixels.GetPixel(x, y).ToColor();
            return new[] { color.R, color.G, color.B, color.A };
        }

        private sealed class StreamFileReader : IFileReader
        {
            private readonly Stream stream;

            public StreamFileReader(Stream stream)
            {
                this.stream = stream;
            }

            public bool CanListFiles(string path) => false;

            public bool CanRead(string path, bool searchSubFolders = true) => true;

            public bool CanReadStream(string path) => true;

            public IEnumerable<string> GetFiles(string path) => Array.Empty<string>();

            public (Stream, bool, DateTime?, EncodingInfo encoding) GetStream(string rootPath, string file) => (stream, false, null, null);

            public long GetTotalSize(string path, string[] extensions = null) => 0;

            public IReadOnlyCollection<IFileInfo> Read(string path, IEnumerable<string> allowedPaths = null, bool searchSubFolders = true) => Array.Empty<IFileInfo>();
        }

        private sealed class TrackingMemoryStream : MemoryStream
        {
            public TrackingMemoryStream(byte[] buffer)
                : base(buffer)
            {
            }

            public bool Disposed { get; private set; }

            protected override void Dispose(bool disposing)
            {
                Disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
