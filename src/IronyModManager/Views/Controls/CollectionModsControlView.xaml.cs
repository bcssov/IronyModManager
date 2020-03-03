// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 03-03-2020
// ***********************************************************************
// <copyright file="CollectionModsControlView.xaml.cs" company="Mario">
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
    /// Class CollectionModsControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.CollectionModsControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.CollectionModsControlViewModel}" />
    public class CollectionModsControlView : BaseControl<CollectionModsControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionModsControlView"/> class.
        /// </summary>
        public CollectionModsControlView()
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
