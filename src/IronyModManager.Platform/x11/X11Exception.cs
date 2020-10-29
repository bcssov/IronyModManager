// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11Exception.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class X11Exception.
    /// Implements the <see cref="System.Exception" />
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class X11Exception : Exception
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11Exception" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public X11Exception(string message) : base(message)
        {
        }

        #endregion Constructors
    }
}
