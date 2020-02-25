// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-12-2020
//
// Last Modified By : Mario
// Last Modified On : 02-12-2020
// ***********************************************************************
// <copyright file="GameServiceTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using FluentAssertions;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

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
                return new Preferences()
                {
                    Game = "game 1"
                };
            });
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns(true);
            var games = new List<IGameType>
            {
                new GameType()
                {
                    Name = "game 1",
                    UserDirectory = "user1",
                    SteamAppId = 1,
                    WorkshopDirectory = "workshop1"
                },
                new GameType()
                {
                    Name = "game 2",
                    UserDirectory = "user2",
                    SteamAppId = 2,
                    WorkshopDirectory = "workshop2"
                }
            };
            storageProvider.Setup(p => p.GetGames()).Returns(games);
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

            var service = new GameService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.Count().Should().Be(2);
            result.GroupBy(p => p.Type).Select(p => p.First()).Count().Should().Be(2);
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

            var service = new GameService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.FirstOrDefault(p => p.IsSelected).Should().NotBeNull();
            result.FirstOrDefault(p => p.IsSelected).Type.Should().Be("game 1");
            result.FirstOrDefault(p => p.IsSelected).UserDirectory.Should().Be("user1");
            result.FirstOrDefault(p => p.IsSelected).SteamAppId.Should().Be(1);
            result.FirstOrDefault(p => p.IsSelected).WorkshopDirectory.Should().Be("workshop1");
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

            var service = new GameService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.GetSelected();
            result.Should().NotBeNull();
            result.Type.Should().Be("game 1");
            result.UserDirectory.Should().Be("user1");
            result.SteamAppId.Should().Be(1);
            result.WorkshopDirectory.Should().Be("workshop1");
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

            var service = new GameService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.Save(new Game()
            {
                IsSelected = true,
                Type = "game 2"
            });
            result.Should().Be(true);
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

            var service = new GameService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            try
            {
                service.Save(new Game()
                {
                    IsSelected = false,
                    Type = "game 2"
                });
            }
            catch (Exception ex)
            {
                ex.GetType().Should().Be(typeof(InvalidOperationException));
            }
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

            var service = new GameService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var games = new List<IGame>
            {
                new Game()
                {
                    IsSelected = true,
                    Type = "game 1"
                },
                new Game()
                {
                    IsSelected = false,
                    Type = "game 2"
                }
            };
            var result = service.SetSelected(games, new Game()
            {
                IsSelected = true,
                Type = "game 2"
            });
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

            var service = new GameService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var games = new List<IGame>
            {
                new Game()
                {
                    IsSelected = true,
                    Type = "game 1"
                },
                new Game()
                {
                    IsSelected = false,
                    Type = "game 2"
                }
            };

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

            var service = new GameService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var themes = new List<IGame>
            {
                new Game()
                {
                    IsSelected = true,
                    Type = "game 1"
                },
                new Game()
                {
                    IsSelected = false,
                    Type = "game 2"
                }
            };
            var result = service.SetSelected(themes, new Game()
            {
                IsSelected = true,
                Type = "game 1"
            });
            result.Should().Be(false);
        }
    }
}
