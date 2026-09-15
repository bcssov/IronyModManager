// ***********************************************************************
// Assembly         : IronyModManager
// ***********************************************************************
using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace IronyModManager.Converters
{
    /// <summary>
    /// Selects the presentation class for a virtual collection mod.
    /// </summary>
    public class VirtualModClassConverter : IValueConverter
    {
        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is true ? "VirtualMod" : string.Empty;
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
