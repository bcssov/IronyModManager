// ***********************************************************************
// Assembly         : IronyModManager.IO.Common
// Author           : Mario
// Created          : 07-11-2022
//
// Last Modified By : Mario
// Last Modified On : 07-11-2022
// ***********************************************************************
// <copyright file="ISteam.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IronyModManager.IO.Common.Platforms
{
    /// <summary>
    /// Interface ISteam
    /// </summary>
    public interface ISteam
    {
        #region Methods

        /// <summary>
        /// Initializes the alternate asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> InitAlternateAsync();

        /// <summary>
        /// Initializes the asynchronous.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> InitAsync(long appId);

        /// <summary>
        /// Shutdowns the API asynchronous.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        Task<bool> ShutdownAPIAsync();

        #endregion Methods
    }
}
