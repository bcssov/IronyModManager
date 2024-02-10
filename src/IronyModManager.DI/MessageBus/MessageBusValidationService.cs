
// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 02-09-2024
//
// Last Modified By : Mario
// Last Modified On : 02-09-2024
// ***********************************************************************
// <copyright file="MessageBusValidationService.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SlimMessageBus.Host;
using SlimMessageBus.Host.Services;

namespace IronyModManager.DI.MessageBus
{

    /// <summary>
    /// Class MessageBusValidationService.
    /// Implements the <see cref="DefaultMessageBusSettingsValidationService" />
    /// </summary>
    /// <seealso cref="DefaultMessageBusSettingsValidationService" />
    internal class MessageBusValidationService : DefaultMessageBusSettingsValidationService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBusValidationService" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public MessageBusValidationService(MessageBusSettings settings) : base(settings)
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Asserts the settings.
        /// </summary>
        public override void AssertSettings()
        {
            // Sigh, let's fix the validation mess
            foreach (var consumerSettings in Settings.Consumers)
            {
                if (consumerSettings.ConsumerMethodInfo != null && consumerSettings.ConsumerMethod == null)
                {
                    consumerSettings.ConsumerMethod = ReflectionUtils.GenerateMethodCallToFunc<Func<object, object, Task>>(consumerSettings.ConsumerMethodInfo, consumerSettings.ConsumerType, typeof(Task), consumerSettings.MessageType);
                }
            }
            base.AssertSettings();
        }

        #endregion Methods
    }
}
