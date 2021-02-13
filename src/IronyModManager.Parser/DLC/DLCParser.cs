// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 02-13-2021
//
// Last Modified By : Mario
// Last Modified On : 02-13-2021
// ***********************************************************************
// <copyright file="DLCParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Parser.Common.DLC;
using IronyModManager.Parser.Common.Parsers;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.DLC
{
    /// <summary>
    /// Class DLCParser.
    /// Implements the <see cref="IronyModManager.Parser.Common.DLC.IDLCParser" />
    /// Implements the <see cref="IronyModManager.Parser.BaseGenericObjectParser" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.BaseGenericObjectParser" />
    /// <seealso cref="IronyModManager.Parser.Common.DLC.IDLCParser" />
    public class DLCParser : BaseGenericObjectParser, IDLCParser
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DLCParser" /> class.
        /// </summary>
        /// <param name="codeParser">The code parser.</param>
        public DLCParser(ICodeParser codeParser) : base(codeParser)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Parses the specified lines.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="lines">The lines.</param>
        /// <returns>IDLCObject.</returns>
        public IDLCObject Parse(string path, IEnumerable<string> lines)
        {
            var data = ParseCode(lines);
            var obj = DIResolver.Get<IDLCObject>();
            if (data != null)
            {
                obj.Name = GetValue<string>(data.Values, "name") ?? string.Empty;
                obj.Path = path.Replace("\\", "/") ?? string.Empty;
            }
            return obj;
        }

        #endregion Methods
    }
}
