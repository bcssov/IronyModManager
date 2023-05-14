// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 05-14-2023
// ***********************************************************************
// <copyright file="GraphicsParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class GraphicsParser.
    /// Implements the <see cref="IronyModManager.Parser.Generic.KeyParser" />
    /// Implements the <see cref="IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Generic.KeyParser" />
    /// <seealso cref="IGenericParser" />
    public class GraphicsParser : KeyParser, IGenericParser
    {
        #region Fields

        /// <summary>
        /// The asset ids
        /// </summary>
        protected static readonly string[] assetIds = new string[] { "animation={", "category={", "entity={", "falloff={", "font={", "light={", "master_compressor={", "particle={", "music={", "sound={", "soundeffect={", "soundgroup={" };

        /// <summary>
        /// The expected graphics folders
        /// </summary>
        protected static readonly string[] expectedGraphicsFolders = new string[] { "gui", "gfx", "interface", "fonts", "dlc", "sound", "music", "pdx_launcher" };

        /// <summary>
        /// The expected graphics ids
        /// </summary>
        protected static readonly string[] expectedGraphicsIds = new string[] { "guiTypes={", "spriteTypes={", "objectTypes={", "bitmapfonts={" };

        /// <summary>
        /// The valid extensions
        /// </summary>
        protected static readonly string[] validExtensions = new string[] { Common.Constants.GuiExtension, Common.Constants.GfxExtension, Common.Constants.AssetExtension };

        /// <summary>
        /// The is generic key type
        /// </summary>
        private bool isGenericKeyType = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public GraphicsParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Generic" + nameof(GraphicsParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public override int Priority => 2;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return validExtensions.Any(a => args.File.EndsWith(a, StringComparison.OrdinalIgnoreCase)) || IsContentGraphics(args, false, out var _);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var canParseArgs = new CanParseArgs()
            {
                File = args.File,
                GameType = args.GameType,
                IsBinary = args.IsBinary,
                Lines = args.Lines
            };
            IEnumerable<IDefinition> result;
            IsContentGraphics(canParseArgs, true, out var isAsset);
            if (isAsset)
            {
                isGenericKeyType = IsKeyType(canParseArgs);
                result = base.ParseRoot(args);
            }
            else
            {
                result = ParseSecondLevel(args);
            }
            // Flatten structure
            var parsedResult = new List<IDefinition>();
            var replaceFolder = Path.DirectorySeparatorChar + "replace";
            var parent = args.File.StandardizeDirectorySeparator().Split(Path.DirectorySeparatorChar)[0];
            bool hasReplaceFolder = Path.GetDirectoryName(args.File).EndsWith(replaceFolder, StringComparison.OrdinalIgnoreCase);
            foreach (var definition in result.Where(p => p.ValueType != Shared.Models.ValueType.Namespace && p.ValueType != Shared.Models.ValueType.Variable))
            {                
                if (hasReplaceFolder)
                {
                    definition.VirtualPath = Path.Combine(parent.Replace(replaceFolder, string.Empty), definition.OriginalId, Path.GetFileName(args.File));
                }
                else
                {
                    definition.VirtualPath = Path.Combine(parent, definition.OriginalId, Path.GetFileName(args.File));
                }
                definition.Type = definition.VirtualPath.FormatDefinitionType();
                parsedResult.Add(definition);
                if (result.Any(p => p.ValueType == Shared.Models.ValueType.Variable || p.ValueType == Shared.Models.ValueType.Namespace))
                {
                    foreach (var item in result.Where(p => p.ValueType == Shared.Models.ValueType.Variable || p.ValueType == Shared.Models.ValueType.Namespace))
                    {
                        var copy = CopyDefinition(item);
                        copy.VirtualPath = definition.VirtualPath;
                        copy.Type = definition.Type;
                        if (!parsedResult.Any(p => p.TypeAndId == copy.TypeAndId))
                        {
                            parsedResult.Add(copy);
                        }
                    }
                }
            }
            return parsedResult;
        }

        /// <summary>
        /// Copies the definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition CopyDefinition(IDefinition definition)
        {
            var newDefinition = DIResolver.Get<IDefinition>();
            newDefinition.Code = definition.Code;
            newDefinition.ContentSHA = definition.ContentSHA;
            newDefinition.DefinitionSHA = definition.DefinitionSHA;
            newDefinition.Dependencies = definition.Dependencies;
            newDefinition.ErrorColumn = definition.ErrorColumn;
            newDefinition.ErrorLine = definition.ErrorLine;
            newDefinition.ErrorMessage = definition.ErrorMessage;
            newDefinition.File = definition.File;
            newDefinition.GeneratedFileNames = definition.GeneratedFileNames;
            newDefinition.OverwrittenFileNames = definition.OverwrittenFileNames;
            newDefinition.AdditionalFileNames = definition.AdditionalFileNames;
            newDefinition.Id = definition.Id;
            newDefinition.ModName = definition.ModName;
            newDefinition.Type = definition.Type;
            newDefinition.UsedParser = definition.UsedParser;
            newDefinition.ValueType = definition.ValueType;
            newDefinition.Tags = definition.Tags;
            newDefinition.OriginalCode = definition.OriginalCode;
            newDefinition.CodeSeparator = definition.CodeSeparator;
            newDefinition.CodeTag = definition.CodeTag;
            newDefinition.Order = definition.Order;
            newDefinition.OriginalModName = definition.OriginalModName;
            newDefinition.OriginalFileName = definition.OriginalFileName;
            newDefinition.DiskFile = definition.DiskFile;
            newDefinition.Variables = definition.Variables;
            newDefinition.ExistsInLastFile = definition.ExistsInLastFile;
            newDefinition.VirtualPath = definition.VirtualPath;
            newDefinition.CustomPriorityOrder = definition.CustomPriorityOrder;
            newDefinition.IsCustomPatch = definition.IsCustomPatch;
            newDefinition.IsFromGame = definition.IsFromGame;
            newDefinition.AllowDuplicate = definition.AllowDuplicate;
            newDefinition.ResetType = definition.ResetType;
            newDefinition.FileNameSuffix = definition.FileNameSuffix;
            newDefinition.IsPlaceholder = definition.IsPlaceholder;
            newDefinition.LastModified = definition.LastModified;
            newDefinition.OriginalId = definition.OriginalId;
            return newDefinition;
        }

        /// <summary>
        /// Evals the element for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected override string EvalElementForId(IScriptElement value)
        {
            if (isGenericKeyType)
            {
                return base.EvalElementForId(value);
            }
            else
            {
                if (Common.Constants.Scripts.GraphicsTypeName.Equals(value.Key, StringComparison.OrdinalIgnoreCase))
                {
                    return value.Value;
                }
                return base.EvalElementForId(value);
            }
        }

        /// <summary>
        /// Determines whether [is content graphics] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="skipExtensionValidation">if set to <c>true</c> [skip extension validation].</param>
        /// <param name="isAsset">if set to <c>true</c> [is asset].</param>
        /// <returns><c>true</c> if [is content graphics] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsContentGraphics(CanParseArgs args, bool skipExtensionValidation, out bool isAsset)
        {
            isAsset = false;
            if (!args.IsBinary)
            {
                var isValidExistingTextFile = Constants.TextExtensions.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase));
                if (!isValidExistingTextFile || skipExtensionValidation)
                {
                    var lines = codeParser.CleanCode(args.File, args.Lines);
                    var parent = args.File.StandardizeDirectorySeparator().Split(Path.DirectorySeparatorChar)[0];
                    if (!string.IsNullOrWhiteSpace(parent) && expectedGraphicsFolders.Any(a => parent.Equals(a, StringComparison.OrdinalIgnoreCase)))
                    {
                        // Means a wise guy used .bak extension
                        var merged = string.Join(string.Empty, lines).ReplaceTabs().Replace(" ", string.Empty);
                        if (merged.Contains(Common.Constants.Scripts.OpenObject))
                        {
                            merged = merged[..(merged.IndexOf(Common.Constants.Scripts.OpenObject) + 1)];
                            if (expectedGraphicsIds.Any(a => merged.Contains(a, StringComparison.OrdinalIgnoreCase)))
                            {
                                return true;
                            }
                            else if (assetIds.Any(a => merged.Contains(a, StringComparison.OrdinalIgnoreCase)))
                            {
                                isAsset = true;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        #endregion Methods
    }
}
