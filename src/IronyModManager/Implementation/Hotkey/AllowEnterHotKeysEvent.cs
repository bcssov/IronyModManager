// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-11-2022
//
// Last Modified By : Mario
// Last Modified On : 02-11-2022
// ***********************************************************************
// <copyright file="AllowEnterHotKeysEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.Hotkey
{
    /// <summary>
    /// Class AllowEnterHotKeysEvent.
    /// </summary>
    public class AllowEnterHotKeysEvent : BaseNonAwaitableEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AllowEnterHotKeys" /> class.
        /// </summary>
        /// <param name="allowEnter">if set to <c>true</c> [allow enter].</param>
        public AllowEnterHotKeysEvent(bool allowEnter)
        {
            AllowEnter = allowEnter;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether [allow enter].
        /// </summary>
        /// <value><c>true</c> if [allow enter]; otherwise, <c>false</c>.</value>
        public bool AllowEnter { get; private set; }

        #endregion Properties
    }
}
