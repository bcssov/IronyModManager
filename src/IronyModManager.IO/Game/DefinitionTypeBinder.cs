
// ***********************************************************************
// Assembly         : IronyModManager.IO
// Author           : Mario
// Created          : 08-12-2022
//
// Last Modified By : Mario
// Last Modified On : 06-25-2023
// ***********************************************************************
// <copyright file="DefinitionTypeBinder.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using IronyModManager.DI;
using IronyModManager.Shared.Models;
using LiteDB;

namespace IronyModManager.IO.Game
{

    /// <summary>
    /// Class DefinitionTypeBinder.
    /// Implements the <see cref="ITypeNameBinder" />
    /// </summary>
    /// <seealso cref="ITypeNameBinder" />
    internal class DefinitionTypeBinder : ITypeNameBinder
    {
        #region Methods

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>System.String.</returns>
        /// <exception cref="System.NotSupportedException">Not supported type.</exception>
        public string GetName(Type type)
        {
            if (typeof(IDefinition).IsAssignableFrom(type))
            {
                return nameof(IDefinition);
            }
            throw new NotSupportedException("Not supported type.");
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Type.</returns>
        /// <exception cref="System.NotSupportedException">Not supported type</exception>
        public Type GetType(string name)
        {
            return name switch
            {
                nameof(IDefinition) => DIResolver.GetImplementationType(typeof(IDefinition)),
                _ => throw new NotSupportedException("Not supported type."),
            };
        }

        #endregion Methods
    }
}
