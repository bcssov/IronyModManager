// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 06-11-2020
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
