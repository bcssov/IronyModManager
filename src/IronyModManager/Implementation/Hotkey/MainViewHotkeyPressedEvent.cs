// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-18-2021
//
// Last Modified By : Mario
// Last Modified On : 02-18-2021
// ***********************************************************************
// <copyright file="MainViewHotkeyPressedEvent.cs" company="Mario">
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
    /// Class MainViewHotkeyPressedEvent.
    /// Implements the <see cref="IronyModManager.Implementation.Hotkey.BaseHotkeyPressedEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.Hotkey.BaseHotkeyPressedEvent" />
    public class MainViewHotkeyPressedEvent : BaseHotkeyPressedEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewHotkeyPressedEvent"/> class.
        /// </summary>
        /// <param name="hotKey">The hot key.</param>
        public MainViewHotkeyPressedEvent(Enums.HotKeys hotKey) : base(hotKey)
        {
        }

        #endregion Constructors
    }
}
