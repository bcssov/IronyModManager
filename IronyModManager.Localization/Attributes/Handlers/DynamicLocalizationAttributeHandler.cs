// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-21-2020
//
// Last Modified By : Mario
// Last Modified On : 01-23-2020
// ***********************************************************************
// <copyright file="DynamicLocalizationAttributeHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System;
using IronyModManager.DI;

namespace IronyModManager.Localization.Attributes.Handlers
{
    /// <summary>
    /// Class DynamicLocalizationAttributeHandler.
    /// Implements the <see cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationAttributeHandler" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationAttributeHandler" />
    public class DynamicLocalizationAttributeHandler : ILocalizationAttributeHandler
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can process the specified attribute.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can process the specified attribute; otherwise, <c>false</c>.</returns>
        public bool CanProcess(AttributeHandlersArgs args)
        {
            return args.Attribute is DynamicLocalizationAttribute;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>System.String.</returns>
        public string GetData(AttributeHandlersArgs args)
        {
            var valAttr = (DynamicLocalizationAttribute)args.Attribute;
            var dependentProp = args.Invocation.InvocationTarget.GetType().GetProperty(valAttr.GetDependentProperty());
            var dependentPropValue = dependentProp.GetValue(args.Invocation.InvocationTarget, null);
            var resKey = $"{valAttr.ResourcePrefix()}{dependentPropValue.ToString()}";
            var translation = DIResolver.Get<ILocalizationManager>().GetResource(resKey);
            return translation;
        }

        /// <summary>
        /// Determines whether the specified attribute has data.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if the specified attribute has data; otherwise, <c>false</c>.</returns>
        public bool HasData(AttributeHandlersArgs args)
        {
            return args.Value != null;
        }

        #endregion Methods
    }
}
