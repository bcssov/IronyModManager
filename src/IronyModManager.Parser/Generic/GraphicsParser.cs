﻿// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 05-23-2025
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
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Generic.KeyParser" />
    public class GraphicsParser : KeyParser
    {
        #region Fields

        /// <summary>
        /// The GUI types identifier
        /// </summary>
        protected const string GuiTypesId = "guiTypes={";

        /// <summary>
        /// The asset ids
        /// </summary>
        protected static readonly string[] AssetIds = ["animation={", "category={", "entity={", "falloff={", "font={", "light={", "master_compressor={", "particle={", "music={", "sound={", "soundeffect={", "soundgroup={"];

        /// <summary>
        /// The expected graphics folders
        /// </summary>
        protected static readonly string[] ExpectedGraphicsFolders = ["gui", "gfx", "interface", "fonts", "dlc", "sound", "music", "pdx_launcher", "integrated_dlc"];

        /// <summary>
        /// The expected graphics ids
        /// </summary>
        protected static readonly string[] ExpectedGraphicsIds = [GuiTypesId, "spriteTypes={", "objectTypes={", "bitmapfonts={"];

        /// <summary>
        /// The valid extensions
        /// </summary>
        protected static readonly string[] ValidExtensions = [Common.Constants.GuiExtension, Common.Constants.GfxExtension, Common.Constants.AssetExtension];

        /// <summary>
        /// The object clone
        /// </summary>
        protected readonly IObjectClone ObjectClone;

        /// <summary>
        /// The is generic key type
        /// </summary>
        protected bool IsGenericKeyType;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GraphicsParser" /> class.
        /// </summary>
        /// <param name="objectClone">The object clone.</param>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public GraphicsParser(IObjectClone objectClone, ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
            ObjectClone = objectClone;
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
            return ValidExtensions.Any(a => args.File.EndsWith(a, StringComparison.OrdinalIgnoreCase)) || IsContentGraphics(args, false, out _, out _);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var canParseArgs = new CanParseArgs { File = args.File, GameType = args.GameType, IsBinary = args.IsBinary, Lines = args.Lines };
            IEnumerable<IDefinition> result;
            IsContentGraphics(canParseArgs, true, out var isAsset, out var type);
            if (isAsset)
            {
                IsGenericKeyType = IsKeyType(canParseArgs);
                result = base.ParseRoot(args);
            }
            else
            {
                result = ParseSecondLevel(args);
            }

            if (result.Any(p => p.ValueType == Shared.Models.ValueType.Invalid))
            {
                return result;
            }

            var parsedResult = new List<IDefinition>();
            var replaceFolder = Path.DirectorySeparatorChar + "replace";
            var parent = args.File.StandardizeDirectorySeparator().Split(Path.DirectorySeparatorChar)[0];
            var hasReplaceFolder = Path.GetDirectoryName(args.File)!.EndsWith(replaceFolder, StringComparison.OrdinalIgnoreCase);
            foreach (var definition in result.Where(p => p.ValueType != Shared.Models.ValueType.Namespace && p.ValueType != Shared.Models.ValueType.Variable))
            {
                definition.VirtualPath = hasReplaceFolder
                    ? Path.Combine(parent.Replace(replaceFolder, string.Empty), definition.OriginalId, Path.GetFileName(args.File)!)
                    : Path.Combine(parent, definition.OriginalId, Path.GetFileName(args.File)!);
                definition.Type = definition.VirtualPath.FormatDefinitionType(type);
                parsedResult.Add(definition);
                if (result.Any(p => p.ValueType is Shared.Models.ValueType.Variable or Shared.Models.ValueType.Namespace))
                {
                    foreach (var item in result.Where(p => p.ValueType is Shared.Models.ValueType.Variable or Shared.Models.ValueType.Namespace))
                    {
                        var copy = ObjectClone.CloneDefinition(item, true);
                        copy.VirtualPath = definition.VirtualPath;
                        copy.Type = definition.Type;
                        if (parsedResult.All(p => p.TypeAndId != copy.TypeAndId))
                        {
                            parsedResult.Add(copy);
                        }
                    }
                }
            }

            return parsedResult;
        }

        /// <summary>
        /// Evals the element for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected override string EvalElementForId(IScriptElement value)
        {
            if (IsGenericKeyType)
            {
                return base.EvalElementForId(value);
            }
            else
            {
                return Common.Constants.Scripts.GraphicsTypeName.Equals(value.Key, StringComparison.OrdinalIgnoreCase) ? value.Value : base.EvalElementForId(value);
            }
        }

        /// <summary>
        /// Evals for inlines.
        /// </summary>
        /// <param name="defaultId">The default identifier.</param>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        protected override bool EvalForInlines(string defaultId, string id)
        {
            // No inlines here
            return false;
        }

        /// <summary>
        /// Determines whether [is content graphics] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <param name="skipExtensionValidation">if set to <c>true</c> [skip extension validation].</param>
        /// <param name="isAsset">if set to <c>true</c> [is asset].</param>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if [is content graphics] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsContentGraphics(CanParseArgs args, bool skipExtensionValidation, out bool isAsset, out string type)
        {
            isAsset = false;
            type = string.Empty;
            if (!args.IsBinary)
            {
                var isValidExistingTextFile = Constants.TextExtensions.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase));
                if (!isValidExistingTextFile || skipExtensionValidation)
                {
                    var lines = CodeParser.CleanCode(args.File, args.Lines);
                    var parent = args.File.StandardizeDirectorySeparator().Split(Path.DirectorySeparatorChar)[0];
                    if (!string.IsNullOrWhiteSpace(parent) && ExpectedGraphicsFolders.Any(a => parent.Equals(a, StringComparison.OrdinalIgnoreCase)))
                    {
                        // Means a wise guy used .bak extension
                        var merged = string.Join(string.Empty, lines).ReplaceTabs().Replace(" ", string.Empty);
                        if (merged.Contains(Common.Constants.Scripts.OpenObject))
                        {
                            merged = merged[..(merged.IndexOf(Common.Constants.Scripts.OpenObject) + 1)];
                            if (ExpectedGraphicsIds.Any(a => merged.Contains(a, StringComparison.OrdinalIgnoreCase)))
                            {
                                type = merged.Contains(GuiTypesId, StringComparison.OrdinalIgnoreCase) ? Path.GetExtension(Common.Constants.GuiExtension).Trim('.') : Path.GetExtension(Common.Constants.GfxExtension).Trim('.');
                                return true;
                            }
                            else if (AssetIds.Any(a => merged.Contains(a, StringComparison.OrdinalIgnoreCase)))
                            {
                                type = Path.GetExtension(Common.Constants.AssetExtension).Trim('.');
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
