// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 04-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-24-2020
// ***********************************************************************
// <copyright file="IScriptNode.cs" company="Mario">
//     Mario
// </copyright>
// <summary>Based on CWTools samples.</summary>
// ***********************************************************************
using System.Collections.Generic;

namespace IronyModManager.Parser.Common.Parsers.Models
{
    /// <summary>
    /// Interface IScriptNode
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.Models.IScriptBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.Models.IScriptBase" />
    public interface IScriptNode : IScriptBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        string Key { get; set; }

        /// <summary>
        /// Gets or sets the key values.
        /// </summary>
        /// <value>The key values.</value>
        IList<IScriptKeyValue> KeyValues { get; set; }

        /// <summary>
        /// Gets or sets the nodes.
        /// </summary>
        /// <value>The nodes.</value>
        IList<IScriptNode> Nodes { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>The values.</value>
        IList<IScriptValue> Values { get; set; }

        #endregion Properties
    }
}
