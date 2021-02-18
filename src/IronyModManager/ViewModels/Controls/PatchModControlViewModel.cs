// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-16-2021
//
// Last Modified By : Mario
// Last Modified On : 02-18-2021
// ***********************************************************************
// <copyright file="PatchModControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation.Actions;
using IronyModManager.Localization;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;
using SmartFormat;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class PatchModControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class PatchModControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The active class
        /// </summary>
        private const string ActiveClass = "PatchModActive";

        /// <summary>
        /// The disabled class
        /// </summary>
        private const string InactiveClass = "PatchModInactive";

        /// <summary>
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager;

        /// <summary>
        /// The mod collection service
        /// </summary>
        private readonly IModCollectionService modCollectionService;

        /// <summary>
        /// The mod service
        /// </summary>
        private readonly IModService modService;

        /// <summary>
        /// The notification action
        /// </summary>
        private readonly INotificationAction notificationAction;

        /// <summary>
        /// The mod collection
        /// </summary>
        private IModCollection modCollection;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchModControlViewModel" /> class.
        /// </summary>
        /// <param name="notificationAction">The notification action.</param>
        /// <param name="modCollectionService">The mod collection service.</param>
        /// <param name="modService">The mod service.</param>
        /// <param name="localizationManager">The localization manager.</param>
        public PatchModControlViewModel(INotificationAction notificationAction, IModCollectionService modCollectionService, IModService modService, ILocalizationManager localizationManager)
        {
            this.notificationAction = notificationAction;
            this.modService = modService;
            this.localizationManager = localizationManager;
            this.modCollectionService = modCollectionService;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the close.
        /// </summary>
        /// <value>The close.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.PatchMod.Actions.Close)]
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
        public virtual string CollectionName { get; set; }

        /// <summary>
        /// Gets or sets the delete.
        /// </summary>
        /// <value>The delete.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.PatchMod.Actions.Delete)]
        public virtual string Delete { get; protected set; }

        /// <summary>
        /// Gets or sets the delete command.
        /// </summary>
        /// <value>The delete command.</value>
        public virtual ReactiveCommand<Unit, Unit> DeleteCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the disable.
        /// </summary>
        /// <value>The disable.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.PatchMod.Actions.Disable)]
        public virtual string Disable { get; protected set; }

        /// <summary>
        /// Gets or sets the disable command.
        /// </summary>
        /// <value>The disable command.</value>
        public virtual ReactiveCommand<Unit, Unit> DisableCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the enable.
        /// </summary>
        /// <value>The enable.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.PatchMod.Actions.Enable)]
        public virtual string Enable { get; protected set; }

        /// <summary>
        /// Gets or sets the enable command.
        /// </summary>
        /// <value>The enable command.</value>
        public virtual ReactiveCommand<Unit, Unit> EnableCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open.
        /// </summary>
        /// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
        public virtual bool IsOpen { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is open visible.
        /// </summary>
        /// <value><c>true</c> if this instance is open visible; otherwise, <c>false</c>.</value>
        public virtual bool IsOpenVisible { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is patch mod enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is patch mod enabled; otherwise, <c>false</c>.</value>
        public virtual bool IsPatchModEnabled { get; protected set; }

        /// <summary>
        /// Gets or sets the open.
        /// </summary>
        /// <value>The open.</value>
        public virtual string Open { get; protected set; }

        /// <summary>
        /// Gets or sets the open class.
        /// </summary>
        /// <value>The open class.</value>
        public virtual string OpenClass { get; protected set; }

        /// <summary>
        /// Gets or sets the open command.
        /// </summary>
        /// <value>The open command.</value>
        public virtual ReactiveCommand<Unit, Unit> OpenCommand { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether [patch deleted].
        /// </summary>
        /// <value><c>true</c> if [patch deleted]; otherwise, <c>false</c>.</value>
        public virtual bool PatchDeleted { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Forces the close.
        /// </summary>
        public virtual void ForceClose()
        {
            IsOpen = false;
        }

        /// <summary>
        /// Called when [locale changed].
        /// </summary>
        /// <param name="newLocale">The new locale.</param>
        /// <param name="oldLocale">The old locale.</param>
        public override void OnLocaleChanged(string newLocale, string oldLocale)
        {
            SetOpenCaption();
            base.OnLocaleChanged(newLocale, oldLocale);
        }

        /// <summary>
        /// Sets the parameters.
        /// </summary>
        /// <param name="modCollection">The mod collection.</param>
        public virtual void SetParameters(IModCollection modCollection)
        {
            if (modCollection != null)
            {
                SetCollectionDataAsync(modCollection).ConfigureAwait(false);
            }
            else
            {
                IsOpenVisible = false;
            }
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            void syncPatchState()
            {
                if (modCollection != null)
                {
                    IsPatchModEnabled = modCollection.PatchModEnabled = !IsPatchModEnabled;
                    OpenClass = IsPatchModEnabled ? ActiveClass : InactiveClass;
                    var collection = modCollectionService.Get(modCollection.Name);
                    collection.PatchModEnabled = IsPatchModEnabled;
                    modCollectionService.Save(collection);
                    ForceClose();
                }
                SetOpenCaption();
            }
            async Task deletePatch()
            {
                if (modCollection != null)
                {
                    var title = localizationManager.GetResource(LocalizationResources.Collection_Mods.PatchMod.DeletePrompt.Title);
                    var message = localizationManager.GetResource(LocalizationResources.Collection_Mods.PatchMod.DeletePrompt.Message);
                    if (await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Warning))
                    {
                        await modService.PurgeModPatchAsync(modCollection.Name);
                        IsOpenVisible = false;
                        SetOpenCaption();
                        PatchDeleted = true;
                    }
                    ForceClose();
                    PatchDeleted = false;
                }
            }

            CloseCommand = ReactiveCommand.Create(() =>
            {
                ForceClose();
            }).DisposeWith(disposables);

            OpenCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = true;
            }).DisposeWith(disposables);

            EnableCommand = ReactiveCommand.Create(() =>
            {
                syncPatchState();
            }).DisposeWith(disposables);

            DisableCommand = ReactiveCommand.Create(() =>
            {
                syncPatchState();
            }).DisposeWith(disposables);

            DeleteCommand = ReactiveCommand.CreateFromTask(() =>
            {
                return deletePatch();
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// set collection data as an asynchronous operation.
        /// </summary>
        /// <param name="modCollection">The mod collection.</param>
        protected async Task SetCollectionDataAsync(IModCollection modCollection)
        {
            this.modCollection = modCollection;
            CollectionName = modCollection.Name;
            IsOpenVisible = await modService.PatchModExistsAsync(modCollection.Name);
            IsPatchModEnabled = modCollection.PatchModEnabled;
            OpenClass = IsPatchModEnabled ? ActiveClass : InactiveClass;
            SetOpenCaption();
        }

        /// <summary>
        /// Sets the open caption.
        /// </summary>
        protected virtual void SetOpenCaption()
        {
            if (modCollection != null)
            {
                if (IsOpenVisible)
                {
                    var status = IsPatchModEnabled ? localizationManager.GetResource(LocalizationResources.Collection_Mods.PatchMod.Enabled) : localizationManager.GetResource(LocalizationResources.Collection_Mods.PatchMod.Disabled);
                    Open = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.PatchMod.Title), new { Status = status });
                }
                else
                {
                    Open = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.PatchMod.Title), new { Status = localizationManager.GetResource(LocalizationResources.Collection_Mods.PatchMod.NotAvailable) });
                }
            }
            else
            {
                Open = Smart.Format(localizationManager.GetResource(LocalizationResources.Collection_Mods.PatchMod.Title), new { Status = localizationManager.GetResource(LocalizationResources.Collection_Mods.PatchMod.NotAvailable) });
            }
        }

        #endregion Methods
    }
}
