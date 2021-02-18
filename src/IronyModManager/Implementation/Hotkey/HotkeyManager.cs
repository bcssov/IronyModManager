// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-17-2021
//
// Last Modified By : Mario
// Last Modified On : 02-18-2021
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
        private static readonly Dictionary<string, Enums.HotKeys> map = new Dictionary<string, Enums.HotKeys>()
        {
            { Constants.CTRL_Down, Enums.HotKeys.Ctrl_Down },
            { Constants.CTRL_Up, Enums.HotKeys.Ctrl_Up }
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

        #region Classes

        /// <summary>
        /// Class Constants.
        /// </summary>
        private class Constants
        {
            #region Fields

            /// <summary>
            /// The control down
            /// </summary>
            public const string CTRL_Down = "CTRL+Down";

            /// <summary>
            /// The control up
            /// </summary>
            public const string CTRL_Up = "CTRL+Up";

            #endregion Fields
        }

        #endregion Classes
    }
}
