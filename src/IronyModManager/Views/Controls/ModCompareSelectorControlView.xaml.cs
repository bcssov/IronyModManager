// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-24-2020
//
// Last Modified By : Mario
// Last Modified On : 09-21-2020
// ***********************************************************************
// <copyright file="ModCompareSelectorControlView.xaml.cs" company="Mario">
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
using IronyModManager.DI;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class ModCompareSelectorControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ModCompareSelectorControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ModCompareSelectorControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModCompareSelectorControlView : BaseControl<ModCompareSelectorControlViewModel>
    {
        #region Fields

        /// <summary>
        /// The mod compare class
        /// </summary>
        private const string ModCompareClass = "ModCompare";

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The cached menu items
        /// </summary>
        private Dictionary<object, List<MenuItem>> cachedMenuItems = new Dictionary<object, List<MenuItem>>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModCompareSelectorControlView" /> class.
        /// </summary>
        public ModCompareSelectorControlView()
        {
            logger = DIResolver.Get<ILogger>();
            this.InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Binds the ListBox pointer.
        /// </summary>
        /// <param name="listBox">The list box.</param>
        protected virtual void BindListBoxPointer(ListBox listBox)
        {
            IEnumerable lastDataSource = null;
            listBox.PointerMoved += (sender, args) =>
            {
                var hoveredItem = listBox.GetLogicalChildren().Cast<ListBoxItem>().FirstOrDefault(p => p.IsPointerOver);
                if (hoveredItem != null)
                {
                    var grid = hoveredItem.GetLogicalChildren().OfType<Grid>().FirstOrDefault();
                    if (grid != null)
                    {
                        ViewModel.SetParameters(hoveredItem.Content as IDefinition);
                        bool retrieved = cachedMenuItems.TryGetValue(hoveredItem.Content, out var cached);
                        if (listBox.Items != lastDataSource || (retrieved && cached != grid.ContextMenu?.Items))
                        {
                            cachedMenuItems = new Dictionary<object, List<MenuItem>>();
                            lastDataSource = listBox.Items;
                        }
                        if (!cachedMenuItems.ContainsKey(hoveredItem.Content))
                        {
                            if (!string.IsNullOrWhiteSpace(ViewModel.ConflictPath))
                            {
                                var menuItems = new List<MenuItem>()
                                {
                                    new MenuItem()
                                    {
                                        Header = ViewModel.OpenFile,
                                        Command = ViewModel.OpenFileCommand
                                    }
                                };
                                if (!ViewModel.ConflictPath.EndsWith(Shared.Constants.ZipExtension, StringComparison.OrdinalIgnoreCase))
                                {
                                    menuItems.Add(new MenuItem()
                                    {
                                        Header = ViewModel.OpenDirectory,
                                        Command = ViewModel.OpenDirectoryCommand
                                    });
                                }
                                if (grid.ContextMenu == null)
                                {
                                    grid.ContextMenu = new ContextMenu();
                                }
                                grid.ContextMenu.Items = menuItems;
                                cachedMenuItems.Add(hoveredItem.Content, menuItems);
                            }
                            else
                            {
                                grid.ContextMenu = null;
                                cachedMenuItems.Add(hoveredItem.Content, null);
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            static void appendClass(ListBox listBox)
            {
                var children = listBox.GetLogicalChildren().Cast<ListBoxItem>();
                foreach (var item in children)
                {
                    if (!item.Classes.Contains(ModCompareClass))
                    {
                        item.Classes.Add(ModCompareClass);
                    }
                }
            }
            var left = this.FindControl<ListBox>("leftSide");
            var right = this.FindControl<ListBox>("rightSide");
            LayoutUpdated += (sender, args) =>
            {
                try
                {
                    left.InvalidateArrange();
                    right.InvalidateArrange();
                    appendClass(left);
                    appendClass(right);
                }
                catch (System.Exception ex)
                {
                    logger.Error(ex);
                }
            };
            BindListBoxPointer(left);
            BindListBoxPointer(right);
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
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #endregion Methods
    }
}
