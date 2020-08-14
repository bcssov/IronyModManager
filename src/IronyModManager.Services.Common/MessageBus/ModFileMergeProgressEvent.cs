// ***********************************************************************
// Assembly         : IronyModManager.Services.Common
// Author           : Mario
// Created          : 08-14-2020
//
// Last Modified By : Mario
// Last Modified On : 08-14-2020
// ***********************************************************************
// <copyright file="ModFileMergeProgressEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Services.Common.MessageBus
{
    /// <summary>
    /// Class ModFileMergeProgressEvent.
    /// Implements the <see cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Services.Common.MessageBus.ModDefinitionProcessEvent" />
    public class ModFileMergeProgressEvent : ModDefinitionProcessEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModFileMergeProgressEvent"/> class.
        /// </summary>
        /// <param name="step">The step.</param>
        /// <param name="percentage">The percentage.</param>
        public ModFileMergeProgressEvent(int step, int percentage) : base(percentage)
        {
            Step = step;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the step.
        /// </summary>
        /// <value>The step.</value>
        public int Step { get; set; }

        #endregion Properties
    }
}
