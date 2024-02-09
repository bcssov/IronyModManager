
// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 06-25-2023
//
// Last Modified By : Mario
// Last Modified On : 02-09-2024
// ***********************************************************************
// <copyright file="MessageBusMemoryProvider.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlimMessageBus.Host;
using SlimMessageBus.Host.Memory;
using SlimMessageBus.Host.Services;

namespace IronyModManager.DI.MessageBus
{

    /// <summary>
    /// Class MessageBusMemoryProvider.
    /// Implements the <see cref="MemoryMessageBus" />
    /// </summary>
    /// <seealso cref="MemoryMessageBus" />
    internal class MessageBusMemoryProvider : MemoryMessageBus
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBusMemoryProvider" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public MessageBusMemoryProvider(MessageBusSettings settings) : base(settings, new MemoryMessageBusSettings() { EnableMessageSerialization = false })
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the validation service.
        /// </summary>
        /// <value>The validation service.</value>
        // Will you make up your mind where you want AssertSettings to be
        protected override IMessageBusSettingsValidationService ValidationService => new MessageBusValidationService(Settings);

        #endregion Properties
    }
}
