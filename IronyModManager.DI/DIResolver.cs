// ***********************************************************************
// Assembly         : IronyModManager.DI
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 01-10-2020
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
            if (DIContainer.Container == null)
            {
                throw new NullReferenceException("Container is null.");
            }
            return DIContainer.Container.GetInstance<T>();
        }

        #endregion Methods
    }
}