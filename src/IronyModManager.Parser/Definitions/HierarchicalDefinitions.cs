// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 03-23-2020
//
// Last Modified By : Mario
// Last Modified On : 05-29-2021
// ***********************************************************************
// <copyright file="HierarchicalDefinitions.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        #region Fields

        /// <summary>
        /// The file names
        /// </summary>
        private readonly List<string> fileNames = new();

        #endregion Fields

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
        public IList<string> FileNames
        {
            get
            {
                if (fileNames.Count > 0 && !fileNames.Contains(Path.Combine(Path.GetDirectoryName(fileNames.FirstOrDefault()), Name)))
                {
                    fileNames.Add(Path.Combine(Path.GetDirectoryName(fileNames.FirstOrDefault()), Name));
                }
                return fileNames;
            }
        }

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
        /// Gets or sets the non game definitions.
        /// </summary>
        /// <value>The non game definitions.</value>
        public int NonGameDefinitions { get; set; }

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
                nameof(FileNames) => FileNames,
                nameof(NonGameDefinitions) => NonGameDefinitions,
                _ => Name
            };
        }

        #endregion Methods
    }
}
