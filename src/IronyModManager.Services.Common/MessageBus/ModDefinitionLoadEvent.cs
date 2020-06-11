// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 06-11-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
// ***********************************************************************
// <copyright file="ModDefinitionLoadEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Services.Common.MessageBus
{
    /// <summary>
    /// Class ModDefinitionLoadEvent.
    /// Implements the <see cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    public class ModDefinitionLoadEvent : ModDefinitionProcessEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModDefinitionLoadEvent" /> class.
        /// </summary>
        /// <param name="percentage">if set to <c>true</c> [percentage].</param>
        public ModDefinitionLoadEvent(int percentage) : base(percentage)
        {
        }

        #endregion Constructors
    }
}
