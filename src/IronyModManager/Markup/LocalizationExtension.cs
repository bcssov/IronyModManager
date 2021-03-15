// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 03-14-2021
//
// Last Modified By : Mario
// Last Modified On : 03-14-2021
// ***********************************************************************
// <copyright file="LocalizationExtension.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Markup.Xaml.MarkupExtensions;
using IronyModManager.Shared;

namespace IronyModManager.Markup
{
    /// <summary>
    /// Class LocalizationExtension.
    /// </summary>
    [ExcludeFromCoverage("Should be tested in functional testing.")]
    public class LocalizationExtension
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalizationExtension" /> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public LocalizationExtension(string key)
        {
            Key = key;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <value>The key.</value>
        public string Key { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Provides the value.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <returns>object.</returns>
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new ReflectionBindingExtension($"[{Key}]")
            {
                Mode = Avalonia.Data.BindingMode.OneWay,
                Source = LocalizationProvider.Instance,
                FallbackValue = string.Empty
            };
            return binding.ProvideValue(serviceProvider);
        }

        #endregion Methods
    }
}
