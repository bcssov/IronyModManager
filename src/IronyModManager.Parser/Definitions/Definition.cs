// ***********************************************************************
// Assembly         : IronyModManager.Parser.Definitions
// Author           : Mario
// Created          : 02-16-2020
//
// Last Modified By : Mario
// Last Modified On : 02-26-2020
// ***********************************************************************
// <copyright file="Definition.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Parser.Common.Definitions;

namespace IronyModManager.Parser.Definitions
{
    /// <summary>
    /// Class Definition.
    /// Implements the <see cref="IronyModManager.Parser.Common.Definitions.IDefinition" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Common.Definitions.IDefinition" />
    public class Definition : IDefinition
    {
        #region Properties

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>The code.</value>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the content sha.
        /// </summary>
        /// <value>The content sha.</value>
        public string ContentSHA { get; set; }

        /// <summary>
        /// Gets or sets the dependencies.
        /// </summary>
        /// <value>The dependencies.</value>
        public IEnumerable<string> Dependencies { get; set; }

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        /// <value>The file.</value>
        public string File { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the mod.
        /// </summary>
        /// <value>The name of the mod.</value>
        public string ModName { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the type of the value.
        /// </summary>
        /// <value>The type of the value.</value>
        public Common.ValueType ValueType { get; set; }

        #endregion Properties
    }
}
