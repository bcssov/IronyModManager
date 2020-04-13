// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-20-2020
//
// Last Modified By : Mario
// Last Modified On : 04-13-2020
// ***********************************************************************
// <copyright file="MergeViewerControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using IronyModManager.Common.Views;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class MergeViewerControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.MergeViewerControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.MergeViewerControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
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
        /// Focuses the conflict.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="leftListBox">The left ListBox.</param>
        /// <param name="rightListBox">The right ListBox.</param>
        protected virtual void FocusConflict(int line, ListBox leftListBox, ListBox rightListBox)
        {
            leftListBox.SelectedIndex = line;
            rightListBox.SelectedIndex = line;
        }

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
                var grid = hoveredItem.GetLogicalChildren().OfType<Grid>().FirstOrDefault();
                if (grid != null)
                {
                    if (!ViewModel.RightSidePatchMod && !ViewModel.LeftSidePatchMod)
                    {
                        grid.ContextMenu.Items = GetNonEditableMenuItems(leftSide);
                    }
                    else
                    {
                        if (leftSide)
                        {
                            grid.ContextMenu.Items = ViewModel.RightSidePatchMod ? GetActionsMenuItems(leftSide) : GetEditableMenuItems(leftSide);
                        }
                        else
                        {
                            grid.ContextMenu.Items = ViewModel.LeftSidePatchMod ? GetActionsMenuItems(leftSide) : GetEditableMenuItems(leftSide);
                        }
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
                        var otherMaxX = Math.Abs(otherListBox.Scroll.Extent.Width - otherListBox.Scroll.Viewport.Width);
                        var otherMaxY = Math.Abs(otherListBox.Scroll.Extent.Height - otherListBox.Scroll.Viewport.Height);
                        var offset = thisListBox.Scroll.Offset;
                        otherListBox.Scroll.Offset = offset;
                        if (thisListBox.Scroll.Offset.X > otherMaxX)
                        {
                            offset = offset.WithX(otherMaxX);
                        }
                        if (thisListBox.Scroll.Offset.Y > otherMaxY)
                        {
                            offset = offset.WithY(otherMaxY);
                        }
                        if (otherListBox.Scroll.Offset.X != offset.X || otherListBox.Scroll.Offset.Y != offset.Y)
                        {
                            otherListBox.InvalidateArrange();
                            thisListBox.InvalidateArrange();
                            otherListBox.Scroll.Offset = offset;
                        }
                        updating = false;
                    }
                };
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
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
            ViewModel.ConflictFound += (line) =>
            {
                FocusConflict(line, leftSide, rightSide);
            };

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Gets the actions menu items.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        /// <returns>List&lt;MenuItem&gt;.</returns>
        private List<MenuItem> GetActionsMenuItems(bool leftSide)
        {
            var menuItems = new List<MenuItem>()
            {
                new MenuItem()
                {
                    Header = ViewModel.NextConflict,
                    Command = ViewModel.NextConflictCommand,
                    CommandParameter = leftSide
                },
                new MenuItem()
                {
                    Header = ViewModel.PrevConflict,
                    Command = ViewModel.PrevConflictCommand,
                    CommandParameter = leftSide
                },
                new MenuItem()
                {
                    Header = "-"
                },
                new MenuItem()
                {
                    Header = ViewModel.CopyText,
                    Command = ViewModel.CopyTextCommand,
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
            return menuItems;
        }

        /// <summary>
        /// Gets the editable menu items.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        /// <returns>List&lt;MenuItem&gt;.</returns>
        private List<MenuItem> GetEditableMenuItems(bool leftSide)
        {
            var menuItems = new List<MenuItem>()
            {
                new MenuItem()
                {
                    Header = ViewModel.NextConflict,
                    Command = ViewModel.NextConflictCommand,
                    CommandParameter = leftSide
                },
                new MenuItem()
                {
                    Header = ViewModel.PrevConflict,
                    Command = ViewModel.PrevConflictCommand,
                    CommandParameter = leftSide
                },
                new MenuItem()
                {
                    Header = "-"
                },
                new MenuItem()
                {
                    Header = ViewModel.EditThis,
                    Command = ViewModel.EditThisCommand,
                    CommandParameter = leftSide
                },
                new MenuItem()
                {
                    Header = ViewModel.CopyText,
                    Command = ViewModel.CopyTextCommand,
                    CommandParameter = leftSide
                },
                new MenuItem()
                {
                    Header = "-"
                },
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
            };
            return menuItems;
        }

        /// <summary>
        /// Gets the non editable menu items.
        /// </summary>
        /// <param name="leftSide">The left side.</param>
        /// <returns>System.Collections.Generic.List&lt;Avalonia.Controls.MenuItem&gt;.</returns>
        private List<MenuItem> GetNonEditableMenuItems(bool leftSide)
        {
            var menuItems = new List<MenuItem>()
            {
                new MenuItem()
                {
                    Header = ViewModel.NextConflict,
                    Command = ViewModel.NextConflictCommand,
                    CommandParameter = leftSide
                },
                new MenuItem()
                {
                    Header = ViewModel.PrevConflict,
                    Command = ViewModel.PrevConflictCommand,
                    CommandParameter = leftSide
                },
                new MenuItem()
                {
                    Header = "-"
                },
                new MenuItem()
                {
                    Header = ViewModel.CopyText,
                    Command = ViewModel.CopyTextCommand,
                    CommandParameter = leftSide
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
