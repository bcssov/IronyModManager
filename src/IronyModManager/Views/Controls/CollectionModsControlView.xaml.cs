// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 03-13-2020
// ***********************************************************************
// <copyright file="CollectionModsControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using IronyModManager.Common.Views;
using IronyModManager.Models.Common;
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
        #region Fields

        /// <summary>
        /// The order name
        /// </summary>
        private const string OrderName = "order";

        /// <summary>
        /// The mod list
        /// </summary>
        private ListBox modList;

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
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(IDisposable disposables)
        {
            modList = this.FindControl<ListBox>("modList");
            SetContextMenus();
            SetOrderParameters();
            base.OnActivated(disposables);
        }

        /// <summary>
        /// Sets the context menus.
        /// </summary>
        protected virtual void SetContextMenus()
        {
            if (modList != null)
            {
                modList.PointerMoved += (sender, args) =>
                {
                    var vm = DataContext as CollectionModsControlViewModel;
                    var hoveredItem = modList.GetLogicalChildren().Cast<ListBoxItem>().FirstOrDefault(p => p.IsPointerOver);
                    if (hoveredItem != null)
                    {
                        var grid = hoveredItem.GetLogicalChildren().OfType<Grid>().FirstOrDefault();
                        if (grid != null)
                        {
                            vm.HoveredMod = hoveredItem.Content as IMod;
                            if (!string.IsNullOrEmpty(vm.GetHoveredModUrl()))
                            {
                                var menuItems = new List<MenuItem>()
                                {
                                    new MenuItem()
                                    {
                                        Header = vm.OpenUrl,
                                        Command = vm.OpenUrlCommand
                                    },
                                    new MenuItem()
                                    {
                                        Header = vm.CopyUrl,
                                        Command = vm.CopyUrlCommand
                                    }
                                };
                                grid.ContextMenu.Items = menuItems;
                            }
                        }
                    }
                };
            }
        }

        /// <summary>
        /// Sets the order parameters.
        /// </summary>
        protected virtual void SetOrderParameters()
        {
            if (modList != null)
            {
                ViewModel.ModReordered += (args) =>
                {
                    FocusOrderTextboxAsync(args).ConfigureAwait(true);
                };
                modList.LayoutUpdated += (sender, args) =>
                {
                    var vm = DataContext as CollectionModsControlViewModel;
                    var listboxItems = modList.GetLogicalChildren().Cast<ListBoxItem>();
                    var items = modList.Items as IEnumerable<IMod>;
                    foreach (var item in listboxItems)
                    {
                        var grid = item.GetLogicalChildren().OfType<Grid>().FirstOrDefault();
                        if (grid != null)
                        {
                            var orderCtrl = grid.GetLogicalChildren().OfType<NumericUpDown>().FirstOrDefault(p => p.Name == OrderName);
                            if (orderCtrl != null)
                            {
                                var mod = item.Content as IMod;
                                orderCtrl.Minimum = 1;
                                orderCtrl.Maximum = items.Count();
                            }
                        }
                    }
                };
            }
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
