// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-21-2020
//
// Last Modified By : Mario
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="ILocalizationAttributeHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;

namespace IronyModManager.Localization.Attributes.Handlers
{
    /// <summary>
    /// Interface ILocalizationAttributeHandler
    /// </summary>
    public interface ILocalizationAttributeHandler
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can process the specified attribute.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can process the specified attribute; otherwise, <c>false</c>.</returns>
        bool CanProcess(AttributeHandlersArgs args);

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        string GetData(AttributeHandlersArgs args);

        /// <summary>
        /// Determines whether the specified attribute has data.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if the specified attribute has data; otherwise, <c>false</c>.</returns>
        bool HasData(AttributeHandlersArgs args);

        #endregion Methods
    }
}
