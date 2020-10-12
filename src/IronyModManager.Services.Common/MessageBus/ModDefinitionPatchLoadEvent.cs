// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 06-11-2020
//
// Last Modified By : Mario
// Last Modified On : 10-12-2020
// ***********************************************************************
// <copyright file="ModDefinitionPatchLoadEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Services.Common.MessageBus
{
    /// <summary>
    /// Class ModDefinitionPatchLoadEvent.
    /// Implements the <see cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    public class ModDefinitionPatchLoadEvent : ModDefinitionProcessEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModDefinitionPatchLoadEvent" /> class.
        /// </summary>
        /// <param name="percentage">if set to <c>true</c> [percentage].</param>
        public ModDefinitionPatchLoadEvent(double percentage) : base(percentage)
        {
        }

        #endregion Constructors
    }
}
