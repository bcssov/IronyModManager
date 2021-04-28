// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 04-27-2021
// ***********************************************************************
// <copyright file="MessageBusDependencyResolver.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using SlimMessageBus.Host.DependencyResolver;

namespace IronyModManager.DI.MessageBus
{
    /// <summary>
    /// Class MessageBusDependencyResolver.
    /// Implements the <see cref="SlimMessageBus.Host.DependencyResolver.IDependencyResolver" />
    /// </summary>
    /// <seealso cref="SlimMessageBus.Host.DependencyResolver.IDependencyResolver" />
    internal class MessageBusDependencyResolver : IDependencyResolver
    {
        #region Methods

        /// <summary>
        /// Creates the scope.
        /// </summary>
        /// <returns>IDependencyResolver.</returns>
        public IDependencyResolver CreateScope() => this;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Resolves the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        public object Resolve(Type type)
        {
            var obj = DIResolver.Get(type);
            return obj;
        }

        #endregion Methods
    }
}
