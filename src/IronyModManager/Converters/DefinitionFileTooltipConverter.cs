// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-21-2020
//
// Last Modified By : Mario
// Last Modified On : 07-12-2022
// ***********************************************************************
// <copyright file="DefinitionFileTooltipConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;
using IronyModManager.DI;
using IronyModManager.Localization;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

namespace IronyModManager.Converters
{
    /// <summary>
    /// Class DefinitionFileTooltipConverter.
    /// Implements the <see cref="Avalonia.Data.Converters.IValueConverter" />
    /// </summary>
    /// <seealso cref="Avalonia.Data.Converters.IValueConverter" />
    public class DefinitionFileTooltipConverter : IValueConverter
    {
        #region Fields

        /// <summary>
        /// The localization manager
        /// </summary>
        private ILocalizationManager localizationManager;

        /// <summary>
        /// The service
        /// </summary>
        private IModPatchCollectionService service;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        /// <remarks>This method should not throw exceptions. If the value is not convertible, return
        /// a <see cref="T:Avalonia.Data.BindingNotification" /> in an error state. Any exceptions thrown will be
        /// treated as an application exception.</remarks>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is IDefinition definition)
            {
                if (service == null)
                {
                    service = DIResolver.Get<IModPatchCollectionService>();
                }
                if (localizationManager == null)
                {
                    localizationManager = DIResolver.Get<ILocalizationManager>();
                }
                if (!service.IsPatchMod(definition.ModName))
                {
                    if (definition.LastModified.HasValue)
                    {
                        var resource = localizationManager.GetResource(LocalizationResources.Conflict_Solver.Tooltips.DefinitionInfo);
                        return IronyFormatter.Format(resource, new { definition.File, DateTime = definition.LastModified });
                    }
                    return definition.File;
                }
            }
            return null;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        /// <remarks>This method should not throw exceptions. If the value is not convertible, return
        /// a <see cref="T:Avalonia.Data.BindingNotification" /> in an error state. Any exceptions thrown will be
        /// treated as an application exception.</remarks>
        [ExcludeFromCoverage("Not being used.")]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion Methods
    }
}
