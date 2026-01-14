// ***********************************************************************
// Assembly         : 
// Author           : Mario
// Created          : 02-25-2024
//
// Last Modified By : Mario
// Last Modified On : 02-25-2024
// ***********************************************************************
// <copyright file="GameLanguageServiceTests.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
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
    /// The game language service tests.
    /// </summary>
    public class GameLanguageServiceTests
    {
        /// <summary>
        /// Setup mocks.
        /// </summary>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="languages">A list of strings</param>
        private static void SetupMocks(Mock<IPreferencesService> preferencesService, bool csSet, params string[] languages)
        {
            DISetup.SetupContainer();
            CurrentLocale.SetCurrent("en");
            preferencesService.Setup(p => p.Get()).Returns(() => new Preferences { ConflictSolverLanguages = [.. languages], ConflictSolverLanguagesSet = csSet});
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns(true);
        }

        /// <summary>
        /// Shoulds a return default to all language.
        /// </summary>
        [Fact]
        public void Should_return_default_to_all_language()
        {
            // So we use a magic string -- because lazy ensure that tests cover it properly
            var preferencesService = new Mock<IPreferencesService>();
            SetupMocks(preferencesService, false, "l_english");
            var service = new GameLanguageService(new Mock<IStorageProvider>().Object, null, preferencesService.Object);
            var result = service.Get();
            result.All(p => p.IsSelected).Should().BeTrue();
        }

        /// <summary>
        /// Shoulds a return valid selection.
        /// </summary>
        [Fact]
        public void Should_return_valid_selection()
        {
            var preferencesService = new Mock<IPreferencesService>();
            SetupMocks(preferencesService, true, "l_english");
            var service = new GameLanguageService(new Mock<IStorageProvider>().Object, null, preferencesService.Object);
            var result = service.Get();
            result.Count(p => p.IsSelected).Should().Be(1);
            result.FirstOrDefault(p => p.IsSelected)!.Type.Should().Be("l_english");
        }

        /// <summary>
        /// Defines the test method Should_return_only_selected.
        /// </summary>
        [Fact]
        public void Should_return_only_selected()
        {
            var preferencesService = new Mock<IPreferencesService>();
            SetupMocks(preferencesService, true,"l_english");
            var service = new GameLanguageService(new Mock<IStorageProvider>().Object, null, preferencesService.Object);
            var result = service.GetSelected();
            result.Count.Should().Be(1);
            result.FirstOrDefault(p => p.IsSelected)!.Type.Should().Be("l_english");
        }

        /// <summary>
        /// Shoulds a return only requested.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public void Should_return_only_requested()
        {
            var preferencesService = new Mock<IPreferencesService>();
            SetupMocks(preferencesService, true, "l_english");
            var service = new GameLanguageService(new Mock<IStorageProvider>().Object, null, preferencesService.Object);
            var result = service.GetByAbrv(["l_german"]);
            result.Count.Should().Be(1);
            result.FirstOrDefault(p => p.IsSelected)!.Type.Should().Be("l_german");
        }

        /// <summary>
        /// Shoulds a save selection.
        /// </summary>
        [Fact]
        public void Should_save_selection()
        {
            var preferencesService = new Mock<IPreferencesService>();
            SetupMocks(preferencesService, false,"l_german");
            IPreferences prefs = null;
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns((IPreferences saved) =>
            {
                prefs = saved;
                return true;
            });
            var service = new GameLanguageService(new Mock<IStorageProvider>().Object, null, preferencesService.Object);
            var langs = service.Get();
            foreach (var lang in langs)
            {
                lang.IsSelected = lang.Type == "l_english";
            }

            service.Save(langs);

            prefs.Should().NotBeNull();
            prefs.ConflictSolverLanguages.Count.Should().Be(1);
            prefs.ConflictSolverLanguages.FirstOrDefault().Should().Be("l_english");
            prefs.ConflictSolverLanguagesSet.Should().BeTrue();
        }
    }
}
