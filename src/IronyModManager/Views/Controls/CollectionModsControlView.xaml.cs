// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 12-12-2020
// ***********************************************************************
// <copyright file="CollectionModsControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using IronyModManager.Common.Views;
using IronyModManager.Controls;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class CollectionModsControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.CollectionModsControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.CollectionModsControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class CollectionModsControlView : BaseControl<CollectionModsControlViewModel>
    {
        #region Fields

        /// <summary>
        /// The order name
        /// </summary>
        private const string OrderName = "order";

        /// <summary>
        /// The cached menu items
        /// </summary>
        private Dictionary<object, List<MenuItem>> cachedMenuItems = new Dictionary<object, List<MenuItem>>();

        /// <summary>
        /// The mod list
        /// </summary>
        private DragDropListBox modList;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionModsControlView" /> class.
        /// </summary>
        public CollectionModsControlView()
        {
            this.InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// focus order textbox as an asynchronous operation.
        /// </summary>
        /// <param name="mod">The mod.</param>
        protected virtual async Task FocusOrderTextboxAsync(IMod mod)
        {
            await Task.Delay(100);
            var listboxItems = modList.GetLogicalChildren().Cast<ListBoxItem>();
            if (mod != null)
            {
                foreach (var item in listboxItems)
                {
                    var grid = item.GetLogicalChildren().OfType<Grid>().FirstOrDefault();
                    if (grid != null)
                    {
                        var contentMod = item.Content as IMod;
                        if (mod == contentMod)
                        {
                            grid.Focus();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the item dragged.
        /// </summary>
        protected virtual void HandleItemDragged()
        {
            modList.ItemDragged += (source, destination) =>
            {
                var sourceMod = source as IMod;
                var destinationMod = destination as IMod;
                ViewModel.InstantReorderSelectedItems(sourceMod, destinationMod.Order);
            };
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            modList = this.FindControl<DragDropListBox>("modList");
            if (modList != null)
            {
                SetContextMenus();
                SetOrderParameters();
                HandleItemDragged();
                this.WhenAnyValue(p => p.IsActivated).Where(p => p).Subscribe(s =>
                {
                    ViewModel.CollectionJumpOnPositionChangeCommand.Subscribe(s =>
                    {
                        cachedMenuItems = new Dictionary<object, List<MenuItem>>();
                    }).DisposeWith(disposables);
                }).DisposeWith(disposables);
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
        /// Sets the pointer events.
        /// </summary>
        protected virtual void SetContextMenus()
        {
            IEnumerable lastDataSource = null;
            modList.PointerMoved += (sender, args) =>
            {
                var allItems = modList.GetLogicalChildren().Cast<ListBoxItem>().Select(p => p.GetLogicalChildren().OfType<Grid>().FirstOrDefault());
                if (allItems.Any(p => p.ContextMenu != null && p.ContextMenu.IsOpen))
                {
                    return;
                }
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
                            if (!string.IsNullOrEmpty(ViewModel.GetHoveredModUrl()) || !string.IsNullOrEmpty(ViewModel.GetHoveredModSteamUrl()) || !string.IsNullOrWhiteSpace(ViewModel.HoveredMod?.FullPath))
                            {
                                var menuItems = new List<MenuItem>
                                {
                                    new MenuItem()
                                    {
                                        Header = ViewModel.CollectionJumpOnPositionChangeLabel,
                                        Command = ViewModel.CollectionJumpOnPositionChangeCommand
                                    },
                                    new MenuItem()
                                    {
                                        Header = "-"
                                    },
                                    new MenuItem()
                                    {
                                        Header = ViewModel.ExportCollectionToClipboard,
                                        Command = ViewModel.ExportCollectionToClipboardCommand
                                    },
                                    new MenuItem()
                                    {
                                        Header = ViewModel.ImportCollectionFromClipboard,
                                        Command = ViewModel.ImportCollectionFromClipboardCommand
                                    },
                                    new MenuItem()
                                    {
                                        Header = "-"
                                    }
                                };
                                var counterOffset = 5;
                                if (ViewModel.CanExportModHashReport)
                                {
                                    menuItems.Add(new MenuItem()
                                    {
                                        Header = ViewModel.ExportReport,
                                        Command = ViewModel.ExportReportCommand
                                    });
                                    menuItems.Add(new MenuItem()
                                    {
                                        Header = ViewModel.ImportReport,
                                        Command = ViewModel.ImportReportCommand
                                    });
                                    menuItems.Add(new MenuItem()
                                    {
                                        Header = "-"
                                    });
                                    counterOffset += 3;
                                }
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
                                    if (menuItems.Count == counterOffset)
                                    {
                                        menuItems.Add(menuItem);
                                    }
                                    else
                                    {
                                        menuItems.Insert(counterOffset + 1, menuItem);
                                    }
                                }
                                if (!string.IsNullOrWhiteSpace(ViewModel.HoveredMod?.FullPath))
                                {
                                    var menuItem = new MenuItem()
                                    {
                                        Header = ViewModel.OpenInAssociatedApp,
                                        Command = ViewModel.OpenInAssociatedAppCommand
                                    };
                                    if (menuItems.Count == counterOffset)
                                    {
                                        menuItems.Add(menuItem);
                                    }
                                    else
                                    {
                                        menuItems.Insert(counterOffset, menuItem);
                                    }
                                }
                                grid.ContextMenu.Items = menuItems;
                                cachedMenuItems.Add(hoveredItem.Content, menuItems);
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Sets the order parameters.
        /// </summary>
        protected virtual void SetOrderParameters()
        {
            void setNumericProperties(bool setMargin = false)
            {
                var listboxItems = modList.GetLogicalChildren().Cast<ListBoxItem>();
                foreach (var item in listboxItems)
                {
                    var grid = item.GetLogicalChildren().OfType<Grid>().FirstOrDefault();
                    if (grid != null)
                    {
                        var orderCtrl = grid.GetLogicalChildren().OfType<MinMaxNumericUpDown>().FirstOrDefault(p => p.Name == OrderName);
                        if (orderCtrl != null)
                        {
                            orderCtrl.Minimum = 1;
                            orderCtrl.Maximum = ViewModel.MaxOrder;
                            if (setMargin)
                            {
                                var left = 0;
                                var right = 0;
                                if (orderCtrl.Bounds.Width > 100)
                                {
                                    right = 10;
                                }
                                else
                                {
                                    left = 6;
                                }
                                orderCtrl.Margin = new Avalonia.Thickness(left, 0, right, 0);
                            }
                        }
                    }
                }
            }

            ViewModel.ModReordered += (args) =>
            {
                setNumericProperties();
                modList.Focus();
                FocusOrderTextboxAsync(args).ConfigureAwait(true);
            };

            this.WhenAnyValue(v => v.ViewModel.MaxOrder).Subscribe(max =>
            {
                setNumericProperties();
            }).DisposeWith(Disposables);

            var previousHashState = false;
            this.WhenAnyValue(v => v.ViewModel.CanExportModHashReport).Subscribe(s =>
            {
                if (s != previousHashState)
                {
                    cachedMenuItems = new Dictionary<object, List<MenuItem>>();
                }
                previousHashState = s;
            }).DisposeWith(Disposables);

            modList.LayoutUpdated += (sender, args) =>
            {
                setNumericProperties(true);
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
