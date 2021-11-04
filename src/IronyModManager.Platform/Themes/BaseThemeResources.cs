// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 11-04-2021
// ***********************************************************************
// <copyright file="BaseThemeResources.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Markup.Xaml.Styling;
using SmartFormat;

namespace IronyModManager.Platform.Themes
{
    /// <summary>
    /// Class BaseThemeResources.
    /// Implements the <see cref="IronyModManager.Platform.Themes.IThemeResources" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Themes.IThemeResources" />
    public abstract class BaseThemeResources : IThemeResources
    {
        #region Fields

        /// <summary>
        /// The CSS
        /// </summary>
        private const string CSS = "body {{ color: {color}; background-color: {backgroundColor}; word-wrap: break-word; {additionalStyles} }}";

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the color of the background HTML.
        /// </summary>
        /// <value>The color of the background HTML.</value>
        public abstract string BackgroundHtmlColor { get; }

        /// <summary>
        /// Gets the color of the foreground HTML.
        /// </summary>
        /// <value>The color of the foreground HTML.</value>
        public abstract string ForegroundHtmlColor { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is light theme.
        /// </summary>
        /// <value><c>true</c> if this instance is light theme; otherwise, <c>false</c>.</value>
        public abstract bool IsLightTheme { get; }

        /// <summary>
        /// Gets the styles.
        /// </summary>
        /// <value>The styles.</value>
        public abstract IReadOnlyCollection<string> Styles { get; }

        /// <summary>
        /// Gets the name of the theme.
        /// </summary>
        /// <value>The name of the theme.</value>
        public abstract string ThemeName { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Gets the HTML base CSS.
        /// </summary>
        /// <param name="additionalStyles">The additional styles.</param>
        /// <returns>System.String.</returns>
        /// <value>The HTML base CSS.</value>
        public virtual string GetHtmlBaseCSS(string additionalStyles)
        {
            return Smart.Format(CSS, new { color = ForegroundHtmlColor, backgroundColor = BackgroundHtmlColor, additionalStyles });
        }

        /// <summary>
        /// Registers this instance.
        /// </summary>
        public virtual void Register()
        {
            RegisterStyles();
        }

        /// <summary>
        /// Compiles the style.
        /// </summary>
        /// <param name="style">The style.</param>
        /// <returns>StyleInclude.</returns>
        protected virtual StyleInclude CompileStyle(string style)
        {
            return new StyleInclude(new Uri("resm:Styles"))
            {
                Source = new Uri(style)
            };
        }

        /// <summary>
        /// Registers the styles.
        /// </summary>
        protected virtual void RegisterStyles()
        {
            var app = Application.Current;
            if (app != null && Styles != null)
            {
                foreach (var item in Styles)
                {
                    var style = CompileStyle(item);
                    app.Styles.Add(style);
                }
            }
        }

        #endregion Methods
    }
}
