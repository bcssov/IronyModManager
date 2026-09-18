// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-02-2020
//
// Last Modified By : Mario
// Last Modified On : 04-05-2020
// ***********************************************************************
// <copyright file="SearchModsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Reactive;
using System.Reactive.Disposables;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation;
using IronyModManager.Localization.Attributes;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class SearchModsControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class SearchModsControlViewModel : BaseViewModel
    {
        private readonly AdvancedModFilterQueryComposer queryComposer = new AdvancedModFilterQueryComposer();

        #region Properties

        [StaticLocalization(LocalizationResources.Filter.Advanced.Achievements)]
        public virtual string AchievementsText { get; protected set; }

        public virtual int AdvancedAchievements { get; set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Apply)]
        public virtual string AdvancedApplyText { get; protected set; }

        public virtual ReactiveCommand<Unit, Unit> AdvancedApplyCommand { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Cancel)]
        public virtual string AdvancedCancelText { get; protected set; }

        public virtual ReactiveCommand<Unit, Unit> AdvancedCancelCommand { get; protected set; }

        public virtual ReactiveCommand<Unit, Unit> AdvancedClearCommand { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.CustomQuery)]
        public virtual string AdvancedCustomQueryText { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Title)]
        public virtual string AdvancedFilterText { get; protected set; }

        public virtual string AdvancedPlainText { get; set; }

        public virtual int AdvancedSelected { get; set; }

        public virtual string AdvancedVersion { get; set; }

        public virtual int AdvancedVersionMode { get; set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Any)]
        public virtual string AnyText { get; protected set; }

        /// <summary>
        /// Gets or sets the clear text.
        /// </summary>
        /// <value>The clear text.</value>
        [StaticLocalization(LocalizationResources.Filter.Clear)]
        public virtual string ClearText { get; protected set; }

        /// <summary>
        /// Gets or sets the clear text command.
        /// </summary>
        /// <value>The clear text command.</value>
        public virtual ReactiveCommand<Unit, Unit> ClearTextCommand { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Compatible)]
        public virtual string CompatibleText { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Exclude)]
        public virtual string ExcludeText { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Include)]
        public virtual string IncludeText { get; protected set; }

        public virtual bool IsAdvancedFilterOpen { get; set; }

        public virtual bool IsCustomAdvancedQuery { get; set; }

        public virtual bool LocalSource { get; set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Local)]
        public virtual string LocalText { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Name)]
        public virtual string NameText { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.No)]
        public virtual string NoText { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.NotCompatible)]
        public virtual string NotCompatibleText { get; protected set; }

        public virtual ReactiveCommand<Unit, Unit> OpenAdvancedFilterCommand { get; protected set; }

        public virtual bool ParadoxSource { get; set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Paradox)]
        public virtual string ParadoxText { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Reset)]
        public virtual string ResetText { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Selected)]
        public virtual string SelectedText { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Source)]
        public virtual string SourceText { get; protected set; }

        public virtual bool SteamSource { get; set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Steam)]
        public virtual string SteamText { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Version)]
        public virtual string VersionText { get; protected set; }

        [StaticLocalization(LocalizationResources.Filter.Advanced.Yes)]
        public virtual string YesText { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterCommands.Achievements)]
        public virtual string AchievementsCommand { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterCommands.Local)]
        public virtual string LocalCommand { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterCommands.No)]
        public virtual string NoCommand { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterOperators.Negate)]
        public virtual string NegateOperator { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterOperators.OrStatementSeparator)]
        public virtual string OrSeparator { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterCommands.Paradox)]
        public virtual string ParadoxCommand { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterCommands.Selected)]
        public virtual string SelectedCommand { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterCommands.Source)]
        public virtual string SourceCommand { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterOperators.StatementSeparator)]
        public virtual string StatementSeparator { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterCommands.Steam)]
        public virtual string SteamCommand { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterOperators.ValueSeparator)]
        public virtual string ValueSeparator { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterCommands.Version)]
        public virtual string VersionCommand { get; protected set; }

        [StaticLocalization(LocalizationResources.FilterCommands.Yes)]
        public virtual string YesCommand { get; protected set; }

        /// <summary>
        /// Gets or sets down arrow command.
        /// </summary>
        /// <value>Down arrow command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<bool>> DownArrowCommand { get; protected set; }

        /// <summary>
        /// Gets or sets down arrow text.
        /// </summary>
        /// <value>Down arrow text.</value>
        [StaticLocalization(LocalizationResources.Filter.DownArrow)]
        public virtual string DownArrowText { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show arrows].
        /// </summary>
        /// <value><c>true</c> if [show arrows]; otherwise, <c>false</c>.</value>
        public virtual bool ShowArrows { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public virtual string Text { get; set; }

        /// <summary>
        /// Gets or sets up arrow command.
        /// </summary>
        /// <value>Up arrow command.</value>
        public virtual ReactiveCommand<Unit, CommandResult<bool>> UpArrowCommand { get; protected set; }

        /// <summary>
        /// Gets or sets up arrow text.
        /// </summary>
        /// <value>Up arrow text.</value>
        [StaticLocalization(LocalizationResources.Filter.UpArrow)]
        public virtual string UpArrowText { get; protected set; }

        /// <summary>
        /// Gets or sets the watermark text.
        /// </summary>
        /// <value>The watermark text.</value>
        public virtual string WatermarkText { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var arrowEnabled = this.WhenAnyValue(v => v.Text, v => !string.IsNullOrWhiteSpace(v));

            ClearTextCommand = ReactiveCommand.Create(() =>
            {
                Text = string.Empty;
            }).DisposeWith(disposables);

            OpenAdvancedFilterCommand = ReactiveCommand.Create(() =>
            {
                IsCustomAdvancedQuery = !queryComposer.TryParse(Text, GetSyntax(), out var state);
                SetAdvancedState(state);
                IsAdvancedFilterOpen = true;
            }).DisposeWith(disposables);

            AdvancedApplyCommand = ReactiveCommand.Create(() =>
            {
                Text = queryComposer.Compose(GetAdvancedState(), GetSyntax());
                IsAdvancedFilterOpen = false;
            }).DisposeWith(disposables);

            AdvancedClearCommand = ReactiveCommand.Create(() =>
            {
                SetAdvancedState(new AdvancedModFilterState());
                Text = string.Empty;
                IsAdvancedFilterOpen = false;
            }).DisposeWith(disposables);

            AdvancedCancelCommand = ReactiveCommand.Create(() =>
            {
                IsAdvancedFilterOpen = false;
            }).DisposeWith(disposables);

            UpArrowCommand = ReactiveCommand.Create(() =>
            {
                return new CommandResult<bool>(true, CommandState.Success);
            }, arrowEnabled).DisposeWith(disposables);

            DownArrowCommand = ReactiveCommand.Create(() =>
            {
                return new CommandResult<bool>(false, CommandState.Success);
            }, arrowEnabled).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        private AdvancedModFilterState GetAdvancedState()
        {
            return new AdvancedModFilterState
            {
                Achievements = (AdvancedFilterBooleanState)AdvancedAchievements,
                Local = LocalSource,
                Paradox = ParadoxSource,
                PlainText = AdvancedPlainText,
                Selected = (AdvancedFilterBooleanState)AdvancedSelected,
                Steam = SteamSource,
                Version = AdvancedVersion,
                VersionExcluded = AdvancedVersionMode == 1
            };
        }

        private AdvancedModFilterQuerySyntax GetSyntax()
        {
            return new AdvancedModFilterQuerySyntax
            {
                Achievements = AchievementsCommand,
                Local = LocalCommand,
                Negate = NegateOperator,
                No = NoCommand,
                OrSeparator = OrSeparator,
                Paradox = ParadoxCommand,
                Selected = SelectedCommand,
                Source = SourceCommand,
                StatementSeparator = StatementSeparator,
                Steam = SteamCommand,
                ValueSeparator = ValueSeparator,
                Version = VersionCommand,
                Yes = YesCommand
            };
        }

        private void SetAdvancedState(AdvancedModFilterState state)
        {
            AdvancedAchievements = (int)state.Achievements;
            AdvancedPlainText = state.PlainText;
            AdvancedSelected = (int)state.Selected;
            AdvancedVersion = state.Version;
            AdvancedVersionMode = state.VersionExcluded ? 1 : 0;
            LocalSource = state.Local;
            ParadoxSource = state.Paradox;
            SteamSource = state.Steam;
        }

        #endregion Methods
    }
}
