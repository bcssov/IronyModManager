// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 07-15-2020
// ***********************************************************************
// <copyright file="InstalledModsControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
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
        #region Fields

        /// <summary>
        /// The cached menu items
        /// </summary>
        private Dictionary<object, List<MenuItem>> cachedMenuItems = new Dictionary<object, List<MenuItem>>();

        #endregion Fields

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
            IEnumerable lastDataSource = null;
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
                            bool retrieved = cachedMenuItems.TryGetValue(hoveredItem.Content, out var cached);
                            if (modList.Items != lastDataSource || (retrieved && cached != grid.ContextMenu?.Items))
                            {
                                cachedMenuItems = new Dictionary<object, List<MenuItem>>();
                                lastDataSource = modList.Items;
                            }
                            if (!cachedMenuItems.ContainsKey(hoveredItem.Content))
                            {
                                var menuItems = !string.IsNullOrEmpty(ViewModel.GetHoveredModUrl()) || !string.IsNullOrEmpty(ViewModel.GetHoveredModSteamUrl()) ? GetAllMenuItems() : GetActionMenuItems();
                                if (grid.ContextMenu == null)
                                {
                                    grid.ContextMenu = new ContextMenu();
                                }
                                if (menuItems.Count == 0)
                                {
                                    grid.ContextMenu = null;
                                }
                                else
                                {
                                    grid.ContextMenu.Items = menuItems;
                                }
                                cachedMenuItems.Add(hoveredItem.Content, menuItems);
                            }
                        }
                    }
                };
            }
            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        protected override void OnLocaleChanged(string newLocale, string oldLocale)
        {
            cachedMenuItems = new Dictionary<object, List<MenuItem>>();
            base.OnLocaleChanged(newLocale, oldLocale);
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
            if (!string.IsNullOrWhiteSpace(ViewModel.HoveredMod?.FullPath))
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
            if (!string.IsNullOrEmpty(ViewModel.GetHoveredModUrl()))
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
            if (!string.IsNullOrEmpty(ViewModel.GetHoveredModSteamUrl()))
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
            if (!string.IsNullOrWhiteSpace(ViewModel.HoveredMod?.FullPath))
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
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #endregion Methods
    }
}
