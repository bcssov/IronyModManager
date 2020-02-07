// ***********************************************************************
// Assembly         : IronyModManager.Localization.Tests
// Author           : Mario
// Created          : 02-06-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2020
// ***********************************************************************
// <copyright file="LocalizationInterceptorTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using FluentAssertions;
using IronyModManager.DI;
using IronyModManager.Localization.Attributes;
using IronyModManager.Localization.Attributes.Handlers;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.Localization.Tests
{
    /// <summary>
    /// Class LocalizationInterceptorTests.
    /// </summary>
    public class LocalizationInterceptorTests
    {

        /// <summary>
        /// Defines the test method Should_return_correct_actual_type.
        /// </summary>
        [Fact]
        public void Should_return_correct_actual_type()
        {
            DISetup.SetupContainer();
            DISetup.Container.RegisterLocalization<FakeLocalizableViewModel>();
            var model = DISetup.Container.GetInstance<FakeLocalizableViewModel>();            
            model.ActualType.Should().Be(typeof(FakeLocalizableViewModel));
        }

        /// <summary>
        /// Defines the test method Should_refresh_localization.
        /// </summary>
        [Fact]
        public void Should_refresh_localization()
        {
            DISetup.SetupContainer();
            DISetup.Container.RegisterLocalization<FakeLocalizableViewModel>();
            DISetup.Container.Collection.Register<ILocalizationRefreshHandler>(typeof(FakeLocalizationRefreshHandler));
            var model = DISetup.Container.GetInstance<FakeLocalizableViewModel>();
            model.OnLocaleChanged("en", "de");
            model.Dummy.Should().Be("Refresh value");
        }

        /// <summary>
        /// Defines the test method Model_should_trigger_single_changed_event.
        /// </summary>
        [Fact]
        public void Model_should_trigger_single_changed_event()
        {
            DISetup.SetupContainer();
            DISetup.Container.RegisterLocalization<FakeLocalizableViewModel>();
            var events = new List<string>();
            var model = DIResolver.Get<FakeLocalizableViewModel>();
            model.PropertyChanged += (s, e) =>
            {
                events.Add(e.PropertyName);
            };
            model.Dummy = "test";
            model.Dummy = "test";
            events.Count.Should().Be(1);
        }


        /// <summary>
        /// Defines the test method Model_should_trigger_single_changing_event.
        /// </summary>
        [Fact]
        public void Model_should_trigger_single_changing_event()
        {
            DISetup.SetupContainer();
            DISetup.Container.RegisterLocalization<FakeLocalizableViewModel>();
            var events = new List<string>();
            var model = DIResolver.Get<FakeLocalizableViewModel>();
            model.PropertyChanging += (s, e) =>
            {
                events.Add(e.PropertyName);
            };
            model.Dummy = "test";
            model.Dummy = "test";
            events.Count.Should().Be(1);
        }

        /// <summary>
        /// Defines the test method Model_should_trigger_double_changed_event.
        /// </summary>
        [Fact]
        public void Model_should_trigger_double_changed_event()
        {
            DISetup.SetupContainer();
            DISetup.Container.RegisterLocalization<FakeLocalizableViewModel>();
            var events = new List<string>();
            var model = DIResolver.Get<FakeLocalizableViewModel>();
            model.PropertyChanged += (s, e) =>
            {
                events.Add(e.PropertyName);
            };
            model.Dummy = "test";
            model.Dummy = "test 2";
            events.Count.Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Model_should_trigger_double_changing_event.
        /// </summary>
        [Fact]
        public void Model_should_trigger_double_changing_event()
        {
            DISetup.SetupContainer();
            DISetup.Container.RegisterLocalization<FakeLocalizableViewModel>();
            var events = new List<string>();
            var model = DIResolver.Get<FakeLocalizableViewModel>();
            model.PropertyChanging += (s, e) =>
            {
                events.Add(e.PropertyName);
            };
            model.Dummy = "test";
            model.Dummy = "test 2";
            events.Count.Should().Be(2);
        }

        [Fact]
        public void Should_localize_property()
        {
            DISetup.SetupContainer();
            DISetup.Container.RegisterLocalization<FakeLocalizableViewModel>();
            DISetup.Container.Collection.Register<ILocalizationAttributeHandler>(typeof(FakeLocalizationAttributeHandler));
            var model = DISetup.Container.GetInstance<FakeLocalizableViewModel>();
            model.Dummy.Should().Be("Dummy data");
        }

        /// <summary>
        /// Class FakeRefreshAttribute.
        /// Implements the <see cref="IronyModManager.Localization.Attributes.LocalizationAttributeBase" />
        /// </summary>
        /// <seealso cref="IronyModManager.Localization.Attributes.LocalizationAttributeBase" />
        [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
        private class FakeRefreshAttribute : LocalizationAttributeBase
        {

        }

        /// <summary>
        /// Class FakeLocalizableViewModel.
        /// Implements the <see cref="IronyModManager.Localization.ILocalizableViewModel" />
        /// </summary>
        /// <seealso cref="IronyModManager.Localization.ILocalizableViewModel" />
        public class FakeLocalizableViewModel : ILocalizableViewModel
        {
            /// <summary>
            /// Occurs when a property value is changing.
            /// </summary>
            public event PropertyChangingEventHandler PropertyChanging;
            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Gets the actual type.
            /// </summary>
            /// <value>The actual type.</value>
            public virtual Type ActualType => typeof(FakeLocalizableViewModel);

            /// <summary>
            /// Gets or sets the dummy.
            /// </summary>
            /// <value>The dummy.</value>
            [FakeRefresh]
            public virtual string Dummy { get; set; }

            /// <summary>
            /// Called when [locale changed].
            /// </summary>
            /// <param name="newLocale">The new locale.</param>
            /// <param name="oldLocale">The old locale.</param>
            public virtual void OnLocaleChanged(string newLocale, string oldLocale)
            {
               
            }

            /// <summary>
            /// Called when [property changed].
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            public virtual void OnPropertyChanged(string methodName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(methodName));
            }

            /// <summary>
            /// Called when [property changing].
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            public virtual void OnPropertyChanging(string methodName)
            {
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(methodName));
            }
        }

        /// <summary>
        /// Class FakeLocalizationAttributeHandler.
        /// Implements the <see cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationAttributeHandler" />
        /// </summary>
        /// <seealso cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationAttributeHandler" />
        private class FakeLocalizationAttributeHandler : ILocalizationAttributeHandler
        {
            /// <summary>
            /// Determines whether this instance can process the specified attribute.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns><c>true</c> if this instance can process the specified attribute; otherwise, <c>false</c>.</returns>
            public bool CanProcess(AttributeHandlersArgs args)
            {
                return args.Attribute is FakeRefreshAttribute;
            }

            /// <summary>
            /// Gets the data.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns>System.String.</returns>
            public string GetData(AttributeHandlersArgs args)
            {
                return "Dummy data";
            }

            /// <summary>
            /// Determines whether the specified attribute has data.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns><c>true</c> if the specified attribute has data; otherwise, <c>false</c>.</returns>
            public bool HasData(AttributeHandlersArgs args)
            {
                return true;
            }
        }

        /// <summary>
        /// Class FakeLocalizationRefreshHandler.
        /// Implements the <see cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationRefreshHandler" />
        /// </summary>
        /// <seealso cref="IronyModManager.Localization.Attributes.Handlers.ILocalizationRefreshHandler" />
        private class FakeLocalizationRefreshHandler : ILocalizationRefreshHandler
        {
            /// <summary>
            /// Determines whether this instance can refresh the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <returns><c>true</c> if this instance can refresh the specified arguments; otherwise, <c>false</c>.</returns>
            /// <exception cref="NotImplementedException"></exception>
            public bool CanRefresh(LocalizationRefreshArgs args)
            {
                return true;
            }

            /// <summary>
            /// Refreshes the specified arguments.
            /// </summary>
            /// <param name="args">The arguments.</param>
            /// <exception cref="NotImplementedException"></exception>
            public void Refresh(LocalizationRefreshArgs args)
            {
                args.Property.SetValue(args.Invocation.InvocationTarget, "Refresh value");
            }
        }
    }
}
