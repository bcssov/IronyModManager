// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 02-06-2020
//
// Last Modified By : Mario
// Last Modified On : 02-06-2020
// ***********************************************************************
// <copyright file="ViewLocatorTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using FluentAssertions;
using IronyModManager.Common;
using IronyModManager.Common.ViewModels;
using IronyModManager.Common.Views;
using IronyModManager.Models;
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class ViewLocatorTests.
    /// </summary>
    public class ViewLocatorTests
    {
        /// <summary>
        /// Defines the test method SupportsRecycling_should_be_false.
        /// </summary>
        [Fact]
        public void SupportsRecycling_should_be_false()
        {
            var locator = new ViewLocator();
            locator.SupportsRecycling.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Match_should_be_false.
        /// </summary>
        [Fact]
        public void Match_should_be_false()
        {
            var locator = new ViewLocator();
            locator.Match(1).Should().BeFalse();
            locator.Match(new Theme()).Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Match_should_be_true.
        /// </summary>
        [Fact]
        public void Match_should_be_true()
        {
            var locator = new ViewLocator();
            locator.Match(new FakeVM()).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_resolve_user_control.
        /// </summary>
        [Fact]
        public void Should_resolve_user_control()
        {
            DISetup.SetupContainer();
            var resolver = new Mock<IViewResolver>();
            resolver.Setup(p => p.FormatUserControlName(It.IsAny<object>())).Returns(string.Empty);
            resolver.Setup(p => p.IsControl(It.IsAny<string>())).Returns(true);
            resolver.Setup(p => p.ResolveUserControl(It.IsAny<object>())).Returns(new FakeControl());
            DISetup.Container.RegisterInstance(resolver.Object);
            var locator = new ViewLocator();
            var result = locator.Build(new FakeVM());
            result.GetType().Should().Be(typeof(FakeControl));
        }

        /// <summary>
        /// Defines the test method Should_not_support_user_control.
        /// </summary>
        [Fact]
        public void Should_not_support_user_control()
        {
            DISetup.SetupContainer();
            var resolver = new Mock<IViewResolver>();
            resolver.Setup(p => p.FormatUserControlName(It.IsAny<object>())).Returns(string.Empty);
            resolver.Setup(p => p.IsControl(It.IsAny<string>())).Returns(false);
            DISetup.Container.RegisterInstance(resolver.Object);
            var logger = new Mock<ILogger>();
            logger.Setup(p => p.Info(It.IsAny<string>()));
            logger.Setup(p => p.Error(It.IsAny<Exception>(), It.IsAny<string>()));
            DISetup.Container.RegisterInstance(logger.Object);

            var locator = new ViewLocator();
            var result = locator.Build(new FakeVM());
            result.GetType().Should().Be(typeof(TextBlock));
            ((TextBlock)result).Text.Should().Contain("Not Supported");
        }

        /// <summary>
        /// Defines the test method Should_not_resolve_user_control.
        /// </summary>
        [Fact]
        public void Should_not_resolve_user_control()
        {
            DISetup.SetupContainer();
            var resolver = new Mock<IViewResolver>();
            resolver.Setup(p => p.FormatUserControlName(It.IsAny<object>())).Returns(string.Empty);
            resolver.Setup(p => p.IsControl(It.IsAny<string>())).Returns(true);
            resolver.Setup(p => p.ResolveUserControl(It.IsAny<object>())).Returns(() =>
             {
                 throw new Exception("Fake");
             });
            DISetup.Container.RegisterInstance(resolver.Object);
            var logger = new Mock<ILogger>();
            logger.Setup(p => p.Info(It.IsAny<string>()));
            logger.Setup(p => p.Error(It.IsAny<Exception>(), It.IsAny<string>()));
            DISetup.Container.RegisterInstance(logger.Object);

            var locator = new ViewLocator();
            var result = locator.Build(new FakeVM());
            result.GetType().Should().Be(typeof(TextBlock));
            ((TextBlock)result).Text.Should().Contain("Not Found");
        }

        /// <summary>
        /// Class FakeVM.
        /// Implements the <see cref="IronyModManager.Common.ViewModels.BaseViewModel" />
        /// </summary>
        /// <seealso cref="IronyModManager.Common.ViewModels.BaseViewModel" />
        private class FakeVM : BaseViewModel
        {

        }

        /// <summary>
        /// Class FakeControl.
        /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.Tests.ViewLocatorTests.FakeVM}" />
        /// </summary>
        /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.Tests.ViewLocatorTests.FakeVM}" />
        private class FakeControl : BaseControl<FakeVM>
        {

        }
    }
}
