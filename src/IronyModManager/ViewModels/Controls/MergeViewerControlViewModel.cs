﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-20-2020
//
// Last Modified By : Mario
// Last Modified On : 05-07-2025
// ***********************************************************************
// <copyright file="MergeViewerControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using DiffPlex;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.AppState;
using IronyModManager.Implementation.AvaloniaEdit;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class MergeViewerControlViewModel.
    /// Implements the <see cref="BaseViewModel" />
    /// </summary>
    /// <seealso cref="BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class MergeViewerControlViewModel(
        IAppStateService appStateService,
        IScrollState scrollState,
        IModPatchCollectionService modPatchCollectionService,
        ConflictSolverViewHotkeyPressedHandler hotkeyPressedHandler,
        IAppAction appAction,
        IExternalEditorService externalEditorService,
        INotificationAction notificationAction,
        ILocalizationManager localizationManager) : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The hotkey lock
        /// </summary>
        private static readonly object hotkeyLock = new { };

        /// <summary>
        /// The URL action
        /// </summary>
        private readonly IAppAction appAction = appAction;

        /// <summary>
        /// A private readonly IAppStateService named appStateService.
        /// </summary>
        private readonly IAppStateService appStateService = appStateService;

        /// <summary>
        /// The external editor service
        /// </summary>
        private readonly IExternalEditorService externalEditorService = externalEditorService;

        /// <summary>
        /// The hotkey pressed handler
        /// </summary>
        private readonly ConflictSolverViewHotkeyPressedHandler hotkeyPressedHandler = hotkeyPressedHandler;

        /// <summary>
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager = localizationManager;

        /// <summary>
        /// The mod patch collection service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService = modPatchCollectionService;

        /// <summary>
        /// The notification action
        /// </summary>
        private readonly INotificationAction notificationAction = notificationAction;

        /// <summary>
        /// The redo stack
        /// </summary>
        private readonly Stack<string> redoStack = new();

        /// <summary>
        /// The scroll state
        /// </summary>
        private readonly IScrollState scrollState = scrollState;

        /// <summary>
        /// The undo stack
        /// </summary>
        private readonly Stack<string> undoStack = new();

        /// <summary>
        /// The left definition
        /// </summary>
        private IDefinition leftDefinition;

        /// <summary>
        /// The right definition
        /// </summary>
        private IDefinition rightDefinition;

        /// <summary>
        /// The syncing selection
        /// </summary>
        private bool syncingSelection;

        #endregion Fields

        #region Delegates

        /// <summary>
        /// Delegate ConflictFoundDelegate
        /// </summary>
        /// <param name="line">The line.</param>
        public delegate void ConflictFoundDelegate(int line);

        /// <summary>
        /// Delegate FocusSideDelegate
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        public delegate void FocusSideDelegate(bool leftSide);

        #endregion Delegates

        #region Events

        /// <summary>
        /// Occurs when [conflict found].
        /// </summary>
        public event ConflictFoundDelegate ConflictFound;

        /// <summary>
        /// Occurs when [hotkey after perform].
        /// </summary>
        public event EventHandler HotkeyAfterPerform;

        /// <summary>
        /// Occurs when [hotkey copy before perform].
        /// </summary>
        public event EventHandler HotkeyBeforePerform;

        /// <summary>
        /// Occurs when [focus side].
        /// </summary>
        public event FocusSideDelegate PostFocusSide;

        /// <summary>
        /// Occurs when [pre focus side].
        /// </summary>
        public event FocusSideDelegate PreFocusSide;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets the bracket mismatch text.
        /// </summary>
        /// <value>The bracket mismatch text.</value>
        public virtual string BracketMismatchText { get; protected set; }

        /// <summary>
        /// Gets or sets the cancel.
        /// </summary>
        /// <value>The cancel.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.Cancel)]
        public virtual string Cancel { get; protected set; }

        /// <summary>
        /// Gets or sets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        public virtual ReactiveCommand<Unit, Unit> CancelCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can perform hot key actions.
        /// </summary>
        /// <value><c>true</c> if this instance can perform hot key actions; otherwise, <c>false</c>.</value>
        public virtual bool CanPerformHotKeyActions { get; set; }

        /// <summary>
        /// Gets or sets a value representing the color converter.<see cref="IronyModManager.Implementation.AvaloniaEdit.EditorColorConverter" />
        /// </summary>
        /// <value>The color converter.</value>
        public virtual EditorColorConverter ColorConverter { get; protected set; }

        /// <summary>
        /// Gets or sets the copy all.
        /// </summary>
        /// <value>The copy all.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.CopyAll)]
        public virtual string CopyAll { get; protected set; }

        /// <summary>
        /// Gets or sets the copy all command.
        /// </summary>
        /// <value>The copy all command.</value>
        public virtual ReactiveCommand<bool, Unit> CopyAllCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the copy text.
        /// </summary>
        /// <value>The copy text.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.CopyText)]
        public virtual string CopyText { get; protected set; }

        /// <summary>
        /// Gets or sets the copy text command.
        /// </summary>
        /// <value>The copy text command.</value>
        public virtual ReactiveCommand<bool, Unit> CopyTextCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the copy this.
        /// </summary>
        /// <value>The copy this.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.CopyThis)]
        public virtual string CopyThis { get; protected set; }

        /// <summary>
        /// Gets or sets the copy this after line.
        /// </summary>
        /// <value>The copy this after line.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.CopyThisAfterLine)]
        public virtual string CopyThisAfterLine { get; protected set; }

        /// <summary>
        /// Gets or sets the copy this after line command.
        /// </summary>
        /// <value>The copy this after line command.</value>
        public virtual ReactiveCommand<bool, Unit> CopyThisAfterLineCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the copy this before line.
        /// </summary>
        /// <value>The copy this before line.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.CopyThisBeforeLine)]
        public virtual string CopyThisBeforeLine { get; protected set; }

        /// <summary>
        /// Gets or sets the copy this before line command.
        /// </summary>
        /// <value>The copy this before line command.</value>
        public virtual ReactiveCommand<bool, Unit> CopyThisBeforeLineCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the copy this command.
        /// </summary>
        /// <value>The copy this command.</value>
        public virtual ReactiveCommand<bool, Unit> CopyThisCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the current edit text.
        /// </summary>
        /// <value>The current edit text.</value>
        public virtual string CurrentEditText { get; set; }

        /// <summary>
        /// Gets or sets the delete text.
        /// </summary>
        /// <value>The delete text.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.Delete)]
        public virtual string DeleteText { get; protected set; }

        /// <summary>
        /// Gets or sets the delete text command.
        /// </summary>
        /// <value>The delete text command.</value>
        public virtual ReactiveCommand<bool, Unit> DeleteTextCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value representing the diff editor bottom line.<see cref="int?" />
        /// </summary>
        /// <value>The diff editor bottom line.</value>
        public virtual int? DiffEditorBottomLine { get; set; }

        /// <summary>
        /// Gets or sets a value representing the diff editor top line.<see cref="int?" />
        /// </summary>
        /// <value>The diff editor top line.</value>
        public virtual int? DiffEditorTopLine { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [editing left].
        /// </summary>
        /// <value><c>true</c> if [editing left]; otherwise, <c>false</c>.</value>
        public virtual bool EditingLeft { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [editing lua].
        /// </summary>
        /// <value><c>true</c> if [editing lua]; otherwise, <c>false</c>.</value>
        public virtual bool EditingLua { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [editing right].
        /// </summary>
        /// <value><c>true</c> if [editing right]; otherwise, <c>false</c>.</value>
        public virtual bool EditingRight { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [editing text].
        /// </summary>
        /// <value><c>true</c> if [editing text]; otherwise, <c>false</c>.</value>
        public virtual bool EditingText { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [editing yaml].
        /// </summary>
        /// <value><c>true</c> if [editing yaml]; otherwise, <c>false</c>.</value>
        public virtual bool EditingYaml { get; set; }

        /// <summary>
        /// Gets or sets the editor.
        /// </summary>
        /// <value>The editor.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.Editor)]
        public virtual string Editor { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [editor available].
        /// </summary>
        /// <value><c>true</c> if [editor available]; otherwise, <c>false</c>.</value>
        public virtual bool EditorAvailable { get; protected set; }

        /// <summary>
        /// Gets or sets the editor command.
        /// </summary>
        /// <value>The editor command.</value>
        public virtual ReactiveCommand<bool, Unit> EditorCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the editor copy.
        /// </summary>
        /// <value>The editor copy.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.EditorContextMenu.Copy)]
        public virtual string EditorCopy { get; protected set; }

        /// <summary>
        /// Gets or sets the editor cut.
        /// </summary>
        /// <value>The editor cut.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.EditorContextMenu.Cut)]
        public virtual string EditorCut { get; protected set; }

        /// <summary>
        /// Gets or sets the editor delete.
        /// </summary>
        /// <value>The editor delete.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.EditorContextMenu.Delete)]
        public virtual string EditorDelete { get; protected set; }

        /// <summary>
        /// Gets or sets the editor find.
        /// </summary>
        /// <value>The editor find.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.EditorContextMenu.Find)]
        public virtual string EditorFind { get; protected set; }

        /// <summary>
        /// Gets or sets the editor paste.
        /// </summary>
        /// <value>The editor paste.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.EditorContextMenu.Paste)]
        public virtual string EditorPaste { get; protected set; }

        /// <summary>
        /// Gets or sets the editor redo.
        /// </summary>
        /// <value>The editor redo.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.EditorContextMenu.Redo)]
        public virtual string EditorRedo { get; protected set; }

        /// <summary>
        /// Gets or sets the editor replace.
        /// </summary>
        /// <value>The editor replace.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.EditorContextMenu.Replace)]
        public virtual string EditorReplace { get; protected set; }

        /// <summary>
        /// Gets or sets the editor select all.
        /// </summary>
        /// <value>The editor select all.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.EditorContextMenu.SelectAll)]
        public virtual string EditorSelectAll { get; protected set; }

        /// <summary>
        /// Gets or sets the editor undo.
        /// </summary>
        /// <value>The editor undo.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.EditorContextMenu.Undo)]
        public virtual string EditorUndo { get; protected set; }

        /// <summary>
        /// Gets or sets the edit this.
        /// </summary>
        /// <value>The edit this.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.Edit)]
        public virtual string EditThis { get; protected set; }

        /// <summary>
        /// Gets or sets the edit this command.
        /// </summary>
        /// <value>The edit this command.</value>
        public virtual ReactiveCommand<bool, Unit> EditThisCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value representing the edit this here.
        /// </summary>
        /// <value>The edit this here.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.EditHere)]
        public virtual string EditThisHere { get; protected set; }

        /// <summary>
        /// Gets or sets a value representing the edit this here command.<see cref="ReactiveUI.ReactiveCommand{bool, System.Reactive.Unit}" />
        /// </summary>
        /// <value>The edit this here command.</value>
        public virtual ReactiveCommand<bool, Unit> EditThisHereCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the focus selected line.
        /// </summary>
        /// <value><c>true</c> if focus selected line; otherwise, <c>false</c>.</value>
        public virtual bool FocusSelectedLine { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is read only mode.
        /// </summary>
        /// <value><c>true</c> if this instance is read only mode; otherwise, <c>false</c>.</value>
        public virtual bool IsReadOnlyMode { get; protected set; }

        /// <summary>
        /// Gets or sets the difference.
        /// </summary>
        /// <value>The difference.</value>
        public virtual IList<DiffPieceWithIndex> LeftDiff { get; protected set; }

        /// <summary>
        /// Gets or sets the left document.
        /// </summary>
        /// <value>The left document.</value>
        public virtual TextDocument LeftDocument { get; protected set; }

        /// <summary>
        /// Gets or sets the left side.
        /// </summary>
        /// <value>The left side.</value>
        public virtual string LeftSide { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [left side patch mod].
        /// </summary>
        /// <value><c>true</c> if [left side patch mod]; otherwise, <c>false</c>.</value>
        public virtual bool LeftSidePatchMod { get; protected set; }

        /// <summary>
        /// Gets or sets the left side selected.
        /// </summary>
        /// <value>The left side selected.</value>
        public virtual IAvaloniaList<DiffPieceWithIndex> LeftSideSelected { get; set; }

        /// <summary>
        /// Gets or sets the move down.
        /// </summary>
        /// <value>The move down.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.MoveDown)]
        public virtual string MoveDown { get; protected set; }

        /// <summary>
        /// Gets or sets the move down command.
        /// </summary>
        /// <value>The move down command.</value>
        public virtual ReactiveCommand<bool, Unit> MoveDownCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the move up.
        /// </summary>
        /// <value>The move up.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.MoveUp)]
        public virtual string MoveUp { get; protected set; }

        /// <summary>
        /// Gets or sets the move up command.
        /// </summary>
        /// <value>The move up command.</value>
        public virtual ReactiveCommand<bool, Unit> MoveUpCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the next conflict.
        /// </summary>
        /// <value>The next conflict.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.NextConflict)]
        public virtual string NextConflict { get; protected set; }

        /// <summary>
        /// Gets or sets the next conflict command.
        /// </summary>
        /// <value>The next conflict command.</value>
        public virtual ReactiveCommand<bool, Unit> NextConflictCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the ok.
        /// </summary>
        /// <value>The ok.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.OK)]
        public virtual string OK { get; protected set; }

        /// <summary>
        /// Gets or sets the ok command.
        /// </summary>
        /// <value>The ok command.</value>
        public virtual ReactiveCommand<Unit, Unit> OKCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the previous conflict.
        /// </summary>
        /// <value>The previous conflict.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.PrevConflict)]
        public virtual string PrevConflict { get; protected set; }

        /// <summary>
        /// Gets or sets the previous conflict command.
        /// </summary>
        /// <value>The previous conflict command.</value>
        public virtual ReactiveCommand<bool, Unit> PrevConflictCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value representing the previous left diff.<see cref="System.Collections.Generic.IList{IronyModManager.ViewModels.Controls.MergeViewerControlViewModel.DiffPieceWithIndex}" />
        /// </summary>
        /// <value>The previous left diff.</value>
        public virtual IList<DiffPieceWithIndex> PreviousLeftDiff { get; set; }

        /// <summary>
        /// Gets or sets a value representing the previous right diff.<see cref="System.Collections.Generic.IList{IronyModManager.ViewModels.Controls.MergeViewerControlViewModel.DiffPieceWithIndex}" />
        /// </summary>
        /// <value>The previous right diff.</value>
        public IList<DiffPieceWithIndex> PreviousRightDiff { get; set; }

        /// <summary>
        /// Gets or sets the read only editor.
        /// </summary>
        /// <value>The read only editor.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.ReadonlyEditor)]
        public virtual string ReadOnlyEditor { get; protected set; }

        /// <summary>
        /// Gets or sets the read only editor command.
        /// </summary>
        /// <value>The read only editor command.</value>
        public virtual ReactiveCommand<bool, Unit> ReadOnlyEditorCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the redo.
        /// </summary>
        /// <value>The redo.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.Redo)]
        public virtual string Redo { get; protected set; }

        /// <summary>
        /// Gets or sets the redo command.
        /// </summary>
        /// <value>The redo command.</value>
        public virtual ReactiveCommand<bool, Unit> RedoCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the right difference.
        /// </summary>
        /// <value>The right difference.</value>
        public virtual IList<DiffPieceWithIndex> RightDiff { get; protected set; }

        /// <summary>
        /// Gets or sets the right document.
        /// </summary>
        /// <value>The right document.</value>
        public virtual TextDocument RightDocument { get; protected set; }

        /// <summary>
        /// Gets or sets the right side.
        /// </summary>
        /// <value>The right side.</value>
        public virtual string RightSide { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [right side patch mod].
        /// </summary>
        /// <value><c>true</c> if [right side patch mod]; otherwise, <c>false</c>.</value>
        public virtual bool RightSidePatchMod { get; protected set; }

        /// <summary>
        /// Gets or sets the right side selected.
        /// </summary>
        /// <value>The right side selected.</value>
        public virtual IAvaloniaList<DiffPieceWithIndex> RightSideSelected { get; set; }

        /// <summary>
        /// Gets or sets a value representing the toggle merge type caption.
        /// </summary>
        /// <value>The toggle merge type caption.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.ToggleCompare)]
        public virtual string ToggleMergeTypeCaption { get; protected set; }

        /// <summary>
        /// Gets or sets a value representing the toggle merge type command.<see cref="ReactiveUI.ReactiveCommand{System.Reactive.Unit, System.Reactive.Unit}" />
        /// </summary>
        /// <value>The toggle merge type command.</value>
        public virtual ReactiveCommand<Unit, Unit> ToggleMergeTypeCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the undo.
        /// </summary>
        /// <value>The undo.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.ContextMenu.Undo)]
        public virtual string Undo { get; protected set; }

        /// <summary>
        /// Gets or sets the undo command.
        /// </summary>
        /// <value>The undo command.</value>
        public virtual ReactiveCommand<bool, Unit> UndoCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the using new merge type.
        /// </summary>
        /// <value><c>true</c> if using new merge type; otherwise, <c>false</c>.</value>
        public virtual bool UsingNewMergeType { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Exits the edit mode.
        /// </summary>
        public virtual void ExitEditMode()
        {
            CurrentEditText = string.Empty;
            EditingLeft = false;
            EditingRight = false;
            EditingText = false;
        }

        /// <summary>
        /// Initializes the parameters.
        /// </summary>
        public virtual void InitParameters()
        {
            EditorAvailable = false;
            var editor = externalEditorService.Get();
            if (!string.IsNullOrWhiteSpace(editor.ExternalEditorLocation) && !string.IsNullOrWhiteSpace(editor.ExternalEditorParameters) && File.Exists(editor.ExternalEditorLocation))
            {
                EditorAvailable = true;
            }

            ColorConverter = new EditorColorConverter(DIResolver.Get<IConflictSolverColorsService>().Get());
            EvaluateMergeType();
        }

        /// <summary>
        /// Determines whether [is redo available].
        /// </summary>
        /// <returns><c>true</c> if [is redo available]; otherwise, <c>false</c>.</returns>
        public virtual bool IsRedoAvailable()
        {
            return redoStack.Count != 0;
        }

        /// <summary>
        /// Determines whether [is undo available].
        /// </summary>
        /// <returns><c>true</c> if [is undo available]; otherwise, <c>false</c>.</returns>
        public virtual bool IsUndoAvailable()
        {
            return undoStack.Count != 0;
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="readOnly">if set to <c>true</c> [read only].</param>
        public virtual void SetParameters(bool readOnly)
        {
            IsReadOnlyMode = readOnly;
        }

        /// <summary>
        /// Sets the side patch mod.
        /// </summary>
        /// <param name="leftDefinition">The left definition.</param>
        /// <param name="rightDefinition">The right definition.</param>
        public virtual void SetSidePatchMod(IDefinition leftDefinition, IDefinition rightDefinition)
        {
            this.leftDefinition = leftDefinition;
            this.rightDefinition = rightDefinition;
            LeftSidePatchMod = modPatchCollectionService.IsPatchMod(leftDefinition?.ModName);
            RightSidePatchMod = modPatchCollectionService.IsPatchMod(rightDefinition?.ModName);
        }

        /// <summary>
        /// Sets the text.
        /// </summary>
        /// <param name="leftSide">The left side.</param>
        /// <param name="rightSide">The right side.</param>
        /// <param name="resetStack">if set to <c>true</c> [reset stack].</param>
        /// <param name="skipStackPush">if set to <c>true</c> [skip stack push].</param>
        /// <param name="lockScroll">if set to <c>true</c> [lock scroll].</param>
        public virtual void SetText(string leftSide, string rightSide, bool resetStack = false, bool skipStackPush = false, bool lockScroll = false)
        {
            void evalStack(string text, string prevText)
            {
                text ??= string.Empty;
                prevText ??= string.Empty;
                if (undoStack.Count == 0 && string.IsNullOrWhiteSpace(prevText))
                {
                }
                else if (undoStack.Count > 0 && undoStack.FirstOrDefault()!.Equals(prevText))
                {
                }
                else if (prevText.Equals(text))
                {
                }
                else
                {
                    undoStack.Push(prevText);
                    redoStack.Clear();
                }
            }

            scrollState.SetState(!lockScroll);
            var prevLeftSide = LeftSide;
            var prevRightSide = RightSide;
            LeftSide = !string.IsNullOrEmpty(leftSide) ? leftSide.ReplaceTabs() : string.Empty;
            RightSide = !string.IsNullOrEmpty(rightSide) ? rightSide.ReplaceTabs() : string.Empty;
            LeftDocument = new TextDocument(LeftSide);
            RightDocument = new TextDocument(RightSide);
            SetBracketText();
            Compare(LeftSide, RightSide);

            if (resetStack)
            {
                undoStack.Clear();
                redoStack.Clear();
            }
            else if (!skipStackPush)
            {
                if (LeftSidePatchMod)
                {
                    evalStack(LeftSide, prevLeftSide);
                }
                else if (RightSidePatchMod)
                {
                    evalStack(RightSide, prevRightSide);
                }
            }

            scrollState.SetState(true);
        }

        /// <summary>
        /// Compare.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        protected virtual void Compare(string left, string right)
        {
            var builder = new SideBySideDiffBuilder(new Differ());
            var diff = builder.BuildDiffModel(left, right, true);
            PreviousLeftDiff = LeftDiff;
            PreviousRightDiff = RightDiff;
            LeftDiff = GetDiffPieceWithIndex(diff.OldText.Lines);
            RightDiff = GetDiffPieceWithIndex(diff.NewText.Lines);
        }

        /// <summary>
        /// Copies the specified selected.
        /// </summary>
        /// <param name="selected">The selected.</param>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void Copy(IEnumerable<DiffPieceWithIndex> selected, IList<DiffPieceWithIndex> source, IList<DiffPieceWithIndex> destination, bool leftSide)
        {
            if (selected?.Count() > 0)
            {
                var idx = 0;
                selected = CopyDiffPieceCollection(selected);
                source = CopyDiffPieceCollection(source);
                destination = CopyDiffPieceCollection(destination);
                var ordered = OrderSelected(selected, source);
                foreach (var item in ordered)
                {
                    destination.RemoveAt(item.Key);
                    destination.Insert(item.Key, item.Value);
                    idx = item.Key;
                }

                if (leftSide)
                {
                    SetText(LeftSide, string.Join(Environment.NewLine, destination.Where(p => p.Text != null).Select(p => p.Text)));
                }
                else
                {
                    SetText(string.Join(Environment.NewLine, destination.Where(p => p.Text != null).Select(p => p.Text)), RightSide);
                }

                ConflictFound?.Invoke(idx);
            }
        }

        /// <summary>
        /// Copies the after lines.
        /// </summary>
        /// <param name="selected">The selected.</param>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void CopyAfterLines(IEnumerable<DiffPieceWithIndex> selected, IList<DiffPieceWithIndex> source, IList<DiffPieceWithIndex> destination, bool leftSide)
        {
            if (selected?.Count() > 0)
            {
                var idx = 0;
                selected = CopyDiffPieceCollection(selected);
                source = CopyDiffPieceCollection(source);
                destination = CopyDiffPieceCollection(destination);
                var ordered = OrderSelected(selected, source);
                var grouped = ordered.Select((x, id) => Tuple.Create(x.Key, x.Value, id == 0 ? 0 : x.Key - ordered.ElementAt(id - 1).Key, id)).ToList();
                Tuple<int, DiffPieceWithIndex, int, int> initial = null;
                var appliedOffset = 0;
                foreach (var item in grouped)
                {
                    if (initial == null || item.Item3 > 1)
                    {
                        initial = null;
                        appliedOffset = 0;
                        var groupCopy = grouped.Skip(item.Item4 + 1).TakeWhile(p => p.Item3 <= 1);
                        if (groupCopy.Any())
                        {
                            if (groupCopy.Last() != null)
                            {
                                initial = groupCopy.Last();
                            }
                        }

                        initial ??= item;
                    }

                    var index = initial.Item1 + item.Item4 + appliedOffset + 1;
                    var count = destination.Count - 1;
                    if (index < 0)
                    {
                        index = 0;
                    }
                    else if (index > count)
                    {
                        index = count;
                    }

                    while (destination[index].Type == ChangeType.Imaginary)
                    {
                        if (index < 0)
                        {
                            index = 0;
                            break;
                        }
                        else if (index > count)
                        {
                            index = count;
                            break;
                        }

                        index++;
                        appliedOffset++;
                    }

                    destination.Insert(index, item.Item2);
                    idx = index;
                }

                if (leftSide)
                {
                    SetText(LeftSide, string.Join(Environment.NewLine, destination.Where(p => p.Text != null).Select(p => p.Text)));
                }
                else
                {
                    SetText(string.Join(Environment.NewLine, destination.Where(p => p.Text != null).Select(p => p.Text)), RightSide);
                }

                ConflictFound?.Invoke(idx);
            }
        }

        /// <summary>
        /// Copies the before lines.
        /// </summary>
        /// <param name="selected">The selected.</param>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void CopyBeforeLines(IEnumerable<DiffPieceWithIndex> selected, IList<DiffPieceWithIndex> source, IList<DiffPieceWithIndex> destination, bool leftSide)
        {
            if (selected?.Count() > 0)
            {
                var idx = 0;
                selected = CopyDiffPieceCollection(selected);
                source = CopyDiffPieceCollection(source);
                destination = CopyDiffPieceCollection(destination);
                var ordered = OrderSelected(selected, source);
                var grouped = ordered.Select((x, id) => { return Tuple.Create(x.Key, x.Value, id == 0 ? 0 : x.Key - ordered.ElementAt(id - 1).Key, id); }).ToList();
                Tuple<int, DiffPieceWithIndex, int, int> initial = null;
                var appliedOffset = 0;
                foreach (var item in grouped)
                {
                    if (initial == null || item.Item3 > 1)
                    {
                        appliedOffset = 0;
                        initial = item;
                    }

                    var index = initial.Item1 + item.Item4 + appliedOffset;
                    var count = destination.Count - 1;
                    if (index < 0)
                    {
                        index = 0;
                    }
                    else if (index > count)
                    {
                        index = count;
                    }

                    while (destination[index].Type == ChangeType.Imaginary)
                    {
                        if (index < 0)
                        {
                            index = 0;
                            break;
                        }
                        else if (index > count)
                        {
                            index = count;
                            break;
                        }

                        index--;
                        appliedOffset--;
                    }

                    destination.Insert(index, item.Item2);
                    idx = index;
                }

                if (leftSide)
                {
                    SetText(LeftSide, string.Join(Environment.NewLine, destination.Where(p => p.Text != null).Select(p => p.Text)));
                }
                else
                {
                    SetText(string.Join(Environment.NewLine, destination.Where(p => p.Text != null).Select(p => p.Text)), RightSide);
                }

                ConflictFound?.Invoke(idx);
            }
        }

        /// <summary>
        /// Copies the difference piece collection.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <returns>System.Collections.Generic.List&lt;IronyModManager.ViewModels.Controls.MergeViewerControlViewModel.DiffPieceWithIndex&gt;.</returns>
        protected virtual List<DiffPieceWithIndex> CopyDiffPieceCollection(IEnumerable<DiffPieceWithIndex> col)
        {
            return [.. col];
        }

        /// <summary>
        /// copy text as an asynchronous operation.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        protected async Task CopyTextAsync(bool leftSide)
        {
            await appAction.CopyAsync(leftSide ? LeftSide : RightSide);
        }

        /// <summary>
        /// Deletes the lines.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void DeleteLines(bool leftSide)
        {
            var idx = 0;
            var selected = leftSide ? LeftSideSelected : RightSideSelected;
            var source = CopyDiffPieceCollection(leftSide ? LeftDiff : RightDiff);
            if (selected != null && source != null && selected.Count > 0 && selected.Count <= source.Count)
            {
                foreach (var item in selected)
                {
                    source.Remove(item);
                    idx = item.Index;
                }

                if (leftSide)
                {
                    SetText(string.Join(Environment.NewLine, source.Where(p => p.Text != null).Select(p => p.Text)), RightSide);
                }
                else
                {
                    SetText(LeftSide, string.Join(Environment.NewLine, source.Where(p => p.Text != null).Select(p => p.Text)));
                }

                ConflictFound?.Invoke(idx);
            }
        }

        /// <summary>
        /// Evaluates a merge type.
        /// </summary>
        protected virtual void EvaluateMergeType()
        {
            UsingNewMergeType = appStateService.Get().UseNewDiffViewer;
        }

        /// <summary>
        /// Finds the conflict.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        /// <param name="moveDown">if set to <c>true</c> [move down].</param>
        /// <param name="skipImaginary">if set to <c>true</c> [skip imaginary].</param>
        protected virtual void FindConflict(bool leftSide, bool moveDown, bool skipImaginary)
        {
            var selectedItems = leftSide ? LeftSideSelected : RightSideSelected;
            if (selectedItems?.Count > 0)
            {
                var source = leftSide ? LeftDiff : RightDiff;
                var idx = source.IndexOf(selectedItems.FirstOrDefault());
                int? matchIdx = null;
                if (idx > -1)
                {
                    if (moveDown)
                    {
                        while (true)
                        {
                            idx++;
                            if (idx > source.Count - 1)
                            {
                                idx = source.Count - 1;
                                break;
                            }

                            var prevIdx = idx - 1;
                            var type = source[prevIdx].Type;
                            if (source[idx].Type == ChangeType.Unchanged || (type == ChangeType.Unchanged && source[idx].Type != ChangeType.Unchanged))
                            {
                                break;
                            }
                        }

                        var line = source.Skip(idx).FirstOrDefault(p => p.SubPieces.Count > 0 || !(p.Type == ChangeType.Unchanged || (skipImaginary && p.Type == ChangeType.Imaginary)));
                        if (line != null)
                        {
                            var index = line.Index - 1;
                            if (index < 0)
                            {
                                index = 0;
                            }

                            line = source.Skip(index).TakeWhile(p => p.SubPieces.Count > 0 || !(p.Type == ChangeType.Unchanged || (skipImaginary && p.Type == ChangeType.Imaginary))).LastOrDefault();
                            if (line != null)
                            {
                                matchIdx = source.IndexOf(line);
                            }
                        }
                    }
                    else
                    {
                        var reverseSrc = source.Reverse().ToList();
                        var reverseIdx = source.Count - idx - 1;
                        while (true)
                        {
                            reverseIdx++;
                            if (reverseIdx > reverseSrc.Count - 1)
                            {
                                reverseIdx = reverseSrc.Count - 1;
                                break;
                            }

                            var prevIdx = reverseIdx - 1;
                            if (prevIdx < 0)
                            {
                                prevIdx = 0;
                            }

                            var type = reverseSrc[prevIdx].Type;
                            if (reverseSrc[reverseIdx].Type == ChangeType.Unchanged || (type == ChangeType.Unchanged && reverseSrc[reverseIdx].Type != ChangeType.Unchanged))
                            {
                                break;
                            }
                        }

                        var line = reverseSrc.Skip(reverseIdx).FirstOrDefault(p => p.SubPieces.Count > 0 || !(p.Type == ChangeType.Unchanged || (skipImaginary && p.Type == ChangeType.Imaginary)));
                        if (line != null)
                        {
                            var index = reverseSrc.Count - line.Index;
                            if (index < 0)
                            {
                                index = 0;
                            }

                            line = reverseSrc.Skip(index).TakeWhile(p => p.SubPieces.Count > 0 || !(p.Type == ChangeType.Unchanged || (skipImaginary && p.Type == ChangeType.Imaginary))).LastOrDefault();
                            if (line != null)
                            {
                                matchIdx = source.IndexOf(line);
                            }
                        }
                    }

                    if (matchIdx.HasValue)
                    {
                        ConflictFound?.Invoke(matchIdx.GetValueOrDefault());
                    }
                }
            }
        }

        /// <summary>
        /// Gets the index of the difference piece with.
        /// </summary>
        /// <param name="lines">The lines.</param>
        /// <returns>List&lt;DiffPieceWithIndex&gt;.</returns>
        protected virtual List<DiffPieceWithIndex> GetDiffPieceWithIndex(List<DiffPiece> lines)
        {
            var col = new List<DiffPieceWithIndex>();
            var counter = 0;
            foreach (var item in lines)
            {
                counter++;
                col.Add(new DiffPieceWithIndex
                {
                    Index = counter,
                    Position = item.Position,
                    SubPieces = item.SubPieces,
                    Text = item.Text,
                    Type = item.Type
                });
            }

            return col;
        }

        /// <summary>
        /// Moves the specified move up.
        /// </summary>
        /// <param name="moveUp">if set to <c>true</c> [move up].</param>
        /// <param name="selected">The selected.</param>
        /// <param name="source">The source.</param>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void Move(bool moveUp, IEnumerable<DiffPieceWithIndex> selected, IList<DiffPieceWithIndex> source, bool leftSide)
        {
            if (selected?.Count() > 0)
            {
                var idx = 0;
                selected = CopyDiffPieceCollection(selected);
                source = CopyDiffPieceCollection(source);
                var ordered = OrderSelected(selected, source);
                foreach (var item in ordered)
                {
                    var count = source.Count - 1;
                    var index = moveUp ? item.Key - 1 : item.Key + 1;
                    if (index < 0)
                    {
                        index = 0;
                    }
                    else if (index > count)
                    {
                        index = count;
                    }

                    while (source[index].Type == ChangeType.Imaginary)
                    {
                        if (index < 0)
                        {
                            index = 0;
                            break;
                        }
                        else if (index > count)
                        {
                            index = count;
                            break;
                        }

                        if (moveUp)
                        {
                            index--;
                        }
                        else
                        {
                            index++;
                        }
                    }

                    source.RemoveAt(item.Key);
                    source.Insert(index, item.Value);
                    idx = index;
                }

                if (leftSide)
                {
                    SetText(string.Join(Environment.NewLine, source.Where(p => p.Text != null).Select(p => p.Text)), RightSide);
                }
                else
                {
                    SetText(LeftSide, string.Join(Environment.NewLine, source.Where(p => p.Text != null).Select(p => p.Text)));
                }

                ConflictFound?.Invoke(idx);
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            LeftSideSelected = new AvaloniaList<DiffPieceWithIndex>();
            RightSideSelected = new AvaloniaList<DiffPieceWithIndex>();

            LeftSideSelected.CollectionChanged += (_, _) =>
            {
                if (syncingSelection)
                {
                    return;
                }

                syncingSelection = true;
                SyncSelectionsAsync(true).ConfigureAwait(true);
            };

            RightSideSelected.CollectionChanged += (_, _) =>
            {
                if (syncingSelection)
                {
                    return;
                }

                syncingSelection = true;
                SyncSelectionsAsync(false).ConfigureAwait(true);
            };

            CopyAllCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                if (leftSide)
                {
                    Copy(LeftDiff, LeftDiff, RightDiff, true);
                }
                else
                {
                    Copy(RightDiff, RightDiff, LeftDiff, false);
                }
            }).DisposeWith(disposables);

            CopyThisCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                if (leftSide)
                {
                    Copy(LeftSideSelected, LeftDiff, RightDiff, true);
                }
                else
                {
                    Copy(RightSideSelected, RightDiff, LeftDiff, false);
                }
            }).DisposeWith(disposables);

            CopyThisBeforeLineCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                if (leftSide)
                {
                    CopyBeforeLines(LeftSideSelected, LeftDiff, RightDiff, true);
                }
                else
                {
                    CopyBeforeLines(RightSideSelected, RightDiff, LeftDiff, false);
                }
            }).DisposeWith(disposables);

            CopyThisAfterLineCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                if (leftSide)
                {
                    CopyAfterLines(LeftSideSelected, LeftDiff, RightDiff, true);
                }
                else
                {
                    CopyAfterLines(RightSideSelected, LeftDiff, RightDiff, false);
                }
            }).DisposeWith(disposables);

            MoveUpCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                if (leftSide)
                {
                    Move(true, LeftSideSelected, LeftDiff, true);
                }
                else
                {
                    Move(true, RightSideSelected, RightDiff, false);
                }
            }).DisposeWith(disposables);

            MoveDownCommand = ReactiveCommand.Create((bool leftSide) =>
            {
                if (leftSide)
                {
                    Move(false, LeftSideSelected, LeftDiff, true);
                }
                else
                {
                    Move(false, RightSideSelected, RightDiff, false);
                }
            }).DisposeWith(disposables);

            var okEnabled = this.WhenAnyValue(v => v.CurrentEditText, v => !string.IsNullOrWhiteSpace(v));

            OKCommand = ReactiveCommand.Create(() =>
            {
                if (EditingLeft)
                {
                    var merged = string.Join(Environment.NewLine, LeftDocument.Text.SplitOnNewLine());
                    SetText(merged, RightSide);
                }
                else
                {
                    var merged = string.Join(Environment.NewLine, RightDocument.Text.SplitOnNewLine());
                    SetText(LeftSide, merged);
                }

                ExitEditMode();
            }, okEnabled).DisposeWith(disposables);

            CancelCommand = ReactiveCommand.Create(ExitEditMode).DisposeWith(disposables);

            EditThisCommand = ReactiveCommand.Create((bool leftSide) => { SetEditThis(leftSide); }).DisposeWith(disposables);

            EditThisHereCommand = ReactiveCommand.Create((bool leftSide) => { SetEditThis(leftSide, true); }).DisposeWith(disposables);

            CopyTextCommand = ReactiveCommand.Create((bool leftSide) => { CopyTextAsync(leftSide).ConfigureAwait(true); }).DisposeWith(disposables);

            NextConflictCommand = ReactiveCommand.Create((bool leftSide) => { FindConflict(leftSide, true, false); }).DisposeWith(disposables);

            PrevConflictCommand = ReactiveCommand.Create((bool leftSide) => { FindConflict(leftSide, false, false); }).DisposeWith(disposables);

            DeleteTextCommand = ReactiveCommand.Create((bool leftSide) => { DeleteLines(leftSide); }).DisposeWith(disposables);

            UndoCommand = ReactiveCommand.Create((bool leftSide) => { PerformUndo(leftSide); }).DisposeWith(disposables);

            RedoCommand = ReactiveCommand.Create((bool leftSide) => { PerformRedo(leftSide); }).DisposeWith(disposables);

            EditorCommand = ReactiveCommand.CreateFromTask((bool leftSide) => LaunchExternalEditorAsync(leftSide)).DisposeWith(disposables);

            ReadOnlyEditorCommand = ReactiveCommand.CreateFromTask((bool leftSide) => LaunchExternalEditorAsync(leftSide, true)).DisposeWith(disposables);

            ToggleMergeTypeCommand = ReactiveCommand.Create(() =>
            {
                var state = appStateService.Get();
                state.UseNewDiffViewer = !state.UseNewDiffViewer;
                appStateService.Save(state);
                EvaluateMergeType();
            }).DisposeWith(disposables);

            var previousEditTextState = false;
            this.WhenAnyValue(v => v.EditingText).Subscribe(s =>
            {
                if (s != previousEditTextState)
                {
                    previousEditTextState = s;
                    MessageBus.Publish(new SuspendHotkeysEvent(s));
                }
            }).DisposeWith(disposables);

            hotkeyPressedHandler.Subscribe(m =>
            {
                void performAction()
                {
                    // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                    switch (m.Hotkey)
                    {
                        case Enums.HotKeys.Ctrl_Up:
                            if (LeftSideSelected.Count == 0)
                            {
                                LeftSideSelected.Add(LeftDiff.FirstOrDefault());
                            }

                            if (UsingNewMergeType)
                            {
                                if (DiffEditorTopLine.HasValue && DiffEditorTopLine.GetValueOrDefault() - 1 < LeftDiff.Count)
                                {
                                    LeftSideSelected.Clear();
                                    LeftSideSelected.Add(LeftDiff[DiffEditorTopLine.GetValueOrDefault() - 1]);
                                }
                            }

                            FindConflict(true, false, false);
                            break;

                        case Enums.HotKeys.Ctrl_Down:
                            if (LeftSideSelected.Count == 0)
                            {
                                LeftSideSelected.Add(LeftDiff.FirstOrDefault());
                            }

                            if (UsingNewMergeType)
                            {
                                if (DiffEditorBottomLine.HasValue && DiffEditorBottomLine.GetValueOrDefault() - 1 < LeftDiff.Count)
                                {
                                    LeftSideSelected.Clear();
                                    LeftSideSelected.Add(LeftDiff[DiffEditorBottomLine.GetValueOrDefault() - 1]);
                                }
                            }

                            FindConflict(true, true, false);
                            break;

                        case Enums.HotKeys.Ctrl_Left:
                            if (LeftSideSelected.Count == 0)
                            {
                                LeftSideSelected.Add(LeftDiff.FirstOrDefault());
                            }

                            if (UsingNewMergeType)
                            {
                                if (DiffEditorTopLine.HasValue && DiffEditorTopLine.GetValueOrDefault() - 1 < LeftDiff.Count)
                                {
                                    LeftSideSelected.Clear();
                                    LeftSideSelected.Add(LeftDiff[DiffEditorTopLine.GetValueOrDefault() - 1]);
                                }
                            }

                            FindConflict(true, false, true);
                            break;

                        case Enums.HotKeys.Ctrl_Right:
                            if (LeftSideSelected.Count == 0)
                            {
                                LeftSideSelected.Add(LeftDiff.FirstOrDefault());
                            }

                            if (UsingNewMergeType)
                            {
                                if (DiffEditorBottomLine.HasValue && DiffEditorBottomLine.GetValueOrDefault() - 1 < LeftDiff.Count)
                                {
                                    LeftSideSelected.Clear();
                                    LeftSideSelected.Add(LeftDiff[DiffEditorBottomLine.GetValueOrDefault() - 1]);
                                }
                            }

                            FindConflict(true, true, true);
                            break;

                        case Enums.HotKeys.Ctrl_E:
                            if (LeftSidePatchMod || RightSidePatchMod)
                            {
                                SetEditThis(LeftSidePatchMod);
                            }

                            break;

                        case Enums.HotKeys.Ctrl_Shift_T:
                            CopyTextAsync(false).ConfigureAwait(true);
                            break;

                        case Enums.HotKeys.Ctrl_T:
                            CopyTextAsync(true).ConfigureAwait(true);
                            break;

                        case Enums.HotKeys.Ctrl_C:
                            if (LeftSidePatchMod)
                            {
                                HotkeyBeforePerform?.Invoke(this, EventArgs.Empty);
                                Copy(RightSideSelected, RightDiff, LeftDiff, false);
                                HotkeyAfterPerform?.Invoke(this, EventArgs.Empty);
                            }
                            else if (RightSidePatchMod)
                            {
                                HotkeyBeforePerform?.Invoke(this, EventArgs.Empty);
                                Copy(LeftSideSelected, LeftDiff, RightDiff, true);
                                HotkeyAfterPerform?.Invoke(this, EventArgs.Empty);
                            }

                            break;

                        case Enums.HotKeys.Ctrl_V:
                            if (LeftSidePatchMod)
                            {
                                HotkeyBeforePerform?.Invoke(this, EventArgs.Empty);
                                CopyBeforeLines(RightSideSelected, RightDiff, LeftDiff, false);
                                HotkeyAfterPerform?.Invoke(this, EventArgs.Empty);
                            }
                            else if (RightSidePatchMod)
                            {
                                HotkeyBeforePerform?.Invoke(this, EventArgs.Empty);
                                CopyBeforeLines(LeftSideSelected, LeftDiff, RightDiff, true);
                                HotkeyAfterPerform?.Invoke(this, EventArgs.Empty);
                            }

                            break;

                        case Enums.HotKeys.Ctrl_B:
                            if (LeftSidePatchMod)
                            {
                                HotkeyBeforePerform?.Invoke(this, EventArgs.Empty);
                                CopyAfterLines(RightSideSelected, LeftDiff, RightDiff, false);
                                HotkeyAfterPerform?.Invoke(this, EventArgs.Empty);
                            }
                            else if (RightSidePatchMod)
                            {
                                HotkeyBeforePerform?.Invoke(this, EventArgs.Empty);
                                CopyAfterLines(LeftSideSelected, LeftDiff, RightDiff, true);
                                HotkeyAfterPerform?.Invoke(this, EventArgs.Empty);
                            }

                            break;

                        case Enums.HotKeys.Ctrl_Z:
                            if (LeftSidePatchMod || RightSidePatchMod)
                            {
                                if (undoStack.Count > 0)
                                {
                                    PreFocusSide?.Invoke(LeftSidePatchMod);
                                    PerformUndo(LeftSidePatchMod);
                                    PostFocusSide?.Invoke(LeftSidePatchMod);
                                }
                            }

                            break;

                        case Enums.HotKeys.Ctrl_Y:
                            if (LeftSidePatchMod || RightSidePatchMod)
                            {
                                if (redoStack.Count > 0)
                                {
                                    PreFocusSide?.Invoke(LeftSidePatchMod);
                                    PerformRedo(LeftSidePatchMod);
                                    PostFocusSide?.Invoke(LeftSidePatchMod);
                                }
                            }

                            break;

                        case Enums.HotKeys.Ctrl_X:
                            if (LeftSidePatchMod || RightSidePatchMod)
                            {
                                LaunchExternalEditorAsync(LeftSidePatchMod).ConfigureAwait(true);
                            }
                            else
                            {
                                LaunchExternalEditorAsync(LeftSidePatchMod, true).ConfigureAwait(true);
                            }

                            break;
                    }
                }

                if (CanPerformHotKeyActions)
                {
                    lock (hotkeyLock)
                    {
                        Dispatcher.UIThread.SafeInvoke(performAction);
                    }
                }
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Orders the selected.
        /// </summary>
        /// <param name="selected">The selected.</param>
        /// <param name="source">The source.</param>
        /// <returns>Dictionary&lt;System.Int32, DiffPiece&gt;.</returns>
        protected virtual Dictionary<int, DiffPieceWithIndex> OrderSelected(IEnumerable<DiffPieceWithIndex> selected, IList<DiffPieceWithIndex> source)
        {
            var orderedSelected = new Dictionary<int, DiffPieceWithIndex>();
            foreach (var item in selected)
            {
                var idx = source.IndexOf(item);
                orderedSelected.Add(idx, item);
            }

            return orderedSelected.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);
        }

        /// <summary>
        /// Performs the redo.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void PerformRedo(bool leftSide)
        {
            if (redoStack.Count == 0)
            {
                return;
            }

            if (leftSide)
            {
                undoStack.Push(LeftSide);
                SetText(redoStack.Pop(), RightSide, skipStackPush: true);
            }
            else
            {
                undoStack.Push(RightSide);
                SetText(LeftSide, redoStack.Pop(), skipStackPush: true);
            }
        }

        /// <summary>
        /// Performs the undo.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        protected virtual void PerformUndo(bool leftSide)
        {
            if (undoStack.Count == 0)
            {
                return;
            }

            if (leftSide)
            {
                redoStack.Push(LeftSide);
                SetText(undoStack.Pop(), RightSide, skipStackPush: true);
            }
            else
            {
                redoStack.Push(RightSide);
                SetText(LeftSide, undoStack.Pop(), skipStackPush: true);
            }
        }

        /// <summary>
        /// Sets the edit this.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        /// <param name="focusLine">The focus line.</param>
        protected virtual void SetEditThis(bool leftSide, bool focusLine = false)
        {
            EditingLeft = leftSide;
            EditingRight = !leftSide;
            EditingText = true;
            CurrentEditText = leftSide ? LeftSide : RightSide;

            LeftDocument = new TextDocument(LeftSide);
            RightDocument = new TextDocument(RightSide);

            if (focusLine)
            {
                FocusSelectedLine = false;
            }

            FocusSelectedLine = focusLine;
        }

        /// <summary>
        /// synchronize selections as an asynchronous operation.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        /// <returns>System.Threading.Tasks.Task.</returns>
        protected virtual async Task SyncSelectionsAsync(bool leftSide)
        {
            await Task.Delay(100);
            var sourceDiff = leftSide ? LeftDiff : RightDiff;
            var syncDiff = !leftSide ? LeftDiff : RightDiff;
            var sourceCol = leftSide ? LeftSideSelected : RightSideSelected;
            var syncCol = !leftSide ? LeftSideSelected : RightSideSelected;
            var clearCol = true;
            if (sourceCol?.Count > 0)
            {
                var filtered = sourceCol.Where(p => p != null); // Must be an underlying bug?
                if (filtered.Any())
                {
                    clearCol = false;
                    syncCol.Clear();
                    foreach (var item in filtered)
                    {
                        syncCol.Add(syncDiff[sourceDiff.IndexOf(item)]);
                    }
                }
            }

            if (clearCol)
            {
                syncCol.Clear();
            }

            syncingSelection = false;
        }

        /// <summary>
        /// Launches the external editor.
        /// </summary>
        /// <param name="leftSide">if set to <c>true</c> [left side].</param>
        /// <param name="noPatchModEdit">if set to <c>true</c> [no patch mod edit].</param>
        /// <returns>A Task&lt;System.Threading.Tasks.Task&gt; representing the asynchronous operation.</returns>
        private async Task LaunchExternalEditorAsync(bool leftSide, bool noPatchModEdit = false)
        {
            var opts = externalEditorService.Get();
            var left = leftDefinition;
            var right = rightDefinition;
            if (left != null && right != null && !string.IsNullOrWhiteSpace(opts.ExternalEditorLocation) && !string.IsNullOrWhiteSpace(opts.ExternalEditorParameters) && File.Exists(opts.ExternalEditorLocation))
            {
                var files = externalEditorService.GetFiles(left, right);
                files.LeftDiff.Text = LeftSide;
                files.RightDiff.Text = RightSide;
                var arguments = externalEditorService.GetLaunchArguments(files.LeftDiff.File, files.RightDiff.File);
                if (IsReadOnlyMode)
                {
                    // Override if analyze mode only
                    noPatchModEdit = true;
                }

                if (await appAction.RunAsync(opts.ExternalEditorLocation, arguments))
                {
                    string title;
                    string message;
                    if (!noPatchModEdit)
                    {
                        message = localizationManager.GetResource(LocalizationResources.Conflict_Solver.Editor.Message);
                        title = localizationManager.GetResource(LocalizationResources.Conflict_Solver.Editor.Title);
                    }
                    else
                    {
                        message = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ReadonlyEditor.Message);
                        title = localizationManager.GetResource(LocalizationResources.Conflict_Solver.ReadonlyEditor.Title);
                    }

                    if (await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Info, !noPatchModEdit ? PromptType.ConfirmCancel : PromptType.OK))
                    {
                        if (!noPatchModEdit)
                        {
                            if (leftSide)
                            {
                                var text = files.LeftDiff.Text ?? string.Empty;
                                var merged = string.Join(Environment.NewLine, text.ReplaceTabs());
                                SetText(merged, RightSide);
                            }
                            else
                            {
                                var text = files.RightDiff.Text ?? string.Empty;
                                var merged = string.Join(Environment.NewLine, text.ReplaceTabs());
                                SetText(LeftSide, merged);
                            }
                        }
                    }
                }

                files.Dispose();
            }
        }

        /// <summary>
        /// sets the bracket mismatch text to be displayed
        /// </summary>
        private void SetBracketText()
        {
            if (LeftSidePatchMod || RightSidePatchMod)
            {
                var bracketCount = modPatchCollectionService.GetBracketCount(EditingLua ? "fake.lua" : "fake.txt", LeftSidePatchMod ? LeftSide : RightSide);
                if (bracketCount.OpenBracketCount != bracketCount.CloseBracketCount)
                {
                    var message = localizationManager.GetResource(LocalizationResources.Conflict_Solver.BracketMismatchError.Message).FormatIronySmart(new { bracketCount.OpenBracketCount, bracketCount.CloseBracketCount });
                    BracketMismatchText = message;
                }
                else
                {
                    BracketMismatchText = string.Empty;
                }
            }
            else
            {
                BracketMismatchText = string.Empty;
            }
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class DiffPieceWithIndex.
        /// Implements the <see cref="DiffPlex.DiffBuilder.Model.DiffPiece" />
        /// Implements the <see cref="IronyModManager.Shared.Models.IQueryableModel" />
        /// </summary>
        /// <seealso cref="IronyModManager.Shared.Models.IQueryableModel" />
        /// <seealso cref="DiffPlex.DiffBuilder.Model.DiffPiece" />
        public class DiffPieceWithIndex : DiffPiece, IQueryableModel
        {
            #region Properties

            /// <summary>
            /// Gets or sets the index.
            /// </summary>
            /// <value>The index.</value>
            public int Index { get; set; }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
            /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
            public override bool Equals(object obj)
            {
                return Equals(obj as DiffPiece);
            }

            /// <summary>
            /// Equals.
            /// </summary>
            /// <param name="other">The other.</param>
            /// <returns>A bool.</returns>
            public new bool Equals(DiffPiece other)
            {
                var result = base.Equals(other);
                if (result && other is DiffPieceWithIndex diffPieceWithIndex)
                {
                    return Index.Equals(diffPieceWithIndex.Index);
                }

                return result;
            }

            /// <summary>
            /// Returns a hash code for this instance.
            /// </summary>
            /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
            public override int GetHashCode() => base.GetHashCode();

            /// <summary>
            /// Determines whether the specified term is match.
            /// </summary>
            /// <param name="term">The term.</param>
            /// <returns><c>true</c> if the specified term is match; otherwise, <c>false</c>.</returns>
            public bool IsMatch(string term)
            {
                if (string.IsNullOrWhiteSpace(Text))
                {
                    return false;
                }

                term ??= string.Empty;
                return Text.Trim().StartsWith(term, StringComparison.OrdinalIgnoreCase);
            }

            #endregion Methods
        }

        #endregion Classes
    }
}
