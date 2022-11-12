// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-05-2022
//
// Last Modified By : Mario
// Last Modified On : 11-12-2022
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
    /// Class EvalModAchievementsCompatibilityEvent.
    /// Implements the <see cref="BaseAwaitableEvent" />
    /// </summary>
    /// <seealso cref="BaseAwaitableEvent" />
    public class EvalModAchievementsCompatibilityEvent : BaseAwaitableEvent
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

        /// <summary>
        /// Initializes a new instance of the <see cref="EvalModAchievementsCompatibilityEvent" /> class.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="showOverlay">if set to <c>true</c> [show overlay].</param>
        public EvalModAchievementsCompatibilityEvent(IEnumerable<IMod> mods, bool showOverlay) : this(mods)
        {
            ShowOverlay = showOverlay;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EvalModAchievementsCompatibilityEvent"/> class.
        /// </summary>
        /// <param name="mods">The mods.</param>
        /// <param name="showOverlay">if set to <c>true</c> [show overlay].</param>
        /// <param name="hasPriority">if set to <c>true</c> [has priority].</param>
        public EvalModAchievementsCompatibilityEvent(IEnumerable<IMod> mods, bool showOverlay, bool hasPriority) : this(mods, showOverlay)
        {
            HasPriority = hasPriority;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance has priority.
        /// </summary>
        /// <value><c>true</c> if this instance has priority; otherwise, <c>false</c>.</value>
        public bool HasPriority { get; private set; }

        /// <summary>
        /// Gets the mods.
        /// </summary>
        /// <value>The mods.</value>
        public IEnumerable<IMod> Mods { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [show overlay].
        /// </summary>
        /// <value><c>true</c> if [show overlay]; otherwise, <c>false</c>.</value>
        public bool ShowOverlay { get; private set; }

        #endregion Properties
    }
}
