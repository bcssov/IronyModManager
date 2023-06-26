
// ***********************************************************************
// Assembly         : IronyModManager.Parser
// Author           : Mario
// Created          : 06-25-2023
//
// Last Modified By : Mario
// Last Modified On : 06-25-2023
// ***********************************************************************
// <copyright file="DefinitionSearchTypeBinder.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace IronyModManager.Parser.Definitions
{

    /// <summary>
    /// Class DefinitionSearchTypeBinder.
    /// Implements the <see cref="ITypeNameBinder" />
    /// </summary>
    /// <seealso cref="ITypeNameBinder" />
    internal class DefinitionSearchTypeBinder : ITypeNameBinder
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
            if (type == typeof(DefinitionSearch))
            {
                return nameof(DefinitionSearch);
            }
            throw new NotSupportedException("Not supported type.");
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Type.</returns>
        /// <exception cref="System.NotSupportedException">Not supported type.</exception>
        public Type GetType(string name)
        {
            if (name == typeof(DefinitionSearch).Name)
            {
                return typeof(DefinitionSearch);
            }
            throw new NotSupportedException("Not supported type.");
        }

        #endregion Methods
    }
}
