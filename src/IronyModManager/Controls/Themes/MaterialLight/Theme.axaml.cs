// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 03-14-2021
// ***********************************************************************
// <copyright file="Theme.axaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia;
using Avalonia.Animation;
using Avalonia.Markup.Xaml;
using IronyModManager.Shared;
using Material.Styles.Additional;

namespace IronyModManager.Controls.Themes.MaterialLight
{
    /// <summary>
    /// Class ThemeOverride.
    /// Implements the <see cref="Avalonia.Styling.Styles" />
    /// </summary>
    /// <seealso cref="Avalonia.Styling.Styles" />
    [ExcludeFromCoverage("UI Elements should be tested in functional testing.")]
    public class Theme : Avalonia.Styling.Styles
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeOverride" /> class.
        /// </summary>
        public Theme()
        {
            AvaloniaXamlLoader.Load(this);
            Animation.RegisterAnimator<RelativePointAnimator>(property => typeof(RelativePoint).IsAssignableFrom(property.PropertyType));
        }

        #endregion Constructors
    }
}
