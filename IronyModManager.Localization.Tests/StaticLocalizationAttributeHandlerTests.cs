// ***********************************************************************
// Assembly         : IronyModManager.Localization.Tests
// Author           : Mario
// Created          : 02-04-2020
//
// Last Modified By : Mario
// Last Modified On : 02-04-2020
// ***********************************************************************
// <copyright file="StaticLocalizationAttributeHandlerTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using IronyModManager.Localization.Attributes;
using IronyModManager.Localization.Attributes.Handlers;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;
namespace IronyModManager.Localization.Tests
{
    /// <summary>
    /// Class StaticLocalizationAttributeHandlerTests.
    /// </summary>
    public class StaticLocalizationAttributeHandlerTests
    {
        /// <summary>
        /// Defines the test method CanProcess_should_be_true.
        /// </summary>
        [Fact]
        public void CanProcess_should_be_true()
        {
            var handler = new StaticLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new StaticLocalizationAttribute("test"), null, null, null);
            handler.CanProcess(args).Should().Be(true);
        }

        /// <summary>
        /// Defines the test method CanProcess_should_be_false.
        /// </summary>
        [Fact]
        public void CanProcess_should_be_false()
        {
            var handler = new DynamicLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new AutoRefreshLocalizationAttribute(), null, null, null);
            handler.CanProcess(args).Should().Be(false);
        }

        /// <summary>
        /// Defines the test method Data_should_not_be_empty.
        /// </summary>
        [Fact]
        public void Data_should_not_be_empty()
        {
            DISetup.SetupContainer();
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s == "test"))).Returns("test localization");
            DISetup.Container.RegisterInstance(locManager.Object);
            var handler = new StaticLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new StaticLocalizationAttribute("test"), null, null, null);
            handler.GetData(args).Should().Be("test localization");
        }


        /// <summary>
        /// Defines the test method Data_should_be_empty.
        /// </summary>
        [Fact]
        public void Data_should_be_empty()
        {
            DISetup.SetupContainer();
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s == "test"))).Returns("test localization");
            DISetup.Container.RegisterInstance(locManager.Object);
            var handler = new StaticLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new StaticLocalizationAttribute("test2"), null, null, null);
            handler.GetData(args).Should().BeNull();
        }

        /// <summary>
        /// Defines the test method HasData_should_be_true.
        /// </summary>
        [Fact]
        public void HasData_should_be_true()
        {
            var handler = new StaticLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new StaticLocalizationAttribute("test"), null, null, null);
            handler.HasData(args).Should().Be(true);
        }
    }
}
