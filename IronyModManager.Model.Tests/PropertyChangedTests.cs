// ***********************************************************************
// Assembly         : IronyModManager.Model.Tests
// Author           : Mario
// Created          : 01-29-2020
//
// Last Modified By : Mario
// Last Modified On : 01-31-2020
// ***********************************************************************
// <copyright file="PropertyChangedTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using IronyModManager.DI;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using SimpleInjector;
using Xunit;

namespace IronyModManager.Model.Tests
{
    /// <summary>
    /// Class PropertyChangedTests.
    /// </summary>
    public class PropertyChangedTests
    {
        /// <summary>
        /// Defines the test method Language_should_trigger_single_changed_event.
        /// </summary>
        [Fact]
        public void Language_should_trigger_single_changed_event()
        {
            RunGenericSinglePropTest<ILanguage>(nameof(Language.Abrv), "test", true);
        }

        /// <summary>
        /// Defines the test method Language_should_trigger_single_changing_event.
        /// </summary>
        [Fact]
        public void Language_should_trigger_single_changing_event()
        {
            RunGenericSinglePropTest<ILanguage>(nameof(Language.Abrv), "test", false);
        }

        /// <summary>
        /// Defines the test method Language_should_trigger_double_changed_event.
        /// </summary>
        [Fact]
        public void Language_should_trigger_double_changed_event()
        {
            RunGenericDoublePropTest<ILanguage>(nameof(Language.Abrv), "test", nameof(Language.Name), "test", true);
        }

        /// <summary>
        /// Defines the test method Language_should_trigger_double_changing_event.
        /// </summary>
        [Fact]
        public void Language_should_trigger_double_changing_event()
        {
            RunGenericDoublePropTest<ILanguage>(nameof(Language.Abrv), "test", nameof(Language.Name), "test", false);
        }

        /// <summary>
        /// Defines the test method Preferences_should_trigger_single_changed_event.
        /// </summary>
        [Fact]
        public void Preferences_should_trigger_single_changed_event()
        {
            RunGenericSinglePropTest<IPreferences>(nameof(Preferences.Locale), "test", true);
        }

        /// <summary>
        /// Defines the test method Preferences_should_trigger_single_changing_event.
        /// </summary>
        [Fact]
        public void Preferences_should_trigger_single_changing_event()
        {
            RunGenericSinglePropTest<IPreferences>(nameof(Preferences.Locale), "test", false);
        }

        /// <summary>
        /// Defines the test method Preferences_should_trigger_double_changed_event.
        /// </summary>
        [Fact]
        public void Preferences_should_trigger_double_changed_event()
        {
            RunGenericDoublePropTest<IPreferences>(nameof(Preferences.Locale), "test", nameof(Preferences.Locale), "test2", true);
        }

        /// <summary>
        /// Defines the test method Preferences_should_trigger_double_changing_event.
        /// </summary>
        [Fact]
        public void Preferences_should_trigger_double_changing_event()
        {
            RunGenericDoublePropTest<IPreferences>(nameof(Preferences.Locale), "test", nameof(Preferences.Locale), "test2", true);
        }

        /// <summary>
        /// Defines the test method Theme_should_trigger_single_changed_event.
        /// </summary>
        [Fact]
        public void Theme_should_trigger_single_changed_event()
        {
            RunGenericSinglePropTest<ITheme>(nameof(Theme.Type), Models.Common.Enums.Theme.Dark, true);
        }

        /// <summary>
        /// Defines the test method Theme_should_trigger_single_changing_event.
        /// </summary>
        [Fact]
        public void Theme_should_trigger_single_changing_event()
        {
            RunGenericSinglePropTest<ITheme>(nameof(Theme.Type), Models.Common.Enums.Theme.Dark, true);
        }

        /// <summary>
        /// Defines the test method Theme_should_trigger_double_changed_event.
        /// </summary>
        [Fact]
        public void Theme_should_trigger_double_changed_event()
        {
            RunGenericDoublePropTest<ITheme>(nameof(Theme.Type), Models.Common.Enums.Theme.Dark, nameof(Theme.IsSelected), true, true);
        }

        /// <summary>
        /// Defines the test method Theme_should_trigger_double_changing_event.
        /// </summary>
        [Fact]
        public void Theme_should_trigger_double_changing_event()
        {
            RunGenericDoublePropTest<ITheme>(nameof(Theme.Type), Models.Common.Enums.Theme.Dark, nameof(Theme.IsSelected), true, false);
        }

        /// <summary>
        /// Defines the test method WindowState_should_trigger_single_changed_event.
        /// </summary>
        [Fact]
        public void WindowState_should_trigger_single_changed_event()
        {
            RunGenericSinglePropTest<IWindowState>(nameof(WindowState.Height), 5, true);
        }

        /// <summary>
        /// Defines the test method WindowState_should_trigger_single_changing_event.
        /// </summary>
        [Fact]
        public void WindowState_should_trigger_single_changing_event()
        {
            RunGenericSinglePropTest<IWindowState>(nameof(WindowState.Height), 5, false);
        }

        /// <summary>
        /// Defines the test method WindowState_should_trigger_double_changed_event.
        /// </summary>
        [Fact]
        public void WindowState_should_trigger_double_changed_event()
        {
            RunGenericDoublePropTest<IWindowState>(nameof(WindowState.Height), 5, nameof(WindowState.Width), 4, true);
        }

        /// <summary>
        /// Defines the test method WindowState_should_trigger_double_changing_event.
        /// </summary>
        [Fact]
        public void WindowState_should_trigger_double_changing_event()
        {
            RunGenericDoublePropTest<IWindowState>(nameof(WindowState.Height), 5, nameof(WindowState.Width), 4, true);
        }


        /// <summary>
        /// Runs the generic single property test.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop">The property.</param>
        /// <param name="value">The value.</param>
        /// <param name="checkChanged">if set to <c>true</c> [check changed].</param>
        private void RunGenericSinglePropTest<T>(string prop, object value, bool checkChanged) where T : class, IModel
        {
            DISetup.SetupContainer();
            var events = new List<string>();
            var model = DIResolver.Get<T>();
            if (checkChanged)
            {
                model.PropertyChanged += (s, e) =>
                {
                    events.Add(e.PropertyName);
                };
            }
            else
            {
                model.PropertyChanging += (s, e) =>
                {
                    events.Add(e.PropertyName);
                };
            }       
            model.GetType().GetProperty(prop).SetValue(model, value);
            model.GetType().GetProperty(prop).SetValue(model, value);            
            events.Count.Should().Be(1);
            events.First().Should().Be(prop);
        }

        /// <summary>
        /// Runs the generic double property test.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prop">The property.</param>
        /// <param name="value">The value.</param>
        /// <param name="prop2">The prop2.</param>
        /// <param name="value2">The value2.</param>
        /// <param name="checkChanged">The check changed.</param>
        private void RunGenericDoublePropTest<T>(string prop, object value, string prop2, object value2, bool checkChanged) where T : class, IModel
        {
            DISetup.SetupContainer();
            var events = new List<string>();
            var model = DIResolver.Get<T>();
            if (checkChanged)
            {
                model.PropertyChanged += (s, e) =>
                {
                    events.Add(e.PropertyName);
                };
            }
            else
            {
                model.PropertyChanging += (s, e) =>
                {
                    events.Add(e.PropertyName);
                };
            }
            model.GetType().GetProperty(prop).SetValue(model, value);
            model.GetType().GetProperty(prop2).SetValue(model, value2);
            events.Count.Should().Be(2);
            events.First().Should().Be(prop);
            events.Last().Should().Be(prop2);
        }
    }
}
