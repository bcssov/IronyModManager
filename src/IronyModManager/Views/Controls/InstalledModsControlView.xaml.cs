// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 03-11-2021
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
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var modList = this.FindControl<IronyModManager.Controls.ListBox>("modList");
            if (modList != null)
            {
                modList.ContextMenuOpening += (sender, args) =>
                {
                    List<MenuItem> menuItems = null;
                    var hoveredItem = modList.GetLogicalChildren().Cast<ListBoxItem>().FirstOrDefault(p => p.IsPointerOver);
                    if (hoveredItem != null)
                    {
                        ViewModel.ContextMenuMod = hoveredItem.Content as IMod;
                        menuItems = !string.IsNullOrEmpty(ViewModel.GetContextMenuModModUrl()) || !string.IsNullOrEmpty(ViewModel.GetContextMenuModModSteamUrl()) ? GetAllMenuItems() : GetActionMenuItems();
                    }
                    if (modList.ItemCount == 0)
                    {
                        menuItems = GetStaticMenuItems();
                    }
                    modList.SetContextMenuItems(menuItems);
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
            var menuItems = new List<MenuItem>()
            {
                new MenuItem()
                {
                    Header = ViewModel.CheckNewMods,
                    Command = ViewModel.CheckNewModsCommand
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
                    Header = "-"
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
            if (!string.IsNullOrWhiteSpace(ViewModel.ContextMenuMod?.FullPath))
            {
                var menuItem = new MenuItem()
                {
                    Header = ViewModel.OpenInAssociatedApp,
                    Command = ViewModel.OpenInAssociatedAppCommand
                };

                menuItems.Insert(0, menuItem);
                menuItems.Insert(1, new MenuItem()
                {
                    Header = "-"
                });
            }
            return menuItems;
        }

        /// <summary>
        /// Gets all menu items.
        /// </summary>
        /// <returns>List&lt;MenuItem&gt;.</returns>
        private List<MenuItem> GetAllMenuItems()
        {
            var menuItems = new List<MenuItem>();
            if (!string.IsNullOrEmpty(ViewModel.GetContextMenuModModUrl()))
            {
                menuItems.Add(new MenuItem()
                {
                    Header = ViewModel.OpenUrl,
                    Command = ViewModel.OpenUrlCommand
                });
                menuItems.Add(new MenuItem()
                {
                    Header = ViewModel.CopyUrl,
                    Command = ViewModel.CopyUrlCommand
                });
            }
            if (!string.IsNullOrEmpty(ViewModel.GetContextMenuModModSteamUrl()))
            {
                var menuItem = new MenuItem()
                {
                    Header = ViewModel.OpenInSteam,
                    Command = ViewModel.OpenInSteamCommand
                };
                if (menuItems.Count == 0)
                {
                    menuItems.Add(menuItem);
                }
                else
                {
                    menuItems.Insert(1, menuItem);
                }
            }
            if (!string.IsNullOrWhiteSpace(ViewModel.ContextMenuMod?.FullPath))
            {
                var menuItem = new MenuItem()
                {
                    Header = ViewModel.OpenInAssociatedApp,
                    Command = ViewModel.OpenInAssociatedAppCommand
                };
                if (menuItems.Count == 0)
                {
                    menuItems.Add(menuItem);
                }
                else
                {
                    menuItems.Insert(0, menuItem);
                }
            }
            menuItems.AddRange(new List<MenuItem>()
            {
                new MenuItem()
                {
                    Header = "-"
                },
                new MenuItem()
                {
                    Header = ViewModel.CheckNewMods,
                    Command = ViewModel.CheckNewModsCommand
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
                    Header = "-"
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
            });
            return menuItems;
        }

        /// <summary>
        /// Gets the static menu items.
        /// </summary>
        /// <returns>System.Collections.Generic.List&lt;Avalonia.Controls.MenuItem&gt;.</returns>
        private List<MenuItem> GetStaticMenuItems()
        {
            var menuItems = new List<MenuItem>()
            {
                new MenuItem()
                {
                    Header = ViewModel.CheckNewMods,
                    Command = ViewModel.CheckNewModsCommand
                },
                new MenuItem()
                {
                    Header = ViewModel.DeleteAllDescriptors,
                    Command = ViewModel.DeleteAllDescriptorsCommand
                }
            };
            return menuItems;
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
