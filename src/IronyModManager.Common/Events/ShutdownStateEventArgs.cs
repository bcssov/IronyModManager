// ***********************************************************************
// Assembly         : IronyModManager.Common
// Author           : Mario
// Created          : 04-19-2020
//
// Last Modified By : Mario
// Last Modified On : 04-19-2020
// ***********************************************************************
// <copyright file="ShutdownStateEventArgs.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Common.Events
{
    /// <summary>
    /// Class ShutdownStateEventArgs.
    /// </summary>
    public class ShutdownStateEventArgs
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [prevent shutdown].
        /// </summary>
        /// <value><c>true</c> if [prevent shutdown]; otherwise, <c>false</c>.</value>
        public bool PreventShutdown { get; set; }

        #endregion Properties
    }
}
