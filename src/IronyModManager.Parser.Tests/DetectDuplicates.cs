// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2020
// ***********************************************************************
// <copyright file="DetectDuplicates.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Parser.Indexer;
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class DetectDuplicates.
    /// </summary>
    public class DetectDuplicates
    {
        #region Fields

        /// <summary>
        /// The stellaris root
        /// </summary>
        private readonly string stellarisRoot = @"D:\Games\Stellaris\";

        /// <summary>
        /// The writer
        /// </summary>
        private readonly ITestOutputHelper writer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DetectDuplicates" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public DetectDuplicates(ITestOutputHelper writer)
        {
            this.writer = writer;
        }

        #endregion Constructors

        /// <summary>
        /// Defines the test method Detect.
        /// </summary>
       #region Methods
#if !DETECT

        [Fact(Skip = "Test is for detection of parser issues.", Timeout = 300000)]
#else
        [Fact(Timeout = 300000)]
#endif
        public void StellarisDetect()
        {
            DISetup.SetupContainer();

            var parser = DIResolver.Get<IParserManager>();
            var files = Directory.EnumerateFiles(stellarisRoot, "*", SearchOption.AllDirectories).Where(s => IsValidExtension(s));
            var result = new List<IDefinition>();
            var undefined = new List<string>();
            foreach (var item in files)
            {
                var relativePath = item.Replace(stellarisRoot, string.Empty);
                if (!relativePath.Contains("\\") || relativePath.Contains("readme", StringComparison.OrdinalIgnoreCase) || relativePath.Contains("example", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                var content = File.ReadAllText(item);
                if (string.IsNullOrWhiteSpace(content))
                {
                    continue;
                }
                //if (relativePath.Contains(@"utl_icons"))
                //{
                //    Debugger.Break();
                //}
                //else
                //{
                //    continue;
                //}
                var lines = content.Contains("\r\n") ? content.Split("\r\n", StringSplitOptions.RemoveEmptyEntries) : content.Split("\n", StringSplitOptions.RemoveEmptyEntries);
                bool notEmpty = false;
                foreach (var line in lines)
                {
                    if (!line.StartsWith("#") && !string.IsNullOrWhiteSpace(line))
                    {
                        notEmpty = true;
                        break;
                    }
                }
                if (!notEmpty)
                {
                    continue;
                }
                var args = new ParserManagerArgs()
                {
                    File = relativePath,
                    Lines = lines,
                    ContentSHA = content.CalculateSHA(),
                    GameType = "Stellaris",
                    ModDependencies = new List<string>(),
                    ModName = "None"
                };
                try
                {
                    var parseResult = parser.Parse(args);
                    if (parseResult?.Count() > 0)
                    {
                        result.AddRange(parseResult);
                    }
                    else
                    {
                        undefined.Add(relativePath);
                    }
                }
                catch (Exception ex)
                {
                    undefined.Add(relativePath);
                }
            }
            var indexed = new IndexedDefinitions();
            indexed.InitMap(result);
            var typesKeys = indexed.GetAllTypeKeys();
            var objects = new List<string>();
            foreach (var item in typesKeys)
            {
                var types = indexed.GetByType(item);

                var grouped = types.GroupBy(p => p.ValueType);
                if (grouped.Count() == 0)
                {
                    undefined.Add(item);
                }
                if (grouped.Count(s => s.Key == ValueType.Object) > 0)
                {
                    var objTypes = grouped.Where(s => s.Key == ValueType.Object).SelectMany(p => p);
                    var idGroup = objTypes.GroupBy(s => s.Id);
                    if (idGroup.Count() != objTypes.Count())
                    {
                        var f = idGroup.Where(p => p.Count() > 1).SelectMany(s => s).ToList();
                        foreach (var fl in f)
                        {
                            if (!objects.Contains(fl.File))
                            {
                                objects.Add($"{fl.File}: {fl.Id}");
                            }
                        }
                    }
                }
            }
            writer.WriteLine($"{Environment.NewLine}-------------------{Environment.NewLine}Undefined{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, undefined.OrderBy(s => s))}");
            writer.WriteLine($"{Environment.NewLine}-------------------{Environment.NewLine}Objects{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, objects.OrderBy(s => s))}");
        }

        /// <summary>
        /// Defines the test method StellarisExtensions.
        /// </summary>
#if !DETECT

        [Fact(Skip = "Test is for detection of parser issues.", Timeout = 300000)]
#else
        [Fact(Timeout = 300000)]
#endif
        public void StellarisExtensions()
        {
            var exts = Directory.GetFiles(stellarisRoot, "*", SearchOption.AllDirectories).Where(s => s.Replace(stellarisRoot, string.Empty).Contains("\\")).GroupBy(s => Path.GetExtension(s)).Select(s => s.First());
            var allowedExtensions = new List<string>();
            foreach (var item in exts)
            {
                var mimeType = MimeMapping.MimeUtility.GetMimeMapping(item);
                if (mimeType.Contains("text", StringComparison.OrdinalIgnoreCase) || mimeType == MimeMapping.MimeUtility.UnknownMimeType)
                {
                    allowedExtensions.Add(Path.GetExtension(item));
                }
            }
            writer.WriteLine(string.Join(Environment.NewLine, allowedExtensions));
        }

        /// <summary>
        /// Determines whether [is valid extension] [the specified file].
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns><c>true</c> if [is valid extension] [the specified file]; otherwise, <c>false</c>.</returns>
        private bool IsValidExtension(string file)
        {
            return Constants.TextExtensions.Any(s => file.EndsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Methods
    }
}
