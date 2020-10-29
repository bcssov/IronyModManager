// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="XError.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class XError.
    /// </summary>
    internal static class XError
    {
        #region Fields

        /// <summary>
        /// The last error
        /// </summary>
        public static XErrorEvent LastError;

        /// <summary>
        /// The s error handler delegate
        /// </summary>
        private static readonly XErrorHandler s_errorHandlerDelegate = Handler;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public static void Init()
        {
            XLib.XSetErrorHandler(s_errorHandlerDelegate);
        }

        /// <summary>
        /// Throws the last error.
        /// </summary>
        /// <param name="desc">The desc.</param>
        /// <exception cref="IronyModManager.Platform.x11.X11Exception"></exception>
        public static void ThrowLastError(string desc)
        {
            var err = LastError;
            LastError = new XErrorEvent();
            if (err.error_code == 0)
                throw new X11Exception(desc);
            throw new X11Exception(desc + ": " + err.error_code);
        }

        /// <summary>
        /// Handlers the specified display.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="error">The error.</param>
        /// <returns>System.Int32.</returns>
        private static int Handler(IntPtr display, ref XErrorEvent error)
        {
            LastError = error;
            return 0;
        }

        #endregion Methods
    }
}
