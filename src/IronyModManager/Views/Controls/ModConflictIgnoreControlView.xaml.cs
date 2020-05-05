// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-05-2020
//
// Last Modified By : Mario
// Last Modified On : 05-05-2020
// ***********************************************************************
// <copyright file="ModConflictIgnoreControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using Avalonia.Markup.Xaml;
using IronyModManager.Common.Views;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class ModConflictIgnoreControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ModConflictIgnoreControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ModConflictIgnoreControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModConflictIgnoreControlView : BaseControl<ModConflictIgnoreControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModConflictIgnoreControlView"/> class.
        /// </summary>
        public ModConflictIgnoreControlView()
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
