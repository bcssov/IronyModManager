// ***********************************************************************
// Assembly         : IronyModManager.Localization.Tests
// Author           : Mario
// Created          : 02-04-2020
//
// Last Modified By : Mario
// Last Modified On : 02-04-2020
// ***********************************************************************
// <copyright file="DynamicLocalizationAttributeHandlerTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
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
    /// Class DynamicLocalizationAttributeHandlerTests.
    /// </summary>
    public class DynamicLocalizationAttributeHandlerTests
    {
        /// <summary>
        /// Defines the test method CanProcess_should_be_true.
        /// </summary>
        [Fact]
        public void CanProcess_should_be_true()
        {
            var handler = new DynamicLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new DynamicLocalizationAttribute("test", "test"), null, null, null);
            handler.CanProcess(args).Should().Be(true);
        }

        /// <summary>
        /// Defines the test method CanProcess_should_be_false.
        /// </summary>
        [Fact]
        public void CanProcess_should_be_false()
        {
            var handler = new DynamicLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new StaticLocalizationAttribute("test"), null, null, null);
            handler.CanProcess(args).Should().Be(false);
        }


        /// <summary>
        /// Defines the test method Data_should_be_empty.
        /// </summary>
        [Fact]
        public void Data_without_prefix_should_not_be_empty()
        {
            DISetup.SetupContainer();
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s == "test"))).Returns("test localization");
            DISetup.Container.RegisterInstance(locManager.Object);
            var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
            invocation.Setup(p => p.InvocationTarget).Returns(new FakeModel()
            {
                DependendProperty = "test"
            });
            var args = new AttributeHandlersArgs(new DynamicLocalizationAttribute("DependendProperty"), invocation.Object, null, null);
            var handler = new DynamicLocalizationAttributeHandler();
            handler.GetData(args).Should().Be("test localization");
        }


        /// <summary>
        /// Defines the test method Data_without_prefix_should_be_empty.
        /// </summary>
        [Fact]
        public void Data_without_prefix_should_be_empty()
        {
            DISetup.SetupContainer();
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s == "test"))).Returns("test localization");
            DISetup.Container.RegisterInstance(locManager.Object);
            var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
            invocation.Setup(p => p.InvocationTarget).Returns(new FakeModel()
            {
                DependendProperty = "test2"
            });
            var args = new AttributeHandlersArgs(new DynamicLocalizationAttribute("DependendProperty"), invocation.Object, null, null);
            var handler = new DynamicLocalizationAttributeHandler();
            handler.GetData(args).Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Data_with_prefix_should_not_be_empty.
        /// </summary>
        [Fact]
        public void Data_with_prefix_should_not_be_empty()
        {
            DISetup.SetupContainer();
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s == "prefix.test"))).Returns("test localization");
            DISetup.Container.RegisterInstance(locManager.Object);
            var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
            invocation.Setup(p => p.InvocationTarget).Returns(new FakeModel()
            {
                DependendProperty = "test"
            });
            var args = new AttributeHandlersArgs(new DynamicLocalizationAttribute("prefix.", "DependendProperty"), invocation.Object, null, null);
            var handler = new DynamicLocalizationAttributeHandler();
            handler.GetData(args).Should().Be("test localization");
        }

        /// <summary>
        /// Defines the test method Data_with_prefix_should_be_empty.
        /// </summary>
        [Fact]
        public void Data_with_prefix_should_be_empty()
        {
            DISetup.SetupContainer();
            var locManager = new Mock<ILocalizationManager>();
            locManager.Setup(s => s.GetResource(It.Is<string>(s => s == "prefix.test"))).Returns("test localization");
            DISetup.Container.RegisterInstance(locManager.Object);
            var invocation = new Mock<Castle.DynamicProxy.IInvocation>();
            invocation.Setup(p => p.InvocationTarget).Returns(new FakeModel()
            {
                DependendProperty = "test2"
            });
            var args = new AttributeHandlersArgs(new DynamicLocalizationAttribute("prefix.", "DependendProperty"), invocation.Object, null, null);
            var handler = new DynamicLocalizationAttributeHandler();
            handler.GetData(args).Should().BeNull();
        }

        /// <summary>
        /// Defines the test method HasData_should_be_true.
        /// </summary>
        [Fact]
        public void HasData_should_be_true()
        {
            var handler = new DynamicLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new DynamicLocalizationAttribute("test"), null, null, 1);
            handler.HasData(args).Should().Be(true);
        }

        /// <summary>
        /// Defines the test method HasData_should_be_true.
        /// </summary>
        [Fact]
        public void HasData_should_not_be_true()
        {
            var handler = new DynamicLocalizationAttributeHandler();
            var args = new AttributeHandlersArgs(new DynamicLocalizationAttribute("test"), null, null, null);
            handler.HasData(args).Should().Be(false);
        }

        /// <summary>
        /// Class FakeModel.
        /// </summary>
        private class FakeModel
        {
            /// <summary>
            /// Gets or sets the fake property.
            /// </summary>
            /// <value>The fake property.</value>
            public string FakeProp { get; set; }

            /// <summary>
            /// Gets or sets the dependend property.
            /// </summary>
            /// <value>The dependend property.</value>
            public string DependendProperty { get; set; }
        }
    }
}
