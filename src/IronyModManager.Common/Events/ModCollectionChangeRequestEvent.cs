// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 01-08-2025
//
// Last Modified By : Mario
// Last Modified On : 01-08-2025
// ***********************************************************************
// <copyright file="ModCollectionChangeRequestEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Class ModCollectionChangeRequestEvent.
    /// Implements the <see cref="BaseNonAwaitableEvent" />
    /// </summary>
    /// <seealso cref="BaseNonAwaitableEvent" />
    public class ModCollectionChangeRequestEvent : BaseNonAwaitableEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModCollectionChangeRequestEvent"/> class.
        /// </summary>
        /// <param name="modCollection">The mod collection.</param>
        public ModCollectionChangeRequestEvent(IModCollection modCollection)
        {
            ModCollection = modCollection;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the mod collection.
        /// </summary>
        /// <value>The mod collection.</value>
        public IModCollection ModCollection { get; private set; }

        #endregion Properties
    }
}
