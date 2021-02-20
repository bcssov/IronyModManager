// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-18-2020
//
// Last Modified By : Mario
// Last Modified On : 02-20-2021
// ***********************************************************************
// <copyright file="MainConflictSolverControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using IronyModManager.Common;
using IronyModManager.Common.Views;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
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
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainConflictSolverControlView" /> class.
        /// </summary>
        public MainConflictSolverControlView()
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
            var conflictList = this.FindControl<IronyModManager.Controls.ListBox>("conflictList");
            CancellationTokenSource cancel = null;
            conflictList.SelectionChanged += (sender, args) =>
            {
                async Task delay(CancellationToken token)
                {
                    await Task.Delay(150, token);
                    if (!token.IsCancellationRequested)
                    {
                        await Dispatcher.UIThread.SafeInvokeAsync(() =>
                        {
                            if (conflictList?.SelectedIndex > -1 && ViewModel.SelectedParentConflict != null)
                            {
                                ViewModel.SelectedConflict = ViewModel.SelectedParentConflict.Children.ElementAt(conflictList.SelectedIndex);
                            }
                            else
                            {
                                ViewModel.SelectedConflict = null;
                            }
                        });
                    }
                }
                if (cancel != null)
                {
                    cancel.Cancel();
                }
                cancel = new CancellationTokenSource();
                delay(cancel.Token).ConfigureAwait(false);
            };
            this.WhenAnyValue(p => p.IsActivated).Where(v => v).Subscribe(s =>
            {
                this.WhenAnyValue(v => v.ViewModel.SelectedParentConflict).Subscribe(s =>
                {
                    if (s?.Children.Count > 0)
                    {
                        Dispatcher.UIThread.SafeInvoke(() =>
                        {
                            if (ViewModel.PreviousConflictIndex.HasValue)
                            {
                                if (conflictList.ItemCount > 0)
                                {
                                    conflictList.SelectedIndex = -1;
                                    conflictList.SelectedIndex = ViewModel.PreviousConflictIndex.GetValueOrDefault();
                                }
                                else
                                {
                                    conflictList.SelectedIndex = -1;
                                }
                            }
                            else
                            {
                                if (conflictList.ItemCount > 0)
                                {
                                    conflictList.SelectedIndex = 0;
                                }
                                else
                                {
                                    conflictList.SelectedIndex = -1;
                                }
                            }
                        });
                    }
                }).DisposeWith(disposables);

                this.WhenAnyValue(v => v.ViewModel.SelectedConflictOverride).Subscribe(s =>
                {
                    if (s.HasValue)
                    {
                        Dispatcher.UIThread.SafeInvoke(() =>
                        {
                            if (conflictList.ItemCount > 0)
                            {
                                conflictList.SelectedIndex = s.GetValueOrDefault();
                            }
                        });
                    }
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            conflictList.ContextMenuOpening += (sender, args) =>
            {
                List<MenuItem> menuItems = null;
                var hoveredItem = conflictList.GetLogicalChildren().Cast<ListBoxItem>().FirstOrDefault(p => p.IsPointerOver);
                if (hoveredItem != null)
                {
                    ViewModel.SetParameters(hoveredItem.Content as IHierarchicalDefinitions);
                    if (!string.IsNullOrWhiteSpace(ViewModel.InvalidConflictPath))
                    {
                        menuItems = new List<MenuItem>()
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
                        if (!ViewModel.InvalidConflictPath.EndsWith(Shared.Constants.ZipExtension, StringComparison.OrdinalIgnoreCase) &&
                            !ViewModel.InvalidConflictPath.EndsWith(Shared.Constants.BinExtension, StringComparison.OrdinalIgnoreCase))
                        {
                            menuItems.Add(new MenuItem()
                            {
                                Header = ViewModel.InvalidOpenDirectory,
                                Command = ViewModel.InvalidOpenDirectoryCommand
                            });
                        }
                    }
                }
                conflictList.SetContextMenu(menuItems);
            };

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
