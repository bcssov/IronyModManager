// ***********************************************************************
// Assembly         : IronyModManager.Localization.Tests
// Author           : Mario
// Created          : 02-05-2020
//
// Last Modified By : Mario
// Last Modified On : 02-05-2020
// ***********************************************************************
// <copyright file="LocalizationModelRefreshHandlerTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using AwesomeAssertions;
using IronyModManager.Localization.Attributes;
using IronyModManager.Localization.Attributes.Handlers;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Localization.Tests
{
    /// <summary>
    /// Class LocalizationModelRefreshHandlerTests.
    /// </summary>
    public class LocalizationModelRefreshHandlerTests
    {
        /// <summary>
        /// Defines the test method CanRefresh_should_be_true.
        /// </summary>
        [Fact]
        public void CanRefresh_should_be_true()
        {
            var fake = new FakeModel();
            var prop = fake.GetType().GetProperty("Dummy2");
            var handler = new LocalizationModelRefreshHandler();
            var args = new LocalizationRefreshArgs(null, prop);
            handler.CanRefresh(args).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CanRefresh_should_be_false.
        /// </summary>
        [Fact]
        public void CanRefresh_should_be_false()
        {
            var fake = new FakeModel();
            var prop = fake.GetType().GetProperty("Dummy1");
            var handler = new LocalizationModelRefreshHandler();
            var args = new LocalizationRefreshArgs(null, prop);
            handler.CanRefresh(args).Should().BeFalse();
        }

        /// <summary>
        /// Shoulds the refresh property.
        /// </summary>
        [Fact] 
        public void Should_refresh_property()
        {
            DISetup.SetupContainer();
            DISetup.Container.Register<FakeModel>();
            var model = DISetup.Container.GetInstance<FakeModel>();
            model.Dummy2 = new FakeLocalizableModel()
            {
                FakeProp = "1"
            };
            var changedEvents = new List<string>();
            var changingEvents = new List<string>();
            var initialValue = model.Dummy2.FakeProp;
            var finalValue = string.Empty;
            model.Dummy2.PropertyChanged += (s, e) =>
            {
                changedEvents.Add(e.PropertyName);
            };
            model.Dummy2.PropertyChanging += (s, e) =>
            {
                changingEvents.Add(e.PropertyName);
            };            
            var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
            invocation.Setup(p => p.InvocationTarget).Returns(model);
            invocation.Setup(p => p.Proxy).Returns(model.Dummy2);
            var prop = model.GetType().GetProperty("Dummy2");
            var args = new LocalizationRefreshArgs(invocation.Object, prop);
            var handler = new LocalizationModelRefreshHandler();
            handler.Refresh(args);
            changedEvents.Count.Should().Be(2);
            changedEvents.GroupBy(s => s).Select(s => s.First()).Count().Should().Be(1);
            changingEvents.Count.Should().Be(2);
            changingEvents.GroupBy(s => s).Select(s => s.First()).Count().Should().Be(1);
            model.Dummy2.FakeProp.Should().Be(initialValue);
        }


        /// <summary>
        /// Defines the test method Should_not_refresh_property.
        /// </summary>
        [Fact]
        public void Should_not_refresh_property()
        {
            DISetup.SetupContainer();
            DISetup.Container.Register<FakeModel>();
            var model = DISetup.Container.GetInstance<FakeModel>();
            model.Dummy3 = new FakeLocalizableModelWithoutAttribute()
            {
                FakeProp = "1"
            };
            var changedEvents = new List<string>();
            var changingEvents = new List<string>();
            model.Dummy3.PropertyChanged += (s, e) =>
            {
                changedEvents.Add(e.PropertyName);
            };
            model.Dummy3.PropertyChanging += (s, e) =>
            {
                changingEvents.Add(e.PropertyName);
            };
            var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
            invocation.Setup(p => p.InvocationTarget).Returns(model);
            invocation.Setup(p => p.Proxy).Returns(model.Dummy3);
            var prop = model.GetType().GetProperty("Dummy3");
            var args = new LocalizationRefreshArgs(invocation.Object, prop);
            var handler = new LocalizationModelRefreshHandler();
            handler.Refresh(args);
            changedEvents.Count.Should().Be(0);
            changingEvents.Count.Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_refresh_readonly_property.
        /// </summary>
        [Fact]
        public void Should_not_refresh_readonly_property()
        {
            DISetup.SetupContainer();
            DISetup.Container.Register<FakeModel>();
            var model = DISetup.Container.GetInstance<FakeModel>();
            model.Dummy4 = new FakeLocalizableWithReadonlyProperty();
            var changedEvents = new List<string>();
            var changingEvents = new List<string>();
            model.Dummy4.PropertyChanged += (s, e) =>
            {
                changedEvents.Add(e.PropertyName);
            };
            model.Dummy4.PropertyChanging += (s, e) =>
            {
                changingEvents.Add(e.PropertyName);
            };
            var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
            invocation.Setup(p => p.InvocationTarget).Returns(model);
            invocation.Setup(p => p.Proxy).Returns(model.Dummy4);
            var prop = model.GetType().GetProperty("Dummy4");
            var args = new LocalizationRefreshArgs(invocation.Object, prop);
            var handler = new LocalizationModelRefreshHandler();
            handler.Refresh(args);
            changedEvents.Count.Should().Be(0);
            changingEvents.Count.Should().Be(0);
        }

        /// <summary>
        /// Class FakeModel.
        /// </summary>
        private class FakeModel
        {
            /// <summary>
            /// Gets or sets the dummy1.
            /// </summary>
            /// <value>The dummy1.</value>
            public string Dummy1 { get; set; }
            /// <summary>
            /// Gets or sets the dummy2.
            /// </summary>
            /// <value>The dummy2.</value>
            public FakeLocalizableModel Dummy2 { get; set; }

            /// <summary>
            /// Gets or sets the dummy3.
            /// </summary>
            /// <value>The dummy3.</value>
            public FakeLocalizableModelWithoutAttribute Dummy3 { get; set; }

            /// <summary>
            /// Gets or sets the dummy4.
            /// </summary>
            /// <value>The dummy4.</value>
            public FakeLocalizableWithReadonlyProperty Dummy4 { get; set; }
        }

        /// <summary>
        /// Class FakeLocalizableWithReadonlyProperty.
        /// Implements the <see cref="IronyModManager.Localization.ILocalizableModel" />
        /// </summary>
        /// <seealso cref="IronyModManager.Localization.ILocalizableModel" />
        private class FakeLocalizableWithReadonlyProperty : ILocalizableModel
        {
            /// <summary>
            /// Gets or sets the fake property.
            /// </summary>
            /// <value>The fake property.</value>
            [AutoRefreshLocalization]
            public string FakeProp { get; }

            /// <summary>
            /// Occurs when a property value is changing.
            /// </summary>
            public event PropertyChangingEventHandler PropertyChanging;
            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Called when [property changed].
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            public void OnPropertyChanged(string methodName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(methodName));
            }

            /// <summary>
            /// Called when [property changing].
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            public void OnPropertyChanging(string methodName)
            {
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(methodName));
            }
        }

        /// <summary>
        /// Class FakeLocalizableModelWithoutAttribute.
        /// Implements the <see cref="IronyModManager.Localization.ILocalizableModel" />
        /// </summary>
        /// <seealso cref="IronyModManager.Localization.ILocalizableModel" />
        private class FakeLocalizableModelWithoutAttribute : ILocalizableModel
        {
            /// <summary>
            /// Gets or sets the fake property.
            /// </summary>
            /// <value>The fake property.</value>
            public string FakeProp { get; set; }

            /// <summary>
            /// Occurs when a property value is changing.
            /// </summary>
            public event PropertyChangingEventHandler PropertyChanging;
            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Called when [property changed].
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            public void OnPropertyChanged(string methodName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(methodName));
            }

            /// <summary>
            /// Called when [property changing].
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            public void OnPropertyChanging(string methodName)
            {
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(methodName));
            }
        }

        /// <summary>
        /// Class FakeLocalizableModel.
        /// Implements the <see cref="IronyModManager.Localization.ILocalizableModel" />
        /// </summary>
        /// <seealso cref="IronyModManager.Localization.ILocalizableModel" />
        private class FakeLocalizableModel : ILocalizableModel
        {
            /// <summary>
            /// Gets or sets the fake property.
            /// </summary>
            /// <value>The fake property.</value>
            [AutoRefreshLocalization]
            public string FakeProp { get; set; }

            /// <summary>
            /// Occurs when a property value is changing.
            /// </summary>
            public event PropertyChangingEventHandler PropertyChanging;
            /// <summary>
            /// Occurs when a property value changes.
            /// </summary>
            public event PropertyChangedEventHandler PropertyChanged;

            /// <summary>
            /// Called when [property changed].
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            public void OnPropertyChanged(string methodName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(methodName));
            }

            /// <summary>
            /// Called when [property changing].
            /// </summary>
            /// <param name="methodName">Name of the method.</param>
            public void OnPropertyChanging(string methodName)
            {
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(methodName));
            }
        }
    }
}
