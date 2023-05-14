// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 01-28-2022
//
// Last Modified By : Mario
// Last Modified On : 01-28-2022
// ***********************************************************************
// <copyright file="ValidationBase.cs" company="Mario">
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
using IronyModManager.Parser.Definitions;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using IronyModManager.Tests.Common;
using Xunit.Abstractions;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class ValidationBase.
    /// </summary>
    public abstract class ValidationBase
    {
        #region Fields

        /// <summary>
        /// The game type
        /// </summary>
        private readonly string gameType;

        /// <summary>
        /// The root path
        /// </summary>
        private readonly string rootPath;

        /// <summary>
        /// The writer
        /// </summary>
        private readonly ITestOutputHelper writer;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationBase"/> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="rootPath">The root path.</param>
        /// <param name="gameType">Type of the game.</param>
        public ValidationBase(ITestOutputHelper writer, string rootPath, string gameType)
        {
            this.writer = writer;
            this.rootPath = rootPath;
            this.gameType = gameType;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Extensionses this instance.
        /// </summary>
        public void Extensions()
        {
#pragma warning disable CA1847 // Use char literal for a single character lookup
            var exts = Directory.GetFiles(rootPath, "*", SearchOption.AllDirectories).Where(s => s.Replace(rootPath, string.Empty).Contains("\\")).GroupBy(s => Path.GetExtension(s)).Select(s => s.First());
#pragma warning restore CA1847 // Use char literal for a single character lookup
            var allowedExtensions = new List<string>();
            foreach (var item in exts)
            {
                var mimeType = MimeTypes.GetMimeType(item);
                if (mimeType.Contains("text", StringComparison.OrdinalIgnoreCase) || mimeType == MimeTypes.FallbackMimeType)
                {
                    allowedExtensions.Add(Path.GetExtension(item));
                }
            }
            writer.WriteLine(string.Join(Environment.NewLine, allowedExtensions));
        }

        /// <summary>
        /// Detects the duplicates and generate parser map.
        /// </summary>
        /// <exception cref="System.ArgumentException">Fatal error. Check parsers.</exception>
        /// <exception cref="System.ArgumentException">Fatal error. Check parsers.</exception>
        protected void DetectDuplicatesAndGenerateParserMap()
        {
            var codeParser = new CodeParser(new Logger());
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
                        if (parsed.Length > 0)
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
            var files = Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories).Where(s => IsValidExtension(s));
            var result = new List<IDefinition>();
            var undefined = new List<string>();
            var assetTypes = new HashSet<string>();
            foreach (var item in files)
            {
                var relativePath = item.Replace(rootPath, string.Empty);
                if (!relativePath.Contains('\\') || relativePath.Contains("readme", StringComparison.OrdinalIgnoreCase) || relativePath.Contains("example", StringComparison.OrdinalIgnoreCase) || relativePath.Contains("changelog", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                var content = File.ReadAllText(item);
                if (string.IsNullOrWhiteSpace(content))
                {
                    continue;
                }
                //if (relativePath.Contains(@"common\defines"))
                //{
                //    System.Diagnostics.Debugger.Break();
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
                    GameType = gameType,
                    ModDependencies = new List<string>(),
                    ModName = "None",
                    ValidationType = ValidationType.SkipAll
                };
                try
                {
                    var parseResult = parser.Parse(args);
                    if (parseResult?.Count() > 0)
                    {
                        result.AddRange(parseResult);
                        foreach (var p in parseResult)
                        {
                            if (p.File.EndsWith(".asset", StringComparison.OrdinalIgnoreCase) || p.File.EndsWith(".gui", StringComparison.OrdinalIgnoreCase) || p.File.EndsWith(".gfx", StringComparison.OrdinalIgnoreCase))
                            {
                                var id = getKey(p.Code.SplitOnNewLine().First(), "=");
                                if (!id.StartsWith("@"))
                                {
                                    assetTypes.Add(id.ToLowerInvariant());
                                }
                            }
                        }
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
            var singleObjects = new List<string>();
            var parserMap = new List<IParserMap>();

            var dirs = indexed.GetAllDirectoryKeys();

            foreach (var dir in dirs)
            {
                var all = indexed.GetByParentDirectory(dir);
                if (all.Count() <= 2 && !all.All(p => p.ValueType == ValueType.WholeTextFile))
                {
                    if (!singleObjects.Contains(all.First().File))
                    {
                        singleObjects.Add($"{all.First().File}");
                    }
                }
            }

            foreach (var item in typesKeys)
            {
                var all = indexed.GetByType(item);
                var types = all.Where(p => p.ValueType != ValueType.Invalid).ToHashSet();
                if (all.Any(p => p.ValueType == ValueType.Invalid))
                {
                    foreach (var def in all.Where(p => p.ValueType == ValueType.Invalid))
                    {
                        if (!invalid.Contains(def.File))
                        {
                            invalid.Add(def.File);
                        }
                    }
                }
                if (types.Count == 0 || types.All(p => p.ValueType == ValueType.EmptyFile))
                {
                    continue;
                }
                var filteredTypes = types.Where(p => p.ValueType != ValueType.EmptyFile);
                string usedParser = filteredTypes.First().UsedParser;
                var groupedParsers = filteredTypes.GroupBy(p => p.UsedParser);
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
                    else if (groupedParsers.All(p => p.First().UsedParser.Contains("WholeTextParser")))
                    {
                        usedParser = "GenericWholeTextParser";
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
                if (!grouped.Any())
                {
                    undefined.Add(item);
                }
                if (grouped.Any(s => s.Key == ValueType.Object))
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
            // Fuck you Microsoft for breaking test copy output!
            var sb = new StringBuilder();
            sb.AppendLine($"{Environment.NewLine}-------------------{Environment.NewLine}Undefined{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, undefined.OrderBy(s => s))}");
            sb.AppendLine($"{Environment.NewLine}-------------------{Environment.NewLine}Invalid{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, invalid.OrderBy(s => s))}");
            sb.AppendLine($"{Environment.NewLine}-------------------{Environment.NewLine}Single Objects{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, singleObjects.OrderBy(s => s))}");
            sb.AppendLine($"{Environment.NewLine}-------------------{Environment.NewLine}Objects{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, objects.OrderBy(s => s))}");
            sb.AppendLine($"{Environment.NewLine}-------------------{Environment.NewLine}Asset types{Environment.NewLine}-------------------{Environment.NewLine}{string.Join(Environment.NewLine, assetTypes.OrderBy(s => s))}");

            if (!Directory.Exists("..\\..\\..\\..\\IronyModManager\\Maps"))
            {
                Directory.CreateDirectory("..\\..\\..\\..\\IronyModManager\\Maps");
            }
            File.WriteAllText($"..\\..\\..\\..\\IronyModManager\\Maps\\{gameType}ParserMap.json.txt", sb.ToString());

            File.WriteAllText($"..\\..\\..\\..\\IronyModManager\\Maps\\{gameType}ParserMap.json", JsonDISerializer.Serialize(parserMap.OrderBy(p => p.DirectoryPath, StringComparer.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Determines whether [is valid extension] [the specified file].
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns><c>true</c> if [is valid extension] [the specified file]; otherwise, <c>false</c>.</returns>
        protected bool IsValidExtension(string file)
        {
            return Shared.Constants.TextExtensions.Any(s => file.EndsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Methods
    }
}
