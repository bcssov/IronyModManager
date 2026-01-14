// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 06-15-2020
//
// Last Modified By : Mario
// Last Modified On : 09-21-2020
// ***********************************************************************
// <copyright file="DefinitionFileTooltipConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using AwesomeAssertions;
using IronyModManager.Converters;
using IronyModManager.Parser.Definitions;
using IronyModManager.Services.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Tests
{
    /// <summary>
    /// Class BoolAndConverterTests.
    /// </summary>
    public class DefinitionFileTooltipConverterTests
    {
        /// <summary>
        /// Defines the test method Should_be_false.
        /// </summary>
        [Fact]
        public void Should_not_be_null()
        {
            DISetup.SetupContainer();
            var converter = new DefinitionFileTooltipConverter();
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.IsPatchMod(It.IsAny<string>())).Returns(false);
            DISetup.Container.RegisterInstance(service.Object);
            var result = converter.Convert(new Definition() {  ModName = "test", File = "test" }, null, null, null);
            result.ToString().Should().Be("test");            
        }

        /// <summary>
        /// Shoulds the be true.
        /// </summary>
        [Fact] 
        public void Should_be_null()
        {
            DISetup.SetupContainer();
            var converter = new DefinitionFileTooltipConverter();
            var service = new Mock<IModPatchCollectionService>();
            service.Setup(p => p.IsPatchMod(It.IsAny<string>())).Returns(true);
            DISetup.Container.RegisterInstance(service.Object);
            var result = converter.Convert(new Definition() { ModName = "test", File = "test" }, null, null, null);
            result.Should().BeNull();
        }
    }
}
