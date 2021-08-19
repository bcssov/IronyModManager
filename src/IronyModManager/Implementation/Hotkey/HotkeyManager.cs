﻿// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-17-2021
//
// Last Modified By : Mario
// Last Modified On : 05-31-2021
// ***********************************************************************
// <copyright file="HotkeyManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IronyModManager.Common.Events;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.Implementation.Hotkey
{
    /// <summary>
    /// Class HotkeyManager.
    /// Implements the <see cref="IronyModManager.Implementation.Hotkey.IHotkeyManager" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.Hotkey.IHotkeyManager" />
    public class HotkeyManager : IHotkeyManager
    {
        #region Fields

        /// <summary>
        /// The map
        /// </summary>
        private static readonly Dictionary<string, Enums.HotKeys> map = new()
        {
            { Constants.CTRL_Down, Enums.HotKeys.Ctrl_Down },
            { Constants.CTRL_Up, Enums.HotKeys.Ctrl_Up },
            { Constants.CTRL_Right, Enums.HotKeys.Ctrl_Right },
            { Constants.CTRL_Left, Enums.HotKeys.Ctrl_Left },
            { Constants.CTRL_SHIFT_Up, Enums.HotKeys.Ctrl_Shift_Up },
            { Constants.CTRL_SHIFT_Down, Enums.HotKeys.Ctrl_Shift_Down },
            { Constants.SHIFT_Down, Enums.HotKeys.Shift_Down },
            { Constants.SHIFT_Up, Enums.HotKeys.Shift_Up },
            { Constants.CTRL_1, Enums.HotKeys.Ctrl_1 },
            { Constants.CTRL_SHIFT_1, Enums.HotKeys.Ctrl_Shift_1 },
            { Constants.CTRL_2, Enums.HotKeys.Ctrl_2 },
            { Constants.CTRL_SHIFT_2, Enums.HotKeys.Ctrl_Shift_2 },
            { Constants.CTRL_3, Enums.HotKeys.Ctrl_3 },
            { Constants.CTRL_SHIFT_3, Enums.HotKeys.Ctrl_Shift_3 },
            { Constants.CTRL_4, Enums.HotKeys.Ctrl_4 },
            { Constants.CTRL_SHIFT_4, Enums.HotKeys.Ctrl_Shift_4 },
            { Constants.CTRL_5, Enums.HotKeys.Ctrl_5 },
            { Constants.CTRL_SHIFT_5, Enums.HotKeys.Ctrl_Shift_5 },
            { Constants.CTRL_6, Enums.HotKeys.Ctrl_6 },
            { Constants.CTRL_SHIFT_6, Enums.HotKeys.Ctrl_Shift_6 },
            { Constants.CTRL_7, Enums.HotKeys.Ctrl_7 },
            { Constants.CTRL_SHIFT_7, Enums.HotKeys.Ctrl_Shift_7 },
            { Constants.CTRL_8, Enums.HotKeys.Ctrl_8 },
            { Constants.CTRL_SHIFT_8, Enums.HotKeys.Ctrl_Shift_8 },
            { Constants.CTRL_9, Enums.HotKeys.Ctrl_9 },
            { Constants.CTRL_SHIFT_9, Enums.HotKeys.Ctrl_Shift_9 },
            { Constants.CTRL_0, Enums.HotKeys.Ctrl_0 },
            { Constants.CTRL_SHIFT_0, Enums.HotKeys.Ctrl_Shift_0 },
            { Constants.CTRL_SHIFT_N, Enums.HotKeys.Ctrl_Shift_N },
            { Constants.CTRL_SHIFT_P, Enums.HotKeys.Ctrl_Shift_P },
            { Constants.CTRL_E, Enums.HotKeys.Ctrl_E },
            { Constants.CTRL_I, Enums.HotKeys.Ctrl_I },
            { Constants.CTRL_R, Enums.HotKeys.Ctrl_R },
            { Constants.CTRL_T, Enums.HotKeys.Ctrl_T },
            { Constants.CTRL_SHIFT_T, Enums.HotKeys.Ctrl_Shift_T },
            { Constants.CTRL_C, Enums.HotKeys.Ctrl_C },
            { Constants.CTRL_V, Enums.HotKeys.Ctrl_V },
            { Constants.CTRL_B, Enums.HotKeys.Ctrl_B },
            { Constants.CTRL_X, Enums.HotKeys.Ctrl_X },
            { Constants.CTRL_Z, Enums.HotKeys.Ctrl_Z },
            { Constants.CTRL_Y, Enums.HotKeys.Ctrl_Y },
            { Constants.RETURN, Enums.HotKeys.Return }
        };

        /// <summary>
        /// The message bus
        /// </summary>
        private readonly IMessageBus messageBus;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HotkeyManager" /> class.
        /// </summary>
        /// <param name="messageBus">The message bus.</param>
        public HotkeyManager(IMessageBus messageBus)
        {
            this.messageBus = messageBus;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <returns>IReadOnlyCollection&lt;System.String&gt;.</returns>
        public IReadOnlyCollection<string> GetKeys()
        {
            return map.Select(p => p.Key).ToList();
        }

        /// <summary>
        /// Hots the key pressed asynchronous.
        /// </summary>
        /// <param name="navigationState">State of the navigation.</param>
        /// <param name="hotKey">The hot key.</param>
        /// <returns>Task.</returns>
        public async Task HotKeyPressedAsync(NavigationState navigationState, string hotKey)
        {
            var pressedKey = MapHotkey(hotKey);
            if (pressedKey != Enums.HotKeys.None)
            {
                switch (navigationState)
                {
                    case NavigationState.ConflictSolver:
                        await messageBus.PublishAsync(new ConflictSolverViewHotkeyPressedEvent(pressedKey));
                        break;

                    default:
                        await messageBus.PublishAsync(new MainViewHotkeyPressedEvent(pressedKey));
                        break;
                }
            }
        }

        /// <summary>
        /// Maps the hotkey.
        /// </summary>
        /// <param name="hotKey">The hot key.</param>
        /// <returns>Enums.HotKeys.</returns>
        private Enums.HotKeys MapHotkey(string hotKey)
        {
            if (map.ContainsKey(hotKey))
            {
                return map[hotKey];
            }
            return Enums.HotKeys.None;
        }

        #endregion Methods
    }
}
