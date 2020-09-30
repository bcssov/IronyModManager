// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 09-30-2020
//
// Last Modified By : Mario
// Last Modified On : 09-30-2020
// ***********************************************************************
// <copyright file="ModReportExportEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Services.Common.MessageBus
{
    /// <summary>
    /// Class ModReportExportEvent.
    /// Implements the <see cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    public class ModReportExportEvent : ModDefinitionProcessEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModReportExportEvent"/> class.
        /// </summary>
        /// <param name="progress">The progress.</param>
        public ModReportExportEvent(int progress) : base(progress)
        {
        }

        #endregion Constructors
    }
}
