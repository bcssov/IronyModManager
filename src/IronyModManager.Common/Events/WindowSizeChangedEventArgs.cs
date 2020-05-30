// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 05-30-2020
//
// Last Modified By : Mario
// Last Modified On : 05-30-2020
// ***********************************************************************
// <copyright file="WindowSizeChangedEventArgs.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Class WindowSizeChangedEventArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class WindowSizeChangedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        public double Height { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        public double Width { get; set; }

        #endregion Properties
    }
}
