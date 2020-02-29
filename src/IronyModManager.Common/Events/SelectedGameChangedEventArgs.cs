// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 02-29-2020
//
// Last Modified By : Mario
// Last Modified On : 02-29-2020
// ***********************************************************************
// <copyright file="SelectedGameChangedEventArgs.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Models.Common;
using IronyModManager.Shared;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Class SelectedGameChangedEventArgs.
    /// Implements the <see cref="System.EventArgs" />
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    [ExcludeFromCoverage("Excluding external message bus.")]
    public class SelectedGameChangedEventArgs : EventArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets the game.
        /// </summary>
        /// <value>The game.</value>
        public IGame Game { get; set; }

        #endregion Properties
    }
}
