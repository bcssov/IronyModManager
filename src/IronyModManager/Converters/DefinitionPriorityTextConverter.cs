// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 04-27-2020
//
// Last Modified By : Mario
// Last Modified On : 12-07-2020
// ***********************************************************************
// <copyright file="DefinitionPriorityTextConverter.cs" company="Mario">
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
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Models;

namespace IronyModManager.Converters
{
    /// <summary>
    /// Class DefinitionModTextConverter.
    /// Implements the <see cref="Avalonia.Data.Converters.IMultiValueConverter" />
    /// </summary>
    /// <seealso cref="Avalonia.Data.Converters.IMultiValueConverter" />
    public class DefinitionPriorityTextConverter : IMultiValueConverter
    {
        #region Methods

        /// <summary>
        /// Converts the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="culture">The culture.</param>
        /// <returns>System.Object.</returns>
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Count == 2)
            {
                if (values[0] is IEnumerable<IDefinition> col && values[1] is IDefinition definition)
                {
                    var service = DIResolver.Get<IModPatchCollectionService>();
                    if (!service.IsPatchMod(definition.ModName))
                    {
                        var locManager = DIResolver.Get<ILocalizationManager>();
                        var clean = new List<IDefinition>();
                        foreach (var item in col)
                        {
                            if (!service.IsPatchMod(item.ModName))
                            {
                                clean.Add(item);
                            }
                        }

                        var priority = service.EvalDefinitionPriority(clean);
                        if (priority?.Definition == definition)
                        {
                            var type = priority.PriorityType switch
                            {
                                DefinitionPriorityType.FIOS => locManager.GetResource(LocalizationResources.Conflict_Solver.PriorityReason.FIOS),
                                DefinitionPriorityType.LIOS => locManager.GetResource(LocalizationResources.Conflict_Solver.PriorityReason.LIOS),
                                DefinitionPriorityType.ModOrder => locManager.GetResource(LocalizationResources.Conflict_Solver.PriorityReason.Order),
                                DefinitionPriorityType.ModOverride => locManager.GetResource(LocalizationResources.Conflict_Solver.PriorityReason.Override),
                                _ => string.Empty
                            };
                            if (!string.IsNullOrWhiteSpace(type))
                            {
                                return $"{definition.ModName} - {definition.Id} {type}";
                            }
                        }
                    }
                    return $"{definition.ModName} - {definition.Id}";
                }
            }
            return string.Empty;
        }

        #endregion Methods
    }
}
