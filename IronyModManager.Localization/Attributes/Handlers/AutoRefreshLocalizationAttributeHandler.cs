// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-21-2020
//
// Last Modified By : Mario
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="AutoRefreshLocalizationAttributeHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;

namespace IronyModManager.Localization.Attributes.Handlers
{
    /// <summary>
    /// Class ForceLocalizeAttributeHandler.
    /// Implements the <see cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationAttributeHandler" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationAttributeHandler" />
    public class AutoRefreshLocalizationAttributeHandler : ILocalizationAttributeHandler
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can process the specified attribute.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can process the specified attribute; otherwise, <c>false</c>.</returns>
        public bool CanProcess(AttributeHandlersArgs args)
        {
            return args.Attribute is AutoRefreshLocalizationAttribute;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        public string GetData(AttributeHandlersArgs args)
        {
            return string.Empty;
        }

        /// <summary>
        /// Determines whether the specified attribute has data.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if the specified attribute has data; otherwise, <c>false</c>.</returns>
        public bool HasData(AttributeHandlersArgs args)
        {
            return true;
        }

        #endregion Methods
    }
}
