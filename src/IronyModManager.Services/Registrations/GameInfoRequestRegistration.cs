// ***********************************************************************
// Assembly         : IronyModManager.Services
// Author           : Mario
// Created          : 05-18-2026
//
// Last Modified By : Mario
// Last Modified On : 05-18-2026
// ***********************************************************************
// <copyright file="GameInfoRequestRegistration.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Services.MessageBus;
using IronyModManager.Shared;

namespace IronyModManager.Services.Registrations
{
    /// <summary>
    /// Class GameInfoRequestRegistration.
    /// Implements the <see cref="PostStartup" />
    /// </summary>
    /// <seealso cref="PostStartup" />
    internal class GameInfoRequestRegistration : PostStartup
    {
        #region Fields

        /// <summary>
        /// The handler
        /// </summary>
        private GameInfoRequestHandler handler;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Called when [post startup].
        /// </summary>
        public override void OnPostStartup()
        {
            handler = DIResolver.Get<GameInfoRequestHandler>();
        }

        #endregion Methods
    }
}
