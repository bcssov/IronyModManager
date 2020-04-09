// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 04-07-2020
// ***********************************************************************
// <copyright file="InstalledModsControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using IronyModManager.Common.Views;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class InstalledModsControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.InstalledModsControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.InstalledModsControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class InstalledModsControlView : BaseControl<InstalledModsControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledModsControlView" /> class.
        /// </summary>
        public InstalledModsControlView()
        {
            this.InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var modList = this.FindControl<ListBox>("modList");
            if (modList != null)
            {
                modList.PointerMoved += (sender, args) =>
                {
                    var hoveredItem = modList.GetLogicalChildren().Cast<ListBoxItem>().FirstOrDefault(p => p.IsPointerOver);
                    if (hoveredItem != null)
                    {
                        var grid = hoveredItem.GetLogicalChildren().OfType<Grid>().FirstOrDefault();
                        if (grid != null)
                        {
                            ViewModel.HoveredMod = hoveredItem.Content as IMod;
                            if (!string.IsNullOrEmpty(ViewModel.GetHoveredModUrl()))
                            {
                                var menuItems = new List<MenuItem>()
                                {
                                    new MenuItem()
                                    {
                                        Header = ViewModel.OpenUrl,
                                        Command = ViewModel.OpenUrlCommand
                                    },
                                    new MenuItem()
                                    {
                                        Header = ViewModel.CopyUrl,
                                        Command = ViewModel.CopyUrlCommand
                                    }
                                };
                                grid.ContextMenu.Items = menuItems;
                            }
                        }
                    }
                };
            }
            base.OnActivated(disposables);
        }

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
