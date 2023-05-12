// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 05-11-2023
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
    /// Implements the <see cref="IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Generic.KeyParser" />
    /// <seealso cref="IGenericParser" />
    public class GraphicsParser : KeyParser, IGenericParser
    {
        #region Fields

        /// <summary>
        /// The asset identifier
        /// </summary>
        protected const string AssetId = "entity={";

        /// <summary>
        /// The expected graphics folders
        /// </summary>
        protected static readonly string[] expectedGraphicsFolders = new string[] { "gfx", "interface" };

        /// <summary>
        /// The expected graphics ids
        /// </summary>
        protected static readonly string[] expectedGraphicsIds = new string[] { "guiTypes={", "spriteTypes={", "objectTypes={", AssetId };

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
            return validExtensions.Any(a => args.File.EndsWith(a, StringComparison.OrdinalIgnoreCase)) || IsContentGraphics(args, out var _);
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
            if (args.File.EndsWith(Common.Constants.AssetExtension, StringComparison.OrdinalIgnoreCase))
            {
                isGenericKeyType = IsKeyType(canParseArgs);
                result = base.ParseRoot(args);
            }
            else if (args.File.EndsWith(Common.Constants.GfxExtension, StringComparison.OrdinalIgnoreCase) || args.File.EndsWith(Common.Constants.GuiExtension, StringComparison.OrdinalIgnoreCase))
            {
                result = ParseSecondLevel(args);
            }
            else
            {
                IsContentGraphics(canParseArgs, out var isAsset);
                if (isAsset)
                {
                    isGenericKeyType = IsKeyType(canParseArgs);
                    result = base.ParseRoot(args);
                }
                else
                {
                    result = ParseSecondLevel(args);
                }
            }
            // Flatten structure
            var replaceFolder = Path.DirectorySeparatorChar + "replace";
            var parent = args.File.StandardizeDirectorySeparator().Split(Path.DirectorySeparatorChar)[0];
            if (Path.GetDirectoryName(args.File).EndsWith(replaceFolder, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var item in result)
                {
                    item.VirtualPath = Path.Combine(parent.Replace(replaceFolder, string.Empty), Path.GetExtension(args.File).Trim("."), Path.GetFileName(args.File));
                    item.Type = item.VirtualPath.FormatDefinitionType();
                }
            }
            else
            {
                foreach (var definition in result)
                {
                    definition.VirtualPath = Path.Combine(parent, Path.GetExtension(args.File).Trim("."), Path.GetFileName(args.File));
                    definition.Type = definition.VirtualPath.FormatDefinitionType();
                }
            }
            return result;
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
        /// <param name="isAsset">if set to <c>true</c> [is asset].</param>
        /// <returns><c>true</c> if [is content graphics] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsContentGraphics(CanParseArgs args, out bool isAsset)
        {
            isAsset = false;
            if (!args.IsBinary)
            {
                var isValidExistingTextFile = Constants.TextExtensions.Any(s => args.File.EndsWith(s, StringComparison.OrdinalIgnoreCase));
                if (!isValidExistingTextFile)
                {
                    var parent = args.File.StandardizeDirectorySeparator().Split(Path.DirectorySeparatorChar)[0];
                    if (!string.IsNullOrWhiteSpace(parent) && expectedGraphicsFolders.Any(a => parent.Equals(a, StringComparison.OrdinalIgnoreCase)))
                    {
                        // Means a wise guy used .bak extension
                        var merged = string.Join(string.Empty, args.Lines).ReplaceTabs().Replace(" ", string.Empty);
                        if (merged.Contains(Common.Constants.Scripts.OpenObject))
                        {
                            merged = merged[..merged.IndexOf(Common.Constants.Scripts.OpenObject)];
                            if (expectedGraphicsIds.Any(a => merged.Contains(a, StringComparison.OrdinalIgnoreCase)))
                            {
                                isAsset = merged.Contains(AssetId, StringComparison.OrdinalIgnoreCase);
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
