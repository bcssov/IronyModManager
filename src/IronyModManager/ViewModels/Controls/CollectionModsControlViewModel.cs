// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-03-2020
//
// Last Modified By : Mario
// Last Modified On : 03-03-2020
// ***********************************************************************
// <copyright file="CollectionModsControlViewModel.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using DynamicData;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.Models.Common;

namespace IronyModManager.ViewModels.Controls
{
    /// <summary>
    /// Class CollectionModsControlViewModel.
    /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
    public class CollectionModsControlViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The mods changed
        /// </summary>
        private IDisposable modsChanged;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the mods.
        /// </summary>
        /// <value>The mods.</value>
        public virtual IEnumerable<IMod> Mods { get; protected set; }

        /// <summary>
        /// Gets or sets the selected mods.
        /// </summary>
        /// <value>The selected mods.</value>
        public virtual IEnumerable<IMod> SelectedMods { get; protected set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Sets the mods.
        /// </summary>
        /// <param name="mods">The mods.</param>
        public void SetMods(IEnumerable<IMod> mods)
        {
            Mods = mods;
            SelectedMods = mods != null ? mods.Where(p => p.IsSelected) : null;

            SubscribeToMods();
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            base.OnActivated(disposables);
            SubscribeToMods();
        }

        /// <summary>
        /// Subscribes to mods.
        /// </summary>
        protected virtual void SubscribeToMods()
        {
            modsChanged?.Dispose();
            modsChanged = null;
            if (Mods != null && Disposables != null)
            {
                modsChanged = Mods.ToSourceList().Connect().WhenAnyPropertyChanged().Subscribe(s =>
                {
                    SelectedMods = Mods.Where(p => p.IsSelected);
                }).DisposeWith(Disposables);
            }
        }

        #endregion Methods
    }
}
