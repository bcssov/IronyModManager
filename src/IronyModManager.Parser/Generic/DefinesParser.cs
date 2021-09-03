// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-21-2020
//
// Last Modified By : Mario
// Last Modified On : 09-03-2021
// ***********************************************************************
// <copyright file="DefinesParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Parser.Generic
{
    /// <summary>
    /// Class DefinesParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.BaseParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IGenericParser" />
    public class DefinesParser : BaseParser, IGenericParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DefinesParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        /// <param name="logger">The logger.</param>
        public DefinesParser(ICodeParser codeParser, ILogger logger) : base(codeParser, logger)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public override string ParserName => "Generic" + nameof(DefinesParser);

        /// <summary>
        /// Gets the priority.
        /// </summary>
        /// <value>The priority.</value>
        public int Priority => 1;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public override bool CanParse(CanParseArgs args)
        {
            return args.File.StartsWith(Common.Constants.DefinesPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public override IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var data = TryParse(args, false);
            if (data.Error != null)
            {
                return new List<IDefinition>() { TranslateScriptError(data.Error, args) };
            }
            var result = new List<IDefinition>();
            if (data.Values?.Count() > 0)
            {
                foreach (var dataItem in data.Values)
                {
                    if (dataItem.Values != null)
                    {
                        foreach (var item in dataItem.Values)
                        {
                            var definition = GetDefinitionInstance();
                            string id = EvalDefinitionId(item.Values, item.Key);
                            MapDefinitionFromArgs(ConstructArgs(args, definition, typeOverride: $"{dataItem.Key}-{Common.Constants.TxtType}"));
                            definition.Id = TrimId(id);
                            definition.ValueType = ValueType.SpecialVariable;
                            definition.Code = FormatCode(item, dataItem.Key);
                            definition.OriginalCode = FormatCode(item, skipVariables: true);
                            definition.CodeSeparator = Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                            definition.CodeTag = dataItem.Key;
                            var tags = ParseScriptTags(item.Values, item.Key);
                            if (tags.Any())
                            {
                                foreach (var tag in tags)
                                {
                                    var lower = tag.ToLowerInvariant();
                                    if (!definition.Tags.Contains(lower))
                                    {
                                        definition.Tags.Add(lower);
                                    }
                                }
                            }
                            result.Add(definition);
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(dataItem.Key) && !string.IsNullOrWhiteSpace(dataItem.Operator) && dataItem.Key.Contains("."))
                    {
                        //Dot notation is used
                        var definition = GetDefinitionInstance();
                        var id = dataItem.Key.Substring(dataItem.Key.LastIndexOf(".") + 1, dataItem.Key.Length - dataItem.Key.LastIndexOf(".") - 1);
                        var type = dataItem.Key.Substring(0, dataItem.Key.LastIndexOf("."));
                        MapDefinitionFromArgs(ConstructArgs(args, definition, typeOverride: $"{type}-{Common.Constants.TxtType}"));
                        definition.Id = TrimId(id);
                        definition.ValueType = ValueType.SpecialVariable;
                        definition.Code = FormatCode(dataItem);
                        definition.OriginalCode = FormatCode(dataItem, skipVariables: true);
                        definition.CodeSeparator = Constants.CodeSeparators.ClosingSeparators.CurlyBracket;
                        definition.CodeTag = dataItem.Key;
                        var tags = ParseScriptTags(new List<IScriptElement>() { dataItem }, id);
                        if (tags.Any())
                        {
                            foreach (var tag in tags)
                            {
                                var lower = tag.ToLowerInvariant();
                                if (!definition.Tags.Contains(lower))
                                {
                                    definition.Tags.Add(lower);
                                }
                            }
                        }
                        result.Add(definition);
                    }
                    else
                    {
                        // No operator detected means something is wrong in the mod file
                        if (string.IsNullOrWhiteSpace(dataItem.Operator))
                        {
                            var definesError = DIResolver.Get<IScriptError>();
                            definesError.Message = $"There appears to be a syntax error detected in: {args.File}";
                            return new List<IDefinition>() { TranslateScriptError(definesError, args) };
                        }
                    }
                }
            }
            return result;
        }

        #endregion Methods
    }
}
