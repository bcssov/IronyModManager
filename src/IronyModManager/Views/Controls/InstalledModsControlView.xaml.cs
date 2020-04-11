// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 04-11-2020
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
                            var menuItems = !string.IsNullOrEmpty(ViewModel.GetHoveredModUrl()) ? GetAllMenuItems() : GetActionMenuItems();
                            grid.ContextMenu.Items = menuItems;
                        }
                    }
                };
            }
            base.OnActivated(disposables);
        }

        /// <summary>
        /// Gets the action menu items.
        /// </summary>
        /// <returns>List&lt;MenuItem&gt;.</returns>
        private List<MenuItem> GetActionMenuItems()
        {
            return new List<MenuItem>()
            {
                new MenuItem()
                {
                    Header = ViewModel.DeleteDescriptor,
                    Command = ViewModel.DeleteDescriptorCommand
                },
                new MenuItem()
                {
                    Header = ViewModel.DeleteAllDescriptors,
                    Command = ViewModel.DeleteAllDescriptorsCommand
                },
                new MenuItem()
                {
                    Header = ViewModel.LockDescriptor,
                    Command = ViewModel.LockDescriptorCommand
                },
                new MenuItem()
                {
                    Header = ViewModel.LockAllDescriptors,
                    Command = ViewModel.LockAllDescriptorsCommand
                },
                new MenuItem()
                {
                    Header = ViewModel.UnlockDescriptor,
                    Command = ViewModel.UnlockDescriptorCommand
                },
                new MenuItem()
                {
                    Header = ViewModel.UnlockAllDescriptors,
                    Command = ViewModel.UnlockAllDescriptorsCommand
                }
            };
        }

        /// <summary>
        /// Gets all menu items.
        /// </summary>
        /// <returns>List&lt;MenuItem&gt;.</returns>
        private List<MenuItem> GetAllMenuItems()
        {
            return new List<MenuItem>()
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
                },
                new MenuItem()
                {
                    Header = "-"
                },
                new MenuItem()
                {
                    Header = ViewModel.DeleteDescriptor,
                    Command = ViewModel.DeleteDescriptorCommand
                },
                new MenuItem()
                {
                    Header = ViewModel.DeleteAllDescriptors,
                    Command = ViewModel.DeleteAllDescriptorsCommand
                },
                new MenuItem()
                {
                    Header = ViewModel.LockDescriptor,
                    Command = ViewModel.LockDescriptorCommand
                },
                new MenuItem()
                {
                    Header = ViewModel.LockAllDescriptors,
                    Command = ViewModel.LockAllDescriptorsCommand
                },
                new MenuItem()
                {
                    Header = ViewModel.UnlockDescriptor,
                    Command = ViewModel.UnlockDescriptorCommand
                },
                new MenuItem()
                {
                    Header = ViewModel.UnlockAllDescriptors,
                    Command = ViewModel.UnlockAllDescriptorsCommand
                }
            };
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
