// ***********************************************************************
// Assembly         : IronyModManager.Localization.Tests
// Author           : Mario
// Created          : 02-04-2020
//
// Last Modified By : Mario
// Last Modified On : 02-04-2020
// ***********************************************************************
// <copyright file="AutoRefreshLocalizationAttributeHandlerTests.cs" company="Mario">
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
using Xunit;

namespace IronyModManager.Localization.Tests
{
    /// <summary>
    /// Class AutoRefreshLocalizationAttributeHandlerTests.
    /// </summary>
    public class AutoRefreshLocalizationAttributeHandlerTests
    {
        /// <summary>
        /// Defines the test method CanProcess_should_be_true.
        /// </summary>
        [Fact]
        public void CanProcess_should_be_true()
        {
            var handler = new AutoRefreshLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new AutoRefreshLocalizationAttribute(), null, null, null);
            handler.CanProcess(args).Should().Be(true);
        }

        /// <summary>
        /// Defines the test method CanProcess_should_be_false.
        /// </summary>
        [Fact]
        public void CanProcess_should_be_false()
        {
            var handler = new AutoRefreshLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new StaticLocalizationAttribute("test"), null, null, null);
            handler.CanProcess(args).Should().Be(false);
        }

        /// <summary>
        /// Defines the test method Data_should_be_empty.
        /// </summary>
        [Fact]
        public void Data_should_be_empty()
        {
            var handler = new AutoRefreshLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new AutoRefreshLocalizationAttribute(), null, null, null);
            handler.GetData(args).Should().Be(string.Empty);
        }

        /// <summary>
        /// Defines the test method HasData_should_be_true.
        /// </summary>
        [Fact]
        public void HasData_should_be_true()
        {
            var handler = new AutoRefreshLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new AutoRefreshLocalizationAttribute(), null, null, null);
            handler.HasData(args).Should().Be(true);
        }
    }
}
