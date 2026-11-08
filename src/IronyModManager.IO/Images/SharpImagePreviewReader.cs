using System;
using System.Buffers.Binary;
using System.IO;
using System.Threading.Tasks;
using SharpImage.Core;
using SharpImage.Formats;
using SharpImage.Image;

namespace IronyModManager.IO.Images
{
    /// <summary>
    /// Limited managed macOS preview conversion. Unsupported layouts are rejected, never
    /// represented by a partial surface. Stream ownership and error logging belong to ImageReader.
    /// </summary>
    internal class SharpImagePreviewReader
    {
        // Bound the extra buffered input and RGBA16 canvas owned by this preview adapter.
        private const int MaxInputBytes = 64 * 1024 * 1024;
        private const int MaxPreviewPixels = 16 * 1024 * 1024;
        private const uint DdsMagic = 0x20534444;
        private const uint Dxt1 = 0x31545844;
        private const uint Dxt3 = 0x33545844;
        private const uint Dxt5 = 0x35545844;
        private const uint Dx10 = 0x30315844;

        public async Task<MemoryStream> Parse(Stream stream, string file)
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
                if (stream.Length > MaxInputBytes)
                {
                    throw new NotSupportedException("Image exceeds the managed preview input limit.");
                }
            }

            using var buffer = new MemoryStream();
            var chunk = new byte[81920];
            int count;
            while ((count = await stream.ReadAsync(chunk)) != 0)
            {
                if (buffer.Length + count > MaxInputBytes)
                {
                    throw new NotSupportedException("Image exceeds the managed preview input limit.");
                }
                buffer.Write(chunk, 0, count);
            }

            var data = buffer.ToArray();
            using var image = data.Length >= 4 && U32(data, 0) == DdsMagic
                ? DecodeDds(data)
                : DecodeOther(data, file);
            var result = new MemoryStream();
            try
            {
                PngCoder.Write(image, result);
                result.Position = 0;
                return result;
            }
            catch
            {
                result.Dispose();
                throw;
            }
        }

        private ImageFrame DecodeOther(byte[] data, string file)
        {
            using var stream = new MemoryStream(data, false);
            // Strong signatures take precedence over the filename, including misleading .tga/.dds.
            if (PngCoder.CanDecode(data)) return PngCoder.Read(stream);
            if (JpegCoder.CanDecode(data)) return JpegCoder.Read(stream);
            if (BmpCoder.CanDecode(data)) return BmpCoder.Read(stream);
            if (GifCoder.CanDecode(data)) return GifCoder.Read(stream);
            if (TiffCoder.CanDecode(data)) return TiffCoder.Read(stream);
            if (TgaCoder.CanDecode(data) || file.EndsWith(".tga", StringComparison.OrdinalIgnoreCase))
            {
                return TgaCoder.Read(stream);
            }
            throw new NotSupportedException("Image format is not supported by the managed preview backend.");
        }

        private ImageFrame DecodeDds(byte[] data)
        {
            if (data.Length < 128 || U32(data, 4) != 124 || U32(data, 76) != 32)
                throw new InvalidDataException("Invalid DDS header.");

            int width = checked((int)U32(data, 16)), height = checked((int)U32(data, 12));
            CheckDimensions(width, height);
            var caps2 = U32(data, 112);
            if (U32(data, 24) > 1 || (caps2 & 0x200000) != 0)
                throw new NotSupportedException("Volume DDS previews are not supported.");

            uint fourCc = U32(data, 84), format = 0;
            int headerSize = 128, blockBytes = 0, pixelBytes = 0;
            bool rgb555 = false, cube = (caps2 & 0xfe00) != 0;
            if (cube && (caps2 & 0xfe00) != 0xfe00)
                throw new NotSupportedException("DDS cubemap previews require all six faces.");

            if ((U32(data, 80) & 4) != 0)
            {
                if (fourCc == Dxt1) blockBytes = 8;
                else if (fourCc == Dxt3 || fourCc == Dxt5) blockBytes = 16;
                else if (fourCc == Dx10)
                {
                    if (data.Length < 148) throw new InvalidDataException("Truncated DDS DX10 header.");
                    headerSize = 148;
                    format = U32(data, 128);
                    if (U32(data, 132) != 3 || U32(data, 140) != 1 ||
                        (U32(data, 136) & ~4u) != 0 || U32(data, 144) != 0)
                        throw new NotSupportedException("DDS arrays, non-2D resources and explicit alpha modes are unsupported.");
                    if (cube && (U32(data, 136) & 4) == 0)
                        throw new InvalidDataException("Inconsistent DDS cubemap headers.");
                    cube |= (U32(data, 136) & 4) != 0;
                    if (format == 98 || format == 99) blockBytes = 16;
                    else if (format == 28 || format == 29 || format == 87 || format == 91) pixelBytes = 4;
                    else throw new NotSupportedException($"Unsupported DDS DXGI format: {format}.");
                }
                else throw new NotSupportedException($"Unsupported DDS FourCC: 0x{fourCc:X8}.");
            }
            else if (U32(data, 80) == 0x40 && U32(data, 88) == 16 &&
                U32(data, 92) == 0x7c00 && U32(data, 96) == 0x3e0 &&
                U32(data, 100) == 0x1f && U32(data, 104) == 0)
            {
                rgb555 = true;
                pixelBytes = 2;
            }
            else throw new NotSupportedException("Unsupported DDS pixel layout.");

            if (cube) CheckDimensions(checked(width * 4), checked(height * 3));
            if (pixelBytes != 0 && (U32(data, 8) & 8) != 0 && U32(data, 20) != checked(width * pixelBytes))
                throw new NotSupportedException("Padded DDS rows are not supported.");

            int mips = Math.Max(1, checked((int)U32(data, 28))), maxMips = 1;
            for (int size = Math.Max(width, height); size > 1; size /= 2) maxMips++;
            if (mips > maxMips) throw new InvalidDataException("Invalid DDS mip count.");
            int stride = 0, firstSize = 0;
            for (int i = 0, w = width, h = height; i < mips; i++, w = Math.Max(1, w / 2), h = Math.Max(1, h / 2))
            {
                int size = blockBytes != 0
                    ? checked(((w + 3) / 4) * ((h + 3) / 4) * blockBytes)
                    : checked(w * h * pixelBytes);
                if (i == 0) firstSize = size;
                stride = checked(stride + size);
            }
            if (data.Length != checked(headerSize + stride * (cube ? 6 : 1)))
                throw new InvalidDataException("DDS payload length does not match the supported surface layout.");

            if (!cube) return DecodeSurface(data, headerSize, firstSize, width, height, fourCc, format, rgb555);

            var canvas = CreateFrame(checked(width * 4), checked(height * 3));
            try
            {
                for (int y = 0; y < height * 3; y++) canvas.GetPixelRowForWrite(y).Clear();
                var positions = new[] { (2, 1), (0, 1), (1, 0), (1, 2), (1, 1), (3, 1) };
                for (int face = 0; face < 6; face++)
                {
                    using var decoded = DecodeSurface(data, checked(headerSize + face * stride), firstSize,
                        width, height, fourCc, format, rgb555);
                    for (int y = 0; y < height; y++)
                        decoded.GetPixelRow(y).CopyTo(canvas.GetPixelRowForWrite(positions[face].Item2 * height + y)
                            .Slice(positions[face].Item1 * width * 4, width * 4));
                }
                return canvas;
            }
            catch
            {
                canvas.Dispose();
                throw;
            }
        }

        private ImageFrame DecodeSurface(byte[] data, int offset, int length, int width, int height,
            uint fourCc, uint format, bool rgb555)
        {
            if (rgb555 || format == 28 || format == 29 || format == 87 || format == 91)
            {
                var image = CreateFrame(width, height);
                try
                {
                    for (int y = 0; y < height; y++)
                    {
                        var row = image.GetPixelRowForWrite(y);
                        for (int x = 0; x < width; x++)
                        {
                            int pixel = x * 4;
                            if (rgb555)
                            {
                                var value = BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(offset + (y * width + x) * 2, 2));
                                row[pixel] = Expand5((value >> 10) & 31);
                                row[pixel + 1] = Expand5((value >> 5) & 31);
                                row[pixel + 2] = Expand5(value & 31);
                                row[pixel + 3] = ushort.MaxValue;
                            }
                            else
                            {
                                int source = offset + (y * width + x) * 4;
                                bool rgba = format == 28 || format == 29;
                                row[pixel] = (ushort)(data[source + (rgba ? 0 : 2)] * 257);
                                row[pixel + 1] = (ushort)(data[source + 1] * 257);
                                row[pixel + 2] = (ushort)(data[source + (rgba ? 2 : 0)] * 257);
                                row[pixel + 3] = (ushort)(data[source + 3] * 257);
                            }
                        }
                    }
                    return image;
                }
                catch
                {
                    image.Dispose();
                    throw;
                }
            }

            int headerSize = fourCc == Dx10 ? 148 : 128;
            var single = new byte[checked(headerSize + length)];
            Array.Copy(data, single, headerSize);
            Array.Copy(data, offset, single, headerSize, length);
            Put(single, 28, 1);
            Put(single, 108, 0x1000);
            Put(single, 112, 0);
            if (fourCc == Dx10) Put(single, 136, 0);
            var decoded = DdsCoder.Decode(single);
            try
            {
                // SharpImage 1.0.13 routes BC2 to BC3. Their color block agrees;
                // replace the interpolated BC3 alpha with BC2's explicit 4-bit alpha.
                if (fourCc == Dxt3)
                {
                    int blocksX = (width + 3) / 4;
                    for (int y = 0; y < height; y++)
                    {
                        var row = decoded.GetPixelRowForWrite(y);
                        for (int x = 0; x < width; x++)
                        {
                            int block = offset + ((y / 4) * blocksX + x / 4) * 16;
                            int index = (y % 4) * 4 + x % 4;
                            row[x * 4 + 3] = (ushort)(((data[block + index / 2] >> ((index & 1) * 4)) & 15) * 4369);
                        }
                    }
                }
                return decoded;
            }
            catch
            {
                decoded.Dispose();
                throw;
            }
        }

        private void CheckDimensions(int width, int height)
        {
            if (width <= 0 || height <= 0 || (long)width * height > MaxPreviewPixels)
                throw new NotSupportedException("Image dimensions exceed the managed preview limit.");
        }

        private ImageFrame CreateFrame(int width, int height)
        {
            CheckDimensions(width, height);
            var image = new ImageFrame();
            try
            {
                image.Initialize(width, height, ColorspaceType.SRGB, true);
                return image;
            }
            catch
            {
                image.Dispose();
                throw;
            }
        }

        private ushort Expand5(int value) => (ushort)(((value * 255 + 15) / 31) * 257);
        private uint U32(byte[] data, int offset) => BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));
        private void Put(byte[] data, int offset, uint value) => BinaryPrimitives.WriteUInt32LittleEndian(data.AsSpan(offset, 4), value);
    }
}
