// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 12-03-2025
// ***********************************************************************
// <copyright file="GameServiceTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using AwesomeAssertions;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Storage;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable UnusedParameter.Local
// ReSharper disable PossibleNullReferenceException

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class GameServiceTests.
    /// </summary>
    public class GameServiceTests
    {
        /// <summary>
        /// Setups the mock case.
        /// </summary>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        private void SetupMockCase(Mock<IPreferencesService> preferencesService, Mock<IStorageProvider> storageProvider)
        {
            DISetup.SetupContainer();
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return new Preferences { Game = "game 1" };
            });
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns(true);
            var games = new List<IGameType>
            {
                new GameType { Name = "game 1", UserDirectory = "user1", SteamAppId = 1, WorkshopDirectory = new List<string> { "workshop1" } },
                new GameType
                {
                    Name = "game 2",
                    UserDirectory = "user2",
                    SteamAppId = 2,
                    WorkshopDirectory = new List<string> { "workshop2" },
                    ExecutableArgs = "args",
                    ExecutablePath = "exePath.exe",
                    BaseSteamGameDirectory = "exePath"
                },
                new GameType { Name = "game 3", UserDirectory = "user2", SteamAppId = 3 }
            };
            storageProvider.Setup(p => p.GetGames()).Returns(games);
            var gameSettings = new List<IGameSettings> { new GameSettings { Type = "game 1", LaunchArguments = "test", ExecutableLocation = "test.exe" } };
            storageProvider.Setup(p => p.GetGameSettings()).Returns(gameSettings);
        }

        /// <summary>
        /// Defines the test method Should_contain_all_games.
        /// </summary>
        [Fact]
        public void Should_contain_all_games()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.Count().Should().Be(3);
            result.GroupBy(p => p.Type).Select(p => p.First()).Count().Should().Be(3);
        }

        /// <summary>
        /// Defines the test method Should_contain_selected_game.
        /// </summary>
        [Fact]
        public void Should_contain_selected_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.FirstOrDefault(p => p.IsSelected).Should().NotBeNull();
            result.FirstOrDefault(p => p.IsSelected).Type.Should().Be("game 1");
            result.FirstOrDefault(p => p.IsSelected).UserDirectory.Should().Be("user1");
            result.FirstOrDefault(p => p.IsSelected).SteamAppId.Should().Be(1);
            result.FirstOrDefault(p => p.IsSelected).WorkshopDirectory.FirstOrDefault().Should().Be("workshop1");
        }

        /// <summary>
        /// Defines the test method Should_include_game_settings.
        /// </summary>
        [Fact]
        public void Should_include_game_settings()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.FirstOrDefault(p => p.IsSelected).Should().NotBeNull();
            result.FirstOrDefault(p => p.IsSelected).Type.Should().Be("game 1");
            result.FirstOrDefault(p => p.IsSelected).UserDirectory.Should().Be("user1");
            result.FirstOrDefault(p => p.IsSelected).SteamAppId.Should().Be(1);
            result.FirstOrDefault(p => p.IsSelected).WorkshopDirectory.FirstOrDefault().Should().Be("workshop1");
            result.FirstOrDefault(p => p.IsSelected).ExecutableLocation.Should().Be("test.exe");
            result.FirstOrDefault(p => p.IsSelected).LaunchArguments.Should().Be("test");
        }

        /// <summary>
        /// Defines the test method Should_return_selected_game.
        /// </summary>
        [Fact]
        public void Should_return_selected_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.GetSelected();
            result.Should().NotBeNull();
            result.Type.Should().Be("game 1");
            result.UserDirectory.Should().Be("user1");
            result.SteamAppId.Should().Be(1);
            result.WorkshopDirectory.FirstOrDefault().Should().Be("workshop1");
        }

        /// <summary>
        /// Defines the test method Should_save_selected_game.
        /// </summary>
        [Fact]
        public void Should_save_selected_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.Save(new Game { IsSelected = true, Type = "game 2" });
            result.Should().Be(true);
        }

        /// <summary>
        /// Defines the test method Should_save_game_settings.
        /// </summary>
        [Fact]
        public void Should_save_game_settings()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            IEnumerable<IGameSettings> gameSettings = null;
            storageProvider.Setup(p => p.SetGameSettings(It.IsAny<IEnumerable<IGameSettings>>())).Returns((IEnumerable<IGameSettings> p) =>
            {
                gameSettings = p;
                return true;
            });

            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.Save(new Game { IsSelected = true, Type = "game 2", LaunchArguments = "test2" });
            result.Should().Be(true);
            gameSettings.Count().Should().Be(2);
            gameSettings.FirstOrDefault(p => p.Type == "game 2").LaunchArguments.Should().Be("test2");
        }

        /// <summary>
        /// Defines the test method Should_throw_exception_when_saving_non_selected_game.
        /// </summary>
        [Fact]
        public void Should_throw_exception_when_saving_non_selected_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            Exception exception = null;
            try
            {
                service.Save(new Game { IsSelected = false, Type = "game 2" });
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            exception.GetType().Should().Be(typeof(InvalidOperationException));
        }

        /// <summary>
        /// Defines the test method Should_set_selected_game.
        /// </summary>
        [Fact]
        public void Should_set_selected_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var games = new List<IGame> { new Game { IsSelected = true, Type = "game 1" }, new Game { IsSelected = false, Type = "game 2" } };
            var result = service.SetSelected(games, new Game { IsSelected = true, Type = "game 2" });
            result.Should().Be(true);
        }

        /// <summary>
        /// Defines the test method Should_throw_validation_errors_when_setting_selected_game.
        /// </summary>
        [Fact]
        public void Should_throw_validation_errors_when_setting_selected_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var games = new List<IGame> { new Game { IsSelected = true, Type = "game 1" }, new Game { IsSelected = false, Type = "game 2" } };

            Exception exception = null;
            try
            {
                service.SetSelected(games, null);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            exception.GetType().Should().Be(typeof(ArgumentNullException));
            exception = null;
            try
            {
                service.SetSelected(null, new Game());
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            exception.GetType().Should().Be(typeof(ArgumentNullException));
            exception = null;

            try
            {
                service.SetSelected(new List<IGame>(), new Game());
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            exception.GetType().Should().Be(typeof(ArgumentNullException));
        }

        /// <summary>
        /// Defines the test method Should_not_set_selected_game.
        /// </summary>
        [Fact]
        public void Should_not_set_selected_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var themes = new List<IGame> { new Game { IsSelected = true, Type = "game 1" }, new Game { IsSelected = false, Type = "game 2" } };
            var result = service.SetSelected(themes, new Game { IsSelected = true, Type = "game 1" });
            result.Should().Be(false);
        }

        /// <summary>
        /// Defines the test method Should_return_steam_launch_command_only.
        /// </summary>
        [Fact]
        public void Should_return_steam_launch_command_only()
        {
            var game = new Game
            {
                SteamAppId = 1,
                IsSelected = true,
                Type = "game 1",
                WorkshopDirectory = new List<string> { "test" },
                ExecutableLocation = "steam://run/1"
            };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var args = service.GetLaunchSettings(game);
            args.ExecutableLocation.Should().Be("steam://run/1");
            args.LaunchArguments.Should().BeNullOrWhiteSpace();
        }

        /// <summary>
        /// Defines the test method Should_return_steam_launch_command.
        /// </summary>
        [Fact]
        public void Should_return_steam_launch_command()
        {
            var game = new Game
            {
                SteamAppId = 1,
                IsSelected = true,
                Type = "game 1",
                WorkshopDirectory = new List<string> { "test" },
                LaunchArguments = "test",
                ExecutableLocation = "steam://run/1"
            };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var args = service.GetLaunchSettings(game);
            args.ExecutableLocation.Should().Be("steam://run/1//test");
            args.LaunchArguments.Should().BeNullOrWhiteSpace();
        }

        /// <summary>
        /// Defines the test method Should_return_native_launch_command_only.
        /// </summary>
        [Fact]
        public void Should_return_native_launch_command_only()
        {
            var game = new Game
            {
                SteamAppId = 1,
                IsSelected = true,
                Type = "game 1",
                WorkshopDirectory = new List<string> { "test" },
                ExecutableLocation = "test.exe"
            };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var args = service.GetLaunchSettings(game);
            args.ExecutableLocation.Should().Be("test.exe");
            args.LaunchArguments.Should().BeNullOrWhiteSpace();
        }

        /// <summary>
        /// Defines the test method Should_return_native_launch_command.
        /// </summary>
        [Fact]
        public void Should_return_native_launch_command()
        {
            var game = new Game
            {
                SteamAppId = 1,
                IsSelected = true,
                Type = "game 1",
                WorkshopDirectory = new List<string> { "test" },
                LaunchArguments = "args",
                ExecutableLocation = "test.exe"
            };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var args = service.GetLaunchSettings(game);
            args.ExecutableLocation.Should().Be("test.exe");
            args.LaunchArguments.Should().Be("args");
        }

        /// <summary>
        /// Defines the test method Should_return_continue_game_args.
        /// </summary>
        [Fact]
        public void Should_return_continue_game_args()
        {
            var game = new Game
            {
                SteamAppId = 1,
                IsSelected = true,
                Type = "game 1",
                WorkshopDirectory = new List<string> { "test" },
                LaunchArguments = " -args",
                ExecutableLocation = "test.exe"
            };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var args = service.GetLaunchSettings(game, true);
            args.ExecutableLocation.Should().Be("test.exe");
            args.LaunchArguments.Should().Be("-args --continuelastsave");
        }

        /// <summary>
        /// Defines the test method Should_return_default_steam_location.
        /// </summary>
        [Fact]
        public void Should_return_default_steam_location()
        {
            var game = new Game
            {
                SteamAppId = 1,
                IsSelected = true,
                Type = "game 1",
                WorkshopDirectory = new List<string> { "test" },
                LaunchArguments = "args",
                ExecutableLocation = "test.exe",
                BaseSteamGameDirectory = @"c:\steam\stellaris"
            };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var args = service.GetDefaultGameSettings(game);
            args.ExecutableLocation.Should().Be("steam://run/1");
        }

        /// <summary>
        /// Defines the test method Should_return_default_user_dir_location.
        /// </summary>
        [Fact]
        public void Should_return_default_user_dir_location()
        {
            var game = new Game
            {
                SteamAppId = 1,
                IsSelected = true,
                Type = "game 1",
                UserDirectory = "user-dir",
                WorkshopDirectory = new List<string> { "test" },
                LaunchArguments = "args",
                ExecutableLocation = "test.exe"
            };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var args = service.GetDefaultGameSettings(game);
            args.UserDirectory.Should().Be("user-dir");
        }

        /// <summary>
        /// Defines the test method Should_return_empty_custom_mod_directory_location_as_default_setting.
        /// </summary>
        [Fact]
        public void Should_return_empty_custom_mod_directory_location_as_default_setting()
        {
            var game = new Game
            {
                SteamAppId = 1,
                IsSelected = true,
                Type = "game 1",
                UserDirectory = "user-dir",
                WorkshopDirectory = new List<string> { "test" },
                LaunchArguments = "args",
                ExecutableLocation = "test.exe"
            };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var args = service.GetDefaultGameSettings(game);
            args.CustomModDirectory.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Defines the test method Should_return_default_exe_location.
        /// </summary>
        [Fact]
        public void Should_return_default_exe_location()
        {
            var game = new Game
            {
                SteamAppId = 2,
                IsSelected = true,
                Type = "game 2",
                LaunchArguments = "args",
                ExecutableLocation = "test.exe"
            };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var args = service.GetDefaultGameSettings(game);
            args.ExecutableLocation.Should().Be("exePath.exe");
        }

        /// <summary>
        /// Defines the test method Should_return_stored_user_dir_location.
        /// </summary>
        [Fact]
        public void Should_return_stored_user_dir_location()
        {
            var game = new Game
            {
                SteamAppId = 2,
                IsSelected = true,
                Type = "game 2",
                LaunchArguments = "args",
                ExecutableLocation = "test.exe"
            };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var args = service.GetDefaultGameSettings(game);
            args.UserDirectory.Should().Be("user2");
        }

        /// <summary>
        /// Defines the test method Should_return_empty_default_exe_location.
        /// </summary>
        [Fact]
        public void Should_return_empty_default_exe_location()
        {
            var game = new Game
            {
                SteamAppId = 3,
                IsSelected = true,
                Type = "game 3",
                LaunchArguments = "args",
                ExecutableLocation = "test.exe"
            };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var args = service.GetDefaultGameSettings(game);
            args.ExecutableLocation.Should().BeNullOrWhiteSpace();
        }

        /// <summary>
        /// Defines the test method Should_be_steam_launch_path.
        /// </summary>
        [Fact]
        public void Should_be_steam_launch_path()
        {
            var gameSettings = new GameSettings { ExecutableLocation = "steam://run/1" };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.IsSteamLaunchPath(gameSettings);
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_be_steam_launch_path.
        /// </summary>
        [Fact]
        public void Should_not_be_steam_launch_path()
        {
            var gameSettings = new GameSettings { ExecutableLocation = "test.exe" };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.IsSteamLaunchPath(gameSettings);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_be_steam_game.
        /// </summary>
        [Fact]
        public void Should_not_be_steam_game()
        {
            var gameSettings = new GameSettings { ExecutableLocation = "test.exe", Type = "game 2" };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return new Preferences { Game = "game 2" };
            });
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.IsSteamGame(gameSettings);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_be_steam_game.
        /// </summary>
        [Fact]
        public void Should_be_steam_game()
        {
            var gameSettings = new GameSettings { ExecutableLocation = "exePath.exe", Type = "game 2" };
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return new Preferences { Game = "game 2" };
            });
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.IsSteamGame(gameSettings);
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_allow_continue_gae.
        /// </summary>
        [Fact]
        public void Should_not_allow_continue_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var service = new GameService(null, null, new Mock<IReader>().Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.IsContinueGameAllowed(new Game { Type = "game 2", UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) });
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_allow_continue_gae.
        /// </summary>
        [Fact]
        public void Should_allow_continue_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var fileInfos = new List<IFileInfo> { new IO.FileInfo { Content = new List<string> { "{ \"title\":	\"save games/save.sav\" }" }, FileName = "continue_game.json", IsBinary = false } };
            var reader = new Mock<IReader>();
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos);
            var service = new GameService(null, null, reader.Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.IsContinueGameAllowed(new Game
            {
                Type = "game 2",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)),
                RemoteSteamUserDirectory = new List<string> { Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)) }
            });
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_return_json_settings_data.
        /// </summary>
        [Fact]
        public void Should_not_return_json_settings_data()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var reader = new Mock<IReader>();
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns((IEnumerable<IFileInfo>)null);
            var service = new GameService(null, null, reader.Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.GetGameSettingsFromJson(new Game { Type = "game 2", UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)), LauncherSettingsFileName = "launcher.json" }, "test");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_return_json_settings_data.
        /// </summary>
        [Fact]
        public void Should_return_json_settings_data()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var fileInfos = new List<IFileInfo>
            {
                new IO.FileInfo
                {
                    Content = new List<string> { "{     \"exeArgs\": [        \"-gdpr-compliant\"    ],    \"exePath\": \"stellaris.exe\",    \"gameDataPath\": \"test\" }" }, FileName = "continue_game.json", IsBinary = false
                }
            };
            var reader = new Mock<IReader>();
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos);
            var service = new GameService(null, null, reader.Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.GetGameSettingsFromJson(new Game { Type = "game 2", UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)), LauncherSettingsFileName = "launcher.json" }, "test");
            result.UserDirectory.Should().Be("test");
            result.ExecutableLocation.Should().Be("test\\stellaris.exe");
            result.LaunchArguments.Should().Be("-gdpr-compliant");
            result.CustomModDirectory.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Defines the test method Should_not_return_version.
        /// </summary>
        [Fact]
        public void Should_not_return_version()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var reader = new Mock<IReader>();
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns((IEnumerable<IFileInfo>)null);
            var service = new GameService(null, null, reader.Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.GetVersion(new Game { Type = "game 2", UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)), LauncherSettingsFileName = "launcher.json" });
            result.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Defines the test method Should_return_version.
        /// </summary>
        [Fact]
        public void Should_return_version()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var fileInfos = new List<IFileInfo>
            {
                new IO.FileInfo
                {
                    Content = new List<string> { "{    \"exeArgs\": [        \"-gdpr-compliant\"    ],    \"exePath\": \"stellaris.exe\",    \"gameDataPath\": \"test\",    \"rawVersion\": \"3.0.3\"}" },
                    FileName = "continue_game.json",
                    IsBinary = false
                }
            };
            var reader = new Mock<IReader>();
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos);
            var service = new GameService(null, null, reader.Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.GetVersion(new Game
            {
                Type = "game 2", UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)), LauncherSettingsFileName = "launcher.json", ExecutableLocation = "c:\\fake"
            });
            result.Should().Be("3.0.3");
        }

        /// <summary>
        /// Defines the test method Should_not_return_versions.
        /// </summary>
        [Fact]
        public void Should_not_return_versions()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var reader = new Mock<IReader>();
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns((IEnumerable<IFileInfo>)null);
            var service = new GameService(null, null, reader.Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.GetVersions(new Game { Type = "game 2", UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)), LauncherSettingsFileName = "launcher.json" });
            result.Should().NotBeNull();
            result.Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_return_versions.
        /// </summary>
        [Fact]
        public void Should_return_versions()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);
            var fileInfos = new List<IFileInfo>
            {
                new IO.FileInfo
                {
                    Content = new List<string> { "{    \"exeArgs\": [        \"-gdpr-compliant\"    ],    \"exePath\": \"stellaris.exe\",    \"gameDataPath\": \"test\",    \"rawVersion\": \"3.0.3\"}" },
                    FileName = "continue_game.json",
                    IsBinary = false
                }
            };
            var reader = new Mock<IReader>();
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos);
            var service = new GameService(null, null, reader.Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.GetVersions(new Game
            {
                Type = "game 2", UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)), LauncherSettingsFileName = "launcher.json", ExecutableLocation = "c:\\fake"
            });
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.FirstOrDefault().Should().Be("3.0.3");
        }

        /// <summary>
        /// Defines the test method Should_not_import_hash.
        /// </summary>
        [Fact]
        public async Task Should_not_import_hash()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var hashExport = new Mock<IReportExportService>();
            var preferencesService = new Mock<IPreferencesService>();
            hashExport.Setup(p => p.GetGameReports(It.IsAny<IReadOnlyCollection<IHashReport>>())).Returns((IReadOnlyCollection<IHashReport> hashReports) =>
            {
                return hashReports?.Where(p => p.ReportType == HashReportType.Game).ToList();
            });
            hashExport.Setup(p => p.CompareReports(It.IsAny<IReadOnlyCollection<IHashReport>>(), It.IsAny<IReadOnlyCollection<IHashReport>>())).Returns((IReadOnlyCollection<IHashReport> first, IReadOnlyCollection<IHashReport> second) =>
            {
                return null;
            });
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var reader = new Mock<IReader>();
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string> { "test\\test" });
            DISetup.SetupContainer();

            var service = new GameService(messageBus.Object, hashExport.Object, reader.Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = await service.ImportHashReportAsync(new Game { GameFolders = new List<string> { "test" } }, null);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_import_hash.
        /// </summary>
        [Fact]
        public async Task Should_import_hash()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var hashExport = new Mock<IReportExportService>();
            var preferencesService = new Mock<IPreferencesService>();
            var innerReports = new List<IHashFileReport> { new HashFileReport { File = "test\\1", Hash = "2" } };
            var outerReports = new List<IHashReport> { new HashReport { Name = "test", Reports = innerReports, ReportType = HashReportType.Game } };
            hashExport.Setup(p => p.GetGameReports(It.IsAny<IReadOnlyCollection<IHashReport>>())).Returns((IReadOnlyCollection<IHashReport> hashReports) =>
            {
                if (hashReports != null)
                {
                    return hashReports.Where(p => p.ReportType == HashReportType.Game).ToList();
                }

                return null;
            });
            hashExport.Setup(p => p.CompareReports(It.IsAny<IReadOnlyCollection<IHashReport>>(), It.IsAny<IReadOnlyCollection<IHashReport>>())).Returns((IReadOnlyCollection<IHashReport> first, IReadOnlyCollection<IHashReport> second) =>
            {
                return outerReports;
            });
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var reader = new Mock<IReader>();
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new IO.FileInfo { FileName = "2", ContentSHA = "3" });
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string> { "test\\test" });
            DISetup.SetupContainer();

            var service = new GameService(messageBus.Object, hashExport.Object, reader.Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = await service.ImportHashReportAsync(new Game { GameFolders = new List<string> { "test" }, ExecutableLocation = AppDomain.CurrentDomain.BaseDirectory + "\\test.exe" }, outerReports);
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.FirstOrDefault().Reports.Count.Should().Be(1);
        }

        /// <summary>
        /// Defines the test method Should_not_export_hash_report.
        /// </summary>
        [Fact]
        public async Task Should_not_export_hash_report()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var hashExport = new Mock<IReportExportService>();
            var preferencesService = new Mock<IPreferencesService>();
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var reader = new Mock<IReader>();
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string> { "test\\test" });
            DISetup.SetupContainer();

            var service = new GameService(messageBus.Object, hashExport.Object, reader.Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);

            var result = await service.ExportHashReportAsync(new Game { GameFolders = new List<string> { "test" } }, string.Empty);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_export_hash_report.
        /// </summary>
        [Fact]
        public async Task Should_export_hash_report()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var hashExport = new Mock<IReportExportService>();
            hashExport.Setup(p => p.GetGameReports(It.IsAny<IReadOnlyCollection<IHashReport>>())).Returns((IReadOnlyCollection<IHashReport> hashReports) =>
            {
                if (hashReports != null)
                {
                    return hashReports.Where(p => p.ReportType == HashReportType.Game).ToList();
                }

                return null;
            });
            var preferencesService = new Mock<IPreferencesService>();
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var reader = new Mock<IReader>();
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new IO.FileInfo { ContentSHA = "2" });
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string> { "test\\test" });
            hashExport.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync((IEnumerable<IHashReport> report, string path) =>
            {
                if (report.Count() == 1 && report.FirstOrDefault().Reports.Count == 1 && report.FirstOrDefault().Reports.FirstOrDefault().File == "test\\test" && report.FirstOrDefault().Reports.FirstOrDefault().Hash == "2")
                {
                    return true;
                }

                return false;
            });
            DISetup.SetupContainer();

            var service = new GameService(messageBus.Object, hashExport.Object, reader.Object, storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);

            var result = await service.ExportHashReportAsync(new Game { GameFolders = new List<string> { "test" }, ExecutableLocation = AppDomain.CurrentDomain.BaseDirectory + "\\test.exe" }, "test");
            result.Should().BeTrue();
        }
    }
}
