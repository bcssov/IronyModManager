// ***********************************************************************
// Assembly         : IronyModManager.Shared
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 02-23-2021
// ***********************************************************************
// <copyright file="IMessageBusEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Shared.MessageBus
{
    /// <summary>
    /// Interface IMessageBusEvent
    /// </summary>
    public interface IMessageBusEvent
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is fire and forget.
        /// </summary>
        /// <value><c>true</c> if this instance is fire and forget; otherwise, <c>false</c>.</value>
        bool IsAwaitable { get; }

        /// <summary>
        /// Gets the tasks completed.
        /// </summary>
        /// <value>The tasks completed.</value>
        int TasksCompleted { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Finalizes the await.
        /// </summary>
        public void FinalizeAwait();

        #endregion Methods
    }
}
