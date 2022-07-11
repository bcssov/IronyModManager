// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 06-10-2020
//
// Last Modified By : Mario
// Last Modified On : 07-11-2022
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
        public IChildDependencyResolver CreateScope() => new ChildDependencyResolver(this);

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

        #region Classes

        /// <summary>
        /// Class ChildDependencyResolver.
        /// Implements the <see cref="SlimMessageBus.Host.DependencyResolver.IChildDependencyResolver" />
        /// </summary>
        /// <seealso cref="SlimMessageBus.Host.DependencyResolver.IChildDependencyResolver" />
        private class ChildDependencyResolver : IChildDependencyResolver
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ChildDependencyResolver" /> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public ChildDependencyResolver(IDependencyResolver parent) => Parent = parent;

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the parent.
            /// </summary>
            /// <value>The parent.</value>
            public IDependencyResolver Parent { get; }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Creates the scope.
            /// </summary>
            /// <returns>IChildDependencyResolver.</returns>
            public IChildDependencyResolver CreateScope() => new ChildDependencyResolver(this);

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
            public object Resolve(Type type) => Parent.Resolve(type);

            #endregion Methods
        }

        #endregion Classes
    }
}
