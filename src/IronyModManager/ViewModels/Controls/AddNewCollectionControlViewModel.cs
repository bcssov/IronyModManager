// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-05-2020
//
// Last Modified By : Mario
// Last Modified On : 03-07-2020
// ***********************************************************************
// <copyright file="AddNewCollectionControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
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
    /// Class AddNewCollectionControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class AddNewCollectionControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The localization manager
        /// </summary>
        private readonly ILocalizationManager localizationManager;

        /// <summary>
        /// The mod collection service
        /// </summary>
        private readonly IModCollectionService modCollectionService;

        /// <summary>
        /// The notification action
        /// </summary>
        private readonly INotificationAction notificationAction;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewCollectionControlViewModel" /> class.
        /// </summary>
        /// <param name="modCollectionService">The mod collection service.</param>
        /// <param name="notificationAction">The notification action.</param>
        /// <param name="localizationManager">The localization manager.</param>
        public AddNewCollectionControlViewModel(IModCollectionService modCollectionService,
            INotificationAction notificationAction, ILocalizationManager localizationManager)
        {
            this.modCollectionService = modCollectionService;
            this.notificationAction = notificationAction;
            this.localizationManager = localizationManager;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the cancel.
        /// </summary>
        /// <value>The cancel.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Cancel)]
        public virtual string Cancel { get; protected set; }

        /// <summary>
        /// Gets or sets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        public virtual ReactiveCommand<Unit, Unit> CancelCommand { get; protected set; }

        /// <summary>
        /// Gets or sets the create.
        /// </summary>
        /// <value>The create.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.OK)]
        public virtual string Create { get; protected set; }

        /// <summary>
        /// Gets or sets the create command.
        /// </summary>
        /// <value>The create command.</value>
        public virtual ReactiveCommand<Unit, string> CreateCommand { get; protected set; }

        /// <summary>
        /// Creates new collectionname.
        /// </summary>
        /// <value>The new name of the collection.</value>
        public virtual string NewCollectionName { get; set; }

        /// <summary>
        /// Creates new collectionwatermark.
        /// </summary>
        /// <value>The new collection watermark.</value>
        [StaticLocalization(LocalizationResources.Collection_Mods.Watermark)]
        public virtual string NewCollectionWatermark { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var createEnabled = this.WhenAnyValue(v => v.NewCollectionName, v => !string.IsNullOrWhiteSpace(v));

            CreateCommand = ReactiveCommand.Create(() =>
            {
                if (!string.IsNullOrWhiteSpace(NewCollectionName))
                {
                    var collections = modCollectionService.GetNames();
                    if (!collections.Contains(NewCollectionName))
                    {
                        var collection = modCollectionService.Create();
                        collection.Name = NewCollectionName;
                        if (modCollectionService.Save(collection))
                        {
                            NewCollectionName = string.Empty;
                            return collection.Name;
                        }
                    }
                    else
                    {
                        var notification = new
                        {
                            CollectionName = NewCollectionName
                        };
                        var title = localizationManager.GetResource(LocalizationResources.Notifications.CollectionExists.Title);
                        var message = Smart.Format(localizationManager.GetResource(LocalizationResources.Notifications.CollectionExists.Message), notification);
                        notificationAction.ShowNotification(title, message, NotificationType.Warning);
                        return string.Empty;
                    }
                }
                return null;
            }, createEnabled).DisposeWith(disposables);

            CancelCommand = ReactiveCommand.Create(() =>
            {
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
