// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 03-14-2021
// ***********************************************************************
// <copyright file="LocalizationProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.Markup
{
    /// <summary>
    /// Class LocalizationProvider.
    /// Implements the <see cref="System.ComponentModel.INotifyPropertyChanged" />
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    [ExcludeFromCoverage("Should be tested in functional testing.")]
    public class LocalizationProvider : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The listening for changes
        /// </summary>
        private bool listeningForChanges = false;

        /// <summary>
        /// The localization manager
        /// </summary>
        private ILocalizationManager localizationManager;

        #endregion Fields

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static LocalizationProvider Instance { get; } = new LocalizationProvider();

        #endregion Properties

        #region Indexers

        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        [IndexerName("Item")]
        public string this[string key]
        {
            get
            {
                if (!listeningForChanges)
                {
                    listeningForChanges = true;
                    var listener = MessageBus.Current.Listen<LocaleChangedEventArgs>();
                    listener.SubscribeObservable(x =>
                    {
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item"));
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
                    });
                }
                if (localizationManager == null)
                {
                    localizationManager = DIResolver.Get<ILocalizationManager>();
                }
                return localizationManager.GetResource(key);
            }
        }

        #endregion Indexers
    }
}
