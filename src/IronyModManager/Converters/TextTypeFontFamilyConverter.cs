// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-01-2021
//
// Last Modified By : Mario
// Last Modified On : 03-13-2021
// ***********************************************************************
// <copyright file="TextTypeFontFamilyConverter.cs" company="Mario">
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
using IronyModManager.Platform.Fonts;
using IronyModManager.Services.Common;

namespace IronyModManager.Converters
{
    /// <summary>
    /// Class TextTypeFontFamilyConverter.
    /// Implements the <see cref="Avalonia.Data.Converters.IValueConverter" />
    /// </summary>
    /// <seealso cref="Avalonia.Data.Converters.IValueConverter" />
    public class TextTypeFontFamilyConverter : IValueConverter
    {
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
            var fontResolver = DIResolver.Get<IFontFamilyManager>();
            var languageService = DIResolver.Get<ILanguagesService>();
            if (value != null)
            {
                var language = languageService.GetLanguageBySupportedNameBlock(value.ToString());
                if (language != null)
                {
                    return fontResolver.ResolveFontFamily(language.Font).GetFontFamily();
                }
            }
            var font = fontResolver.ResolveFontFamily(languageService.GetSelected().Font);
            return font.GetFontFamily();
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
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        #endregion Methods
    }
}
