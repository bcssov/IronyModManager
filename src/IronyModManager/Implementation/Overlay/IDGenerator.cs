// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-19-2020
//
// Last Modified By : Mario
// Last Modified On : 11-19-2020
// ***********************************************************************
// <copyright file="IDGenerator.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Implementation.Overlay
{
    /// <summary>
    /// Class IDGenerator.
    /// Implements the <see cref="IronyModManager.Implementation.Overlay.IIDGenerator" />
    /// </summary>
    /// <seealso cref="IronyModManager.Implementation.Overlay.IIDGenerator" />
    public class IDGenerator : IIDGenerator
    {
        #region Fields

        /// <summary>
        /// The object lock
        /// </summary>
        private static readonly object objectLock = new { };

        /// <summary>
        /// The identifier
        /// </summary>
        private long id = 0;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Gets the next identifier.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public long GetNextId()
        {
            lock (objectLock)
            {
                id++;
                return id;
            }
        }

        #endregion Methods
    }
}
