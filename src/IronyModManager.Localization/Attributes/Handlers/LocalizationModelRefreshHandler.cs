// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-23-2020
//
// Last Modified By : Mario
// Last Modified On : 03-31-2020
// ***********************************************************************
// <copyright file="LocalizationModelRefreshHandler.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace IronyModManager.Localization.Attributes.Handlers
{
    /// <summary>
    /// Class LocalizationModelRefreshHandler.
    /// Implements the <see cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationRefreshHandler" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationRefreshHandler" />
    public class LocalizationModelRefreshHandler : ILocalizationRefreshHandler
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can refresh the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns>
        ///   <c>true</c> if this instance can refresh the specified arguments; otherwise, <c>false</c>.</returns>
        public bool CanRefresh(LocalizationRefreshArgs args)
        {
            return typeof(ILocalizableModel).IsAssignableFrom(args.Property.PropertyType);
        }

        /// <summary>
        /// Refreshes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Refresh(LocalizationRefreshArgs args)
        {
            if (args.Property.GetValue(args.Invocation.InvocationTarget, null) is ILocalizableModel val)
            {
                var props = val.GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(LocalizationAttributeBase)));
                if (props.Count() > 0)
                {
                    foreach (var prop in props)
                    {
                        if (prop.CanWrite)
                        {
                            var oldVal = prop.GetValue(val, null);
                            ((ILocalizableModel)args.Invocation.Proxy).OnPropertyChanging(prop.Name);
                            prop.SetValue(val, null);
                            ((ILocalizableModel)args.Invocation.Proxy).OnPropertyChanged(prop.Name);
                            ((ILocalizableModel)args.Invocation.Proxy).OnPropertyChanging(prop.Name);
                            prop.SetValue(val, oldVal);
                            ((ILocalizableModel)args.Invocation.Proxy).OnPropertyChanged(prop.Name);
                        }
                    }
                }
            }
        }

        #endregion Methods
    }
}
