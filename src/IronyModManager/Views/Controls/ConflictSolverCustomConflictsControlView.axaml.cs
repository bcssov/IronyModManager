// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 07-28-2020
//
// Last Modified By : Mario
// Last Modified On : 07-30-2020
// ***********************************************************************
// <copyright file="ConflictSolverCustomConflictsControlView.axaml.cs" company="Mario">
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
using System.Xml;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.Views;
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
        /// The PDX script
        /// </summary>
        private static IHighlightingDefinition pdxScriptHighlightingDefinition;

        /// <summary>
        /// The yaml highlighting definition
        /// </summary>
        private static IHighlightingDefinition yamlHighlightingDefinition;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictSolverCustomConflictsControlView" /> class.
        /// </summary>
        public ConflictSolverCustomConflictsControlView()
        {
            this.InitializeComponent();
        }

        #endregion Constructors

        #region Methods

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
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var editor = this.FindControl<IronyModManager.Controls.TextEditor>("editor");
            SetEditorOptions(editor);

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
        /// Sets the editor options.
        /// </summary>
        /// <param name="editor">The editor.</param>
        protected virtual void SetEditorOptions(IronyModManager.Controls.TextEditor editor)
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
                    var carretOffset = editor.CaretOffset;
                    for (int i = 1; i <= 3; i++)
                    {
                        var last = lines[lines.Count() - i];
                        if (!string.IsNullOrWhiteSpace(last))
                        {
                            manualAppend = true;
                            editor.AppendText("\r\n");
                        }
                    }
                    if (manualAppend)
                    {
                        editor.CaretOffset = carretOffset;
                    }
                }
                lines = editor.Text.SplitOnNewLine().ToList();
                string text = string.Join(Environment.NewLine, lines);
                ViewModel.CurrentEditText = text;
            };
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
