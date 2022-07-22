// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 07-28-2020
//
// Last Modified By : Mario
// Last Modified On : 07-22-2022
// ***********************************************************************
// <copyright file="ConflictSolverCustomConflictsControlView.axaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaEdit;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.Views;
using IronyModManager.DI;
using IronyModManager.Implementation.AvaloniaEdit;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class ConflictSolverCustomConflictsControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ConflictSolverCustomConflictsControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ConflictSolverCustomConflictsControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ConflictSolverCustomConflictsControlView : BaseControl<ConflictSolverCustomConflictsControlViewModel>
    {
        #region Fields

        /// <summary>
        /// The resource loader
        /// </summary>
        private readonly IResourceLoader resourceLoader;

        /// <summary>
        /// The search panel
        /// </summary>
        private AvaloniaEdit.Search.SearchPanel searchPanel;

        /// <summary>
        /// The text editor
        /// </summary>
        private IronyModManager.Controls.TextEditor textEditor;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictSolverCustomConflictsControlView" /> class.
        /// </summary>
        public ConflictSolverCustomConflictsControlView()
        {
            resourceLoader = DIResolver.Get<IResourceLoader>();
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
            textEditor = this.FindControl<IronyModManager.Controls.TextEditor>("editor");
            SetEditorOptions(textEditor);
            searchPanel = AvaloniaEdit.Search.SearchPanel.Install(textEditor);
            SetEditorContextMenu();

            var popup = this.FindControl<Popup>("popup");
            popup.Closed += (sender, args) =>
            {
                ViewModel.ForceClosePopup();
            };
            MessageBus.Current.Listen<ForceClosePopulsEventArgs>()
            .SubscribeObservable(x =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ViewModel.ForceClosePopup();
                });
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
            SetEditorContextMenu();
            base.OnLocaleChanged(newLocale, oldLocale);
        }

        /// <summary>
        /// Sets the editor options.
        /// </summary>
        /// <param name="editor">The editor.</param>
        protected virtual void SetEditorOptions(IronyModManager.Controls.TextEditor editor)
        {
            editor.Options = new TextEditorOptions()
            {
                ConvertTabsToSpaces = true,
                IndentationSize = 4
            };

            ViewModel.WhenAnyValue(p => p.EditingYaml).Subscribe(s =>
            {
                setEditMode();
            }).DisposeWith(Disposables);
            setEditMode();

            void setEditMode()
            {
                if (ViewModel.EditingYaml)
                {
                    editor.SyntaxHighlighting = resourceLoader.GetYAMLDefinition();
                }
                else
                {
                    editor.SyntaxHighlighting = resourceLoader.GetPDXScriptDefinition();
                }
            }

            editor.TextChanged += (sender, args) =>
            {
                var lines = editor.Text.SplitOnNewLine().ToList();
                string text = string.Join(Environment.NewLine, lines);
                ViewModel.CurrentEditText = text;
            };
        }

        /// <summary>
        /// Handles the editor find or replace.
        /// </summary>
        /// <param name="isReplaceMode">if set to <c>true</c> [is replace mode].</param>
        private void HandleEditorFindOrReplace(bool isReplaceMode)
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
        private void SetEditorContextMenu()
        {
            if (textEditor == null || searchPanel == null)
            {
                return;
            }
            var ctx = new MenuFlyout
            {
                Items = new List<MenuItem>()
                {
                    new MenuItem()
                    {
                        Header = ViewModel.EditorCopy,
                        Command = ReactiveCommand.Create(() =>  textEditor.Copy()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorCut,
                        Command = ReactiveCommand.Create(() =>  textEditor.Cut()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorPaste,
                        Command = ReactiveCommand.Create(() =>  textEditor.Paste()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorDelete,
                        Command = ReactiveCommand.Create(() =>  textEditor.Delete()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorSelectAll,
                        Command = ReactiveCommand.Create(() =>  textEditor.SelectAll()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = "-"
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorUndo,
                        Command = ReactiveCommand.Create(() =>  textEditor.Undo()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorRedo,
                        Command = ReactiveCommand.Create(() =>  textEditor.Redo()).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = "-"
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorFind,
                        Command = ReactiveCommand.Create(() =>  HandleEditorFindOrReplace(false)).DisposeWith(Disposables)
                    },
                    new MenuItem()
                    {
                        Header = ViewModel.EditorReplace,
                        Command = ReactiveCommand.Create(() =>  HandleEditorFindOrReplace(true)).DisposeWith(Disposables)
                    },
                }
            };
            textEditor.ContextFlyout = ctx;
        }

        #endregion Methods
    }
}
