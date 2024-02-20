// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-20-2020
//
// Last Modified By : Mario
// Last Modified On : 02-20-2024
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
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaEdit;
using DiffPlex.DiffBuilder.Model;
using IronyModManager.Common;
using IronyModManager.Common.Views;
using IronyModManager.DI;
using IronyModManager.Implementation.AvaloniaEdit;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;
using static IronyModManager.ViewModels.Controls.MergeViewerControlViewModel;

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
        /// The hotkey pressed handler
        /// </summary>
        private readonly ConflictSolverViewHotkeyPressedHandler hotkeyPressedHandler;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// The resource loader
        /// </summary>
        private readonly IResourceLoader resourceLoader;

        /// <summary>
        /// The editor left
        /// </summary>
        private IronyModManager.Controls.TextEditor editorLeft;

        /// <summary>
        /// The editor right
        /// </summary>
        private IronyModManager.Controls.TextEditor editorRight;

        /// <summary>
        /// The search panel left
        /// </summary>
        private AvaloniaEdit.Search.SearchPanel searchPanelLeft;

        /// <summary>
        /// The search panel right
        /// </summary>
        private AvaloniaEdit.Search.SearchPanel searchPanelRight;

        /// <summary>
        /// The syncing scroll
        /// </summary>
        private bool syncingScroll;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MergeViewerControlView" /> class.
        /// </summary>
        public MergeViewerControlView()
        {
            logger = DIResolver.Get<ILogger>();
            hotkeyPressedHandler = DIResolver.Get<ConflictSolverViewHotkeyPressedHandler>();
            resourceLoader = DIResolver.Get<IResourceLoader>();
            InitializeComponent();
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
        /// <param name="hoveredItem">The hovered item.</param>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void HandleContextMenu(IronyModManager.Controls.ListBox listBox, ListBoxItem hoveredItem, bool leftSide)
        {
            List<MenuItem> menuItems = null;
            if (hoveredItem != null)
            {
                if ((!ViewModel!.RightSidePatchMod && !ViewModel.LeftSidePatchMod) || ViewModel.IsReadOnlyMode)
                {
                    menuItems = GetNonEditableMenuItems(leftSide);
                }
                else
                {
                    if (leftSide)
                    {
                        menuItems = ViewModel.RightSidePatchMod ? GetActionsMenuItems(true) : GetEditableMenuItems(true);
                    }
                    else
                    {
                        menuItems = ViewModel.LeftSidePatchMod ? GetActionsMenuItems(false) : GetEditableMenuItems(false);
                    }
                }
            }

            listBox.SetContextMenuItems(menuItems);
        }

        /// <summary>
        /// Handles the listbox property changed.
        /// </summary>
        /// <param name="args">The <see cref="AvaloniaPropertyChangedEventArgs" /> instance containing the event data.</param>
        /// <param name="thisListBox">This ListBox.</param>
        /// <param name="otherListBox">Other ListBox.</param>
        protected virtual void HandleListBoxPropertyChanged(AvaloniaPropertyChangedEventArgs args, ListBox thisListBox, ListBox otherListBox)
        {
            if (args.Property == ListBox.ScrollProperty && thisListBox.Scroll != null)
            {
                var scroll = (ScrollViewer)thisListBox.Scroll;
                scroll.PropertyChanged += (_, scrollArgs) =>
                {
                    if (scrollArgs.Property == ScrollViewer.HorizontalScrollBarValueProperty || scrollArgs.Property == ScrollViewer.VerticalScrollBarValueProperty)
                    {
                        if (thisListBox.Scroll == null ||
                            otherListBox.Scroll == null ||
                            syncingScroll ||
                            (otherListBox.Scroll.Offset.X.IsNearlyEqual(thisListBox.Scroll.Offset.X) && otherListBox.Scroll.Offset.Y.IsNearlyEqual(thisListBox.Scroll.Offset.Y)))
                        {
                            return;
                        }

                        syncingScroll = true;
                        Dispatcher.UIThread.SafeInvoke(async () =>
                        {
                            await SyncScrollAsync(thisListBox, otherListBox);
                            syncingScroll = false;
                        });
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
            editorLeft = this.FindControl<IronyModManager.Controls.TextEditor>("editorLeft");
            editorRight = this.FindControl<IronyModManager.Controls.TextEditor>("editorRight");
            SetEditorOptions(editorLeft, true);
            SetEditorOptions(editorRight, false);
            searchPanelLeft = AvaloniaEdit.Search.SearchPanel.Install(editorLeft);
            searchPanelRight = AvaloniaEdit.Search.SearchPanel.Install(editorRight);
            SetEditorContextMenu(editorLeft, searchPanelLeft);
            SetEditorContextMenu(editorRight, searchPanelRight);

            var leftSide = this.FindControl<IronyModManager.Controls.ListBox>("leftSide");
            var rightSide = this.FindControl<IronyModManager.Controls.ListBox>("rightSide");

            leftSide.PropertyChanged += (_, args) => { HandleListBoxPropertyChanged(args, leftSide, rightSide); };
            rightSide.PropertyChanged += (_, args) => { HandleListBoxPropertyChanged(args, rightSide, leftSide); };
            leftSide.ContextMenuOpening += item => { HandleContextMenu(leftSide, item, true); };
            rightSide.ContextMenuOpening += item => { HandleContextMenu(rightSide, item, false); };
            ViewModel!.ConflictFound += line => { FocusConflict(line, leftSide, rightSide); };
            int? focusSideScrollItem = null;
            var previousCount = 0;
            var autoScroll = leftSide.AutoScrollToSelectedItem;
            ViewModel.PreFocusSide += left =>
            {
                leftSide.AutoScrollToSelectedItem = rightSide.AutoScrollToSelectedItem = false;
                var listBox = left ? leftSide : rightSide;
                var visibleItems = listBox.ItemContainerGenerator.Containers.ToList();
                if (visibleItems.Count != 0)
                {
                    focusSideScrollItem = visibleItems.LastOrDefault()!.Index;
                    previousCount = (left ? leftSide : rightSide).ItemCount;
                }
            };
            ViewModel.PostFocusSide += left =>
            {
                async Task delay()
                {
                    await Task.Delay(50);
                    leftSide.AutoScrollToSelectedItem = rightSide.AutoScrollToSelectedItem = autoScroll;
                    var listBox = left ? leftSide : rightSide;
                    listBox.Focus();
                    if (focusSideScrollItem.HasValue)
                    {
                        if (listBox.ItemCount != previousCount)
                        {
                            focusSideScrollItem -= Math.Abs(previousCount - listBox.ItemCount);
                        }

                        FocusConflict(-1, leftSide, rightSide);
                        if (focusSideScrollItem.GetValueOrDefault() < 0)
                        {
                            focusSideScrollItem = 0;
                        }
                        else if (focusSideScrollItem.GetValueOrDefault() >= listBox.ItemCount)
                        {
                            focusSideScrollItem = listBox.ItemCount - 1;
                        }

                        listBox.ScrollIntoView(focusSideScrollItem.GetValueOrDefault());
                    }

                    focusSideScrollItem = null;
                }

                Dispatcher.UIThread.SafeInvoke(() => delay().ConfigureAwait(false));
            };

            hotkeyPressedHandler.Subscribe(hotkey =>
            {
                DiffPieceWithIndex findItem(bool searchUp)
                {
                    var visibleItems = leftSide.ItemContainerGenerator.Containers.ToList();
                    if (visibleItems.Count != 0)
                    {
                        if (leftSide.Items is IEnumerable<DiffPieceWithIndex> items)
                        {
                            var itemsList = items.ToList();
                            if (searchUp)
                            {
                                if (visibleItems.FirstOrDefault()!.Item is DiffPieceWithIndex visibleItem)
                                {
                                    var index = itemsList.IndexOf(visibleItem) - 2;
                                    if (index < 0)
                                    {
                                        index = 0;
                                    }

                                    return itemsList[index];
                                }
                            }
                            else
                            {
                                if (visibleItems.LastOrDefault()!.Item is DiffPieceWithIndex visibleItem)
                                {
                                    var index = itemsList.IndexOf(visibleItem) + 2;
                                    if (index > leftSide.ItemCount - 1)
                                    {
                                        index = leftSide.ItemCount - 1;
                                    }

                                    return itemsList[index];
                                }
                            }
                        }
                    }

                    return null;
                }

                void evalKey()
                {
                    // Yeah, it sucks that we can't access a property from a different thread
                    if (ViewModel!.CanPerformHotKeyActions)
                    {
                        DiffPiece item = null;
                        switch (hotkey.Hotkey)
                        {
                            case Enums.HotKeys.Ctrl_Shift_Up:
                                item = findItem(true);
                                break;

                            case Enums.HotKeys.Ctrl_Shift_Down:
                                item = findItem(false);
                                break;
                        }

                        if (item != null)
                        {
                            leftSide.ScrollIntoView(item);
                        }
                    }
                }

                Dispatcher.UIThread.SafeInvoke(evalKey);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        protected override void OnLocaleChanged(string newLocale, string oldLocale)
        {
            SetEditorContextMenu(editorLeft, searchPanelLeft);
            SetEditorContextMenu(editorRight, searchPanelRight);
            base.OnLocaleChanged(newLocale, oldLocale);
        }

        /// <summary>
        /// Sets the editor options.
        /// </summary>
        /// <param name="editor">The editor.</param>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void SetEditorOptions(TextEditor editor, bool leftSide)
        {
            editor.Options = new TextEditorOptions { ConvertTabsToSpaces = true, IndentationSize = 4 };

            ViewModel.WhenAnyValue(p => p.EditingYaml).Subscribe(_ => { setEditMode(); }).DisposeWith(Disposables);
            setEditMode();

            void setEditMode()
            {
                editor.SyntaxHighlighting = ViewModel!.EditingYaml ? resourceLoader.GetYAMLDefinition() : resourceLoader.GetPDXScriptDefinition();
            }

            editor.TextChanged += (_, _) =>
            {
                var lines = editor.Text.SplitOnNewLine().ToList();
                var text = string.Join(Environment.NewLine, lines);
                ViewModel!.CurrentEditText = text;
            };
        }

        /// <summary>
        /// Synchronizes the scroll asynchronous.
        /// </summary>
        /// <param name="thisListBox">This ListBox.</param>
        /// <param name="otherListBox">Other ListBox.</param>
        /// <returns>Task.</returns>
        protected virtual Task SyncScrollAsync(ListBox thisListBox, ListBox otherListBox)
        {
            var thisMaxX = Math.Abs(thisListBox.Scroll.Extent.Width - thisListBox.Scroll.Viewport.Width);
            var thisMaxY = Math.Abs(thisListBox.Scroll.Extent.Height - thisListBox.Scroll.Viewport.Height);
            var otherMaxX = Math.Abs(otherListBox.Scroll.Extent.Width - otherListBox.Scroll.Viewport.Width);
            var otherMaxY = Math.Abs(otherListBox.Scroll.Extent.Height - otherListBox.Scroll.Viewport.Height);
            var offset = thisListBox.Scroll.Offset;
            if (thisListBox.Scroll.Offset.X > otherMaxX || thisListBox.Scroll.Offset.X.IsNearlyEqual(thisMaxX))
            {
                offset = offset.WithX(otherMaxX);
            }

            if (thisListBox.Scroll.Offset.Y > otherMaxY || thisListBox.Scroll.Offset.Y.IsNearlyEqual(thisMaxY))
            {
                offset = offset.WithY(otherMaxY);
            }

            if (!otherListBox.Scroll.Offset.X.IsNearlyEqual(offset.X) || otherListBox.Scroll.Offset.Y.IsNearlyEqual(offset.Y))
            {
                try
                {
                    otherListBox.InvalidateArrange();
                    thisListBox.InvalidateArrange();
                    otherListBox.Scroll.Offset = offset;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            return Task.FromResult(true);
        }

        /// <summary>
        /// Gets the actions menu items.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        /// <returns>List&lt;MenuItem&gt;.</returns>
        private List<MenuItem> GetActionsMenuItems(bool leftSide)
        {
            var menuItems = new List<MenuItem>();
            if (ViewModel!.EditorAvailable)
            {
                menuItems.Add(new MenuItem { Header = ViewModel.Editor, Command = ViewModel.EditorCommand, CommandParameter = !leftSide });
                menuItems.Add(new MenuItem { Header = "-" });
            }

            var mainEditingItems = new List<MenuItem>
            {
                new() { Header = ViewModel.NextConflict, Command = ViewModel.NextConflictCommand, CommandParameter = leftSide },
                new() { Header = ViewModel.PrevConflict, Command = ViewModel.PrevConflictCommand, CommandParameter = leftSide },
                new() { Header = "-" },
                new() { Header = ViewModel.CopyText, Command = ViewModel.CopyTextCommand, CommandParameter = leftSide },
                new()
                {
                    Header = "-" // Separator magic string, and it's documented... NOT really!!!
                },
                new() { Header = ViewModel.CopyAll, Command = ViewModel.CopyAllCommand, CommandParameter = leftSide },
                new() { Header = ViewModel.CopyThis, Command = ViewModel.CopyThisCommand, CommandParameter = leftSide },
                new() { Header = ViewModel.CopyThisBeforeLine, Command = ViewModel.CopyThisBeforeLineCommand, CommandParameter = leftSide },
                new() { Header = ViewModel.CopyThisAfterLine, Command = ViewModel.CopyThisAfterLineCommand, CommandParameter = leftSide },
                new() { Header = "-" },
                new() { Header = ViewModel.UsingNewMergeType ? ViewModel.UseNewMergeTypeCaption : ViewModel.UseOldMergeTypeCaption, Command = ViewModel.ToggleMergeTypeCommand }
            };
            menuItems.AddRange(mainEditingItems);

            return menuItems;
        }

        /// <summary>
        /// Gets the editable menu items.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        /// <returns>List&lt;MenuItem&gt;.</returns>
        private List<MenuItem> GetEditableMenuItems(bool leftSide)
        {
            var menuItems = new List<MenuItem>();
            if (ViewModel!.EditorAvailable)
            {
                menuItems.Add(new MenuItem { Header = ViewModel.Editor, Command = ViewModel.EditorCommand, CommandParameter = leftSide });
                menuItems.Add(new MenuItem { Header = "-" });
            }

            var mainEditingItems = new List<MenuItem>
            {
                new() { Header = ViewModel.NextConflict, Command = ViewModel.NextConflictCommand, CommandParameter = leftSide },
                new() { Header = ViewModel.PrevConflict, Command = ViewModel.PrevConflictCommand, CommandParameter = leftSide },
                new() { Header = "-" },
                new() { Header = ViewModel.EditThis, Command = ViewModel.EditThisCommand, CommandParameter = leftSide },
                new() { Header = ViewModel.CopyText, Command = ViewModel.CopyTextCommand, CommandParameter = leftSide },
                new() { Header = "-" },
                new() { Header = ViewModel.DeleteText, Command = ViewModel.DeleteTextCommand, CommandParameter = leftSide },
                new() { Header = ViewModel.MoveUp, Command = ViewModel.MoveUpCommand, CommandParameter = leftSide },
                new() { Header = ViewModel.MoveDown, Command = ViewModel.MoveDownCommand, CommandParameter = leftSide },
                new() { Header = "-" },
                new() { Header = ViewModel.UsingNewMergeType ? ViewModel.UseNewMergeTypeCaption : ViewModel.UseOldMergeTypeCaption, Command = ViewModel.ToggleMergeTypeCommand }
            };
            menuItems.AddRange(mainEditingItems);

            var redoAvailable = ViewModel.IsRedoAvailable();
            var undoAvailable = ViewModel.IsUndoAvailable();
            if (redoAvailable || undoAvailable)
            {
                menuItems.Add(new MenuItem { Header = "-" });
                if (undoAvailable)
                {
                    menuItems.Add(new MenuItem { Header = ViewModel.Undo, Command = ViewModel.UndoCommand, CommandParameter = leftSide });
                }

                if (redoAvailable)
                {
                    menuItems.Add(new MenuItem { Header = ViewModel.Redo, Command = ViewModel.RedoCommand, CommandParameter = leftSide });
                }
            }

            return menuItems;
        }

        /// <summary>
        /// Gets the non-editable menu items.
        /// </summary>
        /// <param name="leftSide">The left side.</param>
        /// <returns>System.Collections.Generic.List&lt;Avalonia.Controls.MenuItem&gt;.</returns>
        private List<MenuItem> GetNonEditableMenuItems(bool leftSide)
        {
            var menuItems = new List<MenuItem>();
            if (ViewModel!.EditorAvailable)
            {
                menuItems.Add(new MenuItem { Header = ViewModel.ReadOnlyEditor, Command = ViewModel.ReadOnlyEditorCommand, CommandParameter = leftSide });
                menuItems.Add(new MenuItem { Header = "-" });
            }

            var mainEditingItems = new List<MenuItem>
            {
                new() { Header = ViewModel.NextConflict, Command = ViewModel.NextConflictCommand, CommandParameter = leftSide },
                new() { Header = ViewModel.PrevConflict, Command = ViewModel.PrevConflictCommand, CommandParameter = leftSide },
                new() { Header = "-" },
                new() { Header = ViewModel.CopyText, Command = ViewModel.CopyTextCommand, CommandParameter = leftSide }
            };
            menuItems.AddRange(mainEditingItems);

            return menuItems;
        }

        /// <summary>
        /// Handles the editor find or replace.
        /// </summary>
        /// <param name="textEditor">The text editor.</param>
        /// <param name="searchPanel">The search panel.</param>
        /// <param name="isReplaceMode">if set to <c>true</c> [if replace mode].</param>
        private void HandleEditorFindOrReplace(IronyModManager.Controls.TextEditor textEditor, AvaloniaEdit.Search.SearchPanel searchPanel, bool isReplaceMode)
        {
            if (searchPanel == null || textEditor == null)
            {
                return;
            }

            searchPanel.IsReplaceMode = isReplaceMode;
            searchPanel.Open();
            if (!(textEditor.TextArea.Selection.IsEmpty || textEditor.TextArea.Selection.IsMultiline))
            {
                searchPanel.SearchPattern = textEditor.TextArea.Selection.GetText();
            }

            Dispatcher.UIThread.Post(searchPanel.Reactivate, DispatcherPriority.Input);
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Sets the editor context menu.
        /// </summary>
        /// <param name="textEditor">The text editor.</param>
        /// <param name="searchPanel">The search panel.</param>
        private void SetEditorContextMenu(IronyModManager.Controls.TextEditor textEditor, AvaloniaEdit.Search.SearchPanel searchPanel)
        {
            if (textEditor == null || searchPanel == null)
            {
                return;
            }

            var ctx = new MenuFlyout
            {
                Items = new List<MenuItem>
                {
                    new() { Header = ViewModel!.EditorCopy, Command = ReactiveCommand.Create(textEditor.Copy).DisposeWith(Disposables) },
                    new() { Header = ViewModel.EditorCut, Command = ReactiveCommand.Create(textEditor.Cut).DisposeWith(Disposables) },
                    new() { Header = ViewModel.EditorPaste, Command = ReactiveCommand.Create(textEditor.Paste).DisposeWith(Disposables) },
                    new() { Header = ViewModel.EditorDelete, Command = ReactiveCommand.Create(textEditor.Delete).DisposeWith(Disposables) },
                    new() { Header = ViewModel.EditorSelectAll, Command = ReactiveCommand.Create(textEditor.SelectAll).DisposeWith(Disposables) },
                    new() { Header = "-" },
                    new() { Header = ViewModel.EditorUndo, Command = ReactiveCommand.Create(textEditor.Undo).DisposeWith(Disposables) },
                    new() { Header = ViewModel.EditorRedo, Command = ReactiveCommand.Create(textEditor.Redo).DisposeWith(Disposables) },
                    new() { Header = "-" },
                    new() { Header = ViewModel.EditorFind, Command = ReactiveCommand.Create(() => HandleEditorFindOrReplace(textEditor, searchPanel, false)).DisposeWith(Disposables) },
                    new() { Header = ViewModel.EditorReplace, Command = ReactiveCommand.Create(() => HandleEditorFindOrReplace(textEditor, searchPanel, true)).DisposeWith(Disposables) }
                }
            };
            textEditor.ContextFlyout = ctx;
        }

        #endregion Methods
    }
}
