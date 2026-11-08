using System;
using System.Buffers.Binary;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AwesomeAssertions;
using ImageMagick;
using IronyModManager.IO.Readers;
using IronyModManager.Shared;
using Moq;
using Xunit;
using static IronyModManager.IO.Tests.ImageReaderCompatibilityTests;

namespace IronyModManager.IO.Tests
{
    public class SharpImagePreviewTests
    {
        [Theory]
        [InlineData(MagickFormat.Png)]
        [InlineData(MagickFormat.Tga)]
        [InlineData(MagickFormat.Jpeg)]
        [InlineData(MagickFormat.Bmp)]
        [InlineData(MagickFormat.Gif)]
        [InlineData(MagickFormat.Tiff)]
        public async Task Should_preview_ordinary_formats(MagickFormat format)
        {
            var data = CreateOrdinaryImage(format, MagickColors.Red);
            using var result = await Decode(data, "preview." + format.ToString().ToLowerInvariant());
            result.Should().NotBeNull();
            result.Position.Should().Be(0);
            using var image = new MagickImage(result);
            image.Width.Should().Be(3);
            image.Height.Should().Be(2);
            var color = Pixel(image, 0, 0);
            color[0].Should().BeGreaterThan((byte)245);
            color[1].Should().BeLessThan((byte)10);
            color[2].Should().BeLessThan((byte)10);
            color[3].Should().Be(255);
        }

        [Theory]
        [InlineData(MagickFormat.Png)]
        [InlineData(MagickFormat.Tga)]
        public async Task Should_preserve_transparency_and_ordinary_orientation(MagickFormat format)
        {
            using var sample = new MagickImage(MagickColors.Transparent, 3, 2);
            using (var pixels = sample.GetPixels())
            {
                pixels.SetPixel(0, 0, new byte[] { 255, 0, 0, 255 });
                pixels.SetPixel(2, 1, new byte[] { 0, 0, 255, 128 });
            }
            using var result = await Decode(sample.ToByteArray(format), "image.tga");
            result.Should().NotBeNull();
            using var actual = new MagickImage(result);
            Pixel(actual, 0, 0).Should().Equal(255, 0, 0, 255);
            Pixel(actual, 2, 1).Should().Equal(0, 0, 255, 128);
            Pixel(actual, 1, 0)[3].Should().Be(0);
        }

        [Theory]
        [InlineData("DXT1")]
        [InlineData("DXT3")]
        [InlineData("DXT5")]
        public async Task Should_preview_legacy_compressed_dds(string fourCc)
        {
            await AssertParity(CreateDxtDds(fourCc, 0xf800));
        }

        [Fact]
        public async Task Should_preserve_bc1_transparent_mode()
        {
            var data = CreateDxtDds("DXT1", 0);
            data[130] = data[131] = 255;
            Array.Fill(data, (byte)255, 132, 4);
            using var result = await Decode(data);
            using var image = new MagickImage(result);
            Pixel(image, 0, 0)[3].Should().Be(0);
        }

        [Theory]
        [InlineData("DXT3")]
        [InlineData("DXT5")]
        public async Task Should_preserve_varying_alpha_across_multiple_blocks(string fourCc)
        {
            var original = CreateDxtDds(fourCc, 0xf800);
            var data = new byte[160];
            Array.Copy(original, data, original.Length);
            Array.Copy(original, 128, data, 144, 16);
            Put(data, 16, 8);
            Put(data, 20, 32);
            if (fourCc == "DXT3")
            {
                for (int i = 0; i < 8; i++)
                {
                    data[128 + i] = (byte)(i * 2 + ((i * 2 + 1) << 4));
                    data[144 + i] = (byte)(255 - data[128 + i]);
                }
            }
            else
            {
                data[128] = 0; data[129] = 255;
                data[144] = 255; data[145] = 0;
                ulong indices = 0;
                for (int i = 0; i < 16; i++) indices |= (ulong)(i % 8) << (i * 3);
                for (int i = 0; i < 6; i++) data[130 + i] = data[146 + i] = (byte)(indices >> (i * 8));
            }
            await AssertParity(data);
        }

        [Theory]
        [InlineData(28u)]
        [InlineData(29u)]
        [InlineData(87u)]
        [InlineData(91u)]
        public async Task Should_preserve_dxgi_channels_and_alpha(uint format)
        {
            bool rgba = format == 28 || format == 29;
            var data = CreateDx10Dds(format, rgba ? new byte[] { 255, 20, 5, 128 } : new byte[] { 5, 20, 255, 128 });
            using var result = await Decode(data);
            result.Should().NotBeNull();
            using var actual = new MagickImage(result);
            Pixel(actual, 0, 0).Should().Equal(255, 20, 5, 128);
        }

        [Fact]
        public async Task Should_preview_bc7()
        {
            var payload = new byte[16];
            payload[0] = 0x40;
            await AssertParity(CreateDx10Dds(98, payload));
        }

        [Fact]
        public async Task Should_preserve_rgb555_first_mip_and_rounding()
        {
            await AssertParity(CreateRgb555Dds(2, 2, new ushort[] { 0x7c00, 0x3e0, 0x1f, 0x4210 }, new ushort[] { 0 }));
        }

        [Theory]
        [InlineData("DXT1")]
        [InlineData("DXT3")]
        [InlineData("DXT5")]
        public async Task Should_preserve_all_six_faces_mips_and_orientation(string fourCc)
        {
            var cube = CreateCubemapDds();
            if (fourCc != "DXT1")
            {
                var expanded = new byte[320];
                Array.Copy(cube, expanded, 128);
                Put(expanded, 84, fourCc == "DXT3" ? 0x33545844u : 0x35545844u);
                Put(expanded, 20, 16);
                for (int block = 0; block < 12; block++)
                {
                    if (fourCc == "DXT3") Array.Fill(expanded, (byte)255, 128 + block * 16, 8);
                    else expanded[128 + block * 16] = 255;
                    Array.Copy(cube, 128 + block * 8, expanded, 136 + block * 16, 8);
                }
                cube = expanded;
            }
            int blockSize = fourCc == "DXT1" ? 8 : 16;
            int colorOffset = blockSize == 8 ? 4 : 12;
            for (int face = 0; face < 6; face++)
                new byte[] { 0xe4, 0x1b, 0x55, 0xaa }.CopyTo(cube, 128 + face * blockSize * 2 + colorOffset);
            await AssertParity(cube);
        }

        [Fact]
        public async Task Should_compose_raw_dx10_cube_with_correct_alpha_and_face_orientation()
        {
            var source = CreateDx10Dds(29, new byte[4]);
            var cube = new byte[148 + 6 * 16];
            Array.Copy(source, cube, 148);
            Put(cube, 12, 2); Put(cube, 16, 2); Put(cube, 20, 8);
            Put(cube, 136, 4);
            for (int face = 0; face < 6; face++)
                for (int pixel = 0; pixel < 4; pixel++)
                {
                    int offset = 148 + face * 16 + pixel * 4;
                    cube[offset] = (byte)(face * 30);
                    cube[offset + 1] = (byte)(pixel * 50);
                    cube[offset + 2] = 20;
                    cube[offset + 3] = 128;
                }
            using var result = await Decode(cube);
            result.Should().NotBeNull();
            using var image = new MagickImage(result);
            image.Width.Should().Be(8); image.Height.Should().Be(6);
            var positions = new[] { (2, 1), (0, 1), (1, 0), (1, 2), (1, 1), (3, 1) };
            for (int face = 0; face < 6; face++)
                for (int pixel = 0; pixel < 4; pixel++)
                    Pixel(image, positions[face].Item1 * 2 + pixel % 2, positions[face].Item2 * 2 + pixel / 2)
                        .Should().Equal((byte)(face * 30), (byte)(pixel * 50), 20, 128);
            Pixel(image, 0, 0)[3].Should().Be(0);
        }

        [Theory]
        [InlineData("image.dds", MagickFormat.Png)]
        [InlineData("image.tga", MagickFormat.Png)]
        [InlineData("image.png", MagickFormat.Dds)]
        public async Task Should_treat_extensions_as_hints(string file, MagickFormat format)
        {
            var data = format == MagickFormat.Dds ? CreateDxtDds("DXT1", 0xf800) : CreateOrdinaryImage(format, MagickColors.Red);
            using var result = await Decode(data, file);
            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData("partialCube")]
        [InlineData("shortHeader")]
        [InlineData("truncated")]
        [InlineData("overflow")]
        [InlineData("mips")]
        [InlineData("volume")]
        [InlineData("unsupportedFourCc")]
        [InlineData("unsupportedDxgi")]
        [InlineData("dx10Short")]
        [InlineData("array")]
        [InlineData("alphaMode")]
        [InlineData("paddedRow")]
        [InlineData("trailing")]
        public async Task Should_reject_invalid_or_unsupported_surfaces_without_partial_preview(string failure)
        {
            var data = CreateDxtDds("DXT1", 0xf800);
            switch (failure)
            {
                case "partialCube": data = CreateCubemapDds(); Put(data, 112, 0x600); break;
                case "shortHeader": Array.Resize(ref data, 12); break;
                case "truncated": Array.Resize(ref data, data.Length - 1); break;
                case "overflow": Put(data, 16, uint.MaxValue); break;
                case "mips": Put(data, 28, 99); break;
                case "volume": Put(data, 112, 0x200000); break;
                case "unsupportedFourCc": Put(data, 84, 0x32545844); break;
                case "unsupportedDxgi": data = CreateDx10Dds(10, new byte[4]); break;
                case "dx10Short": data = CreateDx10Dds(29, new byte[4]); Array.Resize(ref data, 140); break;
                case "array": data = CreateDx10Dds(29, new byte[4]); Put(data, 140, 2); break;
                case "alphaMode": data = CreateDx10Dds(29, new byte[4]); Put(data, 144, 2); break;
                case "paddedRow": data = CreateDx10Dds(29, new byte[4]); Put(data, 20, 8); break;
                case "trailing": Array.Resize(ref data, data.Length + 8); break;
            }
            var logger = new Mock<ILogger>();
            using var input = new TrackingStream(data);
            using var result = await Parse(input, "image.dds", true, logger.Object);
            result.Should().BeNull();
            input.Disposed.Should().BeTrue();
            logger.Verify(x => x.Error(It.IsAny<Exception>(), It.IsAny<string>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Should_rewind_and_dispose_owned_input()
        {
            using var input = new TrackingStream(CreateOrdinaryImage(MagickFormat.Png, MagickColors.Red));
            input.Position = 9;
            using var result = await Parse(input, "image.png", true);
            result.Should().NotBeNull();
            result.Position.Should().Be(0);
            input.Disposed.Should().BeTrue();
        }

        [Fact]
        public async Task Should_buffer_and_dispose_nonseekable_input()
        {
            using var input = new NonseekableStream(CreateOrdinaryImage(MagickFormat.Png, MagickColors.Red));
            using var result = await Parse(input, "image.png", true);
            result.Should().NotBeNull();
            result.Position.Should().Be(0);
            input.Disposed.Should().BeTrue();
        }

        [Fact]
        public async Task Should_reject_unknown_images_and_oversized_input()
        {
            using var unknown = await Decode(new byte[20], "unknown.bin");
            unknown.Should().BeNull();
            using var input = new OversizedStream();
            using var result = await Parse(input, "image.png", true);
            result.Should().BeNull();
            input.Disposed.Should().BeTrue();
        }

        [Fact]
        public void Should_select_backend_from_the_actual_platform()
        {
            var type = typeof(Reader).Assembly.GetType("IronyModManager.IO.Images.ImageReader", true);
            var reader = Activator.CreateInstance(type, Mock.Of<ILogger>());
            var selected = type.GetField("sharpImageReader", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(reader);
            (selected != null).Should().Be(RuntimeInformation.IsOSPlatform(OSPlatform.OSX));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Should_not_fall_back_to_a_single_cubemap_face_with_misleading_extension(bool managed)
        {
            var cube = CreateCubemapDds();
            using (var complete = await Parse(new MemoryStream(cube), "cube.png", managed))
            {
                complete.Should().NotBeNull();
                using var image = new MagickImage(complete);
                image.Width.Should().Be(16);
                image.Height.Should().Be(12);
            }
            Put(cube, 112, 0x600);
            using var incomplete = await Parse(new MemoryStream(cube), "cube.png", managed);
            incomplete.Should().BeNull();
        }

        private async Task AssertParity(byte[] data)
        {
            using var managed = await Decode(data);
            using var native = await Parse(new MemoryStream(data), "image.dds", false);
            managed.Should().NotBeNull();
            native.Should().NotBeNull();
            using var actual = new MagickImage(managed);
            using var expected = new MagickImage(native);
            actual.Width.Should().Be(expected.Width);
            actual.Height.Should().Be(expected.Height);
            for (int y = 0; y < actual.Height; y++)
                for (int x = 0; x < actual.Width; x++)
                {
                    var left = Pixel(actual, x, y);
                    var right = Pixel(expected, x, y);
                    for (int c = 0; c < 4; c++) Math.Abs(left[c] - right[c]).Should().BeLessThanOrEqualTo(1);
                }
        }

        private Task<MemoryStream> Decode(byte[] data, string file = "image.dds") => Parse(new MemoryStream(data), file, true);

        private Task<MemoryStream> Parse(Stream input, string file, bool useSharpImage, ILogger logger = null)
        {
            var type = typeof(Reader).Assembly.GetType("IronyModManager.IO.Images.ImageReader", true);
            var ctor = type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                new[] { typeof(ILogger), typeof(bool) }, null);
            var reader = ctor.Invoke(new object[] { logger ?? Mock.Of<ILogger>(), useSharpImage });
            return (Task<MemoryStream>)type.GetMethod("Parse").Invoke(reader, new object[] { input, file });
        }

        private byte[] Pixel(MagickImage image, int x, int y)
        {
            using var pixels = image.GetPixels();
            var color = pixels.GetPixel(x, y).ToColor();
            return new[] { color.R, color.G, color.B, color.A };
        }

        private void Put(byte[] data, int offset, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(offset, 4), value);

        private class TrackingStream : MemoryStream
        {
            public TrackingStream(byte[] data) : base(data) { }
            public bool Disposed { get; private set; }
            protected override void Dispose(bool disposing)
            {
                Disposed = true;
                base.Dispose(disposing);
            }
        }

        private sealed class NonseekableStream : TrackingStream
        {
            public NonseekableStream(byte[] data) : base(data) { }
            public override bool CanSeek => false;
            public override long Seek(long offset, SeekOrigin loc) => throw new NotSupportedException();
        }

        private sealed class OversizedStream : TrackingStream
        {
            public OversizedStream() : base(Array.Empty<byte>()) { }
            public override long Length => 64L * 1024 * 1024 + 1;
        }
    }
}
