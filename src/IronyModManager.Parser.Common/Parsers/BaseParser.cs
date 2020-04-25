// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common.Parsers
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
// ***********************************************************************
// <copyright file="BaseParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using IronyModManager.DI;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Parsers.Models;
using IronyModManager.Shared;

namespace IronyModManager.Parser.Common.Parsers
{
    /// <summary>
    /// Class BaseParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.IDefaultParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.IDefaultParser" />
    public abstract class BaseParser : IDefaultParser
    {
        #region Fields

        /// <summary>
        /// The maximum lines
        /// </summary>
        protected const int MaxLines = 20000;

        /// <summary>
        /// The code parser
        /// </summary>
        protected readonly ICodeParser codeParser;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        public BaseParser(ICodeParser codeParser)
        {
            this.codeParser = codeParser;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the name of the parser.
        /// </summary>
        /// <value>The name of the parser.</value>
        public abstract string ParserName { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public abstract IEnumerable<IDefinition> Parse(ParserArgs args);

        /// <summary>
        /// Evals the definition identifier.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="defaultId">The default identifier.</param>
        /// <returns>System.String.</returns>
        protected virtual string EvalDefinitionId(IEnumerable<IScriptKeyValue> values, string defaultId)
        {
            if (values?.Count() > 0)
            {
                foreach (var item in values)
                {
                    string id = EvalKeyValueForId(item);
                    if (!string.IsNullOrWhiteSpace(id))
                    {
                        return id;
                    }
                }
            }
            return defaultId;
        }

        /// <summary>
        /// Evals for errors only.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IronyModManager.Parser.Common.Parsers.Models.IScriptError.</returns>
        protected virtual IEnumerable<IDefinition> EvalForErrorsOnly(ParserArgs args)
        {
            var value = codeParser.ParseScript(args.Lines, args.File);
            if (value.Error != null)
            {
                return new List<IDefinition>() { ParseScriptError(value.Error, args) };
            }
            return null;
        }

        /// <summary>
        /// Evals the key value for identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>System.String.</returns>
        protected virtual string EvalKeyValueForId(IScriptKeyValue value)
        {
            return string.Empty;
        }

        /// <summary>
        /// Formats the code.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parent">The parent.</param>
        /// <returns>System.String.</returns>
        protected virtual string FormatCode(string code, string parent)
        {
            if (string.IsNullOrWhiteSpace(parent))
            {
                return code;
            }
            var lines = code.SplitOnNewLine();
            var sb = new StringBuilder();
            sb.AppendLine($"{parent} = {{");
            foreach (var item in lines)
            {
                sb.AppendLine($"{new string(' ', 4)}{item}");
            }
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// Formats the type.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <returns>System.String.</returns>
        protected virtual string FormatType(string file, string typeOverride = Shared.Constants.EmptyParam)
        {
            var formatted = Path.GetDirectoryName(file);
            var type = Path.GetExtension(file).Trim('.');
            if (!Shared.Constants.TextExtensions.Any(s => s.EndsWith(type, StringComparison.OrdinalIgnoreCase)))
            {
                type = Constants.TxtType;
            }
            return $"{formatted}{Path.DirectorySeparatorChar}{(string.IsNullOrWhiteSpace(typeOverride) ? type : typeOverride)}";
        }

        /// <summary>
        /// Gets the definition instance.
        /// </summary>
        /// <returns>IDefinition.</returns>
        protected IDefinition GetDefinitionInstance()
        {
            return DIResolver.Get<IDefinition>();
        }

        /// <summary>
        /// Maps the definition from arguments.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="typeOverride">The type override.</param>
        protected virtual void MapDefinitionFromArgs(IDefinition definition, ParserArgs args, string typeOverride = Shared.Constants.EmptyParam)
        {
            definition.ContentSHA = args.ContentSHA;
            definition.Dependencies = args.ModDependencies;
            definition.ModName = args.ModName;
            definition.File = args.File;
            definition.Type = FormatType(args.File, typeOverride);
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseFirstLevel(ParserArgs args)
        {
            var result = new List<IDefinition>();
            var value = codeParser.ParseScript(args.Lines, args.File);
            if (value.Error != null)
            {
                result.Add(ParseScriptError(value.Error, args));
            }
            else
            {
                if (value.Value.Nodes?.Count() > 0)
                {
                    foreach (var item in value.Value.Nodes)
                    {
                        result.AddRange(ParseScriptKeyValues(item.KeyValues, args, parent: item.Key));
                        result.AddRange(ParseScriptNodes(item.Nodes, args, parent: item.Key));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Parses the root.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseRoot(ParserArgs args)
        {
            var result = new List<IDefinition>();
            var values = codeParser.ParseScript(args.Lines, args.File);
            if (values.Error != null)
            {
                result.Add(ParseScriptError(values.Error, args));
            }
            else
            {
                result.AddRange(ParseScriptKeyValues(values.Value.KeyValues, args));
                result.AddRange(ParseScriptNodes(values.Value.Nodes, args));
            }
            return result;
        }

        /// <summary>
        /// Parses the script error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition ParseScriptError(IScriptError error, ParserArgs args, string typeOverride = Shared.Constants.EmptyParam)
        {
            var definition = GetDefinitionInstance();
            definition.ErrorColumn = error.Column;
            definition.ErrorLine = error.Line;
            definition.ErrorMessage = error.Message;
            definition.Id = Constants.Scripts.Invalid;
            definition.ValueType = ValueType.Invalid;
            MapDefinitionFromArgs(definition, args);
            return definition;
        }

        /// <summary>
        /// Parses the script key values.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseScriptKeyValues(IEnumerable<IScriptKeyValue> keyValues, ParserArgs args, string parent = Shared.Constants.EmptyParam, string typeOverride = Shared.Constants.EmptyParam)
        {
            var result = new List<IDefinition>();
            if (keyValues?.Count() > 0)
            {
                foreach (var item in keyValues)
                {
                    var definition = GetDefinitionInstance();
                    MapDefinitionFromArgs(definition, args);
                    definition.Code = FormatCode(item.Code, parent);
                    if (item.Key.StartsWith(Constants.Scripts.Namespace, StringComparison.OrdinalIgnoreCase))
                    {
                        definition.Id = $"{Path.GetFileNameWithoutExtension(args.File)}-{item.Key}";
                        definition.ValueType = ValueType.Namespace;
                    }
                    else
                    {
                        definition.Id = item.Key;
                        definition.ValueType = ValueType.Variable;
                    }
                    result.Add(definition);
                }
            }
            return result;
        }

        /// <summary>
        /// Parses the script nodes.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="args">The arguments.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="typeOverride">The type override.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        protected virtual IEnumerable<IDefinition> ParseScriptNodes(IEnumerable<IScriptNode> nodes, ParserArgs args, string parent = Shared.Constants.EmptyParam, string typeOverride = Shared.Constants.EmptyParam)
        {
            var result = new List<IDefinition>();
            if (nodes?.Count() > 0)
            {
                foreach (var item in nodes)
                {
                    var definition = GetDefinitionInstance();
                    var sbLangs = new StringBuilder();
                    if (item.Nodes != null && item.Nodes.Any(s => s.Key.Equals(Constants.Scripts.LanguagesId, StringComparison.OrdinalIgnoreCase)))
                    {
                        var langNode = item.Nodes.FirstOrDefault(p => p.Key.Equals(Constants.Scripts.LanguagesId, StringComparison.OrdinalIgnoreCase));
                        if (langNode.Values?.Count > 0)
                        {
                            foreach (var value in langNode.Values.OrderBy(p => p.Value))
                            {
                                sbLangs.Append($"{value.Value}-");
                            }
                        }
                    }
                    string id = EvalDefinitionId(item.KeyValues, item.Key);
                    if (sbLangs.Length > 0)
                    {
                        id = $"{sbLangs}{id}";
                    }
                    MapDefinitionFromArgs(definition, args, typeOverride);
                    definition.Id = id;
                    definition.ValueType = ValueType.Object;
                    definition.Code = FormatCode(item.Code, parent);
                    result.Add(definition);
                }
            }
            return result;
        }

        #endregion Methods
    }
}
