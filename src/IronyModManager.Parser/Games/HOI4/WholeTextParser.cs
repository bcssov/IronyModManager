// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-12-2025
// ***********************************************************************
// <copyright file="WholeTextParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Games.HOI4
{
    /// <summary>
    /// Class WholeTextParser.
    /// Implements the <see cref="IronyModManager.Parser.Generic.WholeTextParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Generic.WholeTextParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGameParser" />
    public class WholeTextParser : Generic.WholeTextParser, IGameParser
    {
        #region Fields

        /// <summary>
        /// The equals check
        /// </summary>
        private static readonly string[] equalsChecks = [Common.Constants.HOI4.GraphicalCultureType];

        /// <summary>
        /// The starts with checks
        /// </summary>
        private static readonly string[] startsWithChecks =
        [
            Common.Constants.HOI4.Countries, Common.Constants.HOI4.Ideas, Common.Constants.HOI4.AIStrategyPlanes, Common.Constants.HOI4.AIStrategy, Common.Constants.HOI4.IntelligenceAgencies, Common.Constants.HOI4.ScriptedGui,
            Common.Constants.HOI4.Units, Common.Constants.HOI4.History, Common.Constants.HOI4.Generation, Common.Constants.HOI4.IdeaTags, Common.Constants.HOI4.Terrain, Common.Constants.HOI4.ProfilePictures,
            Common.Constants.HOI4.PeaceConference, Common.Constants.HOI4.ProfileBackgrounds, Common.Constants.HOI4.MIO, Common.Constants.HOI4.FocusInlayWindows, Common.Constants.HOI4.Raids,
            Common.Constants.HOI4.SpecialProjects, Common.Constants.HOI4.Portraits
        ];

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WholeTextParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        /// <seealso cref="T:IronyModManager.Parser.Common.Parsers.IDefaultParser" />
        /// <remarks>Initializes a new instance of the <see cref="T:IronyModManager.Parser.Common.Parsers.BaseParser" /> class.</remarks>
        public WholeTextParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "HOI4" + nameof(WholeTextParser);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return args.IsHOI4() && IsValidType(args);
        }

        /// <summary>
        /// Determines whether this instance [can parse equals] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse equals] the specified arguments; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseEquals(CanParseArgs args)
        {
            return equalsChecks.Any(s => args.File.Equals(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether this instance [can parse map CSV file] the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns><c>true</c> if this instance [can parse map CSV file] the specified file; otherwise, <c>false</c>.</returns>
        protected virtual bool CanParseMapCsvFile(string file)
        {
            return file.StartsWith(Common.Constants.HOI4.Map) && file.EndsWith(Common.Constants.TxtExtension);
        }

        /// <summary>
        /// Determines whether this instance [can parse starts with] the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance [can parse starts with] the specified arguments; otherwise, <c>false</c>.</returns>
        protected override bool CanParseStartsWith(CanParseArgs args)
        {
            return startsWithChecks.Any(s => args.File.StartsWith(s, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets the file tag code.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>System.String.</returns>
        protected override string GetFileTagCode(string file, IEnumerable<string> lines)
        {
            var cleaned = CodeParser.CleanCode(file, lines);
            return base.GetFileTagCode(file, cleaned);
        }

        /// <summary>
        /// Determines whether [is any text file] [the specified file].
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="directory">The directory.</param>
        /// <returns><c>true</c> if [is any text file] [the specified file]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsAnyTxtFile(string file, string directory)
        {
            return IsTxtFile(file, directory) || (file.StartsWith(directory + System.IO.Path.DirectorySeparatorChar) && file.EndsWith(Common.Constants.LuaExtension, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is file name tag] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is file name tag] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected override bool IsFileNameTag(ParserArgs args)
        {
            return args.File.Equals(Common.Constants.HOI4.GraphicalCultureType, StringComparison.OrdinalIgnoreCase) ||
                   CanParseMapCsvFile(args.File) ||
                   args.File.StartsWith(Common.Constants.HOI4.Countries) ||
                   args.File.EndsWith(Common.Constants.LuaExtension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether [is text file] [the specified file].
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="directory">The directory.</param>
        /// <returns><c>true</c> if [is text file] [the specified file]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsTxtFile(string file, string directory)
        {
            return file.StartsWith(directory + System.IO.Path.DirectorySeparatorChar) && file.EndsWith(Common.Constants.TxtExtension, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether [is valid type] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is valid type] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected override bool IsValidType(CanParseArgs args)
        {
            return CanParseEquals(args) || CanParseMapCsvFile(args.File) ||
                   CanParseStartsWith(args) ||
                   IsTxtFile(args.File, Common.Constants.HOI4.GFX) || IsTxtFile(args.File, Common.Constants.HOI4.Music) ||
                   IsAnyTxtFile(args.File, Common.Constants.HOI4.Script) || IsAnyTxtFile(args.File, Common.Constants.HOI4.Tests) || IsAnyTxtFile(args.File, Common.Constants.HOI4.Tutorial);
        }

        #endregion Methods
    }
}
