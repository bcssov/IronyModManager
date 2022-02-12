// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 01-28-2022
// ***********************************************************************
// <copyright file="StellarisValidation.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class DetectDuplicates.
    /// </summary>
    public class StellarisValidation : ValidationBase
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StellarisValidation" /> class.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public StellarisValidation(ITestOutputHelper writer) : base(writer, @"D:\Games\Steam\steamapps\common\Stellaris\", "Stellaris")
        {
        }

        #endregion Constructors

        #region Methods

#if !FUNCTIONAL_TEST


        [Fact(Skip = "Test is for detection of parser issues.", Timeout = 300000)]
#else

        [Fact(Timeout = 300000)]
#endif
        /// <summary>
        /// Defines the test method Detect.
        /// </summary>
        /// <exception cref="ArgumentException">Fatal error. Check parsers.</exception>
        public void StellarisDetectDuplicatesAndGenerateParserMap()
        {
            DetectDuplicatesAndGenerateParserMap();
        }

#if !FUNCTIONAL_TEST

        [Fact(Skip = "Test is for detection of parser issues.", Timeout = 300000)]
#else

        [Fact(Timeout = 300000)]
#endif
        /// <summary>
        /// Defines the test method StellarisExtensions.
        /// </summary>
        public void StellarisExtensions()
        {
            Extensions();
        }

        #endregion Methods
    }
}
