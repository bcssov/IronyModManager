// ***********************************************************************
// Assembly         : IronyModManager.Parser.Common
// Author           : Mario
// Created          : 04-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-24-2020
// ***********************************************************************
// <copyright file="ScriptNode.cs" company="Mario">
//     Mario
// </copyright>
// <summary>Based on CWTools samples.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Parser.Common.Parsers.Models;

namespace IronyModManager.Parser.Models
{
    /// <summary>
    /// Class ScriptNode.
    /// Implements the <see cref="IronyModManager.Parser.Models.ScriptBase" />
    /// Implements the <see cref="IronyModManager.Parser.Common.Parsers.Models.IScriptNode" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Models.ScriptBase" />
    /// <seealso cref="IronyModManager.Parser.Common.Parsers.Models.IScriptNode" />
    public class ScriptNode : ScriptBase, IScriptNode
    {
        #region Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public virtual string Key { get; set; }

        /// <summary>
        /// Gets or sets the key values.
        /// </summary>
        /// <value>The key values.</value>
        public virtual IList<IScriptKeyValue> KeyValues { get; set; }

        /// <summary>
        /// Gets or sets the nodes.
        /// </summary>
        /// <value>The nodes.</value>
        public IList<IScriptNode> Nodes { get; set; }

        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        /// <value>The values.</value>
        public IList<IScriptValue> Values { get; set; }

        #endregion Properties
    }
}
