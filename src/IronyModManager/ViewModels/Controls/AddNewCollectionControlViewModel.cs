// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-05-2020
//
// Last Modified By : Mario
// Last Modified On : 05-31-2021
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
using System.Reactive.Linq;
using IronyModManager.Common.ViewModels;
using IronyModManager.Implementation;
using IronyModManager.Implementation.Hotkey;
using IronyModManager.Localization.Attributes;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using ReactiveUI;

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
        /// The hotkey pressed handler
        /// </summary>
        private readonly MainViewHotkeyPressedHandler hotkeyPressedHandler;

        /// <summary>
        /// The mod collection service
        /// </summary>
        private readonly IModCollectionService modCollectionService;

        /// <summary>
        /// The mod patch collection service
        /// </summary>
        private readonly IModPatchCollectionService modPatchCollectionService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AddNewCollectionControlViewModel" /> class.
        /// </summary>
        /// <param name="hotkeyPressedHandler">The hotkey pressed handler.</param>
        /// <param name="modCollectionService">The mod collection service.</param>
        /// <param name="modPatchCollectionService">The mod patch collection service.</param>
        public AddNewCollectionControlViewModel(MainViewHotkeyPressedHandler hotkeyPressedHandler, IModCollectionService modCollectionService, IModPatchCollectionService modPatchCollectionService)
        {
            this.modCollectionService = modCollectionService;
            this.modPatchCollectionService = modPatchCollectionService;
            this.hotkeyPressedHandler = hotkeyPressedHandler;
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
        public virtual ReactiveCommand<Unit, CommandResult<string>> CreateCommand { get; protected set; }

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

        /// <summary>
        /// Gets or sets the renaming collection.
        /// </summary>
        /// <value>The renaming collection.</value>
        public virtual IModCollection RenamingCollection { get; set; }

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
                    var colName = NewCollectionName.Trim();
                    if (modCollectionService.Exists(NewCollectionName))
                    {
                        if (RenamingCollection != null && RenamingCollection.Name.Equals(NewCollectionName, StringComparison.OrdinalIgnoreCase))
                        {
                            return new CommandResult<string>(colName, CommandState.NotExecuted);
                        }
                        else
                        {
                            return new CommandResult<string>(colName, CommandState.Exists);
                        }
                    }
                    var collection = modCollectionService.Create();
                    collection.Name = colName;
                    collection.IsSelected = true;
                    if (RenamingCollection != null)
                    {
                        collection.Mods = RenamingCollection.Mods;
                        collection.PatchModEnabled = RenamingCollection.PatchModEnabled;
                        modCollectionService.Delete(RenamingCollection.Name);
                        modPatchCollectionService.InvalidatePatchModState(RenamingCollection.Name);
                    }
                    if (modCollectionService.Save(collection))
                    {
                        NewCollectionName = string.Empty;
                        return new CommandResult<string>(collection.Name, CommandState.Success);
                    }
                }
                return new CommandResult<string>(!string.IsNullOrEmpty(NewCollectionName) ? NewCollectionName.Trim() : string.Empty, CommandState.Failed);
            }, createEnabled).DisposeWith(disposables);

            CancelCommand = ReactiveCommand.Create(() =>
            {
            }).DisposeWith(disposables);

            hotkeyPressedHandler.Subscribe(hotkey =>
            {
                switch (hotkey.Hotkey)
                {
                    case Enums.HotKeys.Return:
                        Observable.Start(() => { }).InvokeCommand(this, vm => vm.CreateCommand);
                        break;

                    default:
                        break;
                }
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
