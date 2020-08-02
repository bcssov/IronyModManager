// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 07-28-2020
//
// Last Modified By : Mario
// Last Modified On : 07-30-2020
// ***********************************************************************
// <copyright file="ConflictSolverCustomConflictsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AvaloniaEdit.Document;
using IronyModManager.Common.ViewModels;
using IronyModManager.DI;
using IronyModManager.Implementation.Actions;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ConflictSolverCustomConflictsControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ConflictSolverCustomConflictsControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager;

        /// <summary>
        /// The mod patch collection service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

        /// <summary>
        /// The notification action
        /// </summary>
        private readonly INotificationAction notificationAction;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictSolverCustomConflictsControlViewModel" /> class.
        /// </summary>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        /// <param name="notificationAction">The notification action.</param>
        /// <param name="localizationManager">The localization manager.</param>
        public ConflictSolverCustomConflictsControlViewModel(IModPatchCollectionService modPatchCollectionService, INotificationAction notificationAction, ILocalizationManager localizationManager)
        {
            this.modPatchCollectionService = modPatchCollectionService;
            this.notificationAction = notificationAction;
            this.localizationManager = localizationManager;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [allow save].
        /// </summary>
        /// <value><c>true</c> if [allow save]; otherwise, <c>false</c>.</value>
        public virtual bool AllowSave { get; protected set; }

        /// <summary>
        /// Gets or sets the close.
        /// </summary>
        /// <value>The close.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.CustomPatch.Close)]
        public virtual string Close { get; protected set; }

        /// <summary>
        /// Gets or sets the close command.
        /// </summary>
        /// <value>The close command.</value>
        public virtual ReactiveCommand<Unit, Unit> CloseCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public virtual string CollectionName { get; protected set; }

        /// <summary>
        /// Gets or sets the conflict result.
        /// </summary>
        /// <value>The conflict result.</value>
        public virtual IConflictResult ConflictResult { get; protected set; }

        /// <summary>
        /// Gets or sets the current edit text.
        /// </summary>
        /// <value>The current edit text.</value>
        public virtual string CurrentEditText { get; set; }

        /// <summary>
        /// Gets or sets the custom patch.
        /// </summary>
        /// <value>The custom patch.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.CustomPatch.Title)]
        public virtual string CustomPatch { get; protected set; }

        /// <summary>
        /// Gets or sets the custom patch command.
        /// </summary>
        /// <value>The custom patch command.</value>
        public virtual ReactiveCommand<Unit, Unit> CustomPatchCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        /// <value>The document.</value>
        public virtual TextDocument Document { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [editing yaml].
        /// </summary>
        /// <value><c>true</c> if [editing yaml]; otherwise, <c>false</c>.</value>
        public virtual bool EditingYaml { get; protected set; }

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
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public virtual bool IsOpen { get; protected set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The path.</value>
        public virtual string Path { get; protected set; }

        /// <summary>
        /// Gets or sets the path watermark.
        /// </summary>
        /// <value>The path watermark.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.CustomPatch.Path)]
        public virtual string PathWatermark { get; protected set; }

        /// <summary>
        /// Gets or sets the save.
        /// </summary>
        /// <value>The save.</value>
        [StaticLocalization(LocalizationResources.Conflict_Solver.CustomPatch.Save)]
        public virtual string Save { get; protected set; }

        /// <summary>
        /// Gets or sets the save command.
        /// </summary>
        /// <value>The save command.</value>
        public virtual ReactiveCommand<Unit, Unit> SaveCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ConflictSolverCustomConflictsControlViewModel" /> is saved.
        /// </summary>
        /// <value><c>true</c> if saved; otherwise, <c>false</c>.</value>
        public virtual bool Saved { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Forces the close popup.
        /// </summary>
        public virtual void ForceClosePopup()
        {
            IsOpen = false;
        }

        /// <summary>
        /// Sets the content.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="text">The text.</param>
        public virtual void SetContent(string path, string text)
        {
            Path = path;
            CurrentEditText = text;
            Document = new TextDocument(CurrentEditText);
            IsOpen = true;
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="conflictResult">The conflict result.</param>
        /// <param name="collectionName">Name of the collection.</param>
        public virtual void SetParameters(IConflictResult conflictResult, string collectionName)
        {
            ConflictResult = conflictResult;
            CollectionName = collectionName;
        }

        /// <summary>
        /// Evals the conditions.
        /// </summary>
        protected virtual void EvalConditions()
        {
            var code = CurrentEditText ?? string.Empty;
            var path = Path ?? string.Empty;
            var extension = System.IO.Path.GetExtension(path.StandardizeDirectorySeparator());
            AllowSave = !string.IsNullOrWhiteSpace(code) && !string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(extension);
            EditingYaml = path.StartsWith(Shared.Constants.LocalizationDirectory, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Initializes the definition.
        /// </summary>
        /// <returns>IDefinition.</returns>
        protected virtual IDefinition InitDefinition()
        {
            string formatType(string path)
            {
                var formatted = System.IO.Path.GetDirectoryName(path);
                var type = System.IO.Path.GetExtension(path).Trim('.');
                if (!Shared.Constants.TextExtensions.Any(s => s.EndsWith(type, StringComparison.OrdinalIgnoreCase)))
                {
                    type = "txt";
                }
                return $"{formatted.ToLowerInvariant()}{System.IO.Path.DirectorySeparatorChar}{type}";
            }
            var path = Path.StandardizeDirectorySeparator();
            var definition = DIResolver.Get<IDefinition>();
            definition.Code = CurrentEditText;
            definition.File = path;
            definition.Id = System.IO.Path.GetFileName(path).ToLowerInvariant();
            definition.Type = formatType(path);
            return definition;
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var saveEnabled = this.WhenAnyValue(v => v.AllowSave, v => v == true);

            this.WhenAnyValue(v => v.Path).Subscribe(s =>
            {
                EvalConditions();
            }).DisposeWith(disposables);

            this.WhenAnyValue(v => v.CurrentEditText).Subscribe(s =>
            {
                EvalConditions();
            }).DisposeWith(disposables);

            CloseCommand = ReactiveCommand.Create(() =>
            {
                ForceClosePopup();
            }).DisposeWith(disposables);

            CustomPatchCommand = ReactiveCommand.Create(() =>
            {
                Document = new TextDocument(CurrentEditText ?? string.Empty);
                IsOpen = true;
            }).DisposeWith(disposables);

            SaveCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (ConflictResult.CustomConflicts.GetByFile(Path.StandardizeDirectorySeparator()).Count() == 0)
                {
                    if (await modPatchCollectionService.AddCustomModPatchAsync(ConflictResult, InitDefinition(), CollectionName))
                    {
                        Saved = true;
                        Saved = false;
                        Path = string.Empty;
                        CurrentEditText = string.Empty;
                        Document = new TextDocument(CurrentEditText);
                    }
                }
                else
                {
                    notificationAction.ShowNotification(localizationManager.GetResource(LocalizationResources.Notifications.CustomPatchExists.Title),
                        localizationManager.GetResource(LocalizationResources.Notifications.CustomPatchExists.Message), NotificationType.Warning);
                }
            }, saveEnabled).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
