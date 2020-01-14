// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-13-2020
//
// Last Modified By : Mario
// Last Modified On : 01-13-2020
// ***********************************************************************
// <copyright file="ThemeControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class ThemeControlView.
    /// Implements the <see cref="Avalonia.Controls.UserControl" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.UserControl" />
    public class ThemeControlView : UserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeControlView"/> class.
        /// </summary>
        public ThemeControlView()
        {
            this.InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #endregion Methods
    }
}
