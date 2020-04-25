// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 04-25-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
// ***********************************************************************
// <copyright file="PatchModConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using IronyModManager.Converters;
using IronyModManager.Parser.Definitions;
using IronyModManager.Services.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class PatchModConverterTests.
    /// </summary>
    public class PatchModConverterTests
    {
        /// <summary>
        /// Defines the test method Class_should_be_patch_mod.
        /// </summary>
        [Fact]
        public void Class_should_be_patch_mod()
        {
            DISetup.SetupContainer();
            var converter = new PatchModConverter();
            var service = new Mock<IModService>();
            service.Setup(p => p.IsPatchMod(It.IsAny<string>())).Returns(true);
            DISetup.Container.RegisterInstance(service.Object);
            var result = converter.Convert(new Definition() { ModName = "IronyModManager_fake" }, null, null, null);
            result.Should().Be("PatchMod");
        }

        /// <summary>
        /// Defines the test method Class_should_be_empty.
        /// </summary>
        [Fact]
        public void Class_should_be_empty()
        {
            DISetup.SetupContainer();
            var service = new Mock<IModService>();
            service.Setup(p => p.IsPatchMod(It.IsAny<string>())).Returns(false);
            DISetup.Container.RegisterInstance(service.Object);
            var converter = new PatchModConverter();
            var result = converter.Convert(new Definition() { ModName = "fake" }, null, null, null);
            result.ToString().Should().BeNullOrEmpty();
        }
    }
}
