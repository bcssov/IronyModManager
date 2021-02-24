// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-22-2020
//
// Last Modified By : Mario
// Last Modified On : 02-13-2021
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
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Mod
{
    /// <summary>
    /// Class ModParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.Mod.IModParser" />
    /// Implements the <see cref="IronyModManager.Parser.BaseGenericObjectParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.BaseGenericObjectParser" />
    /// <seealso cref="IronyModManager.Parser.Common.Mod.IModParser" />
    public class ModParser : BaseGenericObjectParser, IModParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        public ModParser(ICodeParser codeParser) : base(codeParser)
        {
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
            var data = ParseCode(lines);
            var obj = DIResolver.Get<IModObject>();
            if (data != null)
            {
                obj.ReplacePath = GetKeyedValues<string>(data.Values, "replace_path");
                obj.UserDir = GetKeyedValues<string>(data.Values, "user_dir");
                obj.FileName = GetValue<string>(data.Values, "path", "archive") ?? string.Empty;
                obj.Picture = GetValue<string>(data.Values, "picture") ?? string.Empty;
                obj.Name = GetValue<string>(data.Values, "name") ?? string.Empty;
                obj.Version = GetValue<string>(data.Values, "supported_version", "version") ?? string.Empty;
                obj.Tags = GetValues<string>(data.Values, "tags");
                obj.RemoteId = GetValue<long?>(data.Values, "remote_file_id");
                obj.Dependencies = GetValues<string>(data.Values, "dependencies");
            }
            return obj;
        }

        #endregion Methods
    }
}
