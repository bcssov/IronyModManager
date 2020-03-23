// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-20-2020
//
// Last Modified By : Mario
// Last Modified On : 03-23-2020
// ***********************************************************************
// <copyright file="MergeViewerControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using IronyModManager.Common.Views;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class MergeViewerControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.MergeViewerControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.MergeViewerControlViewModel}" />
    public class MergeViewerControlView : BaseControl<MergeViewerControlViewModel>
    {
        #region Fields

        /// <summary>
        /// The updating
        /// </summary>
        private bool updating = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeViewerControlView" /> class.
        /// </summary>
        public MergeViewerControlView()
        {
            this.InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Handles the context menu.
        /// </summary>
        /// <param name="listBox">The list box.</param>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void HandleContextMenu(ListBox listBox, bool leftSide)
        {
            var hoveredItem = listBox.GetLogicalChildren().Cast<ListBoxItem>().FirstOrDefault(p => p.IsPointerOver);
            if (hoveredItem != null)
            {
                if (hoveredItem != null)
                {
                    var grid = hoveredItem.GetLogicalChildren().OfType<Grid>().FirstOrDefault();
                    if (grid != null)
                    {
                        var menuItems = new List<MenuItem>()
                        {                            
                            new MenuItem()
                            {
                                Header = ViewModel.MoveUp,
                                Command = ViewModel.MoveUpCommand,
                                CommandParameter = leftSide
                            },
                            new MenuItem()
                            {
                                Header = ViewModel.MoveDown,
                                Command = ViewModel.MoveDownCommand,
                                CommandParameter = leftSide
                            },
                            new MenuItem()
                            {
                                Header = "-" // Separator magic string, and it's documented... NOT really!!!
                            },
                            new MenuItem()
                            {
                                Header = ViewModel.CopyThis,
                                Command = ViewModel.CopyThisCommand,
                                CommandParameter = leftSide
                            },
                            new MenuItem()
                            {
                                Header = ViewModel.CopyThisBeforeLine,
                                Command = ViewModel.CopyThisBeforeLineCommand,
                                CommandParameter = leftSide
                            },
                            new MenuItem()
                            {
                                Header = ViewModel.CopyThisAfterLine,
                                Command = ViewModel.CopyThisAfterLineCommand,
                                CommandParameter = leftSide
                            }
                        };
                        grid.ContextMenu.Items = menuItems;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the listbox property changed.
        /// </summary>
        /// <param name="args">The <see cref="AvaloniaPropertyChangedEventArgs" /> instance containing the event data.</param>
        /// <param name="thisListBox">The this ListBox.</param>
        /// <param name="otherListBox">The other ListBox.</param>
        protected virtual void HandleListBoxPropertyChanged(AvaloniaPropertyChangedEventArgs args, ListBox thisListBox, ListBox otherListBox)
        {
            if (args.Property == ListBox.ScrollProperty && thisListBox.Scroll != null)
            {
                var scroll = (ScrollViewer)thisListBox.Scroll;
                scroll.PropertyChanged += (scrollSender, scrollArgs) =>
                {
                    if (scrollArgs.Property == ScrollViewer.HorizontalScrollBarValueProperty || scrollArgs.Property == ScrollViewer.VerticalScrollBarValueProperty)
                    {
                        if (updating || thisListBox.Scroll == null || otherListBox.Scroll == null)
                        {
                            return;
                        }
                        updating = true;
                        otherListBox.Scroll.Offset = thisListBox.Scroll.Offset;
                        updating = false;
                    }
                };
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(IDisposable disposables)
        {
            var leftSide = this.FindControl<ListBox>("leftSide");
            var rightSide = this.FindControl<ListBox>("rightSide");

            updating = false;
            leftSide.PropertyChanged += (sender, args) =>
            {
                HandleListBoxPropertyChanged(args, leftSide, rightSide);
            };
            rightSide.PropertyChanged += (sender, args) =>
            {
                HandleListBoxPropertyChanged(args, rightSide, leftSide);
            };
            leftSide.PointerMoved += (sender, args) =>
            {
                HandleContextMenu(leftSide, true);
            };
            rightSide.PointerMoved += (sender, args) =>
            {
                HandleContextMenu(rightSide, false);
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
