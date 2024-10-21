// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 10-17-2024
// ***********************************************************************
// <copyright file="KeyParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using Constants = IronyModManager.Parser.Common.Constants;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class KeyParser.
    /// Implements the <see cref="IronyModManager.Parser.Generic.BaseLineParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Generic.BaseLineParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    public class KeyParser : BaseLineParser, IGenericParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public KeyParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Generic" + nameof(KeyParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public virtual int Priority => 10;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return IsKeyType(args);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            return ParseRoot(args);
        }

        /// <summary>
        /// Evals the element for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected override string EvalElementForId(IScriptElement value)
        {
            if (Constants.Scripts.GenericKeys.Any(s => s.Equals(value.Key, StringComparison.OrdinalIgnoreCase)))
            {
                return value.Value;
            }

            return base.EvalElementForId(value);
        }

        /// <summary>
        /// Determines whether [is key type] [the specified arguments].
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if [is key type] [the specified arguments]; otherwise, <c>false</c>.</returns>
        protected virtual bool IsKeyType(CanParseArgs args)
        {
            int? openBrackets = null;
            var closeBrackets = 0;
            foreach (var line in args.Lines)
            {
                var cleaned = CodeParser.CleanWhitespace(line);
                if (!openBrackets.HasValue)
                {
                    if (cleaned.Contains(Constants.Scripts.DefinitionSeparatorId) || cleaned.EndsWith(Constants.Scripts.EqualsOperator))
                    {
                        openBrackets = line.Count(s => s == Constants.Scripts.OpenObject);
                        closeBrackets = line.Count(s => s == Constants.Scripts.CloseObject);
                        if (openBrackets - closeBrackets <= 1 && Constants.Scripts.GenericKeyIds.Any(s => !string.IsNullOrWhiteSpace(GetValue(line, s))))
                        {
                            var idLoc = -1;
                            foreach (var item in Constants.Scripts.GenericKeyIds)
                            {
                                idLoc = cleaned.IndexOf(item, StringComparison.OrdinalIgnoreCase);
                                if (idLoc > -1)
                                {
                                    break;
                                }
                            }

                            if (cleaned[..idLoc].Count(s => s == Constants.Scripts.OpenObject) == 1)
                            {
                                return true;
                            }
                        }

                        if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                        {
                            openBrackets = null;
                            closeBrackets = 0;
                        }
                    }
                }
                else
                {
                    openBrackets += line.Count(s => s == Constants.Scripts.OpenObject);
                    closeBrackets += line.Count(s => s == Constants.Scripts.CloseObject);
                    if (openBrackets - closeBrackets <= 1 && Constants.Scripts.GenericKeyIds.Any(s => !string.IsNullOrWhiteSpace(GetValue(line, s))))
                    {
                        var bracketLocation = cleaned.IndexOf(Constants.Scripts.OpenObject.ToString());
                        var idLoc = -1;
                        foreach (var item in Constants.Scripts.GenericKeyIds)
                        {
                            idLoc = cleaned.IndexOf(item, StringComparison.OrdinalIgnoreCase);
                            if (idLoc > -1)
                            {
                                break;
                            }
                        }

                        if (idLoc < bracketLocation || bracketLocation == -1)
                        {
                            return true;
                        }
                    }

                    if (openBrackets.GetValueOrDefault() > 0 && openBrackets == closeBrackets)
                    {
                        openBrackets = null;
                        closeBrackets = 0;
                    }
                }
            }

            return false;
        }

        #endregion Methods
    }
}
