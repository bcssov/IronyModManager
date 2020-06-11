// ***********************************************************************
// Assembly         : IronyModManager.IO.Common.MessageBus
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
// ***********************************************************************
// <copyright file="WritingStateOperationEvent.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.Shared.MessageBus;

namespace IronyModManager.IO.Common.MessageBus
{
    /// <summary>
    /// Class WritingStateOperationEvent.
    /// Implements the <see cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    /// </summary>
    /// <seealso cref="IronyModManager.Shared.MessageBus.IMessageBusEvent" />
    public class WritingStateOperationEvent : IMessageBusEvent
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [started writing].
        /// </summary>
        /// <value><c>true</c> if [started writing]; otherwise, <c>false</c>.</value>
        public bool StartedWriting { get; set; }

        #endregion Properties
    }
}
