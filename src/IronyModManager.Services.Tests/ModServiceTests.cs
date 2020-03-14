// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 03-14-2020
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
        /// Setups the mock case.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="parserManager">The parser manager.</param>
        /// <param name="modParser">The mod parser.</param>
        private void SetupMockCase(Mock<IReader> reader, Mock<IParserManager> parserManager, Mock<IModParser> modParser)
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
                    FileName = values.First()
                };
            });

            parserManager.Setup(s => s.Parse(It.IsAny<ParserManagerArgs>())).Returns((ParserManagerArgs args) =>
            {
                return new List<IDefinition>() { new Definition()
                {
                    Code = args.File,
                    File = args.File,
                    ContentSHA = args.File,
                    Id = args.File,
                    Type = args.ModName
                } };
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
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var mapper = new Mock<IMapper>();
            var modExporter = new Mock<IModExporter>();
            var gameService = new Mock<IGameService>();
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName
                };
            });

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modExporter.Object, gameService.Object, storageProvider.Object, mapper.Object);
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
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modExporter = new Mock<IModExporter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modExporter.Object, gameService.Object, storageProvider.Object, mapper.Object);
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
        /// Defines the test method Should_not_return_any_mod_objects_when_no_game_or_mods.
        /// </summary>
        [Fact]
        public void Should_not_return_any_mod_objects_when_no_game_or_mods()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modExporter = new Mock<IModExporter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modExporter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var result = service.GetModObjects(null, new List<IMod>());
            result.Should().BeNull();

            result = service.GetModObjects(new Game(), new List<IMod>());
            result.Should().BeNull();

            result = service.GetModObjects(new Game(), null);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_return_mod_objects_when_using_fully_qualified_path.
        /// </summary>
        [Fact]
        public void Should_return_mod_objects_when_using_fully_qualified_path()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modExporter = new Mock<IModExporter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modExporter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var result = service.GetModObjects(new Game(), new List<IMod>()
            {
                new Mod()
                {
                    FileName = Assembly.GetExecutingAssembly().Location,
                    Name = "fake"
                }
            });
            result.GetAll().Count().Should().Be(2);
            var ordered = result.GetAll().OrderBy(p => p.Id);
            ordered.First().Id.Should().Be("fake1.txt");
            ordered.Last().Id.Should().Be("fake2.txt");
        }

        /// <summary>
        /// Defines the test method Should_return_mod_objects_when_using_user_directory.
        /// </summary>
        [Fact]
        public void Should_return_mod_objects_when_using_user_directory()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modExporter = new Mock<IModExporter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modExporter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var result = service.GetModObjects(new Game() { UserDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), WorkshopDirectory = "fake1" }, new List<IMod>()
            {
                new Mod()
                {
                    FileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location),
                    Name = "fake"
                }
            });
            result.GetAll().Count().Should().Be(2);
            var ordered = result.GetAll().OrderBy(p => p.Id);
            ordered.First().Id.Should().Be("fake1.txt");
            ordered.Last().Id.Should().Be("fake2.txt");
        }

        /// <summary>
        /// Defines the test method Should_return_mod_objects_when_using_workshop_directory.
        /// </summary>
        [Fact]
        public void Should_return_mod_objects_when_using_workshop_directory()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modExporter = new Mock<IModExporter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modExporter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var result = service.GetModObjects(new Game() { WorkshopDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), UserDirectory = "fake1" }, new List<IMod>()
            {
                new Mod()
                {
                    FileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location),
                    Name = "fake"
                }
            });
            result.GetAll().Count().Should().Be(2);
            var ordered = result.GetAll().OrderBy(p => p.Id);
            ordered.First().Id.Should().Be("fake1.txt");
            ordered.Last().Id.Should().Be("fake2.txt");
        }

        /// <summary>
        /// Defines the test method Should_return_steam_url.
        /// </summary>
        [Fact]
        public void Should_return_steam_url()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modExporter = new Mock<IModExporter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modExporter.Object, gameService.Object, storageProvider.Object, mapper.Object);

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
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modExporter = new Mock<IModExporter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modExporter.Object, gameService.Object, storageProvider.Object, mapper.Object);

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
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modExporter = new Mock<IModExporter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modExporter.Object, gameService.Object, storageProvider.Object, mapper.Object);

            var url = service.BuildModUrl(new Mod()
            {
                RemoteId = 1,
                Source = ModSource.Local
            });
            url.Should().BeNullOrEmpty();
        }

        /// <summary>
        /// Defines the test method Should_export_mods.
        /// </summary>
        [Fact]
        public async Task Should_export_mods()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modExporter = new Mock<IModExporter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "test"
            });
            modExporter.Setup(p => p.ApplyCollectionAsync(It.IsAny<IReadOnlyCollection<IMod>>(), It.IsAny<string>())).Returns((IReadOnlyCollection<IMod> mods, string rootDir) =>
            {
                return Task.FromResult(true);
            });

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modExporter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var result = await service.ExportModsAsync(new List<IMod> { new Mod() });
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
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modExporter = new Mock<IModExporter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            gameService.Setup(s => s.GetSelected()).Returns(() =>
            {
                return null;
            });
            modExporter.Setup(p => p.ApplyCollectionAsync(It.IsAny<IReadOnlyCollection<IMod>>(), It.IsAny<string>())).Returns((IReadOnlyCollection<IMod> mods, string rootDir) =>
            {
                return Task.FromResult(true);
            });

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modExporter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var result = await service.ExportModsAsync(new List<IMod> { new Mod() });
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_export_mods_when_collection_null_or_empty.
        /// </summary>
        [Fact]
        public async Task Should_not_export_mods_when_collection_null_or_empty()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modExporter = new Mock<IModExporter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "test"
            });
            modExporter.Setup(p => p.ApplyCollectionAsync(It.IsAny<IReadOnlyCollection<IMod>>(), It.IsAny<string>())).Returns((IReadOnlyCollection<IMod> mods, string rootDir) =>
            {
                return Task.FromResult(true);
            });

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modExporter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var result = await service.ExportModsAsync(new List<IMod>());
            result.Should().BeFalse();

            result = await service.ExportModsAsync(new List<IMod>());
            result.Should().BeFalse();
        }


        /// <summary>
        /// Defines the test method Stellaris_Performance_profiling.
        /// </summary>

#if FUNCTIONAL_TEST
        [Fact(Timeout = 3000000)]
#else
        [Fact(Skip = "This is for functional testing only")]
#endif
        public void Stellaris_Performance_profiling()
        {
            DISetup.SetupContainer();

            var registration = new Services.Registrations.GameRegistration();
            registration.OnPostStartup();
            var game = DISetup.Container.GetInstance<IGameService>().Get().First(s => s.Type == "Stellaris");
            var svc = DISetup.Container.GetInstance<IModService>();
            var mods = DISetup.Container.GetInstance<IModService>().GetInstalledMods(game);
            var defs = DISetup.Container.GetInstance<IModService>().GetModObjects(game, mods);
        }
    }
}
