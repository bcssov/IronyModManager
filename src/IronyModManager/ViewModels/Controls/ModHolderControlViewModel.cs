// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 03-01-2020
// ***********************************************************************
// <copyright file="ModHolderControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using IronyModManager.Common.ViewModels;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using ReactiveUI;

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
        /// The selected mods
        /// </summary>
        private IEnumerable<IMod> selectedMods;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModHolderControlViewModel" /> class.
        /// </summary>
        /// <param name="installedModsControlViewModel">The installed mods control view model.</param>
        public ModHolderControlViewModel(InstalledModsControlViewModel installedModsControlViewModel)
        {
            InstalledMods = installedModsControlViewModel;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the installed mods.
        /// </summary>
        /// <value>The installed mods.</value>
        public virtual InstalledModsControlViewModel InstalledMods { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            this.WhenAnyValue(s => s.InstalledMods.SelectedMods).Subscribe(s =>
            {
                if (s != null)
                {
                    selectedMods = s;
                }
                else
                {
                    selectedMods = null;
                }
            }).DisposeWith(disposables);
            base.OnActivated(disposables);
        }

        #endregion Methods
    }
}
