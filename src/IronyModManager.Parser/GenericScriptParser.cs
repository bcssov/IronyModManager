// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-16-2020
// ***********************************************************************
// <copyright file="GenericScriptParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronyModManager.DI;

namespace IronyModManager.Parser
{
    /// <summary>
    /// Class GenericScriptParser.
    /// Implements the <see cref="IronyModManager.Parser.IDefaultParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.IDefaultParser" />
    public class GenericScriptParser : IDefaultParser
    {
        #region Properties

        /// <summary>
        /// Gets the type of the parser.
        /// </summary>
        /// <value>The type of the parser.</value>
        public virtual string ParserType => Constants.GenericParserFlag;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether this instance can parse the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can parse the specified arguments; otherwise, <c>false</c>.</returns>
        public virtual bool CanParse(CanParseArgs args)
        {
            // Default parser needs to be invoked manually
            return false;
        }

        /// <summary>
        /// Parses the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>IEnumerable&lt;IDefinition&gt;.</returns>
        public virtual IEnumerable<IDefinition> Parse(ParserArgs args)
        {
            var file = args.File.Trim(Constants.Scripts.PathTrimParameters);
            var result = new List<IDefinition>();
            IDefinition definition = null;
            var sb = new StringBuilder();
            int? openBrackets = null;
            int closeBrackets = 0;
            foreach (var line in args.Lines)
            {
                if (line.Trim().StartsWith(Constants.Scripts.ScriptComment))
                {
                    continue;
                }
                if (!openBrackets.HasValue)
                {
                    if (ClearWhitespace(line).Contains(Constants.Scripts.DefinitionSeparator))
                    {
                        openBrackets = line.Count(s => s == Constants.Scripts.OpeningBracket);
                        closeBrackets = line.Count(s => s == Constants.Scripts.ClosingBracket);
                        sb.Clear();
                        var id = GetOperationKey(line, Constants.Scripts.SeparatorOperators);
                        definition = GetDefinitionInstance();
                        definition.Id = id;
                        OnReadObjectLine(line);
                        sb.AppendLine(line);
                    }
                    else if (line.Trim().Contains(Constants.Scripts.VariableSeparator))
                    {
                        definition = GetDefinitionInstance();
                        var id = GetOperationKey(line, Constants.Scripts.SeparatorOperators);
                        definition.Id = id;
                        definition.Code = line;
                        definition.File = file;
                        var type = GetOperationKey(file, Constants.Scripts.PathTrimParameters);
                        definition.Type = type;
                        result.Add(FinalizeVariableDefinition(definition, line));
                    }
                }
                else
                {
                    OnReadObjectLine(line);
                    sb.AppendLine(line);
                    if (line.Contains(Constants.Scripts.OpeningBracket))
                    {
                        openBrackets += line.Count(s => s == Constants.Scripts.OpeningBracket);
                    }
                    if (line.Contains(Constants.Scripts.ClosingBracket))
                    {
                        closeBrackets += line.Count(s => s == Constants.Scripts.ClosingBracket);
                    }
                    if (openBrackets == closeBrackets)
                    {
                        openBrackets = null;
                        closeBrackets = 0;
                        definition.Code = sb.ToString();
                        definition.File = file;
                        var type = GetOperationKey(file, Constants.Scripts.PathTrimParameters);
                        definition.Type = type;
                        result.Add(FinalizeObjectDefinition(definition, line));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Clears the whitespace.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>System.String.</returns>
        protected virtual string ClearWhitespace(string line)
        {
            return line.Trim().Replace(" ", string.Empty).Replace("\t", string.Empty);
        }

        /// <summary>
        /// Finalizes the object definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="line">The line.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition FinalizeObjectDefinition(IDefinition definition, string line)
        {
            return definition;
        }

        /// <summary>
        /// Finalizes the variable definition.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="line">The line.</param>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition FinalizeVariableDefinition(IDefinition definition, string line)
        {
            return definition;
        }

        /// <summary>
        /// Gets the operation key.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetOperationKey(string line, params string[] keys)
        {
            return line.Split(keys, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        /// <summary>
        /// Gets the operation key.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetOperationKey(string line, params char[] keys)
        {
            return line.Split(keys, StringSplitOptions.RemoveEmptyEntries)[0];
        }

        /// <summary>
        /// Gets the operation value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetOperationValue(string line, params string[] keys)
        {
            return line.Split(keys, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        /// <summary>
        /// Gets the operation value.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="keys">The keys.</param>
        /// <returns>System.String.</returns>
        protected virtual string GetOperationValue(string line, params char[] keys)
        {
            return line.Split(keys, StringSplitOptions.RemoveEmptyEntries)[1];
        }

        /// <summary>
        /// Called when [read object line].
        /// </summary>
        /// <param name="line">The line.</param>
        protected virtual void OnReadObjectLine(string line)
        {
        }

        /// <summary>
        /// Gets the definition instance.
        /// </summary>
        /// <returns>IDefinition.</returns>
        private IDefinition GetDefinitionInstance()
        {
            return DIResolver.Get<IDefinition>();
        }

        #endregion Methods
    }
}
