// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 02-18-2021
//
// Last Modified By : Mario
// Last Modified On : 10-26-2025
// ***********************************************************************
// <copyright file="Constants.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace IronyModManager.Implementation.Hotkey
{
    /// <summary>
    /// Class Constants.
    /// </summary>
    public static class Constants
    {
        #region Fields

        /// <summary>
        /// The control 0
        /// </summary>
        public const string CMD_0 = "CMD+D0";

        /// <summary>
        /// The control 1
        /// </summary>
        public const string CMD_1 = "CMD+D1";

        /// <summary>
        /// The control 2
        /// </summary>
        public const string CMD_2 = "CMD+D2";

        /// <summary>
        /// The control 3
        /// </summary>
        public const string CMD_3 = "CMD+D3";

        /// <summary>
        /// The control 4
        /// </summary>
        public const string CMD_4 = "CMD+D4";

        /// <summary>
        /// The control 5
        /// </summary>
        public const string CMD_5 = "CMD+D5";

        /// <summary>
        /// The control 6
        /// </summary>
        public const string CMD_6 = "CMD+D6";

        /// <summary>
        /// The control 7
        /// </summary>
        public const string CMD_7 = "CMD+D7";

        /// <summary>
        /// The control 8
        /// </summary>
        public const string CMD_8 = "CMD+D8";

        /// <summary>
        /// The control 9
        /// </summary>
        public const string CMD_9 = "CMD+D9";

        /// <summary>
        /// The control b
        /// </summary>
        public const string CMD_B = "CMD+B";

        /// <summary>
        /// The control c
        /// </summary>
        public const string CMD_C = "CMD+C";

        /// <summary>
        /// The control down
        /// </summary>
        public const string CMD_Down = "CMD+Down";

        /// <summary>
        /// The control e
        /// </summary>
        public const string CMD_E = "CMD+E";

        /// <summary>
        /// The control i
        /// </summary>
        public const string CMD_I = "CMD+I";

        /// <summary>
        /// The control left
        /// </summary>
        public const string CMD_Left = "CMD+Left";

        /// <summary>
        /// The control r
        /// </summary>
        public const string CMD_R = "CMD+R";

        /// <summary>
        /// The control right
        /// </summary>
        public const string CMD_Right = "CMD+Right";

        /// <summary>
        /// The control shift 0
        /// </summary>
        public const string CMD_SHIFT_0 = "CMD+SHIFT+D0";

        /// <summary>
        /// The control shift 1
        /// </summary>
        public const string CMD_SHIFT_1 = "CMD+SHIFT+D1";

        /// <summary>
        /// The control shift 2
        /// </summary>
        public const string CMD_SHIFT_2 = "CMD+SHIFT+D2";

        /// <summary>
        /// The control shift 3
        /// </summary>
        public const string CMD_SHIFT_3 = "CMD+SHIFT+D3";

        /// <summary>
        /// The control shift 4
        /// </summary>
        public const string CMD_SHIFT_4 = "CMD+SHIFT+D4";

        /// <summary>
        /// The control shift 5
        /// </summary>
        public const string CMD_SHIFT_5 = "CMD+SHIFT+D5";

        /// <summary>
        /// The control shift 6
        /// </summary>
        public const string CMD_SHIFT_6 = "CMD+SHIFT+D6";

        /// <summary>
        /// The control shift 7
        /// </summary>
        public const string CMD_SHIFT_7 = "CMD+SHIFT+D7";

        /// <summary>
        /// The control shift 8
        /// </summary>
        public const string CMD_SHIFT_8 = "CMD+SHIFT+D8";

        /// <summary>
        /// The control shift 9
        /// </summary>
        public const string CMD_SHIFT_9 = "CMD+SHIFT+D9";

        /// <summary>
        /// The control shift down
        /// </summary>
        public const string CMD_SHIFT_Down = "CMD+SHIFT+Down";

        /// <summary>
        /// The control shift n
        /// </summary>
        public const string CMD_SHIFT_N = "CMD+SHIFT+N";

        /// <summary>
        /// The control shift p
        /// </summary>
        public const string CMD_SHIFT_P = "CMD+SHIFT+P";

        /// <summary>
        /// The control shift t
        /// </summary>
        public const string CMD_SHIFT_T = "CMD+SHIFT+T";

        /// <summary>
        /// The control shift up
        /// </summary>
        public const string CMD_SHIFT_Up = "CMD+SHIFT+Up";

        /// <summary>
        /// The control t
        /// </summary>
        public const string CMD_T = "CMD+T";

        /// <summary>
        /// The control up
        /// </summary>
        public const string CMD_Up = "CMD+Up";

        /// <summary>
        /// The control v
        /// </summary>
        public const string CMD_V = "CMD+V";

        /// <summary>
        /// The control x
        /// </summary>
        public const string CMD_X = "CMD+X";

        /// <summary>
        /// The control y
        /// </summary>
        public const string CMD_Y = "CMD+Y";

        /// <summary>
        /// The control z
        /// </summary>
        public const string CMD_Z = "CMD+Z";

        /// <summary>
        /// The control 0
        /// </summary>
        public const string CTRL_0 = "CTRL+D0";

        /// <summary>
        /// The control 1
        /// </summary>
        public const string CTRL_1 = "CTRL+D1";

        /// <summary>
        /// The control 2
        /// </summary>
        public const string CTRL_2 = "CTRL+D2";

        /// <summary>
        /// The control 3
        /// </summary>
        public const string CTRL_3 = "CTRL+D3";

        /// <summary>
        /// The control 4
        /// </summary>
        public const string CTRL_4 = "CTRL+D4";

        /// <summary>
        /// The control 5
        /// </summary>
        public const string CTRL_5 = "CTRL+D5";

        /// <summary>
        /// The control 6
        /// </summary>
        public const string CTRL_6 = "CTRL+D6";

        /// <summary>
        /// The control 7
        /// </summary>
        public const string CTRL_7 = "CTRL+D7";

        /// <summary>
        /// The control 8
        /// </summary>
        public const string CTRL_8 = "CTRL+D8";

        /// <summary>
        /// The control 9
        /// </summary>
        public const string CTRL_9 = "CTRL+D9";

        /// <summary>
        /// The control b
        /// </summary>
        public const string CTRL_B = "CTRL+B";

        /// <summary>
        /// The control c
        /// </summary>
        public const string CTRL_C = "CTRL+C";

        /// <summary>
        /// The control down
        /// </summary>
        public const string CTRL_Down = "CTRL+Down";

        /// <summary>
        /// The control e
        /// </summary>
        public const string CTRL_E = "CTRL+E";

        /// <summary>
        /// The control i
        /// </summary>
        public const string CTRL_I = "CTRL+I";

        /// <summary>
        /// The control left
        /// </summary>
        public const string CTRL_Left = "CTRL+Left";

        /// <summary>
        /// The control r
        /// </summary>
        public const string CTRL_R = "CTRL+R";

        /// <summary>
        /// The control right
        /// </summary>
        public const string CTRL_Right = "CTRL+Right";

        /// <summary>
        /// The control shift 0
        /// </summary>
        public const string CTRL_SHIFT_0 = "CTRL+SHIFT+D0";

        /// <summary>
        /// The control shift 1
        /// </summary>
        public const string CTRL_SHIFT_1 = "CTRL+SHIFT+D1";

        /// <summary>
        /// The control shift 2
        /// </summary>
        public const string CTRL_SHIFT_2 = "CTRL+SHIFT+D2";

        /// <summary>
        /// The control shift 3
        /// </summary>
        public const string CTRL_SHIFT_3 = "CTRL+SHIFT+D3";

        /// <summary>
        /// The control shift 4
        /// </summary>
        public const string CTRL_SHIFT_4 = "CTRL+SHIFT+D4";

        /// <summary>
        /// The control shift 5
        /// </summary>
        public const string CTRL_SHIFT_5 = "CTRL+SHIFT+D5";

        /// <summary>
        /// The control shift 6
        /// </summary>
        public const string CTRL_SHIFT_6 = "CTRL+SHIFT+D6";

        /// <summary>
        /// The control shift 7
        /// </summary>
        public const string CTRL_SHIFT_7 = "CTRL+SHIFT+D7";

        /// <summary>
        /// The control shift 8
        /// </summary>
        public const string CTRL_SHIFT_8 = "CTRL+SHIFT+D8";

        /// <summary>
        /// The control shift 9
        /// </summary>
        public const string CTRL_SHIFT_9 = "CTRL+SHIFT+D9";

        /// <summary>
        /// The control shift down
        /// </summary>
        public const string CTRL_SHIFT_Down = "CTRL+SHIFT+Down";

        /// <summary>
        /// The control shift n
        /// </summary>
        public const string CTRL_SHIFT_N = "CTRL+SHIFT+N";

        /// <summary>
        /// The control shift p
        /// </summary>
        public const string CTRL_SHIFT_P = "CTRL+SHIFT+P";

        /// <summary>
        /// The control shift t
        /// </summary>
        public const string CTRL_SHIFT_T = "CTRL+SHIFT+T";

        /// <summary>
        /// The control shift up
        /// </summary>
        public const string CTRL_SHIFT_Up = "CTRL+SHIFT+Up";

        /// <summary>
        /// The control t
        /// </summary>
        public const string CTRL_T = "CTRL+T";

        /// <summary>
        /// The control up
        /// </summary>
        public const string CTRL_Up = "CTRL+Up";

        /// <summary>
        /// The control v
        /// </summary>
        public const string CTRL_V = "CTRL+V";

        /// <summary>
        /// The control x
        /// </summary>
        public const string CTRL_X = "CTRL+X";

        /// <summary>
        /// The control y
        /// </summary>
        public const string CTRL_Y = "CTRL+Y";

        /// <summary>
        /// The control z
        /// </summary>
        public const string CTRL_Z = "CTRL+Z";

        /// <summary>
        /// The return
        /// </summary>
        public const string RETURN = "Return";

        /// <summary>
        /// The shift down
        /// </summary>
        public const string SHIFT_Down = "SHIFT+Down";

        /// <summary>
        /// The shift up
        /// </summary>
        public const string SHIFT_Up = "SHIFT+Up";

        #endregion Fields
    }
}
