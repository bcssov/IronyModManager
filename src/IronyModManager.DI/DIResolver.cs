// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 04-07-2020
// ***********************************************************************
// <copyright file="DIResolver.cs" company="IronyModManager.DI">
//     Copyright (c) Mario. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.DI
{
    /// <summary>
    /// Class DIResolver.
    /// </summary>
    public static class DIResolver
    {
        #region Methods

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        /// <exception cref="NullReferenceException">Container is null.</exception>
        public static T Get<T>() where T : class
        {
            Validate();
            return DIContainer.Container.GetInstance<T>();
        }

        /// <summary>
        /// Gets the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="NullReferenceException">Container is null.</exception>
        public static object Get(Type type)
        {
            Validate();
            return DIContainer.Container.GetInstance(type);
        }

        /// <summary>
        /// Gets the type of the implementation.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Type.</returns>
        public static Type GetImplementationType(Type type)
        {
            Validate();
            var reg = DIContainer.Container.GetRegistration(type);
            if (reg == null)
            {
                return null;
            }
            return reg.Registration.ImplementationType;
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <exception cref="NullReferenceException">container</exception>
        private static void Validate()
        {
            if (DIContainer.Container == null)
            {
                throw new NullReferenceException("container");
            }
        }

        #endregion Methods
    }
}
