// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-17-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2025
// ***********************************************************************
// <copyright file="HotkeyManager.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        /// The enter key map
        /// </summary>
        private static readonly Dictionary<string, Enums.HotKeys> enterKeyMap = new()
        {
            { Constants.RETURN, Enums.HotKeys.Return }
        };

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
            { Constants.CTRL_Y, Enums.HotKeys.Ctrl_Y }
        };

        /// <summary>
        /// The map osx
        /// </summary>
        private static readonly Dictionary<string, Enums.HotKeys> mapOSX = new()
        {
            { Constants.CMD_Down, Enums.HotKeys.Ctrl_Down },
            { Constants.CMD_Up, Enums.HotKeys.Ctrl_Up },
            { Constants.CMD_Right, Enums.HotKeys.Ctrl_Right },
            { Constants.CMD_Left, Enums.HotKeys.Ctrl_Left },
            { Constants.CMD_SHIFT_Up, Enums.HotKeys.Ctrl_Shift_Up },
            { Constants.CMD_SHIFT_Down, Enums.HotKeys.Ctrl_Shift_Down },
            { Constants.SHIFT_Down, Enums.HotKeys.Shift_Down },
            { Constants.SHIFT_Up, Enums.HotKeys.Shift_Up },
            { Constants.CMD_1, Enums.HotKeys.Ctrl_1 },
            { Constants.CMD_SHIFT_1, Enums.HotKeys.Ctrl_Shift_1 },
            { Constants.CMD_2, Enums.HotKeys.Ctrl_2 },
            { Constants.CMD_SHIFT_2, Enums.HotKeys.Ctrl_Shift_2 },
            { Constants.CMD_3, Enums.HotKeys.Ctrl_3 },
            { Constants.CMD_SHIFT_3, Enums.HotKeys.Ctrl_Shift_3 },
            { Constants.CMD_4, Enums.HotKeys.Ctrl_4 },
            { Constants.CMD_SHIFT_4, Enums.HotKeys.Ctrl_Shift_4 },
            { Constants.CMD_5, Enums.HotKeys.Ctrl_5 },
            { Constants.CMD_SHIFT_5, Enums.HotKeys.Ctrl_Shift_5 },
            { Constants.CMD_6, Enums.HotKeys.Ctrl_6 },
            { Constants.CMD_SHIFT_6, Enums.HotKeys.Ctrl_Shift_6 },
            { Constants.CMD_7, Enums.HotKeys.Ctrl_7 },
            { Constants.CMD_SHIFT_7, Enums.HotKeys.Ctrl_Shift_7 },
            { Constants.CMD_8, Enums.HotKeys.Ctrl_8 },
            { Constants.CMD_SHIFT_8, Enums.HotKeys.Ctrl_Shift_8 },
            { Constants.CMD_9, Enums.HotKeys.Ctrl_9 },
            { Constants.CMD_SHIFT_9, Enums.HotKeys.Ctrl_Shift_9 },
            { Constants.CMD_0, Enums.HotKeys.Ctrl_0 },
            { Constants.CMD_SHIFT_0, Enums.HotKeys.Ctrl_Shift_0 },
            { Constants.CMD_SHIFT_N, Enums.HotKeys.Ctrl_Shift_N },
            { Constants.CMD_SHIFT_P, Enums.HotKeys.Ctrl_Shift_P },
            { Constants.CMD_E, Enums.HotKeys.Ctrl_E },
            { Constants.CMD_I, Enums.HotKeys.Ctrl_I },
            { Constants.CMD_R, Enums.HotKeys.Ctrl_R },
            { Constants.CMD_T, Enums.HotKeys.Ctrl_T },
            { Constants.CMD_SHIFT_T, Enums.HotKeys.Ctrl_Shift_T },
            { Constants.CMD_C, Enums.HotKeys.Ctrl_C },
            { Constants.CMD_V, Enums.HotKeys.Ctrl_V },
            { Constants.CMD_B, Enums.HotKeys.Ctrl_B },
            { Constants.CMD_X, Enums.HotKeys.Ctrl_X },
            { Constants.CMD_Z, Enums.HotKeys.Ctrl_Z },
            { Constants.CMD_Y, Enums.HotKeys.Ctrl_Y }
        };

        /// <summary>
        /// The merged map
        /// </summary>
        private static readonly Dictionary<string, Enums.HotKeys> mergedMap = new();

        /// <summary>
        /// The map initialized
        /// </summary>
        private static bool mapInitialized;

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
            MergeMaps();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the enter keys.
        /// </summary>
        /// <returns>IReadOnlyCollection&lt;System.String&gt;.</returns>
        public IReadOnlyCollection<string> GetEnterKeys()
        {
            return enterKeyMap.Select(p => p.Key).ToList();
        }

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <returns>IReadOnlyCollection&lt;System.String&gt;.</returns>
        public IReadOnlyCollection<string> GetKeys()
        {
            return GetMap().Select(p => p.Key).ToList();
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
                    case NavigationState.ReadOnlyConflictSolver:

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
        /// Gets the map.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, Hotkey.Enums.HotKeys&gt;.</returns>
        private static Dictionary<string, Enums.HotKeys> GetMap()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? mapOSX : map;
        }

        /// <summary>
        /// Maps the hotkey.
        /// </summary>
        /// <param name="hotKey">The hot key.</param>
        /// <returns>Enums.HotKeys.</returns>
        private Enums.HotKeys MapHotkey(string hotKey)
        {
            return mergedMap.TryGetValue(hotKey, out var value) ? value : Enums.HotKeys.None;
        }

        /// <summary>
        /// Merges the maps.
        /// </summary>
        private void MergeMaps()
        {
            if (mapInitialized)
            {
                return;
            }

            mapInitialized = true;
            foreach (var item in GetMap())
            {
                mergedMap.Add(item.Key, item.Value);
            }

            foreach (var item in enterKeyMap)
            {
                mergedMap.Add(item.Key, item.Value);
            }
        }

        #endregion Methods
    }
}
