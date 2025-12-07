// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 02-06-2020
//
// Last Modified By : Mario
// Last Modified On : 04-17-2020
// ***********************************************************************
// <copyright file="LocalizationResourceProviderTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AwesomeAssertions;
using IronyModManager.Implementation;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class LocalizationResourceProviderTests.
    /// </summary>
    public class LocalizationResourceProviderTests
    {
        /// <summary>
        /// Defines the test method RootPath_should_be_localization_directory.
        /// </summary>
        [Fact]
        public void RootPath_should_be_localization_directory()
        {
            var locPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Constants.LocalizationsPath);
            var provider = new LocalizationResourceProvider();
            provider.RootPath.Should().Be(locPath);
        }

        /// <summary>
        /// Defines the test method Should_return_locales.
        /// </summary>
        [Fact]
        public void Should_return_locales()
        {
            var locales = new string[] { "en", "de", "es" };
            int hits = 0;
            var provider = new LocalizationResourceProvider();
            foreach (var item in provider.GetAvailableLocales())
            {
                if (locales.Contains(item))
                {
                    hits++;
                }
            }
            hits.Should().Be(locales.Count());
        }
    }
}
