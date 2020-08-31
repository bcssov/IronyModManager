// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 08-31-2020
// ***********************************************************************
// <copyright file="StellarisValidation.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using IronyModManager.DI;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Definitions;
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class DetectDuplicates.
    /// </summary>
    public class StellarisValidation
    {
        #region Fields

        /// <summary>
        /// The stellaris root
        /// </summary>
        private readonly string stellarisRoot = @"D:\Games\Steam\steamapps\common\Stellaris\";

        /// <summary>
        /// The writer
        /// </summary>
        private readonly ITestOutputHelper writer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StellarisValidation" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public StellarisValidation(ITestOutputHelper writer)
        {
            this.writer = writer;
        }

        #endregion Constructors

        #region Methods
        /// <summary>
        /// Defines the test method Detect.
        /// </summary>
        /// <exception cref="ArgumentException">Fatal error. Check parsers.</exception>
        /// <exception cref="ArgumentException">Fatal error. Check parsers.</exception>

#if !FUNCTIONAL_TEST

        [Fact(Skip = "Test is for detection of parser issues.", Timeout = 300000)]
#else

        [Fact(Timeout = 300000)]
#endif
        public void StellarisDetectDuplicatesAndGenerateParserMap()
        {
            var codeParser = new CodeParser();
            var quotesRegex = new Regex("\".*?\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            static string cleanParsedText(string text)
            {
                var sb = new StringBuilder();
                foreach (var item in text)
                {
                    if (!char.IsWhiteSpace(item) &&
                        !item.Equals(Common.Constants.Scripts.OpenObject) &&
                        !item.Equals(Common.Constants.Scripts.CloseObject))
                    {
                        sb.Append(item);
                    }
                    else
                    {
                        break;
                    }
                }
                return sb.ToString();
            }
            string getKey(string line, string key)
            {
                var cleaned = codeParser.CleanWhitespace(line);
                if (cleaned.Contains(key, StringComparison.OrdinalIgnoreCase))
                {
                    var prev = cleaned.IndexOf(key, StringComparison.OrdinalIgnoreCase);
                    if (prev == 0 || !char.IsWhiteSpace(cleaned[prev - 1]))
                    {
                        var parsed = cleaned.Split(key, StringSplitOptions.RemoveEmptyEntries);
                        if (parsed.Count() > 0)
                        {
                            if (parsed.First().StartsWith("\""))
                            {
                                return quotesRegex.Match(parsed.First().Trim()).Value.Replace("\"", string.Empty);
                            }
                            else
                            {
                                return cleanParsedText(parsed.First().Trim().Replace("\"", string.Empty));
                            }
                        }
                    }
                }
                return string.Empty;
            }
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
                //if (relativePath.Contains(@"gfx\worldgfx\star_ed_class.txt"))
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
                    if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
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
#pragma warning disable CS0168 // Variable is declared but never used
                catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
                {
                    undefined.Add(relativePath);
                }
            }

            var invalid = new HashSet<string>();
            var indexed = new IndexedDefinitions();
            indexed.InitMap(result);
            var typesKeys = indexed.GetAllTypeKeys();
            var objects = new List<string>();
            var parserMap = new List<IParserMap>();

            foreach (var item in typesKeys)
            {
                var all = indexed.GetByType(item);
                var types = all.Where(p => p.ValueType != Common.ValueType.Invalid).ToHashSet();
                if (all.Any(p => p.ValueType == Common.ValueType.Invalid))
                {
                    foreach (var def in all.Where(p => p.ValueType == Common.ValueType.Invalid))
                    {
                        if (!invalid.Contains(def.File))
                        {
                            invalid.Add(def.File);
                        }
                    }
                }
                if (types.Count == 0)
                {
                    continue;
                }
                string usedParser = types.First().UsedParser;
                var groupedParsers = types.GroupBy(p => p.UsedParser);
                if (groupedParsers.Count() > 1)
                {
                    // Decide based on count which parser is the right one
                    var ordered = groupedParsers.OrderByDescending(p => p.Count());
                    if (groupedParsers.All(p => p.First().UsedParser.Equals("GenericKeyParser") || p.First().UsedParser.Equals("DefaultParser")))
                    {
                        var objectIds = new List<string>();
                        foreach (var type in types)
                        {
                            var id = getKey(type.Code.SplitOnNewLine().First(), "=");
                            objectIds.Add(id);
                        }
                        if (objectIds.GroupBy(p => p).Count() == objectIds.Count)
                        {
                            usedParser = "DefaultParser";
                        }
                        else
                        {
                            usedParser = ordered.First().First().UsedParser;
                        }
                    }
                    else
                    {
                        throw new ArgumentException("Fatal error. Check parsers.");
                    }
                }
                else if (groupedParsers.Count() > 2)
                {
                    throw new ArgumentException("Fatal error. Check parsers.");
                }
                else if (usedParser == "GenericKeyParser")
                {
                    var objectIds = new List<string>();
                    foreach (var type in types)
                    {
                        var reserved = new string[] { "entity", "animation", "music" };
                        var id = getKey(type.Code.SplitOnNewLine().First(), "=");
                        objectIds.Add(reserved.Any(p => p.Equals(id, StringComparison.OrdinalIgnoreCase)) ? "reserved" : id);
                    }
                    if (objectIds.GroupBy(p => p).Count() == objectIds.Count)
                    {
                        usedParser = "DefaultParser";
                    }
                    else
                    {
                        usedParser = types.First().UsedParser;
                    }
                }
                var path = Path.GetDirectoryName(types.First().File);
                if (!parserMap.Any(p => p.DirectoryPath.Equals(path) && p.PreferredParser.Equals(usedParser)))
                {
                    parserMap.Add(new ParserMap()
                    {
                        DirectoryPath = path,
                        PreferredParser = usedParser
                    });
                }

                var grouped = types.GroupBy(p => p.ValueType);
                if (grouped.Count() == 0)
                {
                    undefined.Add(item);
                }
                if (grouped.Count(s => s.Key == Common.ValueType.Object) > 0)
                {
                    var objTypes = grouped.Where(s => s.Key == Common.ValueType.Object).SelectMany(p => p);
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
            // Fuck you Microsoft for breaking test copy output!
            var sb = new StringBuilder();
            sb.AppendLine($"{Environment.NewLine}-------------------{Environment.NewLine}Undefined{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, undefined.OrderBy(s => s))}");
            sb.AppendLine($"{Environment.NewLine}-------------------{Environment.NewLine}Invalid{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, invalid.OrderBy(s => s))}");
            sb.AppendLine($"{Environment.NewLine}-------------------{Environment.NewLine}Objects{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, objects.OrderBy(s => s))}");

            File.WriteAllText("..\\..\\..\\..\\IronyModManager\\Maps\\StellarisParserMap.json.txt", sb.ToString());

            File.WriteAllText("..\\..\\..\\..\\IronyModManager\\Maps\\StellarisParserMap.json", JsonDISerializer.Serialize(parserMap));
        }

        /// <summary>
        /// Defines the test method StellarisExtensions.
        /// </summary>
#if !FUNCTIONAL_TEST

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
            return Shared.Constants.TextExtensions.Any(s => file.EndsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Methods
    }
}
