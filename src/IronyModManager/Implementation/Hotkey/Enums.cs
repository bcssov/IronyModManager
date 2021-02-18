// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-17-2021
//
// Last Modified By : Mario
// Last Modified On : 02-18-2021
// ***********************************************************************
// <copyright file="Enums.cs" company="Mario">
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
    /// Class Enums.
    /// </summary>
    public static class Enums
    {
        #region Enums

        /// <summary>
        /// Enum HotKeys
        /// </summary>
        public enum HotKeys
        {
            /// <summary>
            /// The none
            /// </summary>
            None,

            /// <summary>
            /// The control up
            /// </summary>
            Ctrl_Up,

            /// <summary>
            /// The control down
            /// </summary>
            Ctrl_Down,

            /// <summary>
            /// The control shift up
            /// </summary>
            Ctrl_Shift_Up,

            /// <summary>
            /// The control shift down
            /// </summary>
            Ctrl_Shift_Down,

            /// <summary>
            /// The shift up
            /// </summary>
            Shift_Up,

            /// <summary>
            /// The shift down
            /// </summary>
            Shift_Down
        }

        #endregion Enums
    }
}
