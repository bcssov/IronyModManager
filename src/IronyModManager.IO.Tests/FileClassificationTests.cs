// ***********************************************************************
// Assembly         : IronyModManager.IO.Tests
// Author           : Mario
// Created          : 09-11-2026
//
// Last Modified By : Mario
// Last Modified On : 09-11-2026
// ***********************************************************************
// <copyright file="FileClassificationTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AwesomeAssertions;
using IronyModManager.Shared;
using PommaLabs.MimeTypes;
using UtfUnknown;
using Xunit;

namespace IronyModManager.IO.Tests
{
    /// <summary>
    /// Characterizes the file-classification contract used before parser dispatch.
    /// </summary>
    public class FileClassificationTests
    {
        /// <summary>
        /// Gets representative Paradox text files and common binary formats.
        /// </summary>
        public static IEnumerable<object[]> ClassificationCases
        {
            get
            {
                yield return Case("descriptor.mod", Ascii("name=Test"), "audio/x-mod", "audio/x-mod", true, true);
                yield return Case("notes.txt", Ascii("plain text"), "text/plain", "text/plain", true, true);
                yield return Case("localisation.yml", [0xEF, 0xBB, 0xBF, 0x61], "text/x-yaml", "text/x-yaml", true, true);
                yield return Case("localisation.yaml", Ascii("l_english key value"), "text/x-yaml", "text/x-yaml", true, true);
                yield return Case("data.json", Ascii("{ key value }"), "application/json", "application/json", true, true);
                yield return Case("table.csv", Ascii("a,b\n1,2"), "text/comma-separated-values", "text/comma-separated-values", true, true);
                yield return Case("window.gui", Ascii("containerWindowType"), null, "text/plain", true, true);
                yield return Case("model.asset", Ascii("pdxmesh"), null, "text/plain", true, true);
                yield return Case("effect.gfx", Ascii("spriteTypes"), null, "text/plain", true, true);
                yield return Case("effect.shader", Ascii("PixelShader"), null, "text/plain", true, true);
                yield return Case("include.fxh", Ascii("float4 main"), null, "text/plain", true, true);
                yield return Case("sound.sfx", Ascii("soundeffect"), null, "text/plain", true, true);
                yield return Case("script.lua", Ascii("return true"), "text/x-lua", "text/x-lua", true, true);
                yield return Case("README", Ascii("extensionless text"), null, "text/plain", false, true);
                yield return Case("empty.txt", [], "text/plain", "text/plain", true, true);
                yield return Case("tiny.txt", [0x00], "text/plain", "text/plain", true, true);
                yield return Case("late-null.txt", TextWithLateNull(), "text/plain", "text/plain", true, false);
                yield return Case("binary.txt", Enumerable.Range(0, 600).Select(i => (byte)(i % 256)).ToArray(), "text/plain", "text/plain", true, false);
                yield return Case("file.bin", Enumerable.Range(0, 600).Select(i => (byte)(i % 256)).ToArray(), "application/octet-stream", "application/octet-stream", false, false);
                yield return Case("archive.zip", [0x50, 0x4B, 0x03, 0x04, 0x00, 0x00], "application/x-zip-compressed", "application/x-zip-compressed", false, false);
                yield return Case("image.png", [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A], "image/png", "image/png", false, false);
                yield return Case("image.jpg", [0xFF, 0xD8, 0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46], "image/jpeg", "image/jpeg", false, false);
                yield return Case("image.dds", DdsHeader(), null, null, false, false);
                yield return Case("malformed.weird", Ascii("plain text"), null, "text/plain", false, true);
            }
        }

        /// <summary>
        /// Gets representative encoding-detection cases.
        /// </summary>
        public static IEnumerable<object[]> EncodingCases
        {
            get
            {
                yield return [Ascii("plain text"), "ascii", false, 1.0f];
                yield return [new byte[] { 0xEF, 0xBB, 0xBF, 0x61 }, "utf-8", true, 1.0f];
                yield return [new byte[] { 0xFF, 0xFE, 0x61, 0x00 }, "utf-16le", true, 1.0f];
                yield return [new byte[] { 0xFE, 0xFF, 0x00, 0x61 }, "utf-16be", true, 1.0f];
                yield return [new byte[] { 0x63, 0x61, 0x66, 0xE9, 0x20, 0xE0 }, "iso-8859-1", false, 0.603f];
                yield return [new byte[] { 0x00 }, "ascii", false, 1.0f];
            }
        }

        /// <summary>
        /// Verifies MIME lookup and Irony's filename/content precedence together.
        /// </summary>
        [Theory]
        [MemberData(nameof(ClassificationCases))]
        public void Classification_contract_is_preserved(string file, byte[] content, string filenameMime, string contentMime, bool filenameIsText, bool contentIsText)
        {
            AssertMime(file, filenameMime);

            using var mimeStream = new MemoryStream(content);
            AssertMime(mimeStream, file, contentMime);

            FileSignatureUtility.IsTextFile(file).Should().Be(filenameIsText);

            using var classificationStream = new MemoryStream(content);
            FileSignatureUtility.IsTextFile(file, classificationStream).Should().Be(contentIsText);
            classificationStream.Position.Should().Be(0);
        }

        /// <summary>
        /// Verifies the encodings, BOM flags, and confidence values consumed by IO readers.
        /// </summary>
        [Theory]
        [MemberData(nameof(EncodingCases))]
        public void Encoding_detection_contract_is_preserved(byte[] content, string encodingName, bool hasBom, float confidence)
        {
            var result = CharsetDetector.DetectFromBytes(content);

            result.Should().NotBeNull();
            result.Detected.Should().NotBeNull();
            result.Detected.EncodingName.Should().Be(encodingName);
            result.Detected.HasBOM.Should().Be(hasBom);
            result.Detected.Confidence.Should().BeApproximately(confidence, 0.001f);
        }

        /// <summary>
        /// Verifies that inconclusive input remains inconclusive.
        /// </summary>
        [Fact]
        public void Empty_and_binary_content_have_no_detected_encoding()
        {
            CharsetDetector.DetectFromBytes([]).Detected.Should().BeNull();
            CharsetDetector.DetectFromBytes(Enumerable.Range(0, 600).Select(i => (byte)(i % 256)).ToArray()).Detected.Should().BeNull();
        }

        private static byte[] Ascii(string value)
        {
            return Encoding.ASCII.GetBytes(value);
        }

        private static object[] Case(string file, byte[] content, string filenameMime, string contentMime, bool filenameIsText, bool contentIsText)
        {
            return [file, content, filenameMime, contentMime, filenameIsText, contentIsText];
        }

        private static byte[] DdsHeader()
        {
            var result = new byte[128];
            Ascii("DDS ").CopyTo(result, 0);
            return result;
        }

        private static byte[] TextWithLateNull()
        {
            var result = Enumerable.Repeat((byte)'A', 600).ToArray();
            result[^10] = 0x00;
            return result;
        }

        private static void AssertMime(string file, string expectedMime)
        {
            var matched = MimeTypeMap.TryGetMimeType(Path.GetFileName(file), out var actualMime);

            matched.Should().Be(expectedMime != null);
            if (matched)
            {
                actualMime.Should().Be(expectedMime);
            }
        }

        private static void AssertMime(Stream content, string file, string expectedMime)
        {
            var matched = MimeTypeMap.TryGetMimeType(content, Path.GetFileName(file), out var actualMime);

            matched.Should().Be(expectedMime != null);
            if (matched)
            {
                actualMime.Should().Be(expectedMime);
            }
        }
    }
}
