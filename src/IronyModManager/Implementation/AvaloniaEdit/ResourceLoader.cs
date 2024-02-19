// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-14-2021
//
// Last Modified By : Mario
// Last Modified On : 02-19-2024
// ***********************************************************************
// <copyright file="ResourceLoader.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using IronyModManager.Platform.Themes;
using IronyModManager.Services.Common;
using IronyModManager.Shared;

namespace IronyModManager.Implementation.AvaloniaEdit
{
    /// <summary>
    /// Class ResourceLoader.
    /// Implements the <see cref="IronyModManager.Implementation.AvaloniaEdit.IResourceLoader" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.AvaloniaEdit.IResourceLoader" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="ResourceLoader" /> class.
    /// </remarks>
    /// <param name="themeService">The theme service.</param>
    /// <param name="themeManager">The theme manager.</param>
    public class ResourceLoader(IThemeService themeService, IThemeManager themeManager) : IResourceLoader
    {
        #region Fields

        /// <summary>
        /// The PDX script highlighting definition
        /// </summary>
        private static IHighlightingDefinition pdxScriptHighlightingDefinition;

        /// <summary>
        /// The yaml highlighting definition
        /// </summary>
        private static IHighlightingDefinition yamlHighlightingDefinition;

        /// <summary>
        /// The theme manager
        /// </summary>
        private readonly IThemeManager themeManager = themeManager;

        /// <summary>
        /// The theme service
        /// </summary>
        private readonly IThemeService themeService = themeService;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the PDX script definition.
        /// </summary>
        /// <returns>IHighlightingDefinition.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public IHighlightingDefinition GetPDXScriptDefinition()
        {
            if (pdxScriptHighlightingDefinition == null)
            {
                var resourcePath = themeManager.IsLightTheme(themeService.GetSelected().Type) ? IronyModManager.Constants.Resources.PDXScriptLight : IronyModManager.Constants.Resources.PDXScriptDark;
                pdxScriptHighlightingDefinition = GetHighlightingDefinition(resourcePath);
            }

            return pdxScriptHighlightingDefinition;
        }

        /// <summary>
        /// Gets the yaml definition.
        /// </summary>
        /// <returns>IHighlightingDefinition.</returns>
        public IHighlightingDefinition GetYAMLDefinition()
        {
            if (yamlHighlightingDefinition == null)
            {
                var resourcePath = themeManager.IsLightTheme(themeService.GetSelected().Type) ? IronyModManager.Constants.Resources.YAMLLight : IronyModManager.Constants.Resources.YAMLDark;
                yamlHighlightingDefinition = GetHighlightingDefinition(resourcePath);
            }

            return yamlHighlightingDefinition;
        }

        /// <summary>
        /// Gets the highlighting definition.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IHighlightingDefinition.</returns>
        protected virtual IHighlightingDefinition GetHighlightingDefinition(string path)
        {
            var bytes = ResourceReader.GetEmbeddedResource(path);
            var xml = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            using var sr = new StringReader(xml);
            using var reader = XmlReader.Create(sr);
            return HighlightingLoader.Load(HighlightingLoader.LoadXshd(reader), HighlightingManager.Instance);
        }

        #endregion Methods
    }
}
