// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-05-2022
//
// Last Modified By : Mario
// Last Modified On : 11-05-2022
// ***********************************************************************
// <copyright file="EvalModAchievementsCompatibilityEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.Models.Common;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.MessageBus.Events
{
    /// <summary>
    /// Class EvalModCompatibilityEvent.
    /// Implements the <see cref="BaseNonAwaitableEvent" />
    /// </summary>
    /// <seealso cref="BaseNonAwaitableEvent" />
    public class EvalModAchievementsCompatibilityEvent : BaseNonAwaitableEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EvalModAchievementsCompatibilityEvent" /> class.
        /// </summary>
        /// <param name="mods">The mods.</param>
        public EvalModAchievementsCompatibilityEvent(IEnumerable<IMod> mods)
        {
            Mods = mods;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the mods.
        /// </summary>
        /// <value>The mods.</value>
        public IEnumerable<IMod> Mods { get; private set; }

        #endregion Properties
    }
}
