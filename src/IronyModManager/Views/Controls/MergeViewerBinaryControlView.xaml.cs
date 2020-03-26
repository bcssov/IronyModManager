// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-25-2020
//
// Last Modified By : Mario
// Last Modified On : 03-25-2020
// ***********************************************************************
// <copyright file="MergeViewerBinaryControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Markup.Xaml;
using IronyModManager.Common.Views;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class MergeViewerBinaryControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.MergeViewerBinaryControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.MergeViewerBinaryControlViewModel}" />
    public class MergeViewerBinaryControlView : BaseControl<MergeViewerBinaryControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeViewerBinaryControlView"/> class.
        /// </summary>
        public MergeViewerBinaryControlView()
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
