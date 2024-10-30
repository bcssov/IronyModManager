// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2024
// ***********************************************************************
// <copyright file="MessageBusDependencyResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SlimMessageBus.Host;

namespace IronyModManager.DI.MessageBus
{
    /// <summary>
    /// Class MessageBusDependencyResolver.
    /// Implements the <see cref="SlimMessageBus.Host.DependencyResolver.IDependencyResolver" />
    /// </summary>
    /// <seealso cref="SlimMessageBus.Host.DependencyResolver.IDependencyResolver" />
    internal class MessageBusDependencyResolver : IServiceProvider
    {
        #region Fields

        /// <summary>
        /// The message type resolver
        /// </summary>
        private readonly MessageTypeResolver messageTypeResolver;

        /// <summary>
        /// The resolved collections
        /// </summary>
        private readonly Dictionary<Type, Func<object>> resolvedCollections;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBusDependencyResolver" /> class.
        /// </summary>
        /// <param name="resolvedCollections">The resolved collections.</param>
        /// <param name="messageTypeResolver">The message type resolver.</param>
        public MessageBusDependencyResolver(Dictionary<Type, Func<object>> resolvedCollections, MessageTypeResolver messageTypeResolver)
        {
            this.messageTypeResolver = messageTypeResolver;
            this.resolvedCollections = resolvedCollections;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of type <paramref name="serviceType" />.
        /// -or-
        /// <see langword="null" /> if there is no service object of type <paramref name="serviceType" />.</returns>
        public object GetService(Type serviceType)
        {
            if (typeof(ILoggerFactory).IsAssignableFrom(serviceType))
            {
                return NullLoggerFactory.Instance;
            }
            else if (typeof(IMessageTypeResolver).IsAssignableFrom(serviceType))
            {
                return messageTypeResolver;
            }
            else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(serviceType) && typeof(SlimMessageBus.Host.Interceptor.IMessageBusLifecycleInterceptor).IsAssignableFrom(serviceType.GetGenericArguments()[0]))
            {
                return null;
            }

            if (DIContainer.Container.IsLocked)
            {
                var obj = DIResolver.Get(serviceType);
                return obj;
            }
            else
            {
                return resolvedCollections.TryGetValue(serviceType, out var instance) ? instance() : null;
            }
        }

        #endregion Methods
    }
}
