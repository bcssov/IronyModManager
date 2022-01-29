// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 01-29-2022
//
// Last Modified By : Mario
// Last Modified On : 01-29-2022
// ***********************************************************************
// <copyright file="HOI4DefaultParser.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class HOI4KeyValuePairParserTests.
    /// </summary>
    public class HOI4DefaultParser
    {
        #region Methods

        /// <summary>
        /// Defines the test method CanParse_should_be_false_then_true.
        /// </summary>
        [Fact]
        public void CanParse_should_be_false_then_true()
        {
            var args = new CanParseArgs()
            {
                File = "common\\country\\test.txt",
                GameType = "HeartsofIronIV"
            };
            var parser = new Games.HOI4.DefaultParser(new CodeParser(new Logger()), null);
            parser.CanParse(args).Should().BeFalse();
            args.File = "common\\ai_strategy_plans\\test.txt";
            parser.CanParse(args).Should().BeTrue();
        }

        #endregion Methods
    }
}
