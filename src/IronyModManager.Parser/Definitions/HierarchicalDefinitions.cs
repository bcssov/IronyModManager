// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 03-23-2020
//
// Last Modified By : Mario
// Last Modified On : 12-08-2020
// ***********************************************************************
// <copyright file="HierarchicalDefinitions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using IronyModManager.Shared.Models;

namespace IronyModManager.Parser.Definitions
{
    /// <summary>
    /// Class HierarchicalDefinitions.
    /// Implements the <see cref="IronyModManager.Parser.Common.Definitions.IHierarchicalDefinitions" />
    /// Implements the <see cref="IronyModManager.Shared.Models.IHierarchicalDefinitions" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.Models.IHierarchicalDefinitions" />
    /// <seealso cref="IronyModManager.Parser.Common.Definitions.IHierarchicalDefinitions" />
    public class HierarchicalDefinitions : IHierarchicalDefinitions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the additional data.
        /// </summary>
        /// <value>The additional data.</value>
        public object AdditionalData { get; set; }

        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>The children.</value>
        public ICollection<IHierarchicalDefinitions> Children { get; set; }

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the mods.
        /// </summary>
        /// <value>The mods.</value>
        public IList<string> Mods { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets the name of the virtual file.
        /// </summary>
        /// <value>The name of the virtual file.</value>
        public string VirtualFileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(FileName) && !string.IsNullOrWhiteSpace(Name))
                {
                    return Path.Combine(Path.GetDirectoryName(FileName), Name);
                }
                return string.Empty;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="propName">Name of the property.</param>
        /// <param name="unwrap">if set to <c>true</c> [unwrap].</param>
        /// <returns>System.Object.</returns>
        public object GetValue(string propName, bool unwrap)
        {
            return propName switch
            {
                nameof(Key) => Key,
                nameof(Children) => Children,
                nameof(AdditionalData) => AdditionalData,
                nameof(Mods) => Mods,
                nameof(FileName) => FileName,
                _ => Name,
            };
        }

        #endregion Methods
    }
}
