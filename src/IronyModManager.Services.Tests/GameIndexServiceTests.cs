// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 05-27-2021
//
// Last Modified By : Mario
// Last Modified On : 05-27-2021
// ***********************************************************************
// <copyright file="GameIndexServiceTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using IronyModManager.IO;
using IronyModManager.IO.Common.Game;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Args;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Definitions;
using IronyModManager.Services.Common;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Shared.Models;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class GameIndexServiceTests.
    /// </summary>
    public class GameIndexServiceTests
    {
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="gameIndexer">The game indexer.</param>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="parserManager">The parser manager.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <returns>GameIndexService.</returns>
        private static GameIndexService GetService(Mock<IGameIndexer> gameIndexer, Mock<IStorageProvider> storageProvider, Mock<IModParser> modParser,
                                                   Mock<IParserManager> parserManager, Mock<IReader> reader, Mock<IMapper> mapper, Mock<IModWriter> modWriter,
                                                   Mock<IGameService> gameService, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders = null)
        {
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            return new GameIndexService(messageBus.Object, parserManager.Object, gameIndexer.Object, new Cache(), definitionInfoProviders, reader.Object, modWriter.Object, modParser.Object, gameService.Object, storageProvider.Object, mapper.Object);
        }

        /// <summary>
        /// Defines the test method Should_not_index_definitions_when_no_game.
        /// </summary>
        [Fact]
        public async Task Should_not_index_definitions_when_no_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var gameIndexer = new Mock<IGameIndexer>();

            var service = GetService(gameIndexer, storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService);
            var result = await service.IndexDefinitionsAsync(null, new List<string>() { "3.0.3" }, new IndexedDefinitions());
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_index_definitions_when_no_version.
        /// </summary>
        [Fact]
        public async Task Should_not_index_definitions_when_no_version()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var gameIndexer = new Mock<IGameIndexer>();

            var service = GetService(gameIndexer, storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService);
            var result = await service.IndexDefinitionsAsync(new Game(), new List<string>(), new IndexedDefinitions());
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_index_definitions_when_definition_signature_not_same.
        /// </summary>
        [Fact]
        public async Task Should_index_definitions_when_definition_signature_not_same()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var gameIndexer = new Mock<IGameIndexer>();
            storageProvider.Setup(p => p.GetRootStoragePath()).Returns("c:\\test");
            gameIndexer.Setup(p => p.GameVersionsSameAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<string>>())).Returns((string p1, IGame p2, IEnumerable<string> p3) => Task.FromResult(false));
            gameIndexer.Setup(p => p.CachedDefinitionsSameAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<int>())).Returns((string p1, IGame p2, int p3) => Task.FromResult(true));
            gameIndexer.Setup(p => p.ClearDefinitionAsync(It.IsAny<string>(), It.IsAny<IGame>())).Returns((string p1, IGame p2) => Task.FromResult(false));
            gameIndexer.Setup(p => p.WriteVersionAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<string>>(), It.IsAny<int>())).Returns((string p1, IGame p2, IEnumerable<string> p3, int p4) => Task.FromResult(false));
            gameIndexer.Setup(p => p.FolderCachedAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<string>())).Returns((string p1, IGame p2, string p3) => Task.FromResult(false));
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string>() { "test1\\1.txt", "test2\\2.txt", "test3\\3.txt" });
            var fileInfos1 = new List<IFileInfo>()
            {
                new FileInfo()
                {
                    Content = new List<string>() { "1" },
                    FileName = "test1\\1.txt",
                    IsBinary = false
                }
            };
            var fileInfos2 = new List<IFileInfo>()
            {
                new FileInfo()
                {
                    Content = new List<string>() { "1" },
                    FileName = "test2\\2.txt",
                    IsBinary = false
                }
            };
            reader.Setup(s => s.Read(It.Is<string>(p => p.Contains("test1")), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos1);
            reader.Setup(s => s.Read(It.Is<string>(p => p.Contains("test2")), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos2);
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
            var saved = new ConcurrentBag<string>();
            gameIndexer.Setup(p => p.SaveDefinitionsAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<IDefinition>>())).Returns((string p1, IGame p2, IEnumerable<IDefinition> p3) =>
            {
                saved.Add(p3.FirstOrDefault().ParentDirectory);
                return Task.FromResult(true);
            });

            var service = GetService(gameIndexer, storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService);
            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>() { new Definition() { File = "test1\\1.txt" }, new Definition() { File = "test2\\3.txt" } });
            var result = await service.IndexDefinitionsAsync(new Game() { ExecutableLocation = "c:\\test\\test.exe", GameFolders = new List<string>() { "test1", "test2" } }, new List<string>() { "3.0.3" }, indexed);
            result.Should().BeTrue();
            saved.Count.Should().Be(2);
            saved.FirstOrDefault(p => p.Contains("test1")).Should().NotBeNull();
            saved.FirstOrDefault(p => p.Contains("test2")).Should().NotBeNull();
        }

        /// <summary>
        /// Defines the test method Should_index_definitions_when_game_version_signature_not_same.
        /// </summary>
        [Fact]
        public async Task Should_index_definitions_when_game_version_signature_not_same()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var gameIndexer = new Mock<IGameIndexer>();
            storageProvider.Setup(p => p.GetRootStoragePath()).Returns("c:\\test");
            gameIndexer.Setup(p => p.GameVersionsSameAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<string>>())).Returns((string p1, IGame p2, IEnumerable<string> p3) => Task.FromResult(true));
            gameIndexer.Setup(p => p.CachedDefinitionsSameAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<int>())).Returns((string p1, IGame p2, int p3) => Task.FromResult(false));
            gameIndexer.Setup(p => p.ClearDefinitionAsync(It.IsAny<string>(), It.IsAny<IGame>())).Returns((string p1, IGame p2) => Task.FromResult(false));
            gameIndexer.Setup(p => p.WriteVersionAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<string>>(), It.IsAny<int>())).Returns((string p1, IGame p2, IEnumerable<string> p3, int p4) => Task.FromResult(false));
            gameIndexer.Setup(p => p.FolderCachedAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<string>())).Returns((string p1, IGame p2, string p3) => Task.FromResult(false));
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string>() { "test1\\1.txt", "test2\\2.txt", "test3\\3.txt" });
            var fileInfos1 = new List<IFileInfo>()
            {
                new FileInfo()
                {
                    Content = new List<string>() { "1" },
                    FileName = "test1\\1.txt",
                    IsBinary = false
                }
            };
            var fileInfos2 = new List<IFileInfo>()
            {
                new FileInfo()
                {
                    Content = new List<string>() { "1" },
                    FileName = "test2\\2.txt",
                    IsBinary = false
                }
            };
            reader.Setup(s => s.Read(It.Is<string>(p => p.Contains("test1")), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos1);
            reader.Setup(s => s.Read(It.Is<string>(p => p.Contains("test2")), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos2);
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
            var saved = new ConcurrentBag<string>();
            gameIndexer.Setup(p => p.SaveDefinitionsAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<IDefinition>>())).Returns((string p1, IGame p2, IEnumerable<IDefinition> p3) =>
            {
                saved.Add(p3.FirstOrDefault().ParentDirectory);
                return Task.FromResult(true);
            });

            var service = GetService(gameIndexer, storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService);
            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>() { new Definition() { File = "test1\\1.txt" }, new Definition() { File = "test2\\3.txt" } });
            var result = await service.IndexDefinitionsAsync(new Game() { ExecutableLocation = "c:\\test\\test.exe", GameFolders = new List<string>() { "test1", "test2" } }, new List<string>() { "3.0.3" }, indexed);
            result.Should().BeTrue();
            saved.Count.Should().Be(2);
            saved.FirstOrDefault(p => p.Contains("test1")).Should().NotBeNull();
            saved.FirstOrDefault(p => p.Contains("test2")).Should().NotBeNull();
        }

        /// <summary>
        /// Defines the test method Should_index_definitions_which_are_not_indexed.
        /// </summary>
        [Fact]
        public async Task Should_index_definitions_which_are_not_indexed()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var gameIndexer = new Mock<IGameIndexer>();
            storageProvider.Setup(p => p.GetRootStoragePath()).Returns("c:\\test");
            gameIndexer.Setup(p => p.GameVersionsSameAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<string>>())).Returns((string p1, IGame p2, IEnumerable<string> p3) => Task.FromResult(false));
            gameIndexer.Setup(p => p.CachedDefinitionsSameAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<int>())).Returns((string p1, IGame p2, int p3) => Task.FromResult(false));
            gameIndexer.Setup(p => p.ClearDefinitionAsync(It.IsAny<string>(), It.IsAny<IGame>())).Returns((string p1, IGame p2) => Task.FromResult(false));
            gameIndexer.Setup(p => p.WriteVersionAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<string>>(), It.IsAny<int>())).Returns((string p1, IGame p2, IEnumerable<string> p3, int p4) => Task.FromResult(false));
            gameIndexer.Setup(p => p.FolderCachedAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<string>())).Returns((string p1, IGame p2, string p3) =>
            {
                if (p3.StartsWith("test2"))
                {
                    return Task.FromResult(true);
                }
                return Task.FromResult(false);
            });
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string>() { "test1\\1.txt", "test2\\2.txt", "test3\\3.txt" });
            var fileInfos1 = new List<IFileInfo>()
            {
                new FileInfo()
                {
                    Content = new List<string>() { "1" },
                    FileName = "test1\\1.txt",
                    IsBinary = false
                }
            };
            var fileInfos2 = new List<IFileInfo>()
            {
                new FileInfo()
                {
                    Content = new List<string>() { "1" },
                    FileName = "test2\\2.txt",
                    IsBinary = false
                }
            };
            reader.Setup(s => s.Read(It.Is<string>(p => p.Contains("test1")), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos1);
            reader.Setup(s => s.Read(It.Is<string>(p => p.Contains("test2")), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos2);
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
            var saved = new ConcurrentBag<string>();
            gameIndexer.Setup(p => p.SaveDefinitionsAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<IDefinition>>())).Returns((string p1, IGame p2, IEnumerable<IDefinition> p3) =>
            {
                saved.Add(p3.FirstOrDefault().ParentDirectory);
                return Task.FromResult(true);
            });

            var service = GetService(gameIndexer, storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService);
            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>() { new Definition() { File = "test1\\1.txt" }, new Definition() { File = "test2\\3.txt" } });
            var result = await service.IndexDefinitionsAsync(new Game() { ExecutableLocation = "c:\\test\\test.exe", GameFolders = new List<string>() { "test1", "test2" } }, new List<string>() { "3.0.3" }, indexed);
            result.Should().BeTrue();
            saved.Count.Should().Be(1);
            saved.FirstOrDefault(p => p.Contains("test1")).Should().NotBeNull();
            saved.FirstOrDefault(p => p.Contains("test2")).Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_index_definitions.
        /// </summary>
        [Fact]
        public async Task Should_not_index_definitions()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var gameIndexer = new Mock<IGameIndexer>();
            storageProvider.Setup(p => p.GetRootStoragePath()).Returns("c:\\test");
            gameIndexer.Setup(p => p.GameVersionsSameAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<string>>())).Returns((string p1, IGame p2, IEnumerable<string> p3) => Task.FromResult(false));
            gameIndexer.Setup(p => p.CachedDefinitionsSameAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<int>())).Returns((string p1, IGame p2, int p3) => Task.FromResult(false));
            gameIndexer.Setup(p => p.ClearDefinitionAsync(It.IsAny<string>(), It.IsAny<IGame>())).Returns((string p1, IGame p2) => Task.FromResult(false));
            gameIndexer.Setup(p => p.WriteVersionAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<string>>(), It.IsAny<int>())).Returns((string p1, IGame p2, IEnumerable<string> p3, int p4) => Task.FromResult(false));
            gameIndexer.Setup(p => p.FolderCachedAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<string>())).Returns((string p1, IGame p2, string p3) => Task.FromResult(true));
            reader.Setup(p => p.GetFiles(It.IsAny<string>())).Returns(new List<string>() { "test1\\1.txt", "test2\\2.txt", "test3\\3.txt" });
            var fileInfos1 = new List<IFileInfo>()
            {
                new FileInfo()
                {
                    Content = new List<string>() { "1" },
                    FileName = "test1\\1.txt",
                    IsBinary = false
                }
            };
            var fileInfos2 = new List<IFileInfo>()
            {
                new FileInfo()
                {
                    Content = new List<string>() { "1" },
                    FileName = "test2\\2.txt",
                    IsBinary = false
                }
            };
            reader.Setup(s => s.Read(It.Is<string>(p => p.Contains("test1")), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos1);
            reader.Setup(s => s.Read(It.Is<string>(p => p.Contains("test2")), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos2);
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
            var saved = new ConcurrentBag<string>();
            gameIndexer.Setup(p => p.SaveDefinitionsAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<IDefinition>>())).Returns((string p1, IGame p2, IEnumerable<IDefinition> p3) =>
            {
                saved.Add(p3.FirstOrDefault().ParentDirectory);
                return Task.FromResult(true);
            });

            var service = GetService(gameIndexer, storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService);
            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>() { new Definition() { File = "test1\\1.txt" }, new Definition() { File = "test2\\3.txt" } });
            var result = await service.IndexDefinitionsAsync(new Game() { ExecutableLocation = "c:\\test\\test.exe", GameFolders = new List<string>() { "test1", "test2" } }, new List<string>() { "3.0.3" }, indexed);
            result.Should().BeTrue();
            saved.Count.Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_load_definitions_when_no_game.
        /// </summary>
        [Fact]
        public async Task Should_not_load_definitions_when_no_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var gameIndexer = new Mock<IGameIndexer>();

            var defs = new IndexedDefinitions();
            defs.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Type = "test",
                    Id = "1",
                    ModName = "test"
                }
            });
            var service = GetService(gameIndexer, storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService);
            var result = await service.LoadDefinitionsAsync(defs, null, new List<string>() { "3.0.3" });
            result.Should().Be(defs);
        }

        /// <summary>
        /// Defines the test method Should_not_load_definitions_when_no_version.
        /// </summary>
        [Fact]
        public async Task Should_not_load_definitions_when_no_version()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var gameIndexer = new Mock<IGameIndexer>();

            var defs = new IndexedDefinitions();
            defs.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Type = "test",
                    Id = "1",
                    ModName = "test"
                }
            });
            var service = GetService(gameIndexer, storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService);
            var result = await service.LoadDefinitionsAsync(defs, new Game(), new List<string>() { string.Empty });
            result.Should().Be(defs);
        }

        /// <summary>
        /// Defines the test method Should_not_load_definitions_when_definitions_dont_exist.
        /// </summary>
        [Fact]
        public async Task Should_not_load_definitions_when_definitions_dont_exist()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var gameIndexer = new Mock<IGameIndexer>();
            storageProvider.Setup(p => p.GetRootStoragePath()).Returns("c:\\test");
            gameIndexer.Setup(p => p.GameVersionsSameAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<string>>())).Returns((string p1, IGame p2, IEnumerable<string> p3) => Task.FromResult(false));

            var defs = new IndexedDefinitions();
            defs.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Type = "test",
                    Id = "1",
                    ModName = "test"
                }
            });
            var service = GetService(gameIndexer, storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService);
            var result = await service.LoadDefinitionsAsync(defs, new Game(), new List<string>() { "3.0.3" });
            result.Should().Be(defs);
        }

        /// <summary>
        /// Defines the test method Should_load_definitions.
        /// </summary>
        [Fact]
        public async Task Should_load_definitions()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var gameIndexer = new Mock<IGameIndexer>();
            storageProvider.Setup(p => p.GetRootStoragePath()).Returns("c:\\test");
            gameIndexer.Setup(p => p.GameVersionsSameAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<IEnumerable<string>>())).Returns((string p1, IGame p2, IEnumerable<string> p3) => Task.FromResult(true));
            var gameDefs = new List<IDefinition>() { new Definition() { File = "test\\testgame.txt", Type = "test", Id = "2", ModName = "test game" } };
            gameIndexer.Setup(p => p.GetDefinitionsAsync(It.IsAny<string>(), It.IsAny<IGame>(), It.IsAny<string>())).Returns((string p1, IGame p2, string p3) => Task.FromResult(gameDefs as IEnumerable<IDefinition>));

            var defs = new IndexedDefinitions();
            defs.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    File = "test\\test.txt",
                    Type = "test",
                    Id = "1",
                    ModName = "test"
                }
            });
            var service = GetService(gameIndexer, storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService);
            var result = await service.LoadDefinitionsAsync(defs, new Game() { Name = "fake game" }, new List<string>() { "3.0.3" });
            result.GetAll().Count().Should().Be(2);
            result.GetAll().FirstOrDefault(p => p.ModName == "fake game").Should().NotBeNull();
        }
    }
}
