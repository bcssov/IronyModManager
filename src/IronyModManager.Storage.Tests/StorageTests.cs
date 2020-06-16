// ***********************************************************************
// Assembly         : IronyModManager.Storage.Tests
// Author           : Mario
// Created          : 01-28-2020
//
// Last Modified By : Mario
// Last Modified On : 06-16-2020
// ***********************************************************************
// <copyright file="StorageTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;
using IronyModManager.Models;
using AutoMapper;
using IronyModManager.Models.Common;
using FluentAssertions;
using SimpleInjector;
using IronyModManager.DI;
using IronyModManager.Shared;
using IronyModManager.Tests.Common;
using System.Linq;
using IronyModManager.Storage.Common;

namespace IronyModManager.Storage.Tests
{
    /// <summary>
    /// Class StorageTests.
    /// </summary>
    public class StorageTests
    {

        /// <summary>
        /// Defines the test method Should_return_same_preferences_object.
        /// </summary>
        [Fact]
        public void Should_return_same_preferences_object()
        {
            // I know totally redundant test, done just for a bit of practice
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IPreferences, IPreferences>(It.IsAny<IPreferences>())).Returns(dbMock.Preferences);
            var storage = new Storage(dbMock, mapper.Object);
            var pref = storage.GetPreferences();
            pref.Locale.Should().Be(GetDbMock().Preferences.Locale);
        }

        /// <summary>
        /// Defines the test method Should_return_same_app_state_object.
        /// </summary>
        [Fact]
        public void Should_return_same_app_state_object()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IAppState, IAppState>(It.IsAny<IAppState>())).Returns(dbMock.AppState);
            var storage = new Storage(dbMock, mapper.Object);
            var state = storage.GetAppState();
            state.CollectionModsSearchTerm.Should().Be(GetDbMock().AppState.CollectionModsSearchTerm);
        }


        /// <summary>
        /// Defines the test method Should_return_same_window_state_object.
        /// </summary>
        [Fact]
        public void Should_return_same_window_state_object()
        {
            // I know totally redundant test, done just for a bit of practice
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IWindowState, IWindowState>(It.IsAny<IWindowState>())).Returns(dbMock.WindowState);
            var storage = new Storage(dbMock, mapper.Object);
            var state = storage.GetWindowState();
            state.IsMaximized.Should().Be(GetDbMock().WindowState.IsMaximized);
        }

        /// <summary>
        /// Defines the test method Should_return_same_themes_object.
        /// </summary>
        [Fact]
        public void Should_return_same_themes_object()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<List<IThemeType>>(It.IsAny<IEnumerable<IThemeType>>())).Returns(() =>
            {
                return dbMock.Themes.ToList();
            });
            var storage = new Storage(dbMock, mapper.Object);
            var themes = storage.GetThemes();
            themes.Count().Should().Be(1);
            themes.FirstOrDefault().Name.Should().Be("test");
        }

        /// <summary>
        /// Defines the test method Should_return_same_mod_collection_object.
        /// </summary>
        [Fact]
        public void Should_return_same_mod_collection_object()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<List<IModCollection>>(It.IsAny<IEnumerable<IModCollection>>())).Returns(() =>
            {
                return dbMock.ModCollection.ToList();
            });
            var storage = new Storage(dbMock, mapper.Object);
            var result = storage.GetModCollections();
            result.Count().Should().Be(1);
            result.FirstOrDefault().Name.Should().Be("fake");
        }

        /// <summary>
        /// Defines the test method Should_return_same_game_settings_object.
        /// </summary>
        [Fact]
        public void Should_return_same_game_settings_object()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<List<IGameSettings>>(It.IsAny<IEnumerable<IGameSettings>>())).Returns(() =>
            {
                return dbMock.GameSettings.ToList();
            });
            var storage = new Storage(dbMock, mapper.Object);
            var result = storage.GetGameSettings();
            result.Count().Should().Be(1);
            result.FirstOrDefault().Type.Should().Be("fake");
        }

        /// <summary>
        /// Defines the test method Should_return_same_games_object.
        /// </summary>
        [Fact]
        public void Should_return_same_games_object()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<List<IGameType>>(It.IsAny<IEnumerable<IGameType>>())).Returns(() =>
            {
                return dbMock.Games.ToList();
            });
            var storage = new Storage(dbMock, mapper.Object);
            var result = storage.GetGames();
            result.Count().Should().Be(1);
            result.FirstOrDefault().Name.Should().Be("test");
        }

        /// <summary>
        /// Defines the test method Should_overwrite_preferences_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_preferences_object()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var newPref = new Preferences()
            {
                Locale = "test2"
            };
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IPreferences>(It.IsAny<IPreferences>())).Returns((IPreferences s) =>
            {
                return new Preferences()
                {
                    Locale = s.Locale
                };
            });
            var storage = new Storage(dbMock, mapper.Object);
            storage.SetPreferences(newPref);
            dbMock.Preferences.Locale.Should().Be(newPref.Locale);
        }

        /// <summary>
        /// Defines the test method Should_overwrite_app_state_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_app_state_object()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var state = new AppState()
            {
                CollectionModsSearchTerm = "test2"
            };
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IAppState>(It.IsAny<IAppState>())).Returns((IAppState s) =>
            {
                return new AppState()
                {
                    CollectionModsSearchTerm = s.CollectionModsSearchTerm
                };
            });
            var storage = new Storage(dbMock, mapper.Object);
            var result = storage.SetAppState(state);
            dbMock.AppState.CollectionModsSearchTerm.Should().Be(state.CollectionModsSearchTerm);
        }


        /// <summary>
        /// Defines the test method Should_add_new_theme.
        /// </summary>
        [Fact]
        public void Should_add_new_theme()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var newThemeKey = "test2";
            var newThemeUris = new List<string>() { "4", "5" };
            var storage = new Storage(dbMock, new Mock<IMapper>().Object);
            storage.RegisterTheme(newThemeKey, newThemeUris);
            dbMock.Themes.Count.Should().Be(2);
            dbMock.Themes.FirstOrDefault(p => p.Name == newThemeKey).Should().NotBeNull();
            dbMock.Themes.FirstOrDefault(p => p.Name == newThemeKey).Styles.First().Should().Be(newThemeUris.First());
            dbMock.Themes.FirstOrDefault(p => p.Name == newThemeKey).Styles.Last().Should().Be(newThemeUris.Last());
        }


        /// <summary>
        /// Defines the test method Should_add_new_game.
        /// </summary>
        [Fact]
        public void Should_add_new_game()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var key = "test2";
            var storage = new Storage(dbMock, new Mock<IMapper>().Object);
            storage.RegisterGame(key, 1, "user_directory", "workshop1", "test.log", new List<string>() { "test" });
            dbMock.Games.Count.Should().Be(2);
            dbMock.Games.FirstOrDefault(p => p.Name == key).Should().NotBeNull();
            dbMock.Games.FirstOrDefault(p => p.Name == key).UserDirectory.Should().Be("user_directory");
            dbMock.Games.FirstOrDefault(p => p.Name == key).SteamAppId.Should().Be(1);
            dbMock.Games.FirstOrDefault(p => p.Name == key).WorkshopDirectory.Should().Be("workshop1");
            dbMock.Games.FirstOrDefault(p => p.Name == key).LogLocation.Should().Be("test.log");
            dbMock.Games.FirstOrDefault(p => p.Name == key).ChecksumFolders.FirstOrDefault().Should().Be("test");
        }

        /// <summary>
        /// Defines the test method Should_overwrite_window_state_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_window_state_object()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var state = new WindowState()
            {
                Height = 300
            };
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IWindowState>(It.IsAny<IWindowState>())).Returns((IWindowState s) =>
            {
                return new WindowState()
                {
                    Height = s.Height
                };
            });
            var storage = new Storage(dbMock, mapper.Object);
            storage.SetWindowState(state);
            dbMock.WindowState.Height.Should().Be(state.Height);
        }

        /// <summary>
        /// Defines the test method Should_overwrite_modcollection_objects.
        /// </summary>
        [Fact]
        public void Should_overwrite_modcollection_objects()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var col = new List<IModCollection>()
            {
                new ModCollection()
                {
                    Name = "fake2"
                }
            };
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IEnumerable<IModCollection>>(It.IsAny<IEnumerable<IModCollection>>())).Returns((IEnumerable<IModCollection> s) =>
            {
                return s;
            });
            var storage = new Storage(dbMock, mapper.Object);
            storage.SetModCollections(col);
            dbMock.ModCollection.Count().Should().Be(1);
            dbMock.ModCollection.First().Name.Should().Be(col.First().Name);
        }

        /// <summary>
        /// Defines the test method Should_overwrite_and_return_same_preferences_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_and_return_same_preferences_object()
        {
            DISetup.SetupContainer();
            var newPref = new Preferences()
            {
                Locale = "test2"
            };
            var storage = new Storage(GetDbMock(), DIResolver.Get<IMapper>());
            storage.SetPreferences(newPref);
            var pref = storage.GetPreferences();
            pref.Locale.Should().Be(newPref.Locale);
        }

        /// <summary>
        /// Defines the test method Should_overwrite_and_return_same_app_state_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_and_return_same_app_state_object()
        {
            DISetup.SetupContainer();
            var newState = new AppState()
            {
                CollectionModsSearchTerm = "test2"
            };
            var storage = new Storage(GetDbMock(), DIResolver.Get<IMapper>());
            storage.SetAppState(newState);
            var state = storage.GetAppState();
            state.CollectionModsSearchTerm.Should().Be(newState.CollectionModsSearchTerm);
        }

        /// <summary>
        /// Defines the test method Should_overwrite_and_return_same_mod_collection_objects.
        /// </summary>
        [Fact]
        public void Should_overwrite_and_return_same_mod_collection_objects()
        {
            DISetup.SetupContainer();
            var col = new List<IModCollection>()
            {
                new ModCollection()
                {
                    Name = "fake2"
                }
            };
            var storage = new Storage(GetDbMock(), DIResolver.Get<IMapper>());
            storage.SetModCollections(col);
            var result = storage.GetModCollections();
            result.Count().Should().Be(1);
            result.First().Name.Should().Be(col.First().Name);
        }

        /// <summary>
        /// Defines the test method Should_overwrite_and_return_same_game_settings_objects.
        /// </summary>
        [Fact]
        public void Should_overwrite_and_return_same_game_settings_objects()
        {
            DISetup.SetupContainer();
            var col = new List<IGameSettings>()
            {
                new GameSettings()
                {
                    Type = "fake2"
                }
            };
            var storage = new Storage(GetDbMock(), DIResolver.Get<IMapper>());
            storage.SetGameSettings(col);
            var result = storage.GetGameSettings();
            result.Count().Should().Be(1);
            result.First().Type.Should().Be(col.First().Type);
        }

        /// <summary>
        /// Defines the test method Should_add_and_return_added_theme.
        /// </summary>
        [Fact]
        public void Should_add_and_return_added_theme()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var newThemeKey = "test2";
            var newThemeUris = new List<string>() { "4", "5" };
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<List<IThemeType>>(It.IsAny<IEnumerable<IThemeType>>())).Returns(() =>
            {
                return dbMock.Themes.ToList();
            });
            var storage = new Storage(dbMock, mapper.Object);
            storage.RegisterTheme(newThemeKey, newThemeUris);
            var themes = storage.GetThemes();
            themes.Count().Should().Be(2);
            themes.FirstOrDefault(p => p.Name == newThemeKey).Should().NotBeNull();
            themes.FirstOrDefault(p => p.Name == newThemeKey).Styles.First().Should().Be(newThemeUris.First());
            themes.FirstOrDefault(p => p.Name == newThemeKey).Styles.Last().Should().Be(newThemeUris.Last());
        }

        /// <summary>
        /// Defines the test method Should_add_and_return_added_game.
        /// </summary>
        [Fact]
        public void Should_add_and_return_added_game()
        {
            DISetup.SetupContainer();
            var dbMock = GetDbMock();
            var key = "test2";
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<List<IGameType>>(It.IsAny<IEnumerable<IGameType>>())).Returns(() =>
            {
                return dbMock.Games.ToList();
            });
            var storage = new Storage(dbMock, mapper.Object);
            storage.RegisterGame(key, 1, "user_directory", "workshop1", "test.log", new List<string>() { "test" });
            var result = storage.GetGames();
            result.Count().Should().Be(2);
            result.FirstOrDefault(p => p.Name == key).Should().NotBeNull();
            result.FirstOrDefault(p => p.Name == key).UserDirectory.Should().Be("user_directory");
            result.FirstOrDefault(p => p.Name == key).SteamAppId.Should().Be(1);
            result.FirstOrDefault(p => p.Name == key).WorkshopDirectory.Should().Be("workshop1");
            result.FirstOrDefault(p => p.Name == key).LogLocation.Should().Be("test.log");
            result.FirstOrDefault(p => p.Name == key).ChecksumFolders.FirstOrDefault().Should().Be("test");
        }

        /// <summary>
        /// Defines the test method Should_overwrite_and_return_same_window_state_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_and_return_same_window_state_object()
        {
            DISetup.SetupContainer();
            var newState = new WindowState()
            {
                Height = 300
            };
            var storage = new Storage(GetDbMock(), DIResolver.Get<IMapper>());
            storage.SetWindowState(newState);
            var state = storage.GetWindowState();
            state.Height.Should().Be(newState.Height);

        }

        /// <summary>
        /// Gets the database mock.
        /// </summary>
        /// <returns>Database.</returns>
        private Database GetDbMock()
        {
            return new Database()
            {
                Preferences = new Preferences() { Locale = "test" },
                WindowState = new WindowState() { IsMaximized = true },
                Themes = new List<IThemeType>() { new ThemeType()
                {
                    Name = "test",
                    IsDefault = true,
                    Styles = new List<string> { "1", "2" }
                } },
                Games = new List<IGameType>()
                {
                    new GameType()
                    {
                        Name = "test"
                    }
                },
                AppState = new AppState()
                {
                    CollectionModsSearchTerm = "test"
                },
                ModCollection = new List<IModCollection>()
                {
                    new ModCollection()
                    {
                        Name = "fake"
                    }
                },
                GameSettings = new List<IGameSettings>()
                {
                    new GameSettings()
                    {
                        Type = "fake"
                    }
                }
            };
        }
    }
}
