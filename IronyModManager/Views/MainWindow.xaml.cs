// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-14-2020
// ***********************************************************************
// <copyright file="MainWindow.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IronyModManager.Views
{
    /// <summary>
    /// Class MainWindow.
    /// Implements the <see cref="Avalonia.Controls.Window" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.Window" />
    public class MainWindow : Window
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
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
