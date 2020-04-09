// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-05-2020
//
// Last Modified By : Mario
// Last Modified On : 04-05-2020
// ***********************************************************************
// <copyright file="AddNewCollectionControlView.xaml.cs" company="Mario">
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
    /// Class AddNewCollectionControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.AddNewCollectionControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.AddNewCollectionControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class AddNewCollectionControlView : BaseControl<AddNewCollectionControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewCollectionControlView" /> class.
        /// </summary>
        public AddNewCollectionControlView()
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
