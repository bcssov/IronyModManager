// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
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
        /// The writer
        /// </summary>
        private readonly ITestOutputHelper writer;

        /// <summary>
        /// The stellaris root
        /// </summary>
        private string stellarisRoot = @"D:\Games\Stellaris\";

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

        #region Methods

        /// <summary>
        /// Defines the test method Detect.
        /// </summary>
        [Fact(Skip = "Test is for debuging", Timeout = 300000)]
        //[Fact(Timeout = 300000)]
        public void StellarisDetect()
        {
            DISetup.SetupContainer();

            // backkup path
            var parser = new DefaultParser();
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
                var content = File.ReadAllLines(item);
                var args = new ParserArgs()
                {
                    File = relativePath,
                    Lines = content
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
            var variables = new List<string>();
            var namespaces = new List<string>();
            var objects = new List<string>();
            foreach (var item in typesKeys)
            {
                var types = indexed.GetByType(item);
                // find only variables
                var grouped = types.GroupBy(p => p.ValueType);
                if (grouped.Count() == 0)
                {
                    undefined.Add(item);
                }
                if (grouped.Count(s => s.Key == ValueType.Variable) == 1 && grouped.Count() == 1)
                {
                    variables.Add(item);
                }
                if (grouped.Count(s => s.Key == ValueType.Namespace) == 1 && grouped.Count() == 1)
                {
                    namespaces.Add(item);
                }
                if (grouped.Count(s => s.Key == ValueType.Object) > 0)
                {
                    var objTypes = grouped.Where(s => s.Key == ValueType.Object).SelectMany(p => p);
                    var idGroup = objTypes.GroupBy(s => s.Id);
                    if (idGroup.Count() != objTypes.Count())
                    {
                        objects.Add(item);
                    }
                }
            }
            writer.WriteLine($"{Environment.NewLine}-------------------{Environment.NewLine}Undefined{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, undefined.OrderBy(s => s))}");
            writer.WriteLine($"{Environment.NewLine}-------------------{Environment.NewLine}Variables{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, variables.OrderBy(s => s))}");
            writer.WriteLine($"{Environment.NewLine}-------------------{Environment.NewLine}Namepsaces{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, namespaces.OrderBy(s => s))}");
            writer.WriteLine($"{Environment.NewLine}-------------------{Environment.NewLine}Objects{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, objects.OrderBy(s => s))}");
        }

        /// <summary>
        /// Defines the test method StellarisExtensions.
        /// </summary>
        [Fact(Skip = "Test is for debuging", Timeout = 300000)]
        //[Fact(Timeout = 300000)]
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
