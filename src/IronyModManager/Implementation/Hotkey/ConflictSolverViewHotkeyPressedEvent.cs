// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-18-2021
//
// Last Modified By : Mario
// Last Modified On : 02-18-2021
// ***********************************************************************
// <copyright file="ConflictSolverViewHotkeyPressedEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Implementation.Hotkey
{
    /// <summary>
    /// Class ConflictSolverViewHotkeyPressedEvent.
    /// Implements the <see cref="IronyModManager.Implementation.Hotkey.BaseHotkeyPressedEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.Hotkey.BaseHotkeyPressedEvent" />
    public class ConflictSolverViewHotkeyPressedEvent : BaseHotkeyPressedEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictSolverViewHotkeyPressedEvent"/> class.
        /// </summary>
        /// <param name="hotKey">The hot key.</param>
        public ConflictSolverViewHotkeyPressedEvent(Enums.HotKeys hotKey) : base(hotKey)
        {
        }

        #endregion Constructors
    }
}
