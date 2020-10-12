// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 06-11-2020
//
// Last Modified By : Mario
// Last Modified On : 10-12-2020
// ***********************************************************************
// <copyright file="ModDefinitionAnalyzeEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Services.Common.MessageBus
{
    /// <summary>
    /// Class ModDefinitionAnalyzeEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    /// Implements the <see cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    /// <seealso cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    public class ModDefinitionAnalyzeEvent : ModDefinitionProcessEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModDefinitionAnalyzeEvent" /> class.
        /// </summary>
        /// <param name="percentage">if set to <c>true</c> [percentage].</param>
        public ModDefinitionAnalyzeEvent(double percentage) : base(percentage)
        {
        }

        #endregion Constructors
    }
}
