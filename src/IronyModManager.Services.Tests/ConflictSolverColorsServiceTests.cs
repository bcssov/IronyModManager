// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 03-18-2024
//
// Last Modified By : Mario
// Last Modified On : 03-18-2024
// ***********************************************************************
// <copyright file="ConflictSolverColorsServiceTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AwesomeAssertions;
using IronyModManager.Localization;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class ConflictSolverColorsServiceTests.
    /// </summary>
    public class ConflictSolverColorsServiceTests
    {
        /// <summary>
        /// Setups the mocks.
        /// </summary>
        /// <param name="emptyColors">if set to <c>true</c> [empty colors].</param>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="mapper">The mapper.</param>
        private static void SetupMocks(bool emptyColors, Mock<IPreferencesService> preferencesService, Mock<IMapper> mapper)
        {
            DISetup.SetupContainer();
            CurrentLocale.SetCurrent("en");
            if (emptyColors)
            {
                preferencesService.Setup(p => p.Get()).Returns(() =>
                    new Preferences());
            }
            else
            {
                preferencesService.Setup(p => p.Get()).Returns(() =>
                    new Preferences { ConflictSolverDeletedLineColor = "test1", ConflictSolverImaginaryLineColor = "test2", ConflictSolverInsertedLineColor = "test3", ConflictSolverModifiedLineColor = "test4" });
            }

            mapper.Setup(s => s.Map<IPreferences>(It.IsAny<IConflictSolverColors>())).Returns((IConflictSolverColors o) => new Preferences
            {
                ConflictSolverImaginaryLineColor = o.ConflictSolverImaginaryLineColor,
                ConflictSolverDeletedLineColor = o.ConflictSolverDeletedLineColor,
                ConflictSolverInsertedLineColor = o.ConflictSolverInsertedLineColor,
                ConflictSolverModifiedLineColor = o.ConflictSolverModifiedLineColor
            });
            mapper.Setup(s => s.Map<IConflictSolverColors>(It.IsAny<IPreferences>())).Returns((IPreferences o) => new ConflictSolverColors()
            {
                ConflictSolverImaginaryLineColor = o.ConflictSolverImaginaryLineColor,
                ConflictSolverDeletedLineColor = o.ConflictSolverDeletedLineColor,
                ConflictSolverInsertedLineColor = o.ConflictSolverInsertedLineColor,
                ConflictSolverModifiedLineColor = o.ConflictSolverModifiedLineColor
            });
            mapper.Setup(s => s.Map(It.IsAny<IConflictSolverColors>(), It.IsAny<IPreferences>())).Returns((IConflictSolverColors s, IPreferences o) => new Preferences()
            {
                ConflictSolverImaginaryLineColor = s.ConflictSolverImaginaryLineColor,
                ConflictSolverDeletedLineColor = s.ConflictSolverDeletedLineColor,
                ConflictSolverInsertedLineColor = s.ConflictSolverInsertedLineColor,
                ConflictSolverModifiedLineColor = s.ConflictSolverModifiedLineColor
            });
        }

        /// <summary>
        /// Defines the test method Will_get_empty_colors.
        /// </summary>
        [Fact]
        public void Will_get_empty_colors()
        {
            var preferencesService = new Mock<IPreferencesService>();
            var mapper = new Mock<IMapper>();
            SetupMocks(true, preferencesService, mapper);
            var service = new ConflictSolverColorsService(preferencesService.Object, new Mock<IStorageProvider>().Object, mapper.Object);
            var result = service.Get();
            result.ConflictSolverImaginaryLineColor.Should().BeNullOrWhiteSpace();
            result.ConflictSolverDeletedLineColor.Should().BeNullOrWhiteSpace();
            result.ConflictSolverInsertedLineColor.Should().BeNullOrWhiteSpace();
            result.ConflictSolverModifiedLineColor.Should().BeNullOrWhiteSpace();
        }

        /// <summary>
        /// Defines the test method Will_get_colors.
        /// </summary>
        [Fact]
        public void Will_get_colors()
        {
            var preferencesService = new Mock<IPreferencesService>();
            var mapper = new Mock<IMapper>();
            SetupMocks(false, preferencesService, mapper);
            var service = new ConflictSolverColorsService(preferencesService.Object, new Mock<IStorageProvider>().Object, mapper.Object);
            var result = service.Get();
            result.ConflictSolverImaginaryLineColor.Should().Be("test2");
            result.ConflictSolverDeletedLineColor.Should().Be("test1");
            result.ConflictSolverInsertedLineColor.Should().Be("test3");
            result.ConflictSolverModifiedLineColor.Should().Be("test4");
        }

        /// <summary>
        /// Defines the test method Color_should_be_valid.
        /// </summary>
        [Fact]
        public void Color_should_be_valid()
        {
            var service = new ConflictSolverColorsService(null, null, null);
            service.HasAny(new ConflictSolverColors { ConflictSolverDeletedLineColor = "test" }).Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Color_should_not_be_valid.
        /// </summary>
        [Fact]
        public void Color_should_not_be_valid()
        {
            var service = new ConflictSolverColorsService(null, null, null);
            service.HasAny(new ConflictSolverColors()).Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Color_should_not_be_valid_due_to_null_ref.
        /// </summary>
        [Fact]
        public void Color_should_not_be_valid_due_to_null_ref()
        {
            var service = new ConflictSolverColorsService(null, null, null);
            service.HasAny(null).Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Colors_should_be_saved.
        /// </summary>
        [Fact]
        public void Colors_should_be_saved()
        {
            var preferencesService = new Mock<IPreferencesService>();
            var mapper = new Mock<IMapper>();
            SetupMocks(false, preferencesService, mapper);
            IPreferences prefs = null;
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns((IPreferences saved) =>
            {
                prefs = saved;
                return true;
            });
            var service = new ConflictSolverColorsService(preferencesService.Object, new Mock<IStorageProvider>().Object, mapper.Object);
            var result = service.Get();
            result.ConflictSolverDeletedLineColor = "test";

            service.Save(result);

            prefs.Should().NotBeNull();
            prefs.ConflictSolverDeletedLineColor.Should().Be("test");
        }
    }
}
