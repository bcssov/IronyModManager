// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 04-25-2020
//
// Last Modified By : Mario
// Last Modified On : 04-27-2020
// ***********************************************************************
// <copyright file="DefinitionModConverterTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using IronyModManager.Converters;
using IronyModManager.Parser.Common.Definitions;
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
    public class DefinitionModConverterTests
    {
        /// <summary>
        /// Defines the test method Class_should_be_patch_mod.
        /// </summary>
        [Fact]
        public void Class_should_be_patch_mod()
        {
            DISetup.SetupContainer();
            var converter = new DefinitionModConverter();
            var service = new Mock<IModService>();
            service.Setup(p => p.IsPatchMod(It.IsAny<string>())).Returns(true);
            DISetup.Container.RegisterInstance(service.Object);
            var def = new Definition() { ModName = "IronyModManager_fake" };
            var result = converter.Convert(new List<object>() { new List<IDefinition>() { def }, def }, null, null, null);
            result.Should().Be("PatchMod");
        }

        /// <summary>
        /// Defines the test method Class_should_be_copied_definition.
        /// </summary>
        [Fact]
        public void Class_should_be_copied_definition()
        {
            DISetup.SetupContainer();
            var converter = new DefinitionModConverter();
            var service = new Mock<IModService>();
            service.Setup(p => p.IsPatchMod(It.IsAny<string>())).Returns((string p) =>
            {
                if (p == "IronyModManager_fake3")
                {
                    return true;
                }
                return false;
            });
            DISetup.Container.RegisterInstance(service.Object);
            var def = new Definition() { ModName = "IronyModManager_fake1", File = "test1.txt" };
            var def2 = new Definition() { ModName = "IronyModManager_fake2", File = "test1.txt" };
            var def3 = new Definition() { ModName = "IronyModManager_fake3", File = "test.txt" };
            service.Setup(p => p.EvalDefinitionPriority(It.IsAny<IEnumerable<IDefinition>>())).Returns(def);
            var result = converter.Convert(new List<object>() { new List<IDefinition>() { def, def2, def3 }, def }, null, null, null);
            result.Should().Be("CopiedDefinition");
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
            var converter = new DefinitionModConverter();
            var def = new Definition() { ModName = "IronyModManager_fake" };
            var result = converter.Convert(new List<object>() { new List<IDefinition>() { def }, def }, null, null, null);
            result.ToString().Should().BeNullOrEmpty();
        }
    }
}
