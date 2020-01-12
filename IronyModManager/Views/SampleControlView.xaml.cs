// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-12-2020
//
// Last Modified By : Mario
// Last Modified On : 01-12-2020
// ***********************************************************************
// <copyright file="SampleControl.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace IronyModManager.Views
{
    /// <summary>
    /// Class SampleControlView.
    /// Implements the <see cref="Avalonia.Controls.UserControl" />
    /// </summary>
    /// <seealso cref="Avalonia.Controls.UserControl" />
    public class SampleControlView : UserControl
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleControlView"/> class.
        /// </summary>
        public SampleControlView()
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
