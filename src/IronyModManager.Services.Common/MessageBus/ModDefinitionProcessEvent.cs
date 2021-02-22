// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 06-11-2020
//
// Last Modified By : Mario
// Last Modified On : 02-22-2021
// ***********************************************************************
// <copyright file="ModDefinitionProcessEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using IronyModManager.Shared.MessageBus.Events;

namespace IronyModManager.Services.Common.MessageBus
{
    /// <summary>
    /// Class ModDefinitionProcessEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.Events.BaseNonAwaitableEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.Events.BaseNonAwaitableEvent" />
    public abstract class ModDefinitionProcessEvent : BaseNonAwaitableEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModDefinitionProcessEvent" /> class.
        /// </summary>
        /// <param name="percentage">if set to <c>true</c> [percentage].</param>
        public ModDefinitionProcessEvent(double percentage)
        {
            Percentage = percentage;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        /// <value>The percentage.</value>
        public double Percentage { get; }

        #endregion Properties
    }
}
