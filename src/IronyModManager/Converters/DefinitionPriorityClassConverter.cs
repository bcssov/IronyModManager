// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 04-25-2020
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
// ***********************************************************************
// <copyright file="DefinitionPriorityClassConverter.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using IronyModManager.DI;
using IronyModManager.Services.Common;
using IronyModManager.Shared.Models;

namespace IronyModManager.Converters
{
    /// <summary>
    /// Class DefinitionPriorityClassConverter.
    /// Implements the <see cref="Avalonia.Data.Converters.IMultiValueConverter" />
    /// </summary>
    /// <seealso cref="Avalonia.Data.Converters.IMultiValueConverter" />
    public class DefinitionPriorityClassConverter : IMultiValueConverter
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
        /// <remarks>This method should not throw exceptions. If the value is not convertible, return
        /// <see cref="F:Avalonia.AvaloniaProperty.UnsetValue" />. Any exception thrown will be treated as
        /// an application exception.</remarks>
        public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Count == 2)
            {
                if (values[0] is IEnumerable<IDefinition> col && values[1] is IDefinition definition)
                {
                    var service = DIResolver.Get<IModPatchCollectionService>();
                    if (service.IsPatchMod(definition.ModName))
                    {
                        return "PatchMod";
                    }
                    var clean = new List<IDefinition>();
                    foreach (var item in col)
                    {
                        if (!service.IsPatchMod(item.ModName))
                        {
                            clean.Add(item);
                        }
                    }
                    var priority = service.EvalDefinitionPriority(clean);
                    if (priority != null && priority.Definition == definition && priority.PriorityType != Models.Common.DefinitionPriorityType.NoProvider)
                    {
                        return "CopiedDefinition";
                    }
                }
            }
            return string.Empty;
        }

        #endregion Methods
    }
}
