// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 08-12-2022
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
using IronyModManager.Localization;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Parser;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Common.Mod.Search;
using IronyModManager.Parser.Definitions;
using IronyModManager.Parser.Mod;
using IronyModManager.Parser.Mod.Search;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.Models;
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
        /// <param name="parser">The parser.</param>
        /// <param name="languageService">The language service.</param>
        /// <returns>ModService.</returns>
        private static ModService GetService(Mock<IStorageProvider> storageProvider, Mock<IModParser> modParser,
             Mock<IReader> reader, Mock<IMapper> mapper, Mock<IModWriter> modWriter,
            Mock<IGameService> gameService, Mock<IParser> parser = null, Mock<ILanguagesService> languageService = null)
        {
            return new ModService(languageService?.Object, parser?.Object, null, new Cache(), null, reader.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
        }

        /// <summary>
        /// Setups the mock case.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="modParser">The mod parser.</param>
        private static void SetupMockCase(Mock<IReader> reader, Mock<IModParser> modParser)
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
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos);

            modParser.Setup(s => s.Parse(It.IsAny<IEnumerable<string>>(), It.IsAny<DescriptorModType>())).Returns((IEnumerable<string> values, DescriptorModType t) =>
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
        public async Task Should_return_installed_mods()
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
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.GetInstalledModsAsync(new Game() { UserDirectory = "fake1", WorkshopDirectory = new List<string>() { "fake2" }, Type = "Should_return_installed_mods", CustomModDirectory = string.Empty });
            result.Count().Should().Be(2);
            result.Any(f => f.FileName == "1").Should().BeTrue();
            result.Any(f => f.FileName == "2").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_throw_exception_when_no_game_specified_when_fetching_installed_mods.
        /// </summary>
        [Fact]
        public async Task Should_throw_exception_when_no_game_specified_when_fetching_installed_mods()
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
                await service.GetInstalledModsAsync(null);
            }
            catch (Exception ex)
            {
                ex.GetType().Should().Be(typeof(ArgumentNullException));
            }
        }

        /// <summary>
        /// Defines the test method Should_return_available_mods.
        /// </summary>
        [Fact]
        public async Task Should_return_available_mods()
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
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.GetInstalledModsAsync(new Game() { UserDirectory = "fake1", WorkshopDirectory = new List<string>() { "fake2" }, Type = "Should_return_available_mods", CustomModDirectory = string.Empty });
            result.Count().Should().Be(2);
            result.Any(p => p.FileName == "1").Should().BeTrue();
            result.Any(p => p.FileName == "2").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_throw_exception_when_no_game_specified_when_fetching_available_mods.
        /// </summary>
        [Fact]
        public async Task Should_throw_exception_when_no_game_specified_when_fetching_available_mods()
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
                await service.GetInstalledModsAsync(null);
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
            DISetup.SetupContainer();

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
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns((ModWriterParameters p, bool isPath) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.ApplyModsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.ExportModsAsync(new List<IMod> { new Mod() { DescriptorFile = "mod/fake.mod" } }, new List<IMod> { new Mod() { DescriptorFile = "mod/fake.mod" } }, new ModCollection() { Name = "fake" });
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_export_mods_without_patch_mod.
        /// </summary>
        [Fact]
        public async Task Should_export_mods_without_patch_mod()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            bool noPatchModExported = false;
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
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns((ModWriterParameters p, bool isPath) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.ApplyModsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                noPatchModExported = p.TopPriorityMods == null || p.TopPriorityMods.Count == 0;
                return Task.FromResult(true);
            });

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.ExportModsAsync(new List<IMod> { new Mod() { DescriptorFile = "mod/fake.mod" } }, new List<IMod> { new Mod() { DescriptorFile = "mod/fake.mod" } }, new ModCollection() { PatchModEnabled = false, Name = "fake" });
            result.Should().BeTrue();
            noPatchModExported.Should().BeTrue();
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
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns((ModWriterParameters p, bool isPatch) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.DescriptorExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.ExportModsAsync(new List<IMod> { new Mod() }, new List<IMod> { new Mod() }, new ModCollection() { Name = "fake" });
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
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns((ModWriterParameters p, bool isPatch) =>
            {
                return Task.FromResult(true);
            });
            modWriter.Setup(p => p.DescriptorExistsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.ExportModsAsync(null, null, new ModCollection() { Name = "fake" });
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

            var result = await service.InstallModsAsync(null);
            result.Should().BeNull();
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
                Type = "Should_not_install_mods",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName
                };
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var result = await service.InstallModsAsync(null);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_install_mods_if_no_drive.
        /// </summary>
        [Fact]
        public async Task Should_not_install_mods_if_no_drive()
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
                Type = "Should_install_mods",
                UserDirectory = AppDomain.CurrentDomain.BaseDirectory,
                WorkshopDirectory = new List<string>() { "C:\\workshop" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName
                };
            });
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
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
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            modWriter.Setup(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(false);
            });

            var result = await service.InstallModsAsync(null);
            result.Should().BeNull();
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
                Type = "Should_install_mods",
                UserDirectory = AppDomain.CurrentDomain.BaseDirectory,
                WorkshopDirectory = new List<string>() { "C:\\workshop" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName
                };
            });
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
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
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            modWriter.Setup(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });

            var result = await service.InstallModsAsync(null);
            result.Count.Should().BeGreaterThan(0);
        }

        /// <summary>
        /// Defines the test method Should_install_lockable_mods.
        /// </summary>
        [Fact]
        public async Task Should_install_lockable_mods()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            bool lockSet = false;
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            SetupMockCase(reader, modParser);

            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_install_mods",
                UserDirectory = AppDomain.CurrentDomain.BaseDirectory,
                WorkshopDirectory = new List<string>() { "C:\\workshop" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName
                };
            });
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns((ModWriterParameters mwp, bool wdf) =>
            {
                if (mwp.LockDescriptor)
                {
                    lockSet = true;
                }
                return Task.FromResult(true);
            });
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
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            modWriter.Setup(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });

            var result = await service.InstallModsAsync(new List<IMod> { new Mod()
            {
                DescriptorFile = "mod/fake.mod",
                IsLocked = true
            }});
            result.Count.Should().BeGreaterThan(0);
            lockSet.Should().BeTrue();
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
                Type = "Should_not_lock_descriptors_when_no_mods",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" }
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
                Type = "Should_lock_descriptors",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" }
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
                Type = "Should_not_delete_descriptors_when_no_mods",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" }
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
                Type = "Should_delete_descriptors",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" }
            });
            modWriter.Setup(p => p.DeleteDescriptorAsync(It.IsAny<ModWriterParameters>())).Returns(Task.FromResult(true));
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var result = await service.DeleteDescriptorsAsync(new List<IMod>() { new Mod() });
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_load_file_lists.
        /// </summary>
        [Fact]
        public async Task Should_not_load_file_lists()
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
                Type = "Should_not_load_file_lists",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" }
            });
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string>() { "test" });
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var mod = new Mod();
            var result = await service.PopulateModFilesAsync(new List<IMod>() { mod });
            result.Should().Be(true);
            mod.Files.Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_load_file_lists.
        /// </summary>
        [Fact]
        public async Task Should_load_file_lists()
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
                Type = "Should_load_file_lists",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" }
            });
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string>() { "test" });
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var mod = new Mod()
            {
                IsValid = true
            };
            var result = await service.PopulateModFilesAsync(new List<IMod>() { mod });
            result.Should().Be(true);
            mod.Files.Count().Should().Be(1);
        }

        /// <summary>
        /// Defines the test method Should_not_be_achievement_compatible.
        /// </summary>
        [Fact]
        public void Should_not_be_achievement_compatible()
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
                Type = "Should_not_be_achievement_compatible",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" },
                ChecksumFolders = new List<string>() { "common", "events" }
            });
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string>() { "test" });
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var mod = new Mod()
            {
                IsValid = true,
                Files = new List<string>() { "common\\pop_jobs\\file.txt", "localisation\\yml.yml" }
            };
            var result = service.EvalAchievementCompatibility(new List<IMod>() { mod });
            result.Should().Be(true);
            mod.AchievementStatus.Should().Be(AchievementStatus.NotCompatible);
        }

        /// <summary>
        /// Defines the test method Should_be_achievement_compatible.
        /// </summary>
        [Fact]
        public void Should_be_achievement_compatible()
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
                Type = "Should_be_achievement_compatible",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" },
                ChecksumFolders = new List<string>() { "common", "events" }
            });
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string>() { "test" });
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var mod = new Mod()
            {
                IsValid = true,
                Files = new List<string>() { "gfx\\pop_jobs\\file.txt", "localisation\\yml.yml" }
            };
            var result = service.EvalAchievementCompatibility(new List<IMod>() { mod });
            result.Should().Be(true);
            mod.AchievementStatus.Should().Be(AchievementStatus.Compatible);
        }

        /// <summary>
        /// Defines the test method Should_not_return_mod_image_stream.
        /// </summary>
        [Fact]
        public async Task Should_not_return_mod_image_stream()
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
                    FileName = o.FileName,
                    Name = o.FileName
                };
            });
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            SetupMockCase(reader, modParser);

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.GetImageStreamAsync("test", "test");
            result.Should().BeNull();

            result = await service.GetImageStreamAsync(string.Empty, "test");
            result.Should().BeNull();

            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_not_return_mod_image_stream",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" },
                ChecksumFolders = new List<string>() { "common", "events" },
                CustomModDirectory = string.Empty
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            result = await service.GetImageStreamAsync("test", string.Empty);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_return_mod_image_stream.
        /// </summary>
        [Fact]
        public async Task Should_return_mod_image_stream()
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
                    FileName = o.FileName,
                    Name = o.FileName
                };
            });
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_not_return_mod_image_stream",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" },
                ChecksumFolders = new List<string>() { "common", "events" },
                CustomModDirectory = string.Empty
            });

            SetupMockCase(reader, modParser);
            reader.Setup(p => p.GetImageStreamAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(new MemoryStream()));
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.GetImageStreamAsync("1", "test");
            result.Should().NotBeNull();
        }

        /// <summary>
        /// Defines the test method Mod_directory_should_not_exist_when_no_game.
        /// </summary>
        [Fact]
        public async Task Mod_directory_should_not_exist_when_no_game()
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

            var result = await service.ModDirectoryExistsAsync("test");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Mod_directory_should_exist.
        /// </summary>
        [Fact]
        public async Task Mod_directory_should_exist()
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
                Type = "Mod_directory_should_exist",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" }
            });
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).Returns(Task.FromResult(true));
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var result = await service.ModDirectoryExistsAsync("test");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Patch_Mod_directory_should_not_exist_when_no_game.
        /// </summary>
        [Fact]
        public async Task Patch_Mod_directory_should_not_exist_when_no_game()
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

            var result = await service.PatchModExistsAsync("test");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Patch_Mod_directory_should_exist.
        /// </summary>
        [Fact]
        public async Task Patch_Mod_directory_should_exist()
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
                Type = "Patch_Mod_directory_should_exist",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" }
            });
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).Returns(Task.FromResult(true));
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var result = await service.PatchModExistsAsync("test");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Mod_directory_should_not_purge_when_no_game.
        /// </summary>
        [Fact]
        public async Task Mod_directory_should_not_purge_when_no_game()
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

            var result = await service.PurgeModDirectoryAsync("test");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Mod_directory_should_purge.
        /// </summary>
        [Fact]
        public async Task Mod_directory_should_purge()
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
                Type = "Mod_directory_should_purge",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" }
            });
            modWriter.Setup(p => p.PurgeModDirectoryAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var result = await service.PurgeModDirectoryAsync("test");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Patch_mod_directory_should_not_purge_when_no_game.
        /// </summary>
        [Fact]
        public async Task Patch_mod_directory_should_not_purge_when_no_game()
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

            var result = await service.PurgeModPatchAsync("test");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Patch_mod_directory_should_purge.
        /// </summary>
        [Fact]
        public async Task Patch_mod_directory_should_purge()
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
                Type = "Patch_mod_directory_should_purge",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" }
            });
            modWriter.Setup(p => p.PurgeModDirectoryAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var result = await service.PurgeModPatchAsync("test");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Custom_mod_directory_empty_should_return_true_when_invalid_game.
        /// </summary>
        [Fact]
        public async Task Custom_mod_directory_empty_should_return_true_when_invalid_game()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            gameService.Setup(p => p.Get()).Returns(new List<IGame>()
            {
                new Game()
                {
                    Type = "Custom_mod_directory_empty_should_return_true_when_no_game"
                }
            });

            var result = await service.CustomModDirectoryEmptyAsync("test");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Custom_mod_directory_empty_should_return_true.
        /// </summary>
        [Fact]
        public async Task Custom_mod_directory_empty_should_return_true()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            gameService.Setup(p => p.Get()).Returns(new List<IGame>()
            {
                new Game()
                {
                    Type = "Custom_mod_directory_empty_should_return_true",
                    CustomModDirectory = "c:\\test"
                }
            });
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).Returns(Task.FromResult(false));

            var result = await service.CustomModDirectoryEmptyAsync("Custom_mod_directory_empty_should_return_true");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Custom_mod_directory_empty_should_return_false.
        /// </summary>
        [Fact]
        public async Task Custom_mod_directory_empty_should_return_false()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            gameService.Setup(p => p.Get()).Returns(new List<IGame>()
            {
                new Game()
                {
                    Type = "Custom_mod_directory_empty_should_return_true",
                    CustomModDirectory = "c:\\test"
                }
            });
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).Returns(Task.FromResult(true));

            var result = await service.CustomModDirectoryEmptyAsync("Custom_mod_directory_empty_should_return_true");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_filter_mods.
        /// </summary>
        [Fact]
        public void Should_filter_mods()
        {
            DISetup.SetupContainer();
            CurrentLocale.SetCurrent("en");

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var parser = new Mock<IParser>();
            var lngService = new Mock<ILanguagesService>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, parser: parser, lngService);

            parser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>())).Returns(new SearchParserResult()
            {
                Name = new List<NameFilterResult>() { new NameFilterResult("test") },
                AchievementCompatible = new BoolFilterResult(true),
                Version = new List<VersionTypeResult>() { new VersionTypeResult(new Shared.Version(1, 5)) }
            });
            lngService.Setup(p => p.GetSelected()).Returns(new Language() { Abrv = "en" });

            var mods = new List<IMod>()
            {
                new Mod() { Name = "test", Version = "1.0", AchievementStatus = AchievementStatus.Compatible },
                new Mod() { Name = "test 2", Version = "1.5", AchievementStatus = AchievementStatus.NotCompatible },
                new Mod() { Name = "test 3", Version = "1.5", AchievementStatus = AchievementStatus.Compatible }
            };

            var result = service.FilterMods(mods, "test");
            result.Should().NotBeNull();
            result.Count().Should().Be(1);
            result.FirstOrDefault().Name.Should().Be("test 3");
        }

        /// <summary>
        /// Defines the test method Should_find_mods.
        /// </summary>
        [Fact]
        public void Should_find_mods()
        {
            DISetup.SetupContainer();
            CurrentLocale.SetCurrent("en");

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var parser = new Mock<IParser>();
            var lngService = new Mock<ILanguagesService>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, parser: parser, lngService);

            parser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>())).Returns(new SearchParserResult()
            {
                Name = new List<NameFilterResult>() { new NameFilterResult("test") },
                AchievementCompatible = new BoolFilterResult(true),
                Version = new List<VersionTypeResult>() { new VersionTypeResult(new Shared.Version(1, 5)) }
            });
            lngService.Setup(p => p.GetSelected()).Returns(new Language() { Abrv = "en" });

            var mods = new List<IMod>()
            {
                new Mod() { Name = "test", Version = "1.0", AchievementStatus = AchievementStatus.Compatible },
                new Mod() { Name = "test 2", Version = "1.5", AchievementStatus = AchievementStatus.NotCompatible },
                new Mod() { Name = "test 3", Version = "1.5", AchievementStatus = AchievementStatus.Compatible },
                new Mod() { Name = "test 4", Version = "1.5", AchievementStatus = AchievementStatus.Compatible }
            };

            var result = service.FindMod(mods, "test", false, 1);
            result.Should().NotBeNull();
            result.Name.Should().Be("test 3");

            result = service.FindMod(mods, "test", false, 3);
            result.Should().NotBeNull();
            result.Name.Should().Be("test 4");
        }

        /// <summary>
        /// Defines the test method Should_find_mods_in_reverse.
        /// </summary>
        [Fact]
        public void Should_find_mods_in_reverse()
        {
            DISetup.SetupContainer();
            CurrentLocale.SetCurrent("en");

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var parser = new Mock<IParser>();
            var lngService = new Mock<ILanguagesService>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, parser: parser, lngService);

            parser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>())).Returns(new SearchParserResult()
            {
                Name = new List<NameFilterResult>() { new NameFilterResult("test") },
                AchievementCompatible = new BoolFilterResult(true),
                Version = new List<VersionTypeResult>() { new VersionTypeResult(new Shared.Version(1, 5)) }
            });
            lngService.Setup(p => p.GetSelected()).Returns(new Language() { Abrv = "en" });

            var mods = new List<IMod>()
            {
                new Mod() { Name = "test 5", Version = "1.5", AchievementStatus = AchievementStatus.Compatible },
                new Mod() { Name = "test", Version = "1.0", AchievementStatus = AchievementStatus.Compatible },
                new Mod() { Name = "test 2", Version = "1.5", AchievementStatus = AchievementStatus.NotCompatible },
                new Mod() { Name = "test 3", Version = "1.5", AchievementStatus = AchievementStatus.Compatible },
                new Mod() { Name = "test 4", Version = "1.5", AchievementStatus = AchievementStatus.Compatible }
            };

            var result = service.FindMod(mods, "test", true, 0);
            result.Should().NotBeNull();
            result.Name.Should().Be("test 4");

            result = service.FindMod(mods, "test", true, 2);
            result.Should().NotBeNull();
            result.Name.Should().Be("test 5");
        }

        /// <summary>
        /// Defines the test method Should_contain_achievement_query.
        /// </summary>
        [Fact]
        public void Should_contain_achievement_query()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var parser = new Mock<IParser>();
            var lngService = new Mock<ILanguagesService>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, parser: parser, lngService);

            parser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>())).Returns(new SearchParserResult()
            {
                Name = new List<NameFilterResult>() { new NameFilterResult("test") },
                AchievementCompatible = new BoolFilterResult(true),
                Version = new List<VersionTypeResult>() { new VersionTypeResult(new Shared.Version(1, 5)) }
            });
            lngService.Setup(p => p.GetSelected()).Returns(new Language() { Abrv = "en" });

            var mods = new List<IMod>()
            {
                new Mod() { Name = "test", Version = "1.0", AchievementStatus = AchievementStatus.Compatible },
                new Mod() { Name = "test 2", Version = "1.5", AchievementStatus = AchievementStatus.NotCompatible },
                new Mod() { Name = "test 3", Version = "1.5", AchievementStatus = AchievementStatus.Compatible }
            };

            var result = service.QueryContainsAchievements("test");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_contain_achievement_query.
        /// </summary>
        [Fact]
        public void Should_not_contain_achievement_query()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var parser = new Mock<IParser>();
            var lngService = new Mock<ILanguagesService>();

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, parser: parser, lngService);

            parser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>())).Returns(new SearchParserResult()
            {
                Name = new List<NameFilterResult>() { new NameFilterResult("test") },                
                Version = new List<VersionTypeResult>() { new VersionTypeResult(new Shared.Version(1, 5)) }
            });
            lngService.Setup(p => p.GetSelected()).Returns(new Language() { Abrv = "en" });

            var mods = new List<IMod>()
            {
                new Mod() { Name = "test", Version = "1.0", AchievementStatus = AchievementStatus.Compatible },
                new Mod() { Name = "test 2", Version = "1.5", AchievementStatus = AchievementStatus.NotCompatible },
                new Mod() { Name = "test 3", Version = "1.5", AchievementStatus = AchievementStatus.Compatible }
            };

            var result = service.QueryContainsAchievements("test");
            result.Should().BeFalse();
        }
    }
}
