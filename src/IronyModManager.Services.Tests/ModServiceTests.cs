// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 05-26-2020
// ***********************************************************************
// <copyright file="ModServiceTests.cs" company="Mario">
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
using FluentAssertions;
using IronyModManager.IO.Common;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.IO.Mods.Models;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Parser;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Definitions;
using IronyModManager.Parser.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;
using FileInfo = IronyModManager.IO.FileInfo;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class ModServiceTests.
    /// </summary>
    public class ModServiceTests
    {
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="gameService">The game service.</param>
        /// <returns>ModService.</returns>
        private ModService GetService(Mock<IStorageProvider> storageProvider, Mock<IModParser> modParser,
             Mock<IReader> reader, Mock<IMapper> mapper, Mock<IModWriter> modWriter,
            Mock<IGameService> gameService)
        {
            return new ModService(reader.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
        }

        /// <summary>
        /// Setups the mock case.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="modParser">The mod parser.</param>
        private void SetupMockCase(Mock<IReader> reader, Mock<IModParser> modParser)
        {
            var fileInfos = new List<IFileInfo>()
            {
                new FileInfo()
                {
                    Content = new List<string>() { "1" },
                    FileName = "fake1.txt",
                    IsBinary = false
                },
                new FileInfo()
                {
                    Content = new List<string>() { "2" } ,
                    FileName = "fake2.txt",
                    IsBinary = false
                }
            };
            reader.Setup(s => s.Read(It.IsAny<string>())).Returns(fileInfos);

            modParser.Setup(s => s.Parse(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> values) =>
            {
                return new ModObject()
                {
                    FileName = values.First(),
                    Name = values.First()
                };
            });
        }
        /// <summary>
        /// Defines the test method Should_return_installed_mods.
        /// </summary>
        [Fact]
        public void Should_return_installed_mods()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var mapper = new Mock<IMapper>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName
                };
            });

            SetupMockCase(reader, modParser);

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = service.GetInstalledMods(new Game() { UserDirectory = "fake1", WorkshopDirectory = "fake2" });
            result.Count().Should().Be(2);
            result.First().FileName.Should().Be("1");
            result.Last().FileName.Should().Be("2");
        }

        /// <summary>
        /// Defines the test method Should_throw_exception_when_no_game_specified_when_fetching_installed_mods.
        /// </summary>
        [Fact]
        public void Should_throw_exception_when_no_game_specified_when_fetching_installed_mods()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            try
            {
                service.GetInstalledMods(null);
            }
            catch (Exception ex)
            {
                ex.GetType().Should().Be(typeof(ArgumentNullException));
            }
        }


        /// <summary>
        /// Defines the test method Should_return_steam_url.
        /// </summary>
        [Fact]
        public void Should_return_steam_url()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var url = service.BuildModUrl(new Mod()
            {
                RemoteId = 1,
                Source = ModSource.Steam
            });
            url.Should().Be("https://steamcommunity.com/sharedfiles/filedetails/?id=1");
        }

        /// <summary>
        /// Defines the test method Should_return_paradox_url.
        /// </summary>
        [Fact]
        public void Should_return_paradox_url()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var url = service.BuildModUrl(new Mod()
            {
                RemoteId = 1,
                Source = ModSource.Paradox
            });
            url.Should().Be("https://mods.paradoxplaza.com/mods/1/Any");
        }

        /// <summary>
        /// Defines the test method Should_return_empty_url.
        /// </summary>
        [Fact]
        public void Should_return_empty_url()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var url = service.BuildModUrl(new Mod()
            {
                RemoteId = null,
                Source = ModSource.Local
            });
            url.Should().BeNullOrEmpty();
        }


        /// <summary>
        /// Defines the test method Should_return_steam_protocol_url.
        /// </summary>
        [Fact]
        public void Should_return_steam_protocol_url()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var url = service.BuildSteamUrl(new Mod()
            {
                RemoteId = 1,
                Source = ModSource.Steam
            });
            url.Should().Be("steam://openurl/https://steamcommunity.com/sharedfiles/filedetails/?id=1");
        }

        /// <summary>
        /// Defines the test method Should_not_return_steam_protocol_url.
        /// </summary>
        [Fact]
        public void Should_not_return_steam_protocol_url()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var url = service.BuildSteamUrl(new Mod()
            {
                RemoteId = 1,
                Source = ModSource.Paradox
            });
            url.Should().BeNullOrWhiteSpace();
        }

        /// <summary>
        /// Defines the test method Should_export_mods.
        /// </summary>
        [Fact]
        public async Task Should_export_mods()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName
                };
            });
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "test",
                UserDirectory = "C:\\users\\fake"
            });
            modWriter.Setup(p => p.DescriptorExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.ApplyModsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.ExportModsAsync(new List<IMod> { new Mod() { DescriptorFile = "mod/fake.mod" } }, new List<IMod> { new Mod() { DescriptorFile = "mod/fake.mod" } }, "fake");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_export_mods_when_no_selected_game.
        /// </summary>
        [Fact]
        public async Task Should_not_export_mods_when_no_selected_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName
                };
            });
            gameService.Setup(s => s.GetSelected()).Returns(() =>
            {
                return null;
            });
            modWriter.Setup(p => p.ApplyModsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.DescriptorExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.ExportModsAsync(new List<IMod> { new Mod() }, new List<IMod> { new Mod() }, "fake");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_export_mods_when_collection_null_or_empty.
        /// </summary>
        [Fact]
        public async Task Should_not_export_mods_when_collection_null()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName
                };
            });
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "test"
            });
            modWriter.Setup(p => p.ApplyModsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.DescriptorExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.ExportModsAsync(null, null, "fake");
            result.Should().BeFalse();
        }



        /// <summary>
        /// Defines the test method Should_not_install_mods_when_no_game.
        /// </summary>
        [Fact]
        public async Task Should_not_install_mods_when_no_game()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var result = await service.InstallModsAsync();
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_install_mods.
        /// </summary>
        [Fact]
        public async Task Should_not_install_mods()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            SetupMockCase(reader, modParser);

            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Fake",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = "C:\\workshop"
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName
                };
            });

            var result = await service.InstallModsAsync();
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_install_mods.
        /// </summary>
        [Fact]
        public async Task Should_install_mods()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            SetupMockCase(reader, modParser);

            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Fake",
                UserDirectory = AppDomain.CurrentDomain.BaseDirectory,
                WorkshopDirectory = "C:\\workshop"
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName
                };
            });
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>())).Returns(Task.FromResult(true));
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns((string root, string path) =>
             {
                 var sb = new System.Text.StringBuilder(115);
                 sb.AppendLine(@"path=""c:/fake""");
                 sb.AppendLine(@"name=""Fake""");
                 sb.AppendLine(@"picture=""thumbnail.png""");
                 sb.AppendLine(@"tags={");
                 sb.AppendLine(@"	""Gameplay""");
                 sb.AppendLine(@"	""Fixes""");
                 sb.AppendLine(@"}");
                 sb.AppendLine(@"supported_version=""2.6.*""");

                 return new FileInfo()
                 {
                     Content = sb.ToString().SplitOnNewLine(),
                     ContentSHA = "test",
                     FileName = "fake.mod",
                     IsBinary = false
                 };
             });

            var result = await service.InstallModsAsync();
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_lock_descriptors_when_no_game.
        /// </summary>
        [Fact]
        public async Task Should_not_lock_descriptors_when_no_game()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var result = await service.LockDescriptorsAsync(new List<IMod>(), true);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_lock_descriptors_when_no_mods.
        /// </summary>
        [Fact]
        public async Task Should_not_lock_descriptors_when_no_mods()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Fake",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = "C:\\workshop"
            });
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var result = await service.LockDescriptorsAsync(new List<IMod>(), true);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_lock_descriptors.
        /// </summary>
        [Fact]
        public async Task Should_lock_descriptors()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Fake",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = "C:\\workshop"
            });
            modWriter.Setup(p => p.SetDescriptorLockAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var result = await service.LockDescriptorsAsync(new List<IMod>() { new Mod() }, true);
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_delete_descriptors_when_no_game.
        /// </summary>
        [Fact]
        public async Task Should_not_delete_descriptors_when_no_game()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var result = await service.DeleteDescriptorsAsync(new List<IMod>());
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_delete_descriptors_when_no_mods.
        /// </summary>
        [Fact]
        public async Task Should_not_delete_descriptors_when_no_mods()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Fake",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = "C:\\workshop"
            });
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var result = await service.DeleteDescriptorsAsync(new List<IMod>());
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_delete_descriptors.
        /// </summary>
        [Fact]
        public async Task Should_delete_descriptors()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Fake",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = "C:\\workshop"
            });
            modWriter.Setup(p => p.DeleteDescriptorAsync(It.IsAny<ModWriterParameters>())).Returns(Task.FromResult(true));
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var result = await service.DeleteDescriptorsAsync(new List<IMod>() { new Mod() });
            result.Should().BeTrue();
        }
    }
}
