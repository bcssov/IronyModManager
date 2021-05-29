// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 05-27-2021
//
// Last Modified By : Mario
// Last Modified On : 05-27-2021
// ***********************************************************************
// <copyright file="GameIndexProgressEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Services.Common.MessageBus
{
    /// <summary>
    /// Class GameIndexProgressEvent.
    /// Implements the <see cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    public class GameIndexProgressEvent : ModDefinitionProcessEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GameIndexProgressEvent"/> class.
        /// </summary>
        /// <param name="percentage">if set to <c>true</c> [percentage].</param>
        public GameIndexProgressEvent(double percentage) : base(percentage)
        {
        }

        #endregion Constructors
    }
}
