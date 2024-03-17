// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 01-28-2022
//
// Last Modified By : Mario
// Last Modified On : 03-15-2024
// ***********************************************************************
// <copyright file="HOI4Validation.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class HOI4Validation.
    /// Implements the <see cref="IronyModManager.Parser.Tests.ValidationBase" />
    /// </summary>
    /// <seealso cref="IronyModManager.Parser.Tests.ValidationBase" />
    public class HOI4Validation : ValidationBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StellarisValidation" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public HOI4Validation(ITestOutputHelper writer) : base(writer, @"D:\Games\Steam\steamapps\common\Hearts of Iron IV\\", "HeartsofIronIV")
        {
        }

        #endregion Constructors

#if !FUNCTIONAL_TEST
        [Fact(Skip = "Test is for detection of parser issues.", Timeout = 300000)]
#else


        [Fact(Timeout = 300000)]
#endif
        /// <summary>
        /// Defines the test method Detect.
        /// </summary>
        /// <exception cref="ArgumentException">Fatal error. Check parsers.</exception>
        public async Task HOI4DetectDuplicatesAndGenerateParserMap()
        {
            await DetectDuplicatesAndGenerateParserMap();
        }

#if !FUNCTIONAL_TEST
        [Fact(Skip = "Test is for detection of parser issues.", Timeout = 300000)]
#else

        [Fact(Timeout = 300000)]
#endif
        /// <summary>
        /// Defines the test method HOI4Extensions.
        /// </summary>
        /// <returns>Task.</returns>
        public Task HOI4Extensions()
        {
            Extensions();
            return Task.CompletedTask;
        }

    }
}
