// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 09-09-2026
//
// Last Modified By : Mario
// Last Modified On : 09-09-2026
// ***********************************************************************
// <copyright file="DdsCompatibilityReader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Buffers.Binary;
using System.IO;
using ImageMagick;

namespace IronyModManager.IO.Images
{
    /// <summary>
    /// Handles the narrowly characterized valid DDS layouts which ImageMagick cannot dispatch.
    /// </summary>
    internal class DdsCompatibilityReader : IDdsCompatibilityReader
    {
        #region Fields

        private const int LegacyHeaderLength = 128;
        private const int Dx10HeaderLength = 148;
        private const uint DdsMagic = 0x20534444;
        private const uint DdsHeaderSize = 124;
        private const uint Rgb555HeaderFlags = 0xA1007;
        private const uint Dx10SrgbHeaderFlags = 0x2100F;
        private const uint PixelFormatHeaderSize = 32;
        private const uint PixelFormatFourCc = 0x4;
        private const uint PixelFormatRgb = 0x40;
        private const uint CubeMap = 0x200;
        private const uint Texture2D = 3;
        private const uint TextureCube = 0x4;
        private const uint DxgiR8G8B8A8UnormSrgb = 29;
        private const uint DxgiR8G8B8A8Unorm = 28;
        private const uint DxgiB8G8R8A8UnormSrgb = 91;
        private const uint DxgiB8G8R8A8Unorm = 87;
        private const uint FourCcDx10 = 0x30315844;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Attempts to read a precisely supported compatibility DDS layout.
        /// </summary>
        /// <param name="stream">The DDS stream.</param>
        /// <returns>An image when the exact compatibility layout matches; otherwise, <c>null</c>.</returns>
        public virtual MagickImage TryRead(Stream stream)
        {
            if (!stream.CanSeek)
            {
                return null;
            }

            var header = new byte[LegacyHeaderLength];
            if (!TryReadExactly(stream, header) || !HasValidBaseHeader(header))
            {
                return null;
            }

            var pixelFormatFlags = ReadUInt32(header, 80);
            var fourCc = ReadUInt32(header, 84);
            if ((pixelFormatFlags & PixelFormatFourCc) != 0 && fourCc == FourCcDx10)
            {
                return ReadNormalizedDx10(stream, header);
            }

            if ((pixelFormatFlags & PixelFormatRgb) != 0 && (pixelFormatFlags & PixelFormatFourCc) == 0)
            {
                return ReadRgb555(stream, header);
            }

            return null;
        }

        private bool HasValidBaseHeader(byte[] header)
        {
            return ReadUInt32(header, 0) == DdsMagic &&
                   ReadUInt32(header, 4) == DdsHeaderSize &&
                   ReadUInt32(header, 76) == PixelFormatHeaderSize &&
                   ReadUInt32(header, 12) > 0 &&
                   ReadUInt32(header, 16) > 0;
        }

        private MagickImage ReadRgb555(Stream stream, byte[] header)
        {
            if (ReadUInt32(header, 8) != Rgb555HeaderFlags ||
                ReadUInt32(header, 24) != 0 ||
                ReadUInt32(header, 88) != 16 ||
                ReadUInt32(header, 92) != 0x7C00 ||
                ReadUInt32(header, 96) != 0x03E0 ||
                ReadUInt32(header, 100) != 0x001F ||
                ReadUInt32(header, 104) != 0 ||
                (ReadUInt32(header, 112) & CubeMap) != 0)
            {
                return null;
            }

            var width = ToSupportedDimension(ReadUInt32(header, 16));
            var height = ToSupportedDimension(ReadUInt32(header, 12));
            var pixelCount = checked(width * height);
            var firstMipLength = checked(pixelCount * 2);
            if (ReadUInt32(header, 20) != firstMipLength)
            {
                return null;
            }

            EnsureRemainingLength(stream, firstMipLength);
            var pixels = new byte[firstMipLength];
            ReadExactly(stream, pixels);

            var rgba = new byte[checked(pixelCount * 4)];
            for (var source = 0; source < pixels.Length; source += 2)
            {
                var value = BinaryPrimitives.ReadUInt16LittleEndian(pixels.AsSpan(source, 2));
                var target = (source / 2) * 4;
                rgba[target] = ExpandFiveBits((value >> 10) & 0x1F);
                rgba[target + 1] = ExpandFiveBits((value >> 5) & 0x1F);
                rgba[target + 2] = ExpandFiveBits(value & 0x1F);
                rgba[target + 3] = byte.MaxValue;
            }

            var settings = new MagickReadSettings
            {
                Format = MagickFormat.Rgba,
                Width = checked((uint)width),
                Height = checked((uint)height),
                Depth = 8
            };
            return new MagickImage(rgba, settings);
        }

        private MagickImage ReadNormalizedDx10(Stream stream, byte[] header)
        {
            var dx10Header = new byte[Dx10HeaderLength - LegacyHeaderLength];
            if (!TryReadExactly(stream, dx10Header))
            {
                throw new InvalidDataException("The DDS DX10 header is truncated.");
            }

            var dxgiFormat = ReadUInt32(dx10Header, 0);
            uint normalizedFormat;
            if (dxgiFormat == DxgiR8G8B8A8UnormSrgb)
            {
                normalizedFormat = DxgiR8G8B8A8Unorm;
            }
            else if (dxgiFormat == DxgiB8G8R8A8UnormSrgb)
            {
                normalizedFormat = DxgiB8G8R8A8Unorm;
            }
            else
            {
                return null;
            }

            if (ReadUInt32(header, 8) != Dx10SrgbHeaderFlags ||
                ReadUInt32(header, 24) != 1 ||
                ReadUInt32(dx10Header, 4) != Texture2D ||
                (ReadUInt32(dx10Header, 8) & TextureCube) != 0 ||
                ReadUInt32(dx10Header, 12) != 1 ||
                (ReadUInt32(header, 112) & CubeMap) != 0)
            {
                return null;
            }

            var width = ToSupportedDimension(ReadUInt32(header, 16));
            var height = ToSupportedDimension(ReadUInt32(header, 12));
            var firstMipLength = checked(checked(width * height) * 4);
            if (ReadUInt32(header, 20) != checked(width * 4) ||
                stream.Length != checked(Dx10HeaderLength + firstMipLength))
            {
                return null;
            }

            EnsureRemainingLength(stream, firstMipLength);

            var totalLength = checked((int)stream.Length);
            var normalized = new byte[totalLength];
            Buffer.BlockCopy(header, 0, normalized, 0, header.Length);
            Buffer.BlockCopy(dx10Header, 0, normalized, LegacyHeaderLength, dx10Header.Length);
            BinaryPrimitives.WriteUInt32LittleEndian(normalized.AsSpan(LegacyHeaderLength, 4), normalizedFormat);
            ReadExactly(stream, normalized.AsSpan(Dx10HeaderLength));
            return new MagickImage(normalized);
        }

        private byte ExpandFiveBits(int value)
        {
            return checked((byte)((value * 255 + 15) / 31));
        }

        private void EnsureRemainingLength(Stream stream, int length)
        {
            if (stream.Length - stream.Position < length)
            {
                throw new InvalidDataException("The DDS first mipmap payload is truncated.");
            }
        }

        private int ToSupportedDimension(uint dimension)
        {
            if (dimension == 0 || dimension > int.MaxValue)
            {
                throw new InvalidDataException("The DDS dimensions are invalid.");
            }

            return checked((int)dimension);
        }

        private uint ReadUInt32(byte[] data, int offset)
        {
            return BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset, 4));
        }

        private void ReadExactly(Stream stream, Span<byte> buffer)
        {
            if (!TryReadExactly(stream, buffer))
            {
                throw new InvalidDataException("The DDS payload is truncated.");
            }
        }

        private bool TryReadExactly(Stream stream, Span<byte> buffer)
        {
            var read = 0;
            while (read < buffer.Length)
            {
                var count = stream.Read(buffer.Slice(read));
                if (count == 0)
                {
                    return false;
                }

                read += count;
            }

            return true;
        }

        #endregion Methods
    }
}
