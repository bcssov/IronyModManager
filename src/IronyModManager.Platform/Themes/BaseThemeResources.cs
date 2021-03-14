// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 03-14-2021
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

namespace IronyModManager.Platform.Themes
{
    /// <summary>
    /// Class BaseThemeResources.
    /// Implements the <see cref="IronyModManager.Platform.Themes.IThemeResources" />
    /// </summary>
    /// <seealso cref="IronyModManager.Platform.Themes.IThemeResources" />
    public abstract class BaseThemeResources : IThemeResources
    {
        #region Properties

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
        /// Registers this instance.
        /// </summary>
        public void Register()
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
