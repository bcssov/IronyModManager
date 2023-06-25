
// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 06-25-2023
//
// Last Modified By : Mario
// Last Modified On : 06-25-2023
// ***********************************************************************
// <copyright file="MessageTypeResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using SlimMessageBus.Host;

namespace IronyModManager.DI.MessageBus
{

    /// <summary>
    /// Class MessageTypeResolver.
    /// Implements the <see cref="IMessageTypeResolver" />
    /// </summary>
    /// <seealso cref="IMessageTypeResolver" />
    internal class MessageTypeResolver : IMessageTypeResolver
    {
        #region Fields

        /// <summary>
        /// The registration map
        /// </summary>
        private Dictionary<Type, string> registrationMap;

        /// <summary>
        /// The reverse registration map
        /// </summary>
        private Dictionary<string, Type> reverseRegistrationMap;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageTypeResolver"/> class.
        /// </summary>
        /// <param name="registrationMap">The registration map.</param>
        public MessageTypeResolver(Dictionary<Type, string> registrationMap)
        {
            this.registrationMap = registrationMap;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Converts to name.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns>System.String.</returns>
        public string ToName(Type messageType)
        {
            if (registrationMap.TryGetValue(messageType, out var name))
            {
                return name;
            }
            return string.Empty;
        }

        /// <summary>
        /// Converts to type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Type.</returns>
        public Type ToType(string name)
        {
            if (reverseRegistrationMap == null)
            {
                reverseRegistrationMap = registrationMap.ToDictionary(p => p.Value, p => p.Key);
            }
            if (reverseRegistrationMap.TryGetValue(name, out var type))
            {
                return type;
            }
            return null;
        }

        #endregion Methods
    }
}
