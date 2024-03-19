// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-20-2020
//
// Last Modified By : Mario
// Last Modified On : 03-19-2024
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
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Document;
using AvaloniaEdit.Search;
using AvaloniaEdit.Utils;
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
        /// A private IronyModManager.Controls.TextEditor named diffLeft.
        /// </summary>
        private IronyModManager.Controls.TextEditor diffLeft;

        /// <summary>
        /// A private IronyModManager.Controls.TextEditor named diffRight.
        /// </summary>
        private IronyModManager.Controls.TextEditor diffRight;

        /// <summary>
        /// A private AvaloniaEdit.Search.SearchPanel named diffSearchPanelLeft.
        /// </summary>
        private SearchPanel diffSearchPanelLeft;

        /// <summary>
        /// A private AvaloniaEdit.Search.SearchPanel named diffSearchPanelRight.
        /// </summary>
        private SearchPanel diffSearchPanelRight;

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
        private SearchPanel editorSearchPanelLeft;

        /// <summary>
        /// The search panel right
        /// </summary>
        private SearchPanel editorSearchPanelRight;

        /// <summary>
        /// A private bool named syncingDiffScroll.
        /// </summary>
        private bool syncingDiffScroll;

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
            diffLeft.ScrollToLine(line);
            diffRight.ScrollToLine(line);
            ViewModel!.DiffEditorTopLine = diffLeft.GetTopVisibleLine();
            ViewModel.DiffEditorBottomLine = diffLeft.GetBottomVisibleLine();
            if (!(ViewModel.DiffEditorTopLine <= line && ViewModel.DiffEditorBottomLine >= line))
            {
                diffLeft.TextArea.Caret.Location = new TextLocation(line, 1);
                diffRight.TextArea.Caret.Location = new TextLocation(line, 1);
            }
        }

        /// <summary>
        /// Gets a text editor selected text range.
        /// </summary>
        /// <param name="editor">The editor.</param>
        /// <returns>a Tuple with  int,int .<see cref="Tuple{int, int}" /></returns>
        protected virtual Tuple<int, int> GetTextEditorSelectedTextRange(TextEditor editor)
        {
            var segments = editor.TextArea.Selection.Segments;
            var doc = editor.TextArea.Document;
            foreach (var segment in segments)
            {
                var start = doc.GetLineByOffset(segment.StartOffset).LineNumber;
                var end = doc.GetLineByOffset(segment.EndOffset).LineNumber;
                var min = Math.Min(start, end);
                var max = Math.Max(start, end);
                return Tuple.Create(min - 1, max - 1);
            }

            return Tuple.Create(-1, -1);
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
        /// Handles an editor context menu.
        /// </summary>
        /// <param name="editor">The editor.</param>
        /// <param name="searchPanel">The search panel.</param>
        /// <param name="leftSide">If true, left side.</param>
        protected virtual void HandleEditorContextMenu(IronyModManager.Controls.TextEditor editor, SearchPanel searchPanel, bool leftSide)
        {
            if (editor == null || searchPanel == null)
            {
                return;
            }

            var ctx = new MenuFlyout { Items = new List<MenuItem>() };

            void setMenuItems()
            {
                var canReplace = false;
                List<MenuItem> menuItems;
                if ((!ViewModel!.RightSidePatchMod && !ViewModel.LeftSidePatchMod) || ViewModel.IsReadOnlyMode)
                {
                    menuItems = GetNonEditableMenuItems(leftSide);
                }
                else
                {
                    if (leftSide)
                    {
                        menuItems = ViewModel.RightSidePatchMod ? GetActionsMenuItems(true) : GetEditableMenuItems(true);
                        canReplace = ViewModel.LeftSidePatchMod;
                    }
                    else
                    {
                        menuItems = ViewModel.LeftSidePatchMod ? GetActionsMenuItems(false) : GetEditableMenuItems(false);
                        canReplace = ViewModel.RightSidePatchMod;
                    }
                }

                menuItems.AddRange(
                [
                    new MenuItem { Header = "-" },
                    new MenuItem { Header = ViewModel.EditorFind, Command = ReactiveCommand.Create(() => HandleEditorFindOrReplace(editor, searchPanel, false)).DisposeWith(Disposables) }
                ]);
                if (canReplace)
                {
                    menuItems.Add(new MenuItem { Header = ViewModel.EditorReplace, Command = ReactiveCommand.Create(() => HandleEditorFindOrReplace(editor, searchPanel, true)).DisposeWith(Disposables) });
                }

                ctx.Items = menuItems;
            }

            editor.ContextFlyout = ctx;
            editor.ContextFlyout.Opening += (_, _) => setMenuItems();
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
        /// Handles a text editor property changed.
        /// </summary>
        /// <param name="thisTextEditor">This text editor.</param>
        /// <param name="otherTextEditor">Other text editor.</param>
        protected virtual void HandleTextEditorPropertyChanged(IronyModManager.Controls.TextEditor thisTextEditor, IronyModManager.Controls.TextEditor otherTextEditor)
        {
            thisTextEditor.ScrollViewer.ScrollChanged += (_, _) =>
            {
                if (syncingDiffScroll || thisTextEditor.ScrollViewer == null || otherTextEditor.ScrollViewer == null || (otherTextEditor.ScrollViewer.Offset.X.IsNearlyEqual(thisTextEditor.ScrollViewer.Offset.X) &&
                                                                                                                         otherTextEditor.ScrollViewer.Offset.Y.IsNearlyEqual(thisTextEditor.ScrollViewer.Offset.Y)))
                {
                    return;
                }

                syncingDiffScroll = true;
                Dispatcher.UIThread.SafeInvoke(async () =>
                {
                    await SyncDiffScrollAsync(thisTextEditor, otherTextEditor);
                    ViewModel!.DiffEditorTopLine = thisTextEditor.GetTopVisibleLine();
                    ViewModel.DiffEditorBottomLine = thisTextEditor.GetBottomVisibleLine();
                    syncingDiffScroll = false;
                });
            };
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            editorLeft = this.FindControl<IronyModManager.Controls.TextEditor>("editorLeft");
            editorRight = this.FindControl<IronyModManager.Controls.TextEditor>("editorRight");
            SetEditorOptions(editorLeft);
            SetEditorOptions(editorRight);
            editorSearchPanelLeft = SearchPanel.Install(editorLeft);
            editorSearchPanelRight = SearchPanel.Install(editorRight);
            SetEditorContextMenu(editorLeft, editorSearchPanelLeft);
            SetEditorContextMenu(editorRight, editorSearchPanelRight);

            diffLeft = this.FindControl<IronyModManager.Controls.TextEditor>("diffLeft");
            diffRight = this.FindControl<IronyModManager.Controls.TextEditor>("diffRight");
            SetDiffOptions(diffLeft, true);
            SetDiffOptions(diffRight, false);
            diffSearchPanelLeft = SearchPanel.Install(diffLeft);
            diffSearchPanelRight = SearchPanel.Install(diffRight);
            HandleEditorContextMenu(diffLeft, diffSearchPanelLeft, true);
            HandleEditorContextMenu(diffRight, diffSearchPanelRight, false);
            diffLeft.ScrollInitialized += (_, _) => HandleTextEditorPropertyChanged(diffLeft, diffRight);
            diffRight.ScrollInitialized += (_, _) => HandleTextEditorPropertyChanged(diffRight, diffLeft);
            var diffLeftMargin = new DiffMargin { Lines = ViewModel!.LeftDiff, ColorConverter = ViewModel.ColorConverter };
            var diffRightMargin = new DiffMargin { Lines = ViewModel.RightDiff, ColorConverter = ViewModel.ColorConverter };
            var diffLeftRenderer = new DiffBackgroundRenderer { Lines = ViewModel.LeftDiff, ColorConverter = ViewModel.ColorConverter };
            var diffRightRenderer = new DiffBackgroundRenderer { Lines = ViewModel.RightDiff, ColorConverter = ViewModel.ColorConverter };
            diffLeft.TextArea.LeftMargins.Add(diffLeftMargin);
            diffLeft.TextArea.TextView.BackgroundRenderers.Add(diffLeftRenderer);
            diffRight.TextArea.LeftMargins.Add(diffRightMargin);
            diffRight.TextArea.TextView.BackgroundRenderers.Add(diffRightRenderer);
            diffLeft.SafeSetText(string.Join(Environment.NewLine, ViewModel.LeftDiff));
            diffRight.SafeSetText(string.Join(Environment.NewLine, ViewModel.RightDiff));
            diffLeft.IsReadOnly = !ViewModel.LeftSidePatchMod;
            diffRight.IsReadOnly = !ViewModel.RightSidePatchMod;

            var leftSide = this.FindControl<IronyModManager.Controls.ListBox>("leftSide");
            var rightSide = this.FindControl<IronyModManager.Controls.ListBox>("rightSide");
            leftSide.PropertyChanged += (_, args) => { HandleListBoxPropertyChanged(args, leftSide, rightSide); };
            rightSide.PropertyChanged += (_, args) => { HandleListBoxPropertyChanged(args, rightSide, leftSide); };
            leftSide.ContextMenuOpening += item => { HandleContextMenu(leftSide, item, true); };
            rightSide.ContextMenuOpening += item => { HandleContextMenu(rightSide, item, false); };

            ViewModel!.ConflictFound += line => { FocusConflict(line, leftSide, rightSide); };

            var diffHotkeyActionLeft = -1;
            var diffHotkeyActionRight = -1;
            int? focusSideScrollItem = null;
            var previousCount = 0;
            var autoScroll = leftSide.AutoScrollToSelectedItem;
            ViewModel.PreFocusSide += left =>
            {
                if (ViewModel.UsingNewMergeType)
                {
                    diffHotkeyActionLeft = diffLeft.GetMiddleVisibleLine();
                    diffHotkeyActionRight = diffRight.GetMiddleVisibleLine();
                }
                else
                {
                    leftSide.AutoScrollToSelectedItem = rightSide.AutoScrollToSelectedItem = false;
                    var listBox = left ? leftSide : rightSide;
                    var visibleItems = listBox.ItemContainerGenerator.Containers.ToList();
                    if (visibleItems.Count != 0)
                    {
                        focusSideScrollItem = visibleItems.LastOrDefault()!.Index;
                        previousCount = (left ? leftSide : rightSide).ItemCount;
                    }
                }
            };
            ViewModel.PostFocusSide += left =>
            {
                async Task delay()
                {
                    await Task.Delay(50);
                    if (ViewModel!.UsingNewMergeType)
                    {
                        diffLeft.ScrollToLine(diffHotkeyActionLeft);
                        diffRight.ScrollToLine(diffHotkeyActionRight);
                        ViewModel!.DiffEditorTopLine = diffLeft.GetTopVisibleLine();
                        ViewModel.DiffEditorBottomLine = diffLeft.GetBottomVisibleLine();
                        if (!(ViewModel.DiffEditorTopLine <= diffHotkeyActionLeft && ViewModel.DiffEditorBottomLine >= diffHotkeyActionLeft))
                        {
                            diffLeft.TextArea.Caret.Location = new TextLocation(diffHotkeyActionLeft, 1);
                            diffRight.TextArea.Caret.Location = new TextLocation(diffHotkeyActionLeft, 1);
                        }

                        RedrawEditorDiffs();
                    }
                    else
                    {
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
                        if (ViewModel.UsingNewMergeType)
                        {
                            switch (hotkey.Hotkey)
                            {
                                case Enums.HotKeys.Ctrl_Shift_Up:
                                    diffLeft.ScrollViewer.LineUp();
                                    break;
                                case Enums.HotKeys.Ctrl_Shift_Down:
                                    diffLeft.ScrollViewer.LineDown();
                                    break;
                            }
                        }
                        else
                        {
                            DiffPiece item = hotkey.Hotkey switch
                            {
                                Enums.HotKeys.Ctrl_Shift_Up => findItem(true),
                                Enums.HotKeys.Ctrl_Shift_Down => findItem(false),
                                _ => null
                            };

                            if (item != null)
                            {
                                leftSide.ScrollIntoView(item);
                            }
                        }
                    }
                }

                Dispatcher.UIThread.SafeInvoke(evalKey);
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.LeftDiff).Subscribe(s =>
            {
                diffLeftMargin.Lines = s;
                diffLeftRenderer.Lines = s;

                var oldText = diffLeft.Text;
                var newText = string.Join(Environment.NewLine, s.Select(p => p.Text));
                if (oldText.Equals(newText))
                {
                    return;
                }

                var lineDeleted = ViewModel!.PreviousLeftDiff != null && s != null && s.Count < ViewModel!.PreviousLeftDiff.Count && s.Count > 0 && ViewModel!.PreviousLeftDiff.Count > 0;
                var locLine = diffLeft.TextArea.Caret.Location.Line;
                if (lineDeleted)
                {
                    locLine--;
                    if (locLine < 1)
                    {
                        locLine = 1;
                    }
                }

                var locColumn = diffLeft.TextArea.Caret.Location.Column;
                diffLeft.SafeSetText(newText);

                if (lineDeleted)
                {
                    locColumn = diffLeft.GetLastColumnByLine(locLine);
                }

                diffLeft.TextArea.Caret.Location = new TextLocation(locLine, locColumn);
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.RightDiff).Subscribe(s =>
            {
                diffRightMargin.Lines = s;
                diffRightRenderer.Lines = s;

                var oldText = diffRight.Text;
                var newText = string.Join(Environment.NewLine, s.Select(p => p.Text));
                if (oldText.Equals(newText))
                {
                    return;
                }

                var lineDeleted = ViewModel!.PreviousRightDiff != null && s != null && s.Count < ViewModel!.PreviousRightDiff.Count && s.Count > 0 && ViewModel!.PreviousRightDiff.Count > 0;
                var locLine = diffRight.TextArea.Caret.Location.Line;
                if (lineDeleted)
                {
                    locLine--;
                    if (locLine < 1)
                    {
                        locLine = 1;
                    }
                }

                var locColumn = diffRight.TextArea.Caret.Location.Column;
                diffRight.SafeSetText(newText);

                if (lineDeleted)
                {
                    locColumn = diffRight.GetLastColumnByLine(locLine);
                }

                diffRight.TextArea.Caret.Location = new TextLocation(locLine, locColumn);
            });

            this.WhenAnyValue(v => v.ViewModel.LeftSidePatchMod).Subscribe(s =>
            {
                diffLeft.IsReadOnly = !s;
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.ViewModel.RightSidePatchMod).Subscribe(s =>
            {
                diffRight.IsReadOnly = !s;
            }).DisposeWith(disposables);

            ViewModel!.HotkeyBeforePerform += (_, _) =>
            {
                diffHotkeyActionLeft = diffLeft.GetMiddleVisibleLine();
                diffHotkeyActionRight = diffRight.GetMiddleVisibleLine();
            };
            ViewModel.HotkeyAfterPerform += (_, _) =>
            {
                diffLeft.ScrollToLine(diffHotkeyActionLeft);
                diffRight.ScrollToLine(diffHotkeyActionRight);
                ViewModel!.DiffEditorTopLine = diffLeft.GetTopVisibleLine();
                ViewModel.DiffEditorBottomLine = diffLeft.GetBottomVisibleLine();
                if (!(ViewModel.DiffEditorTopLine <= diffHotkeyActionLeft && ViewModel.DiffEditorBottomLine >= diffHotkeyActionLeft))
                {
                    diffLeft.TextArea.Caret.Location = new TextLocation(diffHotkeyActionLeft, 1);
                    diffRight.TextArea.Caret.Location = new TextLocation(diffHotkeyActionLeft, 1);
                }

                RedrawEditorDiffs();
            };
            this.WhenAnyValue(v => v.ViewModel.ColorConverter).Subscribe(s =>
            {
                diffLeftMargin.ColorConverter = s;
                diffRightMargin.ColorConverter = s;
                diffLeftRenderer.ColorConverter = s;
                diffRightRenderer.ColorConverter = s;
                Dispatcher.UIThread.SafeInvoke(RedrawEditorDiffs);
            });

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        protected override void OnLocaleChanged(string newLocale, string oldLocale)
        {
            SetEditorContextMenu(editorLeft, editorSearchPanelLeft);
            SetEditorContextMenu(editorRight, editorSearchPanelRight);
            HandleEditorContextMenu(diffLeft, diffSearchPanelLeft, true);
            HandleEditorContextMenu(diffRight, diffSearchPanelRight, false);
            base.OnLocaleChanged(newLocale, oldLocale);
        }

        /// <summary>
        /// Sets a diff options.
        /// </summary>
        /// <param name="diff">The diff.</param>
        /// <param name="leftDiff">If true, left diff.</param>
        protected virtual void SetDiffOptions(TextEditor diff, bool leftDiff)
        {
            diff.Options = new TextEditorOptions { ConvertTabsToSpaces = true, IndentationSize = 4 };
            ViewModel.WhenAnyValue(p => p.EditingYaml).Subscribe(_ => { setEditMode(); }).DisposeWith(Disposables);
            setEditMode();

            void setEditMode()
            {
                diff.SyntaxHighlighting = ViewModel!.EditingYaml ? resourceLoader.GetYAMLDefinition() : resourceLoader.GetPDXScriptDefinition();
            }

            diff.TextChanged += (_, _) =>
            {
                var lines = diff.Text.SplitOnNewLine().ToList();
                var text = string.Join(Environment.NewLine, lines);
                text = text.Trim().Trim(Environment.NewLine);
                if (leftDiff)
                {
                    if (text.Equals(ViewModel!.LeftSide.Trim().Trim(Environment.NewLine)))
                    {
                        return;
                    }
                }
                else
                {
                    if (text.Equals(ViewModel!.RightSide.Trim().Trim(Environment.NewLine)))
                    {
                        return;
                    }
                }

                if (leftDiff)
                {
                    ViewModel!.SetText(text, ViewModel.RightSide);
                    RedrawEditorDiffs();
                }
                else
                {
                    ViewModel!.SetText(ViewModel.LeftSide, text);
                    RedrawEditorDiffs();
                }
            };
            diff.TextArea.SelectionChanged += (_, _) =>
            {
                var range = GetTextEditorSelectedTextRange(diff);
                if (range.Item1 == -1 || range.Item2 == -1)
                {
                    return;
                }

                var col = leftDiff ? ViewModel!.LeftSideSelected : ViewModel!.RightSideSelected;
                var sourceCol = leftDiff ? ViewModel.LeftDiff : ViewModel.RightDiff;

                if (range.Item1 > sourceCol.Count - 1 || range.Item2 > sourceCol.Count)
                {
                    return;
                }

                col.Clear();
                for (var i = range.Item1; i < range.Item2 + 1; i++)
                {
                    col.Add(sourceCol[i]);
                }
            };

            void handlePointerPressed(PointerPressedEventArgs args)
            {
                var pos = args.GetPosition(diff);
                pos = new Point(0, pos.Y.CoerceValue(0, diff.Bounds.Height) + diff.VerticalOffset);
                var line = diff.TextArea.TextView.GetVisualLineFromVisualTop(pos.Y);
                if (line != null)
                {
                    var col = leftDiff ? ViewModel!.LeftSideSelected : ViewModel!.RightSideSelected;
                    var sourceCol = leftDiff ? ViewModel.LeftDiff : ViewModel.RightDiff;
                    var idx = line.FirstDocumentLine.LineNumber - 1;
                    if (idx >= sourceCol.Count)
                    {
                        return;
                    }

                    if (col.Count == 0)
                    {
                        col.Add(sourceCol[idx]);
                    }
                    else
                    {
                        var item = sourceCol[idx];
                        if (!col.Contains(item))
                        {
                            col.Clear();
                            col.Add(sourceCol[idx]);
                        }
                    }
                }
            }

            diff.PointerPressed += (_, args) =>
            {
                handlePointerPressed(args);
            };

            ((IronyModManager.Controls.TextArea)diff.TextArea).LeftPointerPressed += (_, eventArgs) =>
            {
                handlePointerPressed(eventArgs);
            };
        }

        /// <summary>
        /// Sets an editor options.
        /// </summary>
        /// <param name="editor">The editor.</param>
        protected virtual void SetEditorOptions(TextEditor editor)
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
        /// Syncs a diff scroll async.
        /// </summary>
        /// <param name="thisTextEditor">This text editor.</param>
        /// <param name="otherTextEditor">The other text editor.</param>
        /// <returns>A Task.<see cref="Task" /></returns>
        protected virtual Task SyncDiffScrollAsync(IronyModManager.Controls.TextEditor thisTextEditor, IronyModManager.Controls.TextEditor otherTextEditor)
        {
            var thisMaxX = Math.Abs(thisTextEditor.ScrollViewer.Extent.Width - thisTextEditor.ScrollViewer.Viewport.Width);
            var thisMaxY = Math.Abs(thisTextEditor.ScrollViewer.Extent.Height - thisTextEditor.ScrollViewer.Viewport.Height);
            var otherMaxX = Math.Abs(otherTextEditor.ScrollViewer.Extent.Width - otherTextEditor.ScrollViewer.Viewport.Width);
            var otherMaxY = Math.Abs(otherTextEditor.ScrollViewer.Extent.Height - otherTextEditor.ScrollViewer.Viewport.Height);
            var offset = thisTextEditor.ScrollViewer.Offset;
            if (thisTextEditor.ScrollViewer.Offset.X > otherMaxX || thisTextEditor.ScrollViewer.Offset.X.IsNearlyEqual(thisMaxX))
            {
                offset = offset.WithX(otherMaxX);
            }

            if (thisTextEditor.ScrollViewer.Offset.Y > otherMaxY || thisTextEditor.ScrollViewer.Offset.Y.IsNearlyEqual(thisMaxY))
            {
                offset = offset.WithY(otherMaxY);
            }

            if (!otherTextEditor.ScrollViewer.Offset.X.IsNearlyEqual(offset.X) || !otherTextEditor.ScrollViewer.Offset.Y.IsNearlyEqual(offset.Y))
            {
                try
                {
                    otherTextEditor.InvalidateArrange();
                    thisTextEditor.InvalidateArrange();
                    otherTextEditor.ScrollViewer.Offset = offset;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            return Task.CompletedTask;
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

            if (!otherListBox.Scroll.Offset.X.IsNearlyEqual(offset.X) || !otherListBox.Scroll.Offset.Y.IsNearlyEqual(offset.Y))
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

            return Task.CompletedTask;
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
                menuItems.Add(new MenuItem { Header = ViewModel.ToggleMergeTypeCaption, Command = ViewModel.ToggleMergeTypeCommand });
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
                new()
                {
                    Header = ViewModel.CopyAll,
                    Command = ReactiveCommand.Create(() =>
                    {
                        var visibleLine = diffLeft.GetMiddleVisibleLine();
                        ViewModel.CopyAllCommand.Execute(leftSide).Subscribe();
                        diffLeft.ScrollToLine(visibleLine);
                        diffRight.ScrollToLine(visibleLine);
                        ViewModel!.DiffEditorTopLine = diffLeft.GetTopVisibleLine();
                        ViewModel.DiffEditorBottomLine = diffLeft.GetBottomVisibleLine();
                        if (!(ViewModel.DiffEditorTopLine <= visibleLine && ViewModel.DiffEditorBottomLine >= visibleLine))
                        {
                            diffLeft.TextArea.Caret.Location = new TextLocation(visibleLine, 1);
                            diffRight.TextArea.Caret.Location = new TextLocation(visibleLine, 1);
                        }

                        RedrawEditorDiffs();
                    })
                },
                new()
                {
                    Header = ViewModel.CopyThis,
                    Command = ReactiveCommand.Create(() =>
                    {
                        ViewModel.CopyThisCommand.Execute(leftSide).Subscribe();
                        RedrawEditorDiffs();
                    })
                },
                new()
                {
                    Header = ViewModel.CopyThisBeforeLine,
                    Command = ReactiveCommand.Create(() =>
                    {
                        ViewModel.CopyThisBeforeLineCommand.Execute(leftSide).Subscribe();
                        RedrawEditorDiffs();
                    })
                },
                new()
                {
                    Header = ViewModel.CopyThisAfterLine,
                    Command = ReactiveCommand.Create(() =>
                    {
                        ViewModel.CopyThisAfterLineCommand.Execute(leftSide).Subscribe();
                        RedrawEditorDiffs();
                    })
                }
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
                menuItems.Add(new MenuItem { Header = ViewModel.ToggleMergeTypeCaption, Command = ViewModel.ToggleMergeTypeCommand });
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
                new() { Header = ViewModel.MoveDown, Command = ViewModel.MoveDownCommand, CommandParameter = leftSide }
            };
            menuItems.AddRange(mainEditingItems);

            var redoAvailable = ViewModel.IsRedoAvailable();
            var undoAvailable = ViewModel.IsUndoAvailable();
            if (redoAvailable || undoAvailable)
            {
                menuItems.Add(new MenuItem { Header = "-" });
                if (undoAvailable)
                {
                    menuItems.Add(new MenuItem
                    {
                        Header = ViewModel.Undo,
                        Command = ReactiveCommand.Create(() =>
                        {
                            var visibleLine = diffLeft.GetMiddleVisibleLine();
                            ViewModel.UndoCommand.Execute(leftSide).Subscribe();
                            diffLeft.ScrollToLine(visibleLine);
                            diffRight.ScrollToLine(visibleLine);
                            ViewModel!.DiffEditorTopLine = diffLeft.GetTopVisibleLine();
                            ViewModel.DiffEditorBottomLine = diffLeft.GetBottomVisibleLine();
                            if (!(ViewModel.DiffEditorTopLine <= visibleLine && ViewModel.DiffEditorBottomLine >= visibleLine))
                            {
                                diffLeft.TextArea.Caret.Location = new TextLocation(visibleLine, 1);
                                diffRight.TextArea.Caret.Location = new TextLocation(visibleLine, 1);
                            }

                            RedrawEditorDiffs();
                        })
                    });
                }

                if (redoAvailable)
                {
                    menuItems.Add(new MenuItem
                    {
                        Header = ViewModel.Redo,
                        Command = ReactiveCommand.Create(() =>
                        {
                            var visibleLine = diffLeft.GetMiddleVisibleLine();
                            ViewModel.RedoCommand.Execute(leftSide).Subscribe();
                            diffLeft.ScrollToLine(visibleLine);
                            diffRight.ScrollToLine(visibleLine);
                            ViewModel!.DiffEditorTopLine = diffLeft.GetTopVisibleLine();
                            ViewModel.DiffEditorBottomLine = diffLeft.GetBottomVisibleLine();
                            if (!(ViewModel.DiffEditorTopLine <= visibleLine && ViewModel.DiffEditorBottomLine >= visibleLine))
                            {
                                diffLeft.TextArea.Caret.Location = new TextLocation(visibleLine, 1);
                                diffRight.TextArea.Caret.Location = new TextLocation(visibleLine, 1);
                            }

                            RedrawEditorDiffs();
                        })
                    });
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
        private void HandleEditorFindOrReplace(IronyModManager.Controls.TextEditor textEditor, SearchPanel searchPanel, bool isReplaceMode)
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
        /// Redraws an editor diffs.
        /// </summary>
        private void RedrawEditorDiffs()
        {
            diffLeft.TextArea.TextView.Redraw();
            diffRight.TextArea.TextView.Redraw();
        }

        /// <summary>
        /// Sets the editor context menu.
        /// </summary>
        /// <param name="textEditor">The text editor.</param>
        /// <param name="searchPanel">The search panel.</param>
        private void SetEditorContextMenu(IronyModManager.Controls.TextEditor textEditor, SearchPanel searchPanel)
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
