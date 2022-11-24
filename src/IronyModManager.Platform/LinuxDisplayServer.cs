// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 11-23-2022
//
// Last Modified By : Mario
// Last Modified On : 11-24-2022
// ***********************************************************************
// <copyright file="LinuxDisplayServer.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Platform
{
    /// <summary>
    /// Class LinuxDisplayServer.
    /// </summary>
    public class LinuxDisplayServer
    {
        #region Fields

        /// <summary>
        /// The automatic
        /// </summary>
        public const string Auto = "auto";

        /// <summary>
        /// The wayland
        /// </summary>
        public const string Wayland = "wayland";

        /// <summary>
        /// The X11
        /// </summary>
        public const string X11 = "x11";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Determines whether the specified entry is automatic.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns><c>true</c> if the specified entry is automatic; otherwise, <c>false</c>.</returns>
        public static bool IsAuto(string entry)
        {
            return Auto.Equals(entry, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified entry is wayland.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns><c>true</c> if the specified entry is wayland; otherwise, <c>false</c>.</returns>
        public static bool IsWayland(string entry)
        {
            return Wayland.Equals(entry, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified entry is X11.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns><c>true</c> if the specified entry is X11; otherwise, <c>false</c>.</returns>
        public static bool IsX11(string entry)
        {
            return X11.Equals(entry, StringComparison.OrdinalIgnoreCase);
        }

        #endregion Methods
    }
}
