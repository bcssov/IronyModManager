// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-17-2021
//
// Last Modified By : Mario
// Last Modified On : 02-17-2021
// ***********************************************************************
// <copyright file="IHotkeyManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IronyModManager.Implementation.Hotkey
{
    /// <summary>
    /// Interface IHotkeyManager
    /// </summary>
    public interface IHotkeyManager
    {
        #region Methods

        /// <summary>
        /// Hots the key pressed asynchronous.
        /// </summary>
        /// <param name="hotKey">The hot key.</param>
        /// <returns>Task.</returns>
        Task HotKeyPressedAsync(string hotKey);

        #endregion Methods
    }
}
