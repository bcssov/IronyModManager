// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 03-17-2021
//
// Last Modified By : Mario
// Last Modified On : 03-17-2021
// ***********************************************************************
// <copyright file="ModMergeFreeSpaceCheckEvent.cs" company="Mario">
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
    /// Class ModMergeFreeSpaceCheckEvent.
    /// Implements the <see cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    public class ModMergeFreeSpaceCheckEvent : ModDefinitionProcessEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModMergeFreeSpaceCheckEvent"/> class.
        /// </summary>
        /// <param name="percentage">if set to <c>true</c> [percentage].</param>
        public ModMergeFreeSpaceCheckEvent(double percentage) : base(percentage)
        {
        }

        #endregion Constructors
    }
}
