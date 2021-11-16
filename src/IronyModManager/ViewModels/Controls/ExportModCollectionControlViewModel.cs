// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-09-2020
//
// Last Modified By : Mario
// Last Modified On : 11-16-2021
// ***********************************************************************
// <copyright file="ExportModCollectionControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation;
using IronyModManager.Implementation.Actions;
using IronyModManager.Localization.Attributes;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ExportModCollectionControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ExportModCollectionControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The file dialog action
        /// </summary>
        private readonly IFileDialogAction fileDialogAction;

        /// <summary>
        /// The game service
        /// </summary>
        private readonly IGameService gameService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportModCollectionControlViewModel" /> class.
        /// </summary>
        /// <param name="gameService">The game service.</param>
        /// <param name="fileDialogAction">The file dialog action.</param>
        public ExportModCollectionControlViewModel(IGameService gameService, IFileDialogAction fileDialogAction)
        {
            this.fileDialogAction = fileDialogAction;
            this.gameService = gameService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [allow mod selection].
        /// </summary>
        /// <value><c>true</c> if [allow mod selection]; otherwise, <c>false</c>.</value>
        public virtual bool AllowModSelection { get; set; }

        /// <summary>
        /// Gets or sets the name of the collection.
        /// </summary>
        /// <value>The name of the collection.</value>
        public virtual string CollectionName { get; set; }

        /// <summary>
        /// Gets or sets the export.
        /// </summary>
        /// <value>The export.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Export)]
        public virtual string Export { get; protected set; }

        /// <summary>
        /// Gets or sets the export command.
        /// </summary>
        /// <value>The export command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<string>> ExportCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the export dialog title.
        /// </summary>
        /// <value>The export dialog title.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Export_Dialog_Title)]
        public virtual string ExportDialogTitle { get; protected set; }

        /// <summary>
        /// Gets or sets the export order only.
        /// </summary>
        /// <value>The export order only.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ExportOther.OrderOnly)]
        public virtual string ExportOrderOnly { get; protected set; }

        /// <summary>
        /// Gets or sets the export order only command.
        /// </summary>
        /// <value>The export order only command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<string>> ExportOrderOnlyCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the export other.
        /// </summary>
        /// <value>The export other.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ExportOther.Title)]
        public virtual string ExportOther { get; protected set; }

        /// <summary>
        /// Gets or sets the export other close.
        /// </summary>
        /// <value>The export other close.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ExportOther.Close)]
        public virtual string ExportOtherClose { get; protected set; }

        /// <summary>
        /// Gets or sets the export other close command.
        /// </summary>
        /// <value>The export other close command.</value>
        public virtual ReactiveCommand<Unit, Unit> ExportOtherCloseCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the export other command.
        /// </summary>
        /// <value>The export other command.</value>
        public virtual ReactiveCommand<Unit, Unit> ExportOtherCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the export paradox launcher json.
        /// </summary>
        /// <value>The export paradox launcher json.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ExportOther.ParadoxLauncherJson)]
        public virtual string ExportParadoxLauncherJson { get; protected set; }

        /// <summary>
        /// Gets or sets the export paradox launcher json202110.
        /// </summary>
        /// <value>The export paradox launcher json202110.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ExportOther.ParadoxLauncherJson202110)]
        public virtual string ExportParadoxLauncherJson202110 { get; protected set; }

        /// <summary>
        /// Gets or sets the export paradox launcher json202110 command.
        /// </summary>
        /// <value>The export paradox launcher json202110 command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<string>> ExportParadoxLauncherJson202110Command { get; protected set; }

        /// <summary>
        /// Gets or sets the export paradox launcher json command.
        /// </summary>
        /// <value>The export paradox launcher json command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<string>> ExportParadoxLauncherJsonCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the export whole collection.
        /// </summary>
        /// <value>The export whole collection.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ExportOther.WholeCollection)]
        public virtual string ExportWholeCollection { get; protected set; }

        /// <summary>
        /// Gets or sets the export whole collection command.
        /// </summary>
        /// <value>The export whole collection command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<string>> ExportWholeCollectionCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the import.
        /// </summary>
        /// <value>The import.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Import)]
        public virtual string Import { get; protected set; }

        /// <summary>
        /// Gets or sets the import command.
        /// </summary>
        /// <value>The import command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<string>> ImportCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the import dialog.
        /// </summary>
        /// <value>The import dialog.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Import_Dialog_Title)]
        public virtual string ImportDialogTitle { get; protected set; }

        /// <summary>
        /// Gets or sets the import other.
        /// </summary>
        /// <value>The import other.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ImportOther.Title)]
        public virtual string ImportOther { get; protected set; }

        /// <summary>
        /// Gets or sets the import other close.
        /// </summary>
        /// <value>The import other close.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ImportOther.Close)]
        public virtual string ImportOtherClose { get; protected set; }

        /// <summary>
        /// Gets or sets the import other close command.
        /// </summary>
        /// <value>The import other close command.</value>
        public virtual ReactiveCommand<Unit, Unit> ImportOtherCloseCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the import other command.
        /// </summary>
        /// <value>The import other command.</value>
        public virtual ReactiveCommand<Unit, Unit> ImportOtherCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the import other paradox.
        /// </summary>
        /// <value>The import other paradox.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ImportOther.Paradox)]
        public virtual string ImportOtherParadox { get; protected set; }

        /// <summary>
        /// Gets or sets the import other paradox beta launcher.
        /// </summary>
        /// <value>The import other paradox beta launcher.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ImportOther.ParadoxLauncherBeta)]
        public virtual string ImportOtherParadoxBetaLauncher { get; protected set; }

        /// <summary>
        /// Gets or sets the import other paradox command.
        /// </summary>
        /// <value>The import other paradox command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<string>> ImportOtherParadoxCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the import other paradox json launcher.
        /// </summary>
        /// <value>The import other paradox json launcher.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ImportOther.ParadoxLauncherJson)]
        public virtual string ImportOtherParadoxJsonLauncher { get; protected set; }

        /// <summary>
        /// Gets or sets the import other paradox launcher.
        /// </summary>
        /// <value>The import other paradox launcher.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ImportOther.ParadoxLauncher)]
        public virtual string ImportOtherParadoxLauncher { get; protected set; }

        /// <summary>
        /// Gets or sets the import other paradox launcher beta command.
        /// </summary>
        /// <value>The import other paradox launcher beta command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<string>> ImportOtherParadoxLauncherBetaCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the import other paradox launcher command.
        /// </summary>
        /// <value>The import other paradox launcher command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<string>> ImportOtherParadoxLauncherCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the import other paradox launcher json command.
        /// </summary>
        /// <value>The import other paradox launcher json command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<string>> ImportOtherParadoxLauncherJsonCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the import other paradoxos.
        /// </summary>
        /// <value>The import other paradoxos.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.ImportOther.Paradoxos)]
        public virtual string ImportOtherParadoxos { get; protected set; }

        /// <summary>
        /// Gets or sets the import other paradoxos command.
        /// </summary>
        /// <value>The import other paradoxos command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<string>> ImportOtherParadoxosCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is export open.
        /// </summary>
        /// <value><c>true</c> if this instance is export open; otherwise, <c>false</c>.</value>
        public virtual bool IsExportOpen { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public virtual bool IsImportOpen { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show advanced features].
        /// </summary>
        /// <value><c>true</c> if [show advanced features]; otherwise, <c>false</c>.</value>
        public virtual bool ShowAdvancedFeatures { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Forces the close.
        /// </summary>
        public virtual void ForceClose()
        {
            IsImportOpen = false;
            IsExportOpen = false;
        }

        /// <summary>
        /// Evals the advanced features visibility.
        /// </summary>
        protected virtual void EvalAdvancedFeaturesVisibility()
        {
            var game = gameService.GetSelected();
            ShowAdvancedFeatures = (game?.AdvancedFeaturesSupported).GetValueOrDefault();
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            EvalAdvancedFeaturesVisibility();

            var allowModSelectionEnabled = this.WhenAnyValue(v => v.AllowModSelection);

            ExportCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await fileDialogAction.SaveDialogAsync(ExportDialogTitle, CollectionName, Shared.Constants.ZipExtensionWithoutDot);
                return new CommandResult<string>(result, !string.IsNullOrWhiteSpace(result) ? CommandState.Success : CommandState.Failed);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            ImportCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await fileDialogAction.OpenDialogAsync(ImportDialogTitle, string.Empty, Shared.Constants.ZipExtensionWithoutDot);
                return new CommandResult<string>(result, !string.IsNullOrWhiteSpace(result) ? CommandState.Success : CommandState.Failed);
            }).DisposeWith(disposables);

            ImportOtherParadoxosCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await fileDialogAction.OpenDialogAsync(ImportDialogTitle, string.Empty, $"{Shared.Constants.JsonExtensionWithoutDot}, {Shared.Constants.XMLExtensionWithoutDot}",
                    Shared.Constants.JsonExtensionWithoutDot, Shared.Constants.XMLExtensionWithoutDot);
                return new CommandResult<string>(result, !string.IsNullOrWhiteSpace(result) ? CommandState.Success : CommandState.Failed);
            }).DisposeWith(disposables);

            ImportOtherParadoxCommand = ReactiveCommand.Create(() =>
            {
                return new CommandResult<string>(string.Empty, CommandState.Success);
            }).DisposeWith(disposables);

            ImportOtherParadoxLauncherCommand = ReactiveCommand.Create(() =>
            {
                return new CommandResult<string>(string.Empty, CommandState.Success);
            }).DisposeWith(disposables);

            ImportOtherParadoxLauncherBetaCommand = ReactiveCommand.Create(() =>
            {
                return new CommandResult<string>(string.Empty, CommandState.Success);
            }).DisposeWith(disposables);

            ImportOtherParadoxLauncherJsonCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await fileDialogAction.OpenDialogAsync(ImportDialogTitle, string.Empty, Shared.Constants.JsonExtensionWithoutDot);
                return new CommandResult<string>(result, !string.IsNullOrWhiteSpace(result) ? CommandState.Success : CommandState.Failed);
            }).DisposeWith(disposables);

            ImportOtherCommand = ReactiveCommand.Create(() =>
            {
                IsImportOpen = true;
            }).DisposeWith(disposables);

            ImportOtherCloseCommand = ReactiveCommand.Create(() =>
            {
                IsImportOpen = false;
            }).DisposeWith(disposables);

            ExportOtherCommand = ReactiveCommand.Create(() =>
            {
                IsExportOpen = true;
            }).DisposeWith(disposables);

            ExportOtherCloseCommand = ReactiveCommand.Create(() =>
            {
                IsExportOpen = false;
            }).DisposeWith(disposables);

            ExportOrderOnlyCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await fileDialogAction.SaveDialogAsync(ExportDialogTitle, CollectionName, Shared.Constants.ZipExtensionWithoutDot);
                return new CommandResult<string>(result, !string.IsNullOrWhiteSpace(result) ? CommandState.Success : CommandState.Failed);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            ExportWholeCollectionCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await fileDialogAction.SaveDialogAsync(ExportDialogTitle, CollectionName, Shared.Constants.ZipExtensionWithoutDot);
                return new CommandResult<string>(result, !string.IsNullOrWhiteSpace(result) ? CommandState.Success : CommandState.Failed);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            ExportParadoxLauncherJsonCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await fileDialogAction.SaveDialogAsync(ExportDialogTitle, CollectionName, Shared.Constants.JsonExtensionWithoutDot);
                return new CommandResult<string>(result, !string.IsNullOrWhiteSpace(result) ? CommandState.Success : CommandState.Failed);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            ExportParadoxLauncherJson202110Command = ReactiveCommand.CreateFromTask(async () =>
            {
                var result = await fileDialogAction.SaveDialogAsync(ExportDialogTitle, CollectionName, Shared.Constants.JsonExtensionWithoutDot);
                return new CommandResult<string>(result, !string.IsNullOrWhiteSpace(result) ? CommandState.Success : CommandState.Failed);
            }, allowModSelectionEnabled).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Called when [selected game changed].
        /// </summary>
        /// <param name="game">The game.</param>
        protected override void OnSelectedGameChanged(Models.Common.IGame game)
        {
            base.OnSelectedGameChanged(game);
            EvalAdvancedFeaturesVisibility();
        }

        #endregion Methods
    }
}
