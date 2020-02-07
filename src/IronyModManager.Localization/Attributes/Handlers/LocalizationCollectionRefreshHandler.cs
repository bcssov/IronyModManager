// ***********************************************************************
// Assembly         : IronyModManager.Localization
// Author           : Mario
// Created          : 01-23-2020
//
// Last Modified By : Mario
// Last Modified On : 02-05-2020
// ***********************************************************************
// <copyright file="LocalizationCollectionRefreshHandler.cs" company="Mario">
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
    /// Class LocalizationCollectionRefreshHandler.
    /// Implements the <see cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationRefreshHandler" />
    /// </summary>
    /// <seealso cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationRefreshHandler" />
    public class LocalizationCollectionRefreshHandler : ILocalizationRefreshHandler
    {
        #region Methods

        /// <summary>
        /// Determines whether this instance can refresh the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        /// <returns><c>true</c> if this instance can refresh the specified arguments; otherwise, <c>false</c>.</returns>
        public bool CanRefresh(LocalizationRefreshArgs args)
        {
            return typeof(IEnumerable<ILocalizableModel>).IsAssignableFrom(args.Property.PropertyType);
        }

        /// <summary>
        /// Refreshes the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public void Refresh(LocalizationRefreshArgs args)
        {
            var val = args.Property.GetValue(args.Invocation.InvocationTarget, null) as IEnumerable<ILocalizableModel>;
            if (val?.Count() > 0)
            {
                foreach (var item in val)
                {
                    var props = item.GetType().GetProperties().Where(p => Attribute.IsDefined(p, typeof(LocalizationAttributeBase)));
                    if (props.Count() > 0)
                    {
                        foreach (var prop in props)
                        {
                            if (prop.CanWrite)
                            {
                                var oldVal = prop.GetValue(item, null);
                                item.OnPropertyChanging(prop.Name);
                                prop.SetValue(item, null);
                                item.OnPropertyChanged(prop.Name);
                                item.OnPropertyChanging(prop.Name);
                                prop.SetValue(item, oldVal);
                                item.OnPropertyChanged(prop.Name);
                            }
                        }
                    }
                }
            }
        }

        #endregion Methods
    }
}
