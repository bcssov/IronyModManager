// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-20-2020
//
// Last Modified By : Mario
// Last Modified On : 04-27-2020
// ***********************************************************************
// <copyright file="MergeViewerControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using IronyModManager.Common.Views;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

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
        /// The PDX script
        /// </summary>
        private static IHighlightingDefinition pdxScriptHighlightingDefinition;

        /// <summary>
        /// The yaml highlighting definition
        /// </summary>
        private static IHighlightingDefinition yamlHighlightingDefinition;

        /// <summary>
        /// The scroll source
        /// </summary>
        private ListBox scrollSource;

        /// <summary>
        /// The scroll token
        /// </summary>
        private CancellationTokenSource scrollToken;

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
        /// Delays the synchronize scroll asynchronous.
        /// </summary>
        /// <param name="thisListBox">The this ListBox.</param>
        /// <param name="otherListBox">The other ListBox.</param>
        /// <returns>Task.</returns>
        protected virtual Task DelaySyncScrollAsync(ListBox thisListBox, ListBox otherListBox)
        {
            var thisMaxX = Math.Abs(thisListBox.Scroll.Extent.Width - thisListBox.Scroll.Viewport.Width);
            var thisMaxY = Math.Abs(thisListBox.Scroll.Extent.Height - thisListBox.Scroll.Viewport.Height);
            var otherMaxX = Math.Abs(otherListBox.Scroll.Extent.Width - otherListBox.Scroll.Viewport.Width);
            var otherMaxY = Math.Abs(otherListBox.Scroll.Extent.Height - otherListBox.Scroll.Viewport.Height);
            var offset = thisListBox.Scroll.Offset;
            if (thisListBox.Scroll.Offset.X > otherMaxX || thisListBox.Scroll.Offset.X == thisMaxX)
            {
                offset = offset.WithX(otherMaxX);
            }
            if (thisListBox.Scroll.Offset.Y > otherMaxY || thisListBox.Scroll.Offset.Y == thisMaxY)
            {
                offset = offset.WithY(otherMaxY);
            }
            if (otherListBox.Scroll.Offset.X != offset.X || otherListBox.Scroll.Offset.Y != offset.Y)
            {
                otherListBox.InvalidateArrange();
                thisListBox.InvalidateArrange();
                otherListBox.Scroll.Offset = offset;
            }
            return Task.FromResult(true);
        }

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
        /// Gets the highlighting definition.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IHighlightingDefinition.</returns>
        protected virtual IHighlightingDefinition GetHighlightingDefinition(string path)
        {
            var bytes = ResourceReader.GetEmbeddedResource(path);
            var xml = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            using var sr = new StringReader(xml);
            using var reader = XmlReader.Create(sr);
            return HighlightingLoader.Load(HighlightingLoader.LoadXshd(reader), HighlightingManager.Instance);
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
                        if (thisListBox.Scroll == null || otherListBox.Scroll == null || scrollSource == otherListBox)
                        {
                            return;
                        }
                        scrollSource = thisListBox;
                        if (scrollToken == null)
                        {
                            scrollToken = new CancellationTokenSource();
                        }
                        else
                        {
                            scrollToken.Cancel();
                            scrollToken = new CancellationTokenSource();
                        }
                        Dispatcher.UIThread.InvokeAsync(async () =>
                        {
                            await Task.Delay(50, scrollToken.Token);
                            await DelaySyncScrollAsync(thisListBox, otherListBox);
                            await Task.Delay(10);
                            scrollSource = null;
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
            var editorLeft = this.FindControl<IronyModManager.Controls.TextEditor>("editorLeft");
            var editorRight = this.FindControl<IronyModManager.Controls.TextEditor>("editorRight");
            SetEditorOptions(editorLeft, true);
            SetEditorOptions(editorRight, false);

            var leftSide = this.FindControl<ListBox>("leftSide");
            var rightSide = this.FindControl<ListBox>("rightSide");

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
        /// Sets the editor options.
        /// </summary>
        /// <param name="editor">The editor.</param>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void SetEditorOptions(TextEditor editor, bool leftSide)
        {
            var ctx = new ContextMenu
            {
                Items = new List<MenuItem>()
                {
                    new MenuItem()
                    {
                        Header = ViewModel.EditorCopy,
                        Command = ReactiveCommand.Create(() =>  editor.Copy()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorCut,
                        Command = ReactiveCommand.Create(() =>  editor.Cut()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorPaste,
                        Command = ReactiveCommand.Create(() =>  editor.Paste()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorDelete,
                        Command = ReactiveCommand.Create(() =>  editor.Delete()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorSelectAll,
                        Command = ReactiveCommand.Create(() =>  editor.SelectAll()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = "-"
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorUndo,
                        Command = ReactiveCommand.Create(() =>  editor.Undo()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorRedo,
                        Command = ReactiveCommand.Create(() =>  editor.Redo()).DisposeWith(Disposables)
                    }
                }
            };
            editor.ContextMenu = ctx;

            editor.Options = new TextEditorOptions()
            {
                ConvertTabsToSpaces = true,
                IndentationSize = 4
            };
            editor.TextArea.ActiveInputHandler = new Implementation.AvaloniaEdit.TextAreaInputHandler(editor);

            ViewModel.WhenAnyValue(p => p.EditingYaml).Subscribe(s =>
            {
                setEditMode();
            }).DisposeWith(Disposables);
            setEditMode();

            void setEditMode()
            {
                if (ViewModel.EditingYaml)
                {
                    if (yamlHighlightingDefinition == null)
                    {
                        yamlHighlightingDefinition = GetHighlightingDefinition(Constants.Resources.YAML);
                    }
                    editor.SyntaxHighlighting = yamlHighlightingDefinition;
                }
                else
                {
                    if (pdxScriptHighlightingDefinition == null)
                    {
                        pdxScriptHighlightingDefinition = GetHighlightingDefinition(Constants.Resources.PDXScript);
                    }
                    editor.SyntaxHighlighting = pdxScriptHighlightingDefinition;
                }
            }

            bool manualAppend = false;
            editor.TextChanged += (sender, args) =>
            {
                // It's a hack I know see: https://github.com/AvaloniaUI/AvaloniaEdit/issues/99.
                // I'd need to go into the code to fix it and it ain't worth it. There doesn't seem to be any feedback on this issue as well.
                var lines = editor.Text.SplitOnNewLine(false).ToList();
                if (lines.Count() > 3)
                {
                    if (manualAppend)
                    {
                        manualAppend = false;
                        return;
                    }
                    for (int i = 1; i <= 3; i++)
                    {
                        var last = lines[lines.Count() - i];
                        if (!string.IsNullOrWhiteSpace(last))
                        {
                            manualAppend = true;
                            editor.AppendText("\r\n");
                        }
                    }
                }
                lines = editor.Text.SplitOnNewLine().ToList();
                string text = string.Join(Environment.NewLine, lines);
                ViewModel.CurrentEditText = text;
            };
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
                    Header = ViewModel.DeleteText,
                    Command = ViewModel.DeleteTextCommand,
                    CommandParameter = leftSide
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
