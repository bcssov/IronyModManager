// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 05-18-2026
// ***********************************************************************
// <copyright file="ModServiceTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AwesomeAssertions;
using Castle.DynamicProxy;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.FileSystem;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Localization;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Common.Mod.Search;
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

// ReSharper disable UnusedParameter.Local

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class ModServiceTests.
    /// </summary>
    [SuppressMessage("Style", "IDE0028:Simplify collection initialization", Justification = "Unit tests")]
    [SuppressMessage("ReSharper", "ConvertToLambdaExpression")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Resharper")]
    public class ModServiceTests
    {
        private const int CloudFileProviderNotRunningHResult = unchecked((int)0x8007016A);

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
            Mock<IGameService> gameService, Mock<IParser> parser = null, Mock<ILanguagesService> languageService = null,
            Mock<IFileSystemStateProbe> fileSystemStateProbe = null, IGameStateSafetyService gameStateSafetyService = null, ICache cache = null)
        {
            if (fileSystemStateProbe == null)
            {
                fileSystemStateProbe = new Mock<IFileSystemStateProbe>();
                fileSystemStateProbe.Setup(p => p.CheckDirectory(It.IsAny<string>())).Returns((string path) => new FileSystemPathCheckResult { Path = path, State = FileSystemPathState.Available });
            }

            gameStateSafetyService ??= new GameStateSafetyService(fileSystemStateProbe.Object, Mock.Of<ILogger>());
            return new ModService(languageService?.Object, parser?.Object, null, cache ?? new Cache(), null, reader.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object,
                fileSystemStateProbe.Object, gameStateSafetyService, () => new Mod());
        }

        private static IMod CreateProxyMod(string descriptor = null, string path = null, string name = null,
            ModSource source = ModSource.Local, long? remoteId = null, bool isVirtual = false)
        {
            var mod = new ProxyGenerator().CreateClassProxy<Mod>();
            mod.DescriptorFile = descriptor;
            mod.FullPath = path;
            mod.Name = name;
            mod.Source = source;
            mod.RemoteId = remoteId;
            mod.IsVirtual = isVirtual;
            return mod;
        }

        /// <summary>
        /// Setups the mock case.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="modParser">The mod parser.</param>
        private static void SetupMockCase(Mock<IReader> reader, Mock<IModParser> modParser)
        {
            var fileInfos = new List<IFileInfo>
            {
                new FileInfo { Content = new List<string> { "1" }, FileName = "fake1.txt", IsBinary = false }, new FileInfo { Content = new List<string> { "2" }, FileName = "fake2.txt", IsBinary = false }
            };
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos);

            modParser.Setup(s => s.Parse(It.IsAny<IEnumerable<string>>(), It.IsAny<DescriptorModType>(), It.IsAny<ModParserArgs>())).Returns((IEnumerable<string> values, DescriptorModType t, ModParserArgs a) =>
            {
                return new ModObject { FileName = values.First(), Name = values.First() };
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
                return new Mod { FileName = o.FileName };
            });

            SetupMockCase(reader, modParser);
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.GetInstalledModsAsync(new Game { UserDirectory = "fake1", WorkshopDirectory = new List<string> { "fake2" }, Type = "Should_return_installed_mods", CustomModDirectory = string.Empty });
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
                return new Mod { FileName = o.FileName };
            });

            SetupMockCase(reader, modParser);
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.GetInstalledModsAsync(new Game { UserDirectory = "fake1", WorkshopDirectory = new List<string> { "fake2" }, Type = "Should_return_available_mods", CustomModDirectory = string.Empty });
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
            var url = service.BuildModUrl(new Mod { RemoteId = 1, Source = ModSource.Steam });
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
            var url = service.BuildModUrl(new Mod { RemoteId = 1, Source = ModSource.Paradox });
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
            var url = service.BuildModUrl(new Mod { RemoteId = null, Source = ModSource.Local });
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
            var url = service.BuildSteamUrl(new Mod { RemoteId = 1, Source = ModSource.Steam });
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
            var url = service.BuildSteamUrl(new Mod { RemoteId = 1, Source = ModSource.Paradox });
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
                return new Mod { FileName = o.FileName };
            });
            gameService.Setup(s => s.GetSelected()).Returns(new Game { Type = "test", UserDirectory = "C:\\users\\fake" });
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
            var result = await service.ExportModsAsync(new List<IMod> { new Mod { DescriptorFile = "mod/fake.mod" } }, new List<IMod> { new Mod { DescriptorFile = "mod/fake.mod" } }, new ModCollection { Name = "fake" });
            result.Succeeded.Should().BeTrue();
            result.SkippedVirtualMods.Should().Be(0);
        }

        [Fact]
        public async Task Should_skip_virtual_mods_when_applying_without_mutating_collection_membership()
        {
            DISetup.SetupContainer();

            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var game = new Game { Type = "test", UserDirectory = "C:\\users\\fake" };
            gameService.Setup(p => p.GetSelected()).Returns(game);
            modWriter.Setup(p => p.ModDirectoryExistsAsync(It.IsAny<ModWriterParameters>())).ReturnsAsync(false);
            ModWriterParameters applied = null;
            modWriter.Setup(p => p.ApplyModsAsync(It.IsAny<ModWriterParameters>()))
                .Callback<ModWriterParameters>(p => applied = p)
                .ReturnsAsync(true);
            var service = GetService(new Mock<IStorageProvider>(), new Mock<IModParser>(), new Mock<IReader>(),
                new Mock<IMapper>(), modWriter, gameService);
            var first = new Mod { DescriptorFile = "mod/a.mod", Name = "A", IsSelected = true };
            var missingOne = new Mod { DescriptorFile = "mod/b.mod", Name = "B", IsSelected = true, IsVirtual = true };
            var third = new Mod { DescriptorFile = "mod/c.mod", Name = "C", IsSelected = true };
            var missingTwo = new Mod { DescriptorFile = "mod/d.mod", Name = "D", IsSelected = true, IsVirtual = true };
            var fifth = new Mod { DescriptorFile = "mod/e.mod", Name = "E", IsSelected = true };
            var selected = new List<IMod> { first, missingOne, third, missingTwo, fifth };
            var persisted = new ModCollection
            {
                Name = "collection",
                Mods = ["mod/a.mod", "mod/b.mod", "mod/c.mod", "mod/d.mod", "mod/e.mod"]
            };

            var result = await service.ExportModsAsync(selected, selected, persisted);

            result.Succeeded.Should().BeTrue();
            result.SkippedVirtualMods.Should().Be(2);
            applied.EnabledMods.Should().Equal(first, third, fifth);
            applied.OtherMods.Should().BeEmpty();
            selected.Should().Equal(first, missingOne, third, missingTwo, fifth);
            persisted.Mods.Should().Equal("mod/a.mod", "mod/b.mod", "mod/c.mod", "mod/d.mod", "mod/e.mod");

            missingOne.IsVirtual = false;
            result = await service.ExportModsAsync(selected, selected, persisted);
            result.Succeeded.Should().BeTrue();
            result.SkippedVirtualMods.Should().Be(1);
            applied.EnabledMods.Should().Equal(first, missingOne, third, fifth);

            missingTwo.IsVirtual = false;
            result = await service.ExportModsAsync(selected, selected, persisted);
            result.Succeeded.Should().BeTrue();
            result.SkippedVirtualMods.Should().Be(0);
            applied.EnabledMods.Should().Equal(first, missingOne, third, missingTwo, fifth);
            persisted.Mods.Should().Equal("mod/a.mod", "mod/b.mod", "mod/c.mod", "mod/d.mod", "mod/e.mod");
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
            var noPatchModExported = false;
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod { FileName = o.FileName };
            });
            gameService.Setup(s => s.GetSelected()).Returns(new Game { Type = "test", UserDirectory = "C:\\users\\fake" });
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
            var result = await service.ExportModsAsync(new List<IMod> { new Mod { DescriptorFile = "mod/fake.mod" } }, new List<IMod> { new Mod { DescriptorFile = "mod/fake.mod" } },
                new ModCollection { PatchModEnabled = false, Name = "fake" });
            result.Succeeded.Should().BeTrue();
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
                return new Mod { FileName = o.FileName };
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
            var result = await service.ExportModsAsync(new List<IMod> { new Mod() }, new List<IMod> { new Mod() }, new ModCollection { Name = "fake" });
            result.Succeeded.Should().BeFalse();
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
                return new Mod { FileName = o.FileName };
            });
            gameService.Setup(s => s.GetSelected()).Returns(new Game { Type = "test" });
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
            var result = await service.ExportModsAsync(null, null, new ModCollection { Name = "fake" });
            result.Succeeded.Should().BeFalse();
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

            gameService.Setup(p => p.GetSelected()).Returns(new Game { Type = "Should_not_install_mods", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" }, CustomModDirectory = string.Empty });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod { FileName = o.FileName };
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

            gameService.Setup(p => p.GetSelected()).Returns(new Game
            {
                Type = "Should_install_mods", UserDirectory = AppDomain.CurrentDomain.BaseDirectory, WorkshopDirectory = new List<string> { "C:\\workshop" }, CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod { FileName = o.FileName };
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

                return new FileInfo { Content = sb.ToString().SplitOnNewLine(), ContentSHA = "test", FileName = "fake.mod", IsBinary = false };
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

            gameService.Setup(p => p.GetSelected()).Returns(new Game
            {
                Type = "Should_install_mods", UserDirectory = AppDomain.CurrentDomain.BaseDirectory, WorkshopDirectory = new List<string> { "C:\\workshop" }, CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod { FileName = o.FileName };
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

                return new FileInfo { Content = sb.ToString().SplitOnNewLine(), ContentSHA = "test", FileName = "fake.mod", IsBinary = false };
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

            var lockSet = false;
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            SetupMockCase(reader, modParser);

            gameService.Setup(p => p.GetSelected()).Returns(new Game
            {
                Type = "Should_install_mods", UserDirectory = AppDomain.CurrentDomain.BaseDirectory, WorkshopDirectory = new List<string> { "C:\\workshop" }, CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod { FileName = o.FileName };
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

                return new FileInfo { Content = sb.ToString().SplitOnNewLine(), ContentSHA = "test", FileName = "fake.mod", IsBinary = false };
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            modWriter.Setup(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });

            var result = await service.InstallModsAsync(new List<IMod> { new Mod { DescriptorFile = "mod/fake.mod", IsLocked = true } });
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
            gameService.Setup(p => p.GetSelected()).Returns(new Game { Type = "Should_not_lock_descriptors_when_no_mods", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" } });
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
            gameService.Setup(p => p.GetSelected()).Returns(new Game { Type = "Should_lock_descriptors", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" } });
            modWriter.Setup(p => p.SetDescriptorLockAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var result = await service.LockDescriptorsAsync(new List<IMod> { new Mod() }, true);
            result.Should().BeTrue();
        }

        [Fact]
        public async Task Descriptor_lock_failure_should_not_mutate_model_or_report_success()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.SetDescriptorLockAsync(It.IsAny<ModWriterParameters>(), true))
                .ThrowsAsync(new UnauthorizedAccessException());
            var gameService = new Mock<IGameService>();
            var game = new Game { Type = "descriptor-failure", UserDirectory = "user", WorkshopDirectory = [] };
            gameService.Setup(p => p.GetSelected()).Returns(game);
            var mapper = new Mock<IMapper>();
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.IsFileSystemAccessFailure(It.IsAny<Exception>())).Returns(true);
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var target = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService,
                fileSystemStateProbe: probe, gameStateSafetyService: safety);
            var service = new ProxyGenerator().CreateInterfaceProxyWithTarget<IModService>(target,
                new GameStateSafetyInterceptor(gameService.Object, safety, probe.Object));
            var mod = new Mod { IsLocked = false };

            var result = await service.LockDescriptorsAsync([mod], true);

            result.Should().BeFalse();
            mod.IsLocked.Should().BeFalse();
            safety.GetLock(game).Reason.Should().Be(GameStateLockReason.WriteAccessFailure);
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
            gameService.Setup(p => p.GetSelected()).Returns(new Game { Type = "Should_not_delete_descriptors_when_no_mods", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" } });
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
            gameService.Setup(p => p.GetSelected()).Returns(new Game { Type = "Should_delete_descriptors", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" } });
            modWriter.Setup(p => p.DeleteDescriptorAsync(It.IsAny<ModWriterParameters>())).Returns(Task.FromResult(true));
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var result = await service.DeleteDescriptorsAsync(new List<IMod> { new Mod() });
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
            gameService.Setup(p => p.GetSelected()).Returns(new Game { Type = "Should_not_load_file_lists", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" } });
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string> { "test" });
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var mod = new Mod();
            var result = await service.PopulateModFilesAsync(new List<IMod> { mod });
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
            gameService.Setup(p => p.GetSelected()).Returns(new Game { Type = "Should_load_file_lists", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" } });
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string> { "test" });
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var mod = new Mod { IsValid = true };
            var result = await service.PopulateModFilesAsync(new List<IMod> { mod });
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
            gameService.Setup(p => p.GetSelected()).Returns(new Game
            {
                Type = "Should_not_be_achievement_compatible", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" }, ChecksumFolders = new List<string> { "common", "events" }
            });
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string> { "test" });
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var mod = new Mod { IsValid = true, Files = new List<string> { "common\\pop_jobs\\file.txt", "localisation\\yml.yml" } };
            var result = service.EvalAchievementCompatibility(new List<IMod> { mod });
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
            gameService.Setup(p => p.GetSelected()).Returns(new Game
            {
                Type = "Should_be_achievement_compatible", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" }, ChecksumFolders = new List<string> { "common", "events" }
            });
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string> { "test" });
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var mod = new Mod { IsValid = true, Files = new List<string> { "gfx\\pop_jobs\\file.txt", "localisation\\yml.yml" } };
            var result = service.EvalAchievementCompatibility(new List<IMod> { mod });
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
                return new Mod { FileName = o.FileName, Name = o.FileName };
            });
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            SetupMockCase(reader, modParser);

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);
            var result = await service.GetImageStreamAsync("test", "test");
            result.Should().BeNull();

            result = await service.GetImageStreamAsync(string.Empty, "test");
            result.Should().BeNull();

            gameService.Setup(p => p.GetSelected()).Returns(new Game
            {
                Type = "Should_not_return_mod_image_stream",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string> { "C:\\workshop" },
                ChecksumFolders = new List<string> { "common", "events" },
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
                return new Mod { FileName = o.FileName, Name = o.FileName };
            });
            gameService.Setup(p => p.GetSelected()).Returns(new Game
            {
                Type = "Should_not_return_mod_image_stream",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string> { "C:\\workshop" },
                ChecksumFolders = new List<string> { "common", "events" },
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

            gameService.Setup(p => p.GetSelected()).Returns(new Game { Type = "Mod_directory_should_exist", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" } });
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

            gameService.Setup(p => p.GetSelected()).Returns(new Game { Type = "Patch_Mod_directory_should_exist", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" } });
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

            gameService.Setup(p => p.GetSelected()).Returns(new Game { Type = "Mod_directory_should_purge", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" } });
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

            gameService.Setup(p => p.GetSelected()).Returns(new Game { Type = "Patch_mod_directory_should_purge", UserDirectory = "C:\\Users\\Fake", WorkshopDirectory = new List<string> { "C:\\workshop" } });
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

            gameService.Setup(p => p.Get()).Returns(new List<IGame> { new Game { Type = "Custom_mod_directory_empty_should_return_true_when_no_game" } });

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

            gameService.Setup(p => p.Get()).Returns(new List<IGame> { new Game { Type = "Custom_mod_directory_empty_should_return_true", CustomModDirectory = "c:\\test" } });
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

            gameService.Setup(p => p.Get()).Returns(new List<IGame> { new Game { Type = "Custom_mod_directory_empty_should_return_true", CustomModDirectory = "c:\\test" } });
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

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, parser, lngService);

            parser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>())).Returns(new SearchParserResult
            {
                Name = new List<NameFilterResult> { new("test") }, AchievementCompatible = new BoolFilterResult(true), Version = new List<VersionTypeResult> { new(new Shared.Version(1, 5)) }
            });
            lngService.Setup(p => p.GetSelected()).Returns(new Language { Abrv = "en" });

            var mods = new List<IMod>
            {
                new Mod { Name = "test", Version = "1.0", AchievementStatus = AchievementStatus.Compatible },
                new Mod { Name = "test 2", Version = "1.5", AchievementStatus = AchievementStatus.NotCompatible },
                new Mod { Name = "test 3", Version = "1.5", AchievementStatus = AchievementStatus.Compatible }
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

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, parser, lngService);

            parser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>())).Returns(new SearchParserResult
            {
                Name = new List<NameFilterResult> { new("test") }, AchievementCompatible = new BoolFilterResult(true), Version = new List<VersionTypeResult> { new(new Shared.Version(1, 5)) }
            });
            lngService.Setup(p => p.GetSelected()).Returns(new Language { Abrv = "en" });

            var mods = new List<IMod>
            {
                new Mod { Name = "test", Version = "1.0", AchievementStatus = AchievementStatus.Compatible },
                new Mod { Name = "test 2", Version = "1.5", AchievementStatus = AchievementStatus.NotCompatible },
                new Mod { Name = "test 3", Version = "1.5", AchievementStatus = AchievementStatus.Compatible },
                new Mod { Name = "test 4", Version = "1.5", AchievementStatus = AchievementStatus.Compatible }
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

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, parser, lngService);

            parser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>())).Returns(new SearchParserResult
            {
                Name = new List<NameFilterResult> { new("test") }, AchievementCompatible = new BoolFilterResult(true), Version = new List<VersionTypeResult> { new(new Shared.Version(1, 5)) }
            });
            lngService.Setup(p => p.GetSelected()).Returns(new Language { Abrv = "en" });

            var mods = new List<IMod>
            {
                new Mod { Name = "test 5", Version = "1.5", AchievementStatus = AchievementStatus.Compatible },
                new Mod { Name = "test", Version = "1.0", AchievementStatus = AchievementStatus.Compatible },
                new Mod { Name = "test 2", Version = "1.5", AchievementStatus = AchievementStatus.NotCompatible },
                new Mod { Name = "test 3", Version = "1.5", AchievementStatus = AchievementStatus.Compatible },
                new Mod { Name = "test 4", Version = "1.5", AchievementStatus = AchievementStatus.Compatible }
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

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, parser, lngService);

            parser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>())).Returns(new SearchParserResult
            {
                Name = new List<NameFilterResult> { new("test") }, AchievementCompatible = new BoolFilterResult(true), Version = new List<VersionTypeResult> { new(new Shared.Version(1, 5)) }
            });
            lngService.Setup(p => p.GetSelected()).Returns(new Language { Abrv = "en" });
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

            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, parser, lngService);

            parser.Setup(p => p.Parse(It.IsAny<string>(), It.IsAny<string>())).Returns(new SearchParserResult { Name = new List<NameFilterResult> { new("test") }, Version = new List<VersionTypeResult> { new(new Shared.Version(1, 5)) } });
            lngService.Setup(p => p.GetSelected()).Returns(new Language { Abrv = "en" });
            var result = service.QueryContainsAchievements("test");
            result.Should().BeFalse();
        }

        [Fact]
        public async Task Authoritative_empty_scan_should_return_empty_without_locking_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            reader.Setup(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns([]);
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.CheckDirectory(It.IsAny<string>())).Returns((string path) => new FileSystemPathCheckResult { Path = path, State = FileSystemPathState.Available });
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var game = new Game { Type = "empty", UserDirectory = "user", WorkshopDirectory = [], CustomModDirectory = string.Empty };
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, fileSystemStateProbe: probe, gameStateSafetyService: safety);

            var result = await service.RefreshInstalledModsAsync(game);

            result.IsAuthoritative.Should().BeTrue();
            result.Mods.Should().BeEmpty();
            safety.IsLocked(game).Should().BeFalse();
        }

        [Fact]
        public async Task Missing_configured_custom_source_should_lock_without_scanning()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.CheckDirectory(It.IsAny<string>())).Returns((string path) => new FileSystemPathCheckResult
            {
                Path = path,
                State = path == "custom" ? FileSystemPathState.Missing : FileSystemPathState.Available
            });
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var game = new Game { Type = "missing-custom", UserDirectory = "user", WorkshopDirectory = ["workshop"], CustomModDirectory = "custom" };
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, fileSystemStateProbe: probe, gameStateSafetyService: safety);

            var result = await service.RefreshInstalledModsAsync(game);

            result.IsAuthoritative.Should().BeFalse();
            safety.GetLock(game).Reason.Should().Be(GameStateLockReason.ExpectedSourceMissing);
            safety.GetLock(game).Context.Should().Be("custom");
            reader.Verify(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()), Times.Never);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Missing_descriptor_or_workshop_source_should_lock_without_scanning(bool descriptorSource)
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var missingPath = descriptorSource ? Path.Combine("user", Shared.Constants.ModDirectory) : "workshop";
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.CheckDirectory(It.IsAny<string>())).Returns((string path) => new FileSystemPathCheckResult
            {
                Path = path,
                State = path == missingPath ? FileSystemPathState.Missing : FileSystemPathState.Available
            });
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var game = new Game { Type = "missing-source", UserDirectory = "user", WorkshopDirectory = ["workshop"] };
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, fileSystemStateProbe: probe, gameStateSafetyService: safety);

            var result = await service.RefreshInstalledModsAsync(game);

            result.IsAuthoritative.Should().BeFalse();
            safety.GetLock(game).Context.Should().Be(missingPath);
            reader.Verify(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Blank_optional_sources_should_not_be_probed_or_lock_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            reader.Setup(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns([]);
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var probedPaths = new List<string>();
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.CheckDirectory(It.IsAny<string>())).Returns((string path) =>
            {
                probedPaths.Add(path);
                return new FileSystemPathCheckResult { Path = path, State = FileSystemPathState.Available };
            });
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var game = new Game { Type = "optional", UserDirectory = "user", WorkshopDirectory = [string.Empty, " "], CustomModDirectory = null };
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, fileSystemStateProbe: probe, gameStateSafetyService: safety);

            var result = await service.RefreshInstalledModsAsync(game);

            result.IsAuthoritative.Should().BeTrue();
            probedPaths.Should().Equal(Path.Combine("user", Shared.Constants.ModDirectory));
            safety.IsLocked(game).Should().BeFalse();
        }

        [Fact]
        public async Task Locked_game_should_only_scan_through_explicit_revalidation_and_remain_locked_until_completion()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            reader.Setup(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns([]);
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.CheckDirectory(It.IsAny<string>())).Returns((string path) => new FileSystemPathCheckResult { Path = path, State = FileSystemPathState.Available });
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var game = new Game { Type = "locked", UserDirectory = "user", WorkshopDirectory = [] };
            safety.Lock(game, GameStateLockReason.DiscoveryUnavailable, "provider");
            var revalidationLock = safety.BeginRevalidation(game, "new user directory");
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, fileSystemStateProbe: probe, gameStateSafetyService: safety);

            (await service.RefreshInstalledModsAsync(game)).IsAuthoritative.Should().BeFalse();
            reader.Verify(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()), Times.Never);

            (await service.RevalidateInstalledModsAsync(game, revalidationLock)).IsAuthoritative.Should().BeTrue();
            safety.IsLocked(game).Should().BeTrue();
            safety.CompleteRevalidation(game, revalidationLock).Should().BeTrue();
            safety.IsLocked(game).Should().BeFalse();
            reader.Verify(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task Superseded_revalidation_should_not_scan_or_replace_known_good_cache()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            reader.Setup(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns([]);
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.CheckDirectory(It.IsAny<string>())).Returns((string path) =>
                new FileSystemPathCheckResult { Path = path, State = FileSystemPathState.Available });
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var cache = new Cache();
            var game = new Game { Type = "generation", UserDirectory = "user", WorkshopDirectory = [] };
            var knownGood = new List<IMod> { new Mod { DescriptorFile = "mod/known.mod", Game = game.Type } };
            cache.Set(new CacheAddParameters<IEnumerable<IMod>>
                { Region = "Mods", Prefix = game.Type, Key = "RegularMods", Value = knownGood });
            var first = safety.BeginRevalidation(game, "first path");
            var second = safety.BeginRevalidation(game, "second path");
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService,
                fileSystemStateProbe: probe, gameStateSafetyService: safety, cache: cache);

            var staleResult = await service.RevalidateInstalledModsAsync(game, first);

            staleResult.IsAuthoritative.Should().BeFalse();
            staleResult.Mods.Should().Equal(knownGood);
            cache.Get<IEnumerable<IMod>>(new CacheGetParameters
                { Region = "Mods", Prefix = game.Type, Key = "RegularMods" }).Should().Equal(knownGood);
            safety.GetLock(game).Should().BeSameAs(second);
            reader.Verify(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Failed_refresh_should_restore_known_good_cache_and_never_cache_empty_result()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            reader.Setup(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()))
                .Throws(new IOException("Locale-independent test fixture", CloudFileProviderNotRunningHResult));
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.CheckDirectory(It.IsAny<string>())).Returns((string path) => new FileSystemPathCheckResult { Path = path, State = FileSystemPathState.Available });
            probe.Setup(p => p.IsFileSystemAccessFailure(It.IsAny<Exception>())).Returns(true);
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var cache = new Cache();
            var game = new Game { Type = "cloud", UserDirectory = "user", WorkshopDirectory = ["workshop"], CustomModDirectory = string.Empty };
            var knownGood = new List<IMod> { new Mod { DescriptorFile = "mod/known.mod", Game = game.Type } };
            cache.Set(new CacheAddParameters<IEnumerable<IMod>> { Region = "Mods", Prefix = game.Type, Key = "RegularMods", Value = knownGood });
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, fileSystemStateProbe: probe, gameStateSafetyService: safety, cache: cache);

            var result = await service.RefreshInstalledModsAsync(game);

            result.IsAuthoritative.Should().BeFalse();
            result.Mods.Should().Equal(knownGood);
            cache.Get<IEnumerable<IMod>>(new CacheGetParameters { Region = "Mods", Prefix = game.Type, Key = "RegularMods" }).Should().Equal(knownGood);
            safety.GetLock(game).Reason.Should().Be(GameStateLockReason.DiscoveryUnavailable);
        }

        [Fact]
        public async Task Cold_start_failed_refresh_should_be_non_authoritative_without_populating_cache()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            reader.Setup(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()))
                .Throws(new IOException("Locale-independent test fixture", CloudFileProviderNotRunningHResult));
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.CheckDirectory(It.IsAny<string>())).Returns((string path) => new FileSystemPathCheckResult { Path = path, State = FileSystemPathState.Available });
            probe.Setup(p => p.IsFileSystemAccessFailure(It.IsAny<Exception>())).Returns(true);
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var cache = new Cache();
            var game = new Game { Type = "cold", UserDirectory = "user", WorkshopDirectory = [] };
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, fileSystemStateProbe: probe, gameStateSafetyService: safety, cache: cache);

            var result = await service.RefreshInstalledModsAsync(game);

            result.IsAuthoritative.Should().BeFalse();
            result.Mods.Should().BeEmpty();
            cache.Get<IEnumerable<IMod>>(new CacheGetParameters { Region = "Mods", Prefix = game.Type, Key = "RegularMods" }).Should().BeNull();
            safety.GetLock(game).Reason.Should().Be(GameStateLockReason.DiscoveryUnavailable);
        }

        [Fact]
        public async Task Available_mod_lookup_should_not_scan_or_populate_cache_while_locked()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var probe = new Mock<IFileSystemStateProbe>();
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var cache = new Cache();
            var game = new Game { Type = "locked", UserDirectory = "user", WorkshopDirectory = [] };
            safety.Lock(game, GameStateLockReason.DiscoveryUnavailable, "provider");
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, fileSystemStateProbe: probe, gameStateSafetyService: safety, cache: cache);

            var result = await service.GetAvailableModsAsync(game);

            result.Should().BeEmpty();
            reader.Verify(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()), Times.Never);
            cache.Get<IEnumerable<IMod>>(new CacheGetParameters { Region = "Mods", Prefix = game.Type, Key = "RegularMods" }).Should().BeNull();
        }

        [Fact]
        public void Collection_resolution_should_create_ordered_virtual_with_persisted_metadata_and_restore_real_mod()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var first = new Mod { DescriptorFile = "mod/first.mod", FullPath = "first", Name = "First", Game = "game", IsValid = true };
            var restored = new Mod { DescriptorFile = "mod/missing.mod", FullPath = "missing", Name = "Restored", Game = "game", IsValid = true };
            var collection = new ModCollection
            {
                Game = "game",
                Mods = ["mod/first.mod", "mod/missing.mod"],
                ModPaths = ["first", "missing"],
                ModNames = ["First", "Missing title"],
                ModIds = [new ModCollectionSourceInfo(), new ModCollectionSourceInfo { SteamId = 42 }]
            };
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService);

            var missingResult = service.ResolveCollectionMods([first], collection);

            missingResult.Should().HaveCount(2);
            missingResult.First().Should().BeSameAs(first);
            var virtualMod = missingResult.Last();
            virtualMod.IsVirtual.Should().BeTrue();
            virtualMod.IsValid.Should().BeFalse();
            virtualMod.IsSelected.Should().BeTrue();
            virtualMod.DescriptorFile.Should().Be("mod/missing.mod");
            virtualMod.FullPath.Should().Be("missing");
            virtualMod.Name.Should().Be("Missing title");
            virtualMod.RemoteId.Should().Be(42);
            virtualMod.Source.Should().Be(ModSource.Steam);

            var restoredResult = service.ResolveCollectionMods([first, restored], collection, missingResult);
            restoredResult.Should().Equal(first, restored);
            restoredResult.Should().OnlyContain(p => !p.IsVirtual);
        }

        [Fact]
        public void Removed_virtual_membership_should_not_return_after_refresh_or_installation()
        {
            var service = GetService(new Mock<IStorageProvider>(), new Mock<IModParser>(), new Mock<IReader>(),
                new Mock<IMapper>(), new Mock<IModWriter>(), new Mock<IGameService>());
            var first = new Mod { DescriptorFile = "mod/first.mod", FullPath = "first", Name = "First", Game = "game", IsValid = true };
            var removed = new Mod { DescriptorFile = "mod/removed.mod", FullPath = "removed", Name = "Removed", Game = "game", IsValid = true };
            var last = new Mod { DescriptorFile = "mod/last.mod", FullPath = "last", Name = "Last", Game = "game", IsValid = true };
            var persistedAfterRemoval = new ModCollection
            {
                Game = "game",
                Mods = [first.DescriptorFile, last.DescriptorFile],
                ModPaths = [first.FullPath, last.FullPath],
                ModNames = [first.Name, last.Name],
                ModIds = [new ModCollectionSourceInfo(), new ModCollectionSourceInfo()]
            };

            service.ResolveCollectionMods([first, last], persistedAfterRemoval)
                .Should().Equal(first, last);
            service.ResolveCollectionMods([first, removed, last], persistedAfterRemoval)
                .Should().Equal(first, last);
        }

        [Fact]
        public void Imported_collection_should_preserve_missing_members_and_use_persisted_path_fallback()
        {
            var installed = new Mod { DescriptorFile = "new/descriptor.mod", FullPath = "same/path", Name = "Installed", Game = "game" };
            var collection = new ModCollection
            {
                Game = "game",
                Mods = ["missing.mod", "old/descriptor.mod"],
                ModPaths = ["missing/path", "same/path"],
                ModNames = ["Missing imported mod", "Imported installed mod"],
                ModIds = [new ModCollectionSourceInfo { ParadoxId = 84 }, new ModCollectionSourceInfo()]
            };
            var service = GetService(new Mock<IStorageProvider>(), new Mock<IModParser>(), new Mock<IReader>(),
                new Mock<IMapper>(), new Mock<IModWriter>(), new Mock<IGameService>());

            var result = service.ResolveCollectionMods([installed], collection);

            result.Should().HaveCount(2);
            result.First().IsVirtual.Should().BeTrue();
            result.First().Name.Should().Be("Missing imported mod");
            result.First().RemoteId.Should().Be(84);
            result.Last().Should().BeSameAs(installed);
        }

        [Fact]
        public void Mod_state_identity_should_compare_equivalent_distinct_instances()
        {
            var service = GetService(new Mock<IStorageProvider>(), new Mock<IModParser>(), new Mock<IReader>(),
                new Mock<IMapper>(), new Mock<IModWriter>(), new Mock<IGameService>());
            var mod = new Mod
            {
                DescriptorFile = "mod/example.mod", Version = "1", Name = "Example", Dependencies = ["dependency"],
                RemoteId = 42, ReplacePath = ["common"], UserDir = ["user"], JsonId = "example"
            };
            var equivalent = new Mod
            {
                DescriptorFile = "MOD/EXAMPLE.MOD", Version = "1", Name = "Example", Dependencies = ["dependency"],
                RemoteId = 42, ReplacePath = ["common"], UserDir = ["user"], JsonId = "EXAMPLE"
            };

            service.AreModDefinitionsEquivalent(mod, equivalent).Should().BeTrue();
            equivalent.Version = "2";
            service.AreModDefinitionsEquivalent(mod, equivalent).Should().BeFalse();
        }

        [Fact]
        public void Mod_identity_should_match_distinct_castle_proxies_by_case_insensitive_descriptor()
        {
            var service = GetService(new Mock<IStorageProvider>(), new Mock<IModParser>(), new Mock<IReader>(),
                new Mock<IMapper>(), new Mock<IModWriter>(), new Mock<IGameService>());
            var virtualMod = CreateProxyMod("mod/example.mod", "old/path", "Virtual", isVirtual: true);
            var realMod = CreateProxyMod("MOD/EXAMPLE.MOD", "new/path", "Real");

            service.AreModIdentitiesEquivalent(virtualMod, realMod).Should().BeTrue();
            virtualMod.Should().NotBeSameAs(realMod);
            virtualMod.GetType().Should().NotBe(typeof(Mod));
            realMod.GetType().Should().NotBe(typeof(Mod));
        }

        [Fact]
        public void Mod_identity_should_use_case_insensitive_persisted_path_fallback()
        {
            var service = GetService(new Mock<IStorageProvider>(), new Mock<IModParser>(), new Mock<IReader>(),
                new Mock<IMapper>(), new Mock<IModWriter>(), new Mock<IGameService>());
            var missingDescriptor = CreateProxyMod(string.Empty, "mods/example", "First");
            var reconstructed = CreateProxyMod(null, "MODS/EXAMPLE", "Second");
            var staleDescriptor = CreateProxyMod("old/example.mod", "mods/example");
            var currentDescriptor = CreateProxyMod("new/example.mod", "MODS/EXAMPLE");

            service.AreModIdentitiesEquivalent(missingDescriptor, reconstructed).Should().BeTrue();
            service.AreModIdentitiesEquivalent(staleDescriptor, currentDescriptor).Should().BeTrue();
        }

        [Fact]
        public void Mod_identity_should_reject_name_and_remote_metadata_without_descriptor_or_path_match()
        {
            var service = GetService(new Mock<IStorageProvider>(), new Mock<IModParser>(), new Mock<IReader>(),
                new Mock<IMapper>(), new Mock<IModWriter>(), new Mock<IGameService>());
            var first = CreateProxyMod("first.mod", "first/path", "Same", ModSource.Steam, 42);
            var second = CreateProxyMod("second.mod", "second/path", "Same", ModSource.Steam, 42);

            service.AreModIdentitiesEquivalent(first, second).Should().BeFalse();
        }

        [Fact]
        public void Mod_identity_should_ignore_nullable_parser_definition_fields_without_weakening_definition_comparison()
        {
            var service = GetService(new Mock<IStorageProvider>(), new Mock<IModParser>(), new Mock<IReader>(),
                new Mock<IMapper>(), new Mock<IModWriter>(), new Mock<IGameService>());
            var first = CreateProxyMod("same.mod", "first/path", "Same");
            var second = CreateProxyMod("same.mod", "second/path", "Same");
            first.Version = "1";
            second.Version = "1";
            first.Dependencies = null;
            second.Dependencies = null;
            first.ReplacePath = null;
            second.ReplacePath = null;
            first.UserDir = null;
            second.UserDir = null;

            service.AreModIdentitiesEquivalent(first, second).Should().BeTrue();
            service.AreModDefinitionsEquivalent(first, second).Should().BeFalse();
        }

        [Fact]
        public async Task Locked_game_should_reject_apply_without_calling_writer()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var game = new Game { Type = "locked-apply" };
            gameService.Setup(p => p.GetSelected()).Returns(game);
            var probe = new Mock<IFileSystemStateProbe>();
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            safety.Lock(game, GameStateLockReason.DiscoveryUnavailable, "source");
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, fileSystemStateProbe: probe, gameStateSafetyService: safety);

            var result = await service.ExportModsAsync([], [], new ModCollection { Name = "collection" });

            result.Succeeded.Should().BeFalse();
            modWriter.Verify(p => p.ApplyModsAsync(It.IsAny<ModWriterParameters>()), Times.Never);
        }

        [Fact]
        public async Task Unwritable_mod_directory_should_abort_and_lock_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>())).ReturnsAsync(false);
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var game = new Game { Type = "unwritable", UserDirectory = "user", CustomModDirectory = "custom", WorkshopDirectory = [] };
            gameService.Setup(p => p.GetSelected()).Returns(game);
            var probe = new Mock<IFileSystemStateProbe>();
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService, fileSystemStateProbe: probe, gameStateSafetyService: safety);

            var result = await service.InstallModsAsync([]);

            result.Should().BeNull();
            safety.GetLock(game).Reason.Should().Be(GameStateLockReason.WriteAccessFailure);
            safety.GetLock(game).Context.Should().Be("custom");
            modWriter.Verify(p => p.CreateModDirectoryAsync(It.IsAny<ModWriterParameters>()), Times.Never);
        }

        [Fact]
        public async Task Historical_cloud_provider_descriptor_read_failure_should_abort_install_and_lock_as_discovery_unavailable()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var descriptorDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Shared.Constants.ModDirectory);
            var cloudProviderException = new IOException("Locale-independent test fixture", CloudFileProviderNotRunningHResult);
            reader.Setup(p => p.Read(descriptorDirectory, It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()))
                .Throws(cloudProviderException);
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>())).ReturnsAsync(true);
            var gameService = new Mock<IGameService>();
            var game = new Game
            {
                Type = "install-read-failure", UserDirectory = AppDomain.CurrentDomain.BaseDirectory,
                WorkshopDirectory = ["workshop"], CustomModDirectory = string.Empty
            };
            gameService.Setup(p => p.GetSelected()).Returns(game);
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.IsFileSystemAccessFailure(It.IsAny<Exception>()))
                .Returns((Exception exception) => exception is IOException or UnauthorizedAccessException);
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var unaffectedGame = new Game { Type = "unaffected" };
            var service = GetService(storageProvider, modParser, reader, new Mock<IMapper>(), modWriter, gameService,
                fileSystemStateProbe: probe, gameStateSafetyService: safety);

            var result = await service.InstallModsAsync([]);

            result.Should().BeNull();
            safety.GetLock(game).Reason.Should().Be(GameStateLockReason.DiscoveryUnavailable);
            safety.GetLock(game).Context.Should().Be("Read mod descriptor sources");
            safety.IsLocked(unaffectedGame).Should().BeFalse();
            cloudProviderException.HResult.Should().Be(CloudFileProviderNotRunningHResult);
            reader.Verify(p => p.Read(descriptorDirectory, It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()), Times.Once);
            modWriter.Verify(p => p.CreateModDirectoryAsync(It.IsAny<ModWriterParameters>()), Times.Never);
            modWriter.Verify(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Install_destination_failure_should_lock_as_write_access_failure()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            SetupMockCase(reader, modParser);
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject value) =>
                new Mod { FileName = value.FileName, DescriptorFile = $"mod/{value.FileName}.mod" });
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo
            {
                Content = ["name=\"Fake\"", "path=\"c:/fake\""], ContentSHA = "test", FileName = "fake.mod", IsBinary = false
            });
            var modWriter = new Mock<IModWriter>();
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns(false);
            modWriter.Setup(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>())).ReturnsAsync(true);
            modWriter.Setup(p => p.CreateModDirectoryAsync(It.IsAny<ModWriterParameters>()))
                .ThrowsAsync(new UnauthorizedAccessException());
            var gameService = new Mock<IGameService>();
            var game = new Game
            {
                Type = "install-write-failure", UserDirectory = AppDomain.CurrentDomain.BaseDirectory,
                WorkshopDirectory = ["workshop"], CustomModDirectory = string.Empty
            };
            gameService.Setup(p => p.GetSelected()).Returns(game);
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.IsFileSystemAccessFailure(It.IsAny<Exception>()))
                .Returns((Exception exception) => exception is IOException or UnauthorizedAccessException);
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var service = GetService(storageProvider, modParser, reader, mapper, modWriter, gameService,
                fileSystemStateProbe: probe, gameStateSafetyService: safety);

            var result = await service.InstallModsAsync([]);

            result.Should().BeNull();
            safety.GetLock(game).Reason.Should().Be(GameStateLockReason.WriteAccessFailure);
        }

        [Fact]
        public async Task Controlled_custom_directory_synchronization_should_write_then_refresh_new_descriptors()
        {
            var (service, game, reader, writer, safety, _) = GetControlledInstallService("controlled-install");
            var notifications = 0;
            safety.GameLocked += _ => notifications++;
            var revalidationLock = safety.BeginRevalidation(game, "custom directory changed");

            var synchronized = await service.InstallModsAsync(game, [], revalidationLock);
            var refreshed = await service.RevalidateInstalledModsAsync(game, revalidationLock);

            synchronized.Should().BeTrue();
            refreshed.IsAuthoritative.Should().BeTrue();
            refreshed.Mods.Should().NotBeEmpty();
            safety.IsLocked(game).Should().BeTrue();
            notifications.Should().Be(0);
            writer.Verify(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>()), Times.AtLeastOnce);
            reader.Verify(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Invalid_custom_root_should_not_start_controlled_descriptor_synchronization()
        {
            var (service, game, _, writer, safety, probe) = GetControlledInstallService("missing-custom");
            probe.Setup(p => p.CheckDirectory(game.CustomModDirectory)).Returns(new FileSystemPathCheckResult
                { Path = game.CustomModDirectory, State = FileSystemPathState.Missing });
            var notifications = 0;
            safety.GameLocked += _ => notifications++;
            var revalidationLock = safety.BeginRevalidation(game, "custom directory changed");

            var synchronized = await service.InstallModsAsync(game, [], revalidationLock);

            synchronized.Should().BeFalse();
            safety.GetLock(game).Reason.Should().Be(GameStateLockReason.ExpectedSourceMissing);
            notifications.Should().Be(1);
            writer.Verify(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>()), Times.Never);
            writer.Verify(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Controlled_descriptor_write_failure_should_leave_write_failure_lock()
        {
            var (service, game, _, writer, safety, _) = GetControlledInstallService("controlled-write-failure");
            writer.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).ReturnsAsync(false);
            var notifications = 0;
            safety.GameLocked += _ => notifications++;
            var revalidationLock = safety.BeginRevalidation(game, "custom directory changed");

            var synchronized = await service.InstallModsAsync(game, [], revalidationLock);

            synchronized.Should().BeFalse();
            safety.GetLock(game).Reason.Should().Be(GameStateLockReason.WriteAccessFailure);
            safety.IsLocked(game).Should().BeTrue();
            notifications.Should().Be(1);
        }

        [Fact]
        public async Task Controlled_descriptor_read_failure_should_leave_discovery_failure_lock()
        {
            var (service, game, reader, writer, safety, _) = GetControlledInstallService("controlled-read-failure");
            reader.Setup(p => p.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>()))
                .Throws(new IOException("Locale-independent test fixture"));
            var revalidationLock = safety.BeginRevalidation(game, "custom directory changed");

            var synchronized = await service.InstallModsAsync(game, [], revalidationLock);

            synchronized.Should().BeFalse();
            safety.GetLock(game).Reason.Should().Be(GameStateLockReason.DiscoveryUnavailable);
            safety.IsLocked(game).Should().BeTrue();
            writer.Verify(p => p.CreateModDirectoryAsync(It.IsAny<ModWriterParameters>()), Times.Never);
            writer.Verify(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Superseded_custom_directory_generation_should_not_synchronize_descriptors()
        {
            var (service, game, _, writer, safety, _) = GetControlledInstallService("superseded-install");
            var notifications = 0;
            safety.GameLocked += _ => notifications++;
            var staleLock = safety.BeginRevalidation(game, "first custom directory");
            writer.Setup(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>())).Returns(() =>
            {
                safety.BeginRevalidation(game, "second custom directory");
                return Task.FromResult(true);
            });

            var synchronized = await service.InstallModsAsync(game, [], staleLock);

            synchronized.Should().BeFalse();
            notifications.Should().Be(0);
            writer.Verify(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>()), Times.Once);
            writer.Verify(p => p.CreateModDirectoryAsync(It.IsAny<ModWriterParameters>()), Times.Never);
            writer.Verify(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Ordinary_installed_mod_refresh_should_not_synchronize_descriptors()
        {
            var (service, game, _, writer, _, _) = GetControlledInstallService("ordinary-refresh");

            var refreshed = await service.RefreshInstalledModsAsync(game);

            refreshed.IsAuthoritative.Should().BeTrue();
            writer.Verify(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>()), Times.Never);
            writer.Verify(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>()), Times.Never);
        }

        private static (ModService Service, IGame Game, Mock<IReader> Reader, Mock<IModWriter> Writer,
            GameStateSafetyService Safety, Mock<IFileSystemStateProbe> Probe) GetControlledInstallService(string gameType)
        {
            DISetup.SetupContainer();
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            SetupMockCase(reader, modParser);
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo
            {
                Content = ["name=\"Fake\"", "path=\"c:/fake\""], ContentSHA = "test", FileName = "fake.mod", IsBinary = false
            });
            var mapper = new Mock<IMapper>();
            mapper.Setup(p => p.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject value) =>
                new Mod { FileName = value.FileName, DescriptorFile = $"mod/{value.FileName}.mod" });
            var writer = new Mock<IModWriter>();
            writer.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns(false);
            writer.Setup(p => p.CanWriteToModDirectoryAsync(It.IsAny<ModWriterParameters>())).ReturnsAsync(true);
            writer.Setup(p => p.CreateModDirectoryAsync(It.IsAny<ModWriterParameters>())).ReturnsAsync(false);
            writer.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).ReturnsAsync(true);
            var game = new Game
            {
                Type = gameType,
                UserDirectory = AppDomain.CurrentDomain.BaseDirectory,
                WorkshopDirectory = [],
                CustomModDirectory = AppDomain.CurrentDomain.BaseDirectory
            };
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.GetSelected()).Returns(game);
            var probe = new Mock<IFileSystemStateProbe>();
            probe.Setup(p => p.CheckDirectory(It.IsAny<string>())).Returns((string path) =>
                new FileSystemPathCheckResult { Path = path, State = FileSystemPathState.Available });
            probe.Setup(p => p.IsFileSystemAccessFailure(It.IsAny<Exception>())).Returns(
                (Exception exception) => exception is IOException or UnauthorizedAccessException);
            var safety = new GameStateSafetyService(probe.Object, Mock.Of<ILogger>());
            var service = GetService(storageProvider, modParser, reader, mapper, writer, gameService,
                fileSystemStateProbe: probe, gameStateSafetyService: safety);
            return (service, game, reader, writer, safety, probe);
        }
    }
}
