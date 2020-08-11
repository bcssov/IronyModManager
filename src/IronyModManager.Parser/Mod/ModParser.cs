// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 08-11-2020
// ***********************************************************************
// <copyright file="ModParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Common.Parsers;

namespace IronyModManager.Parser.Mod
{
    /// <summary>
    /// Class ModParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.IModParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Mod.IModParser" />
    public class ModParser : IModParser
    {
        #region Fields

        /// <summary>
        /// The text parser
        /// </summary>
        private readonly ICodeParser textParser;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModParser" /> class.
        /// </summary>
        /// <param name="textParser">The text parser.</param>
        public ModParser(ICodeParser textParser)
        {
            this.textParser = textParser;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Parses the specified lines.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>IModObject.</returns>
        public IModObject Parse(IEnumerable<string> lines)
        {
            var obj = DIResolver.Get<IModObject>();
            List<string> arrayProps = null;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith(Common.Constants.Scripts.ScriptCommentId))
                {
                    continue;
                }
                var cleaned = textParser.CleanWhitespace(line);
                if (cleaned.Contains(Common.Constants.Scripts.VariableSeparatorId))
                {
                    var key = textParser.GetKey(cleaned, Common.Constants.Scripts.VariableSeparatorId);
                    switch (key)
                    {
                        case "replace_path":
                            obj.ReplacePath = textParser.GetValue(cleaned, $"{key}{Common.Constants.Scripts.VariableSeparatorId}");
                            break;

                        case "user_dir":
                            obj.UserDir = textParser.GetValue(cleaned, $"{key}{Common.Constants.Scripts.VariableSeparatorId}");
                            break;

                        case "path":
                        case "archive":
                            obj.FileName = textParser.GetValue(cleaned, $"{key}{Common.Constants.Scripts.VariableSeparatorId}");
                            break;

                        case "picture":
                            obj.Picture = textParser.GetValue(cleaned, $"{key}{Common.Constants.Scripts.VariableSeparatorId}");
                            break;

                        case "name":
                            obj.Name = textParser.GetValue(cleaned, $"{key}{Common.Constants.Scripts.VariableSeparatorId}");
                            break;

                        case "version":
                            // supported version tag has priority
                            if (string.IsNullOrWhiteSpace(obj.Version))
                            {
                                obj.Version = textParser.GetValue(cleaned, $"{key}{Common.Constants.Scripts.VariableSeparatorId}");
                            }
                            break;

                        case "supported_version":
                            obj.Version = textParser.GetValue(cleaned, $"{key}{Common.Constants.Scripts.VariableSeparatorId}");
                            break;

                        case "tags":
                            obj.Tags = arrayProps = new List<string>();
                            break;

                        case "remote_file_id":
                            if (long.TryParse(textParser.GetValue(cleaned, $"{key}{Common.Constants.Scripts.VariableSeparatorId}"), out long value))
                            {
                                obj.RemoteId = value;
                            }
                            break;

                        case "dependencies":
                            obj.Dependencies = arrayProps = new List<string>();
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    if (arrayProps != null)
                    {
                        if (cleaned.Contains(Common.Constants.Scripts.ClosingBracket))
                        {
                            arrayProps = null;
                        }
                        else
                        {
                            arrayProps.Add(cleaned.Replace("\"", string.Empty));
                        }
                    }
                }
            }
            return obj;
        }

        #endregion Methods
    }
}
