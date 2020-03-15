// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 03-15-2020
// ***********************************************************************
// <copyright file="ModHolderControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation.Actions;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;
using SmartFormat;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class ModHolderViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModHolderControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

        /// <summary>
        /// The notification action
        /// </summary>
        private readonly INotificationAction notificationAction;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModHolderControlViewModel" /> class.
        /// </summary>
        /// <param name="modService">The mod service.</param>
        /// <param name="notificationAction">The notification action.</param>
        /// <param name="localizationManager">The localization manager.</param>
        /// <param name="installedModsControlViewModel">The installed mods control view model.</param>
        /// <param name="collectionModsControlViewModel">The collection mods control view model.</param>
        public ModHolderControlViewModel(IModService modService, INotificationAction notificationAction, ILocalizationManager localizationManager,
            InstalledModsControlViewModel installedModsControlViewModel, CollectionModsControlViewModel collectionModsControlViewModel)
        {
            this.modService = modService;
            this.notificationAction = notificationAction;
            this.localizationManager = localizationManager;
            InstalledMods = installedModsControlViewModel;
            CollectionMods = collectionModsControlViewModel;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the apply.
        /// </summary>
        /// <value>The apply.</value>
        [StaticLocalization(LocalizationResources.Mod_Actions.Apply)]
        public virtual string Apply { get; protected set; }

        /// <summary>
        /// Gets or sets the apply command.
        /// </summary>
        /// <value>The apply command.</value>
        public virtual ReactiveCommand<Unit, Unit> ApplyCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the collection mods.
        /// </summary>
        /// <value>The collection mods.</value>
        public virtual CollectionModsControlViewModel CollectionMods { get; protected set; }

        /// <summary>
        /// Gets or sets the installed mods.
        /// </summary>
        /// <value>The installed mods.</value>
        public virtual InstalledModsControlViewModel InstalledMods { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// apply collection as an asynchronous operation.
        /// </summary>
        protected virtual async Task ApplyCollectionAsync()
        {
            if (CollectionMods.SelectedModCollection != null)
            {
                TriggerOverlay(true, localizationManager.GetResource(LocalizationResources.Mod_Actions.Overlay_Apply_Message));
                var notificationType = NotificationType.Success;
                var result = await modService.ExportModsAsync(CollectionMods.SelectedMods.ToList());
                string title;
                string message;
                if (result)
                {
                    title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionApplied.Title);
                    message = Smart.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionApplied.Message), new { CollectionName = CollectionMods.SelectedModCollection.Name });
                }
                else
                {
                    title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionNotApplied.Title);
                    message = Smart.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionNotApplied.Message), new { CollectionName = CollectionMods.SelectedModCollection.Name });
                    notificationType = NotificationType.Error;
                }
                notificationAction.ShowNotification(title, message, notificationType, 5);
                TriggerOverlay(false);
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            this.WhenAnyValue(v => v.InstalledMods.Mods).Subscribe(v =>
            {
                CollectionMods.SetMods(v);
            });

            ApplyCommand = ReactiveCommand.Create(() =>
            {
                ApplyCollectionAsync().ConfigureAwait(true);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
