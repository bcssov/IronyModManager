// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 07-28-2020
// ***********************************************************************
// <copyright file="MainConflictSolverControlView.xaml.cs" company="Mario">
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
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Shared;
using IronyModManager.ViewModels;
using ReactiveUI;

namespace IronyModManager.Views
{
    /// <summary>
    /// Class MainConflictSolverControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.MainConflictSolverControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.MainConflictSolverControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class MainConflictSolverControlView : BaseControl<MainConflictSolverControlViewModel>
    {
        #region Fields

        /// <summary>
        /// The cached menu items
        /// </summary>
        private Dictionary<object, List<MenuItem>> cachedMenuItems = new Dictionary<object, List<MenuItem>>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainConflictSolverControlView" /> class.
        /// </summary>
        public MainConflictSolverControlView()
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
            var conflictList = this.FindControl<ListBox>("conflictList");
            conflictList.SelectionChanged += (sender, args) =>
            {
                if (conflictList?.SelectedIndex > -1 && ViewModel.SelectedParentConflict != null)
                {
                    ViewModel.SelectedConflict = ViewModel.SelectedParentConflict.Children.ElementAt(conflictList.SelectedIndex);
                }
                else
                {
                    ViewModel.SelectedConflict = null;
                }
            };
            this.WhenAnyValue(v => v.ViewModel.SelectedParentConflict).Subscribe(s =>
            {
                if (s?.Children.Count > 0)
                {
                    if (ViewModel.PreviousConflictIndex.HasValue)
                    {
                        conflictList.SelectedIndex = ViewModel.PreviousConflictIndex.GetValueOrDefault();
                    }
                    else
                    {
                        conflictList.SelectedIndex = 0;
                    }
                }
            }).DisposeWith(disposables);

            IEnumerable lastDataSource = null;
            conflictList.PointerMoved += (sender, args) =>
            {
                var hoveredItem = conflictList.GetLogicalChildren().Cast<ListBoxItem>().FirstOrDefault(p => p.IsPointerOver);
                if (hoveredItem != null)
                {
                    var grid = hoveredItem.GetLogicalChildren().OfType<Grid>().FirstOrDefault();
                    if (grid != null)
                    {
                        ViewModel.SetParameters(hoveredItem.Content as IHierarchicalDefinitions);
                        bool retrieved = cachedMenuItems.TryGetValue(hoveredItem.Content, out var cached);
                        if (conflictList.Items != lastDataSource || (retrieved && cached != grid.ContextMenu?.Items))
                        {
                            cachedMenuItems = new Dictionary<object, List<MenuItem>>();
                            lastDataSource = conflictList.Items;
                        }
                        if (!cachedMenuItems.ContainsKey(hoveredItem.Content))
                        {
                            if (!string.IsNullOrWhiteSpace(ViewModel.InvalidConflictPath))
                            {
                                var menuItems = new List<MenuItem>()
                                {
                                    new MenuItem()
                                    {
                                        Header = ViewModel.InvalidCustomPatch,
                                        Command = ViewModel.InvalidCustomPatchCommand
                                    },
                                    new MenuItem()
                                    {
                                        Header = "-"
                                    },
                                    new MenuItem()
                                    {
                                        Header = ViewModel.InvalidOpenFile,
                                        Command = ViewModel.InvalidOpenFileCommand
                                    }
                                };
                                if (!ViewModel.InvalidConflictPath.EndsWith(Shared.Constants.ZipExtension, StringComparison.OrdinalIgnoreCase))
                                {
                                    menuItems.Add(new MenuItem()
                                    {
                                        Header = ViewModel.InvalidOpenDirectory,
                                        Command = ViewModel.InvalidOpenDirectoryCommand
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
