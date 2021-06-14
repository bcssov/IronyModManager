// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-14-2021
//
// Last Modified By : Mario
// Last Modified On : 06-14-2021
// ***********************************************************************
// <copyright file="IResourceLoader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaEdit.Highlighting;

namespace IronyModManager.Implementation.AvaloniaEdit
{
    /// <summary>
    /// Interface IResourceLoader
    /// </summary>
    public interface IResourceLoader
    {
        #region Methods

        /// <summary>
        /// Gets the PDX script definition.
        /// </summary>
        /// <returns>IHighlightingDefinition.</returns>
        IHighlightingDefinition GetPDXScriptDefinition();

        /// <summary>
        /// Gets the yaml definition.
        /// </summary>
        /// <returns>IHighlightingDefinition.</returns>
        IHighlightingDefinition GetYAMLDefinition();

        #endregion Methods
    }
}
