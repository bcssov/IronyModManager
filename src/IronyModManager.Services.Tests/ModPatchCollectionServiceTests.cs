// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 05-26-2020
//
// Last Modified By : Mario
// Last Modified On : 08-23-2021
// ***********************************************************************
// <copyright file="ModPatchCollectionServiceTests.cs" company="Mario">
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
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Definitions;
using IronyModManager.Parser.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Shared.Models;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;
using FileInfo = IronyModManager.IO.FileInfo;
using ValueType = IronyModManager.Shared.Models.ValueType;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class ModPatchCollectionServiceTests.
    /// </summary>
    public class ModPatchCollectionServiceTests
    {
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <param name="storageProvider">The storage provider.</param>
        /// <param name="modParser">The mod parser.</param>
        /// <param name="parserManager">The parser manager.</param>
        /// <param name="reader">The reader.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="modWriter">The mod writer.</param>
        /// <param name="gameService">The game service.</param>
        /// <param name="modPatchExporter">The mod patch exporter.</param>
        /// <param name="definitionInfoProviders">The definition information providers.</param>
        /// <returns>ModService.</returns>
        private static ModPatchCollectionService GetService(Mock<IStorageProvider> storageProvider, Mock<IModParser> modParser,
            Mock<IParserManager> parserManager, Mock<IReader> reader, Mock<IMapper> mapper, Mock<IModWriter> modWriter,
            Mock<IGameService> gameService, Mock<IModPatchExporter> modPatchExporter, IEnumerable<IDefinitionInfoProvider> definitionInfoProviders = null)
        {
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            return new ModPatchCollectionService(new Cache(), messageBus.Object, parserManager.Object, definitionInfoProviders, modPatchExporter.Object, reader.Object, modWriter.Object, modParser.Object, gameService.Object, storageProvider.Object, mapper.Object);
        }

        /// <summary>
        /// Setups the mock case.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="parserManager">The parser manager.</param>
        /// <param name="modParser">The mod parser.</param>
        private static void SetupMockCase(Mock<IReader> reader, Mock<IParserManager> parserManager, Mock<IModParser> modParser)
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

            modParser.Setup(s => s.Parse(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> values) =>
            {
                return new ModObject()
                {
                    FileName = values.First(),
                    Name = values.First()
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
        /// Defines the test method Should_not_return_any_mod_objects_when_no_game_or_mods.
        /// </summary>
        [Fact]
        public async Task Should_not_return_any_mod_objects_when_no_game_or_mods()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.GetModObjectsAsync(null, new List<IMod>(), string.Empty);
            result.Should().BeNull();

            result = await service.GetModObjectsAsync(new Game(), new List<IMod>(), string.Empty);
            result.Should().BeNull();

            result = await service.GetModObjectsAsync(new Game(), null, string.Empty);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_return_mod_objects_when_using_fully_qualified_path.
        /// </summary>
        [Fact]
        public async Task Should_return_mod_objects_when_using_fully_qualified_path()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.GetModObjectsAsync(new Game() { UserDirectory = "c:\\fake" }, new List<IMod>()
            {
                new Mod()
                {
                    FileName = Assembly.GetExecutingAssembly().Location,
                    Name = "fake"
                }
            }, string.Empty);
            result.GetAll().Count().Should().Be(2);
            var ordered = result.GetAll().OrderBy(p => p.Id);
            ordered.First().Id.Should().Be("fake1.txt");
            ordered.Last().Id.Should().Be("fake2.txt");
        }

        /// <summary>
        /// Defines the test method Should_return_mod_objects_when_using_user_directory.
        /// </summary>
        [Fact]
        public async Task Should_return_mod_objects_when_using_user_directory()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.GetModObjectsAsync(new Game() { UserDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), WorkshopDirectory = new List<string>() { "fake1" } }, new List<IMod>()
            {
                new Mod()
                {
                    FileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location),
                    Name = "fake"
                }
            }, string.Empty);
            result.GetAll().Count().Should().Be(2);
            var ordered = result.GetAll().OrderBy(p => p.Id);
            ordered.First().Id.Should().Be("fake1.txt");
            ordered.Last().Id.Should().Be("fake2.txt");
        }

        /// <summary>
        /// Defines the test method Should_return_mod_objects_when_using_workshop_directory.
        /// </summary>
        [Fact]
        public async Task Should_return_mod_objects_when_using_workshop_directory()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.GetModObjectsAsync(new Game() { WorkshopDirectory = new List<string>() { Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) }, UserDirectory = "fake1" }, new List<IMod>()
            {
                new Mod()
                {
                    FileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location),
                    Name = "fake"
                }
            }, string.Empty);
            result.GetAll().Count().Should().Be(2);
            var ordered = result.GetAll().OrderBy(p => p.Id);
            ordered.First().Id.Should().Be("fake1.txt");
            ordered.Last().Id.Should().Be("fake2.txt");
        }


        /// <summary>
        /// Defines the test method Should_find_filename_conflicts.
        /// </summary>
        [Fact]
        public void Should_find_filename_conflicts()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed, new List<string>(), IronyModManager.Models.Common.PatchStateMode.Default);
            result.Conflicts.GetAll().Count().Should().Be(2);
            result.Conflicts.GetAllFileKeys().Count().Should().Be(1);
            result.OrphanConflicts.GetAll().Count().Should().Be(0);
            result.Conflicts.GetAll().All(p => p.ModName == "test1" || p.ModName == "test2").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_find_orphan_filename_conflicts.
        /// </summary>
        [Fact]
        public void Should_find_orphan_filename_conflicts()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "c",
                    ModName = "test2",
                    ValueType = ValueType.Object
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed, new List<string>(), IronyModManager.Models.Common.PatchStateMode.Default);
            result.Conflicts.GetAll().Count().Should().Be(2);
            result.Conflicts.GetAllFileKeys().Count().Should().Be(1);
            result.OrphanConflicts.GetAll().Count().Should().Be(1);
            result.Conflicts.GetAll().All(p => p.ModName == "test1" || p.ModName == "test2").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_find_definition_conflicts.
        /// </summary>
        [Fact]
        public void Should_find_definition_conflicts()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\2.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed, new List<string>(), IronyModManager.Models.Common.PatchStateMode.Default);
            result.Conflicts.GetAll().Count().Should().Be(2);
            result.Conflicts.GetAllFileKeys().Count().Should().Be(2);
            result.OrphanConflicts.GetAll().Count().Should().Be(0);
            result.Conflicts.GetAll().All(p => p.ModName == "test1" || p.ModName == "test2").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_find_override_conflicts.
        /// </summary>
        [Fact]
        public void Should_not_find_override_conflicts()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test3",
                    ValueType = ValueType.Object,
                    Dependencies = new List<string>() { "test1", "test2" }
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed, new List<string>(), IronyModManager.Models.Common.PatchStateMode.Default);
            result.Conflicts.GetAll().Count().Should().Be(0);
            result.Conflicts.GetAllFileKeys().Count().Should().Be(0);
            result.OrphanConflicts.GetAll().Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_find_dependency_conflicts.
        /// </summary>
        [Fact]
        public void Should_not_find_dependency_conflicts()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test3",
                    ValueType = ValueType.Object,
                    Dependencies = new List<string>() { "test1" }
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed, new List<string>(), IronyModManager.Models.Common.PatchStateMode.Default);
            result.Conflicts.GetAll().Count().Should().Be(0);
            result.Conflicts.GetAllFileKeys().Count().Should().Be(0);
            result.OrphanConflicts.GetAll().Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_find_dependency_conflicts.
        /// </summary>
        [Fact]
        public void Should_find_dependency_conflicts()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test3",
                    ValueType = ValueType.Object,
                    Dependencies = new List<string>() { "test2" }
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed, new List<string>(), IronyModManager.Models.Common.PatchStateMode.Default);
            result.Conflicts.GetAll().Count().Should().Be(2);
            result.Conflicts.GetAllFileKeys().Count().Should().Be(1);
            result.OrphanConflicts.GetAll().Count().Should().Be(0);
            result.Conflicts.GetAll().All(p => p.ModName == "test1" || p.ModName == "test3").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_find_multiple_dependency_conflicts.
        /// </summary>
        [Fact]
        public void Should_find_multiple_dependency_conflicts()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test3",
                    ValueType = ValueType.Object,
                    Dependencies = new List<string>() { "test1", "test2" }
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "f",
                    Type = "events",
                    Id = "a",
                    ModName = "test4",
                    ValueType = ValueType.Object
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed, new List<string>(), IronyModManager.Models.Common.PatchStateMode.Default);
            result.Conflicts.GetAll().Count().Should().Be(2);
            result.Conflicts.GetAllFileKeys().Count().Should().Be(1);
            result.OrphanConflicts.GetAll().Count().Should().Be(0);
            result.Conflicts.GetAll().All(p => p.ModName == "test3" || p.ModName == "test4").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_find_variable_conflicts_in_different_filenames.
        /// </summary>
        [Fact]
        public void Should_not_include_variable_conflicts()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a1",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a2",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Variable
                },
                new Definition()
                {
                    File = "events\\2.txt",
                    Code = "a",
                    Type = "events",
                    Id = "a1",
                    ModName = "test2",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\2.txt",
                    Code = "b",
                    Id = "a2",
                    Type= "events",
                    ModName = "test2",
                    ValueType = ValueType.Variable
                },
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed, new List<string>(), IronyModManager.Models.Common.PatchStateMode.Default);
            result.Conflicts.GetAll().Count().Should().Be(0);
            result.Conflicts.GetAllFileKeys().Count().Should().Be(0);
            result.OrphanConflicts.GetAll().Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_return_all_conflicts.
        /// </summary>
        [Fact]
        public void Should_return_all_conflicts()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();

            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed, new List<string>(), IronyModManager.Models.Common.PatchStateMode.Default);
            result.AllConflicts.GetAll().Count().Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_not_apply_mod_patch_when_no_game_selected_or_parameters_null.
        /// </summary>
        [Fact]
        public async Task Should_not_apply_mod_patch_when_no_game_selected_or_parameters_null()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            var c = new ConflictResult();
            var result = await service.ApplyModPatchAsync(c, new Definition(), "colname");
            result.Should().BeFalse();

            result = await service.ApplyModPatchAsync(null, new Definition(), "colname");
            result.Should().BeFalse();

            result = await service.ApplyModPatchAsync(c, null, "colname");
            result.Should().BeFalse();

            result = await service.ApplyModPatchAsync(c, new Definition(), null);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_apply_mod_patch_when_nothing_to_merge.
        /// </summary>
        [Fact]
        public async Task Should_not_apply_mod_patch_when_nothing_to_merge()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_not_apply_mod_patch_when_nothing_to_merge",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>());
            var c = new ConflictResult()
            {
                AllConflicts = indexed,
                Conflicts = indexed,
                OrphanConflicts = indexed,
                ResolvedConflicts = indexed
            };
            var result = await service.ApplyModPatchAsync(c, new Definition() { ModName = "test", ValueType = ValueType.Object }, "colname");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_return_true_when_applying_patches.
        /// </summary>
        [Fact]
        public async Task Should_return_true_when_applying_patches()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_return_true_when_applying_patches",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fake1.txt", "mod/fake2.txt"},
                    Name = "test",
                    Game = "Should_return_true_when_applying_patches"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            modWriter.Setup(p => p.CreateModDirectoryAsync(It.IsAny<ModWriterParameters>())).Returns(Task.FromResult(true));
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            modPatchExporter.Setup(p => p.SaveStateAsync(It.IsAny<ModPatchExporterParameters>())).Returns(Task.FromResult(true));
            modWriter.Setup(p => p.PurgeModDirectoryAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            modPatchExporter.Setup(p => p.ExportDefinitionAsync(It.IsAny<ModPatchExporterParameters>())).ReturnsAsync((ModPatchExporterParameters p) =>
            {
                if (p.Definitions.Any())
                {
                    return true;
                }
                return false;
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\2.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                },
            };
            var all = new IndexedDefinitions();
            all.InitMap(definitions);

            var orphan = new IndexedDefinitions();
            orphan.InitMap(new List<IDefinition>());

            var resolved = new IndexedDefinitions();
            resolved.InitMap(new List<IDefinition>());

            var c = new ConflictResult()
            {
                AllConflicts = all,
                Conflicts = all,
                OrphanConflicts = orphan,
                ResolvedConflicts = resolved,
                OverwrittenConflicts = orphan
            };
            var result = await service.ApplyModPatchAsync(c, new Definition() { ModName = "1" }, "colname");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_create_patch_definition.
        /// </summary>
        [Fact]
        public async Task Should_not_create_patch_definition()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_not_create_patch_definition",
                UserDirectory = "C:\\Users\\Fake"
            });
            mapper.Setup(s => s.Map<IDefinition>(It.IsAny<IDefinition>())).Returns((IDefinition o) =>
            {
                return new Definition()
                {
                    File = o.File
                };
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.CreatePatchDefinitionAsync(null, "fake");
            result.Should().BeNull();

            result = await service.CreatePatchDefinitionAsync(new Definition() { File = "1" }, null);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_create_patch_definition_when_no_selected_game.
        /// </summary>
        [Fact]
        public async Task Should_not_create_patch_definition_when_no_selected_game()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);
            mapper.Setup(s => s.Map<IDefinition>(It.IsAny<IDefinition>())).Returns((IDefinition o) =>
            {
                return new Definition()
                {
                    File = o.File
                };
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            var result = await service.CreatePatchDefinitionAsync(new Definition() { File = "1" }, "fake");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_create_patch_definition.
        /// </summary>
        [Fact]
        public async Task Should_create_patch_definition()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_create_patch_definition",
                UserDirectory = "C:\\Users\\Fake"
            });
            mapper.Setup(s => s.Map<IDefinition>(It.IsAny<IDefinition>())).Returns((IDefinition o) =>
            {
                return new Definition()
                {
                    File = o.File
                };
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.CreatePatchDefinitionAsync(new Definition() { File = "1" }, "fake");
            result.Should().NotBeNull();
            result.ModName.Should().Be("IronyModManager_fake");
        }

        /// <summary>
        /// Defines the test method Should_create_patch_definition_and_overwrite_code_from_history.
        /// </summary>
        [Fact]
        public async Task Should_create_patch_definition_and_overwrite_code_from_history()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_create_patch_definition_and_overwrite_code_from_history",
                UserDirectory = "C:\\Users\\Fake"
            });
            mapper.Setup(s => s.Map<IDefinition>(It.IsAny<IDefinition>())).Returns((IDefinition o) =>
            {
                return new Definition()
                {
                    File = o.File,
                    Id = o.Id,
                    Type = o.Type
                };
            });
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).ReturnsAsync((ModPatchExporterParameters p, bool load) =>
            {
                var res = new PatchState()
                {
                    Conflicts = new List<IDefinition>(),
                    ResolvedConflicts = new List<IDefinition>(),
                    OrphanConflicts = new List<IDefinition>(),
                    ConflictHistory = new List<IDefinition>() { new Definition() { File = "1", Id = "test", Type = "events", Code = "ab" } }
                };
                return res;
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.CreatePatchDefinitionAsync(new Definition() { File = "1", Id = "test", Type = "events", Code = "a" }, "fake");
            result.Should().NotBeNull();
            result.ModName.Should().Be("IronyModManager_fake");
            result.Code.Should().Be("ab");
        }

        /// <summary>
        /// Defines the test method Should_not_initialize_patch_state_when_no_selected_game.
        /// </summary>
        [Fact]
        public async Task Should_not_initialize_patch_state_when_no_selected_game()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>());
            var c = new ConflictResult()
            {
                AllConflicts = indexed,
                Conflicts = indexed,
                OrphanConflicts = indexed,
                ResolvedConflicts = indexed
            };
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.InitializePatchStateAsync(c, "fake");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_initialize_patch_state.
        /// </summary>
        [Fact]
        public async Task Should_not_initialize_patch_state()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>());
            var c = new ConflictResult()
            {
                AllConflicts = indexed,
                Conflicts = indexed,
                OrphanConflicts = indexed,
                ResolvedConflicts = indexed
            };
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.InitializePatchStateAsync(c, null);
            result.Should().BeNull();

            result = await service.InitializePatchStateAsync(null, "fake");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_initialize_patch_state.
        /// </summary>
        [Fact]
        public async Task Should_initialize_patch_state()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_sync_patch_state",
                UserDirectory = "C:\\Users\\Fake"
            });
            mapper.Setup(s => s.Map<IConflictResult>(It.IsAny<IConflictResult>())).Returns((IConflictResult o) =>
            {
                return new ConflictResult()
                {
                    AllConflicts = o.Conflicts,
                    Conflicts = o.Conflicts,
                    OrphanConflicts = o.OrphanConflicts,
                    ResolvedConflicts = o.ResolvedConflicts,
                    OverwrittenConflicts = o.OverwrittenConflicts
                };
            });
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\2.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                },
            };
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).ReturnsAsync((ModPatchExporterParameters p, bool load) =>
            {
                var res = new PatchState()
                {
                    Conflicts = definitions,
                    ResolvedConflicts = definitions,
                    OrphanConflicts = new List<IDefinition>(),
                    OverwrittenConflicts = new List<IDefinition>()
                };
                return res;
            });
            modWriter.Setup(p => p.PurgeModDirectoryAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));

            var all = new IndexedDefinitions();
            all.InitMap(definitions);

            var conflicts = new IndexedDefinitions();
            conflicts.InitMap(definitions);

            var orphan = new IndexedDefinitions();
            orphan.InitMap(new List<IDefinition>());

            var resolved = new IndexedDefinitions();
            resolved.InitMap(new List<IDefinition>());

            var c = new ConflictResult()
            {
                AllConflicts = all,
                Conflicts = conflicts,
                OrphanConflicts = orphan,
                OverwrittenConflicts = orphan
            };
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.InitializePatchStateAsync(c, "fake");
            result.Conflicts.GetAll().Count().Should().Be(2);
            result.ResolvedConflicts.GetAll().Count().Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_initialize_patch_state_and_remove_different.
        /// </summary>
        [Fact]
        public async Task Should_initialize_patch_state_and_remove_different()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_sync_patch_state_and_remove_different",
                UserDirectory = "C:\\Users\\Fake"
            });
            mapper.Setup(s => s.Map<IConflictResult>(It.IsAny<IConflictResult>())).Returns((IConflictResult o) =>
            {
                return new ConflictResult()
                {
                    AllConflicts = o.Conflicts,
                    Conflicts = o.Conflicts,
                    OrphanConflicts = o.OrphanConflicts,
                    ResolvedConflicts = o.ResolvedConflicts,
                    OverwrittenConflicts = o.OverwrittenConflicts
                };
            });
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\2.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                },
            };
            var definitions2 = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "ab",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\2.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                }
            };
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).ReturnsAsync((ModPatchExporterParameters p, bool load) =>
            {
                var res = new PatchState()
                {
                    Conflicts = definitions2,
                    ResolvedConflicts = definitions2,
                    OrphanConflicts = new List<IDefinition>(),
                    OverwrittenConflicts = new List<IDefinition>()
                };
                return res;
            });
            modWriter.Setup(p => p.PurgeModDirectoryAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));

            var all = new IndexedDefinitions();
            all.InitMap(definitions);

            var conflicts = new IndexedDefinitions();
            conflicts.InitMap(definitions);

            var orphan = new IndexedDefinitions();
            orphan.InitMap(new List<IDefinition>());

            var resolved = new IndexedDefinitions();
            resolved.InitMap(new List<IDefinition>());

            var c = new ConflictResult()
            {
                AllConflicts = all,
                Conflicts = conflicts,
                OrphanConflicts = orphan,
                OverwrittenConflicts = orphan
            };
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.InitializePatchStateAsync(c, "fake");
            result.Conflicts.GetAll().Count().Should().Be(2);
            result.ResolvedConflicts.GetAll().Count().Should().Be(0);
        }


        /// <summary>
        /// Defines the test method Should_not_be_a_patch_mod.
        /// </summary>
        [Fact]
        public void Should_not_be_a_patch_mod()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.IsPatchMod(new Mod()
            {
                Name = "test"
            });
            result.Should().BeFalse();
            result = service.IsPatchMod(default(Mod));
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_be_a_patch_mod.
        /// </summary>
        [Fact]
        public void Should_be_a_patch_mod()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.IsPatchMod(new Mod()
            {
                Name = "IronyModManager_fake_collection"
            });
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_clean_collection_patch.
        /// </summary>
        [Fact]
        public async Task Should_not_clean_collection_patch()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var result = await service.CleanPatchCollectionAsync("fake");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_clean_collection_patch.
        /// </summary>
        [Fact]
        public async Task Should_clean_collection_patch()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_clean_collection_patch",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\workshop" }
            });
            modWriter.Setup(p => p.DeleteDescriptorAsync(It.IsAny<ModWriterParameters>())).Returns(Task.FromResult(true));
            modWriter.Setup(p => p.PurgeModDirectoryAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));

            var result = await service.CleanPatchCollectionAsync("fake");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_ignore_mod_patch_when_no_game_selected_or_parameters_null.
        /// </summary>
        [Fact]
        public async Task Should_not_ignore_mod_patch_when_no_game_selected_or_parameters_null()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            var c = new ConflictResult();
            var result = await service.IgnoreModPatchAsync(c, new Definition(), "colname");
            result.Should().BeFalse();

            result = await service.IgnoreModPatchAsync(null, new Definition(), "colname");
            result.Should().BeFalse();

            result = await service.IgnoreModPatchAsync(c, null, "colname");
            result.Should().BeFalse();

            result = await service.IgnoreModPatchAsync(c, new Definition(), null);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_ignore_mod_patch_when_nothing_to_merge.
        /// </summary>
        [Fact]
        public async Task Should_not_ignore_mod_patch_when_nothing_to_merge()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_not_ignore_mod_patch_when_nothing_to_merge",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>());
            var c = new ConflictResult()
            {
                AllConflicts = indexed,
                Conflicts = indexed,
                OrphanConflicts = indexed,
                ResolvedConflicts = indexed,
                IgnoredConflicts = indexed
            };
            var result = await service.IgnoreModPatchAsync(c, new Definition() { ModName = "test", ValueType = ValueType.Object }, "colname");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_return_true_when_ignoring_patches.
        /// </summary>
        [Fact]
        public async Task Should_return_true_when_ignoring_patches()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_return_true_when_ignoring_patches",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            modWriter.Setup(p => p.CreateModDirectoryAsync(It.IsAny<ModWriterParameters>())).Returns(Task.FromResult(true));
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            modPatchExporter.Setup(p => p.SaveStateAsync(It.IsAny<ModPatchExporterParameters>())).Returns(Task.FromResult(true));
            modWriter.Setup(p => p.PurgeModDirectoryAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            modPatchExporter.Setup(p => p.ExportDefinitionAsync(It.IsAny<ModPatchExporterParameters>())).ReturnsAsync((ModPatchExporterParameters p) =>
            {
                if (p.Definitions.Any())
                {
                    return true;
                }
                return false;
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\2.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                },
            };
            var all = new IndexedDefinitions();
            all.InitMap(definitions);

            var orphan = new IndexedDefinitions();
            orphan.InitMap(new List<IDefinition>());

            var resolved = new IndexedDefinitions();
            resolved.InitMap(new List<IDefinition>());

            var ignored = new IndexedDefinitions();
            ignored.InitMap(new List<IDefinition>());


            var c = new ConflictResult()
            {
                AllConflicts = all,
                Conflicts = all,
                OrphanConflicts = orphan,
                ResolvedConflicts = resolved,
                IgnoredConflicts = ignored
            };
            var result = await service.IgnoreModPatchAsync(c, new Definition() { ModName = "1" }, "colname");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_null.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_null()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var result = service.EvalDefinitionPriority(null);
            result.Definition.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_first_object.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_first_object()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_first_object",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition();
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def });
            result.Definition.Should().Be(def);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_first_game_object.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_first_game_object()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_first_game_object",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { IsFromGame = true };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def });
            result.Definition.Should().Be(def);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_first_only_valid_object.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_first_only_valid_object()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_first_only_valid_object",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { };
            var def2 = new Definition() { ExistsInLastFile = false };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2 });
            result.Definition.Should().Be(def);
            result.PriorityType.Should().Be(DefinitionPriorityType.ModOrder);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_last_object.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_last_object()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_last_object",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = "test.txt", ModName = "1" };
            var def2 = new Definition() { File = "test.txt", ModName = "2" };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2 });
            result.Definition.Should().Be(def2);
            result.PriorityType.Should().Be(DefinitionPriorityType.ModOrder);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_last_object_as_cutom_patch.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_last_object_as_cutom_patch()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_last_object_as_cutom_patch",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = "test.txt", ModName = "1", IsCustomPatch = true };
            var def2 = new Definition() { File = "test.txt", ModName = "2" };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2 });
            result.Definition.Should().Be(def);
            result.PriorityType.Should().Be(DefinitionPriorityType.ModOrder);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_last_non_game_object.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_last_non_game_object()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_last_non_game_object",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = "test.txt", ModName = "1" };
            var def2 = new Definition() { File = "test.txt", ModName = "2" };
            var def3 = new Definition() { File = "test.txt", ModName = "Game", IsFromGame = true };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def3, def, def2 });
            result.Definition.Should().Be(def2);
            result.PriorityType.Should().Be(DefinitionPriorityType.ModOrder);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_first_object_due_to_FIOS.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_first_object_due_to_FIOS()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_first_object_due_to_FIOS",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = "test1.txt", ModName = "1" };
            var def2 = new Definition() { File = "test2.txt", ModName = "2" };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2 });
            result.Definition.Should().Be(def);
            result.PriorityType.Should().Be(DefinitionPriorityType.FIOS);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_first_object_due_to_FIOS_and_ignore_game_object.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_first_object_due_to_FIOS_and_ignore_game_object()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_first_object_due_to_FIOS_and_ignore_game_object",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = "test1.txt", ModName = "1" };
            var def2 = new Definition() { File = "test2.txt", ModName = "2" };
            var def3 = new Definition() { File = "test1.txt", ModName = "Game", IsFromGame = true };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def3, def, def2 });
            result.Definition.Should().Be(def);
            result.PriorityType.Should().Be(DefinitionPriorityType.FIOS);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_object_due_to_Override.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_object_due_to_Override()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_object_due_to_Override",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = "test1.txt", ModName = "1", Dependencies = new List<string>() { "2" } };
            var def2 = new Definition() { File = "test1.txt", ModName = "2" };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2 });
            result.Definition.Should().Be(def);
            result.PriorityType.Should().Be(DefinitionPriorityType.ModOverride);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_object_due_to_game_object_filtering.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_object_due_to_game_object_filtering()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_object_due_to_game_object_filtering",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = "test1.txt", ModName = "1", IsFromGame = true };
            var def2 = new Definition() { File = "test1.txt", ModName = "2" };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2 });
            result.Definition.Should().Be(def2);
            result.PriorityType.Should().Be(DefinitionPriorityType.ModOrder);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_object_due_to_Override_and_ignore_game_object.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_object_due_to_Override_and_ignore_game_object()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_object_due_to_Override_and_ignore_game_object",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = "test1.txt", ModName = "1", Dependencies = new List<string>() { "2" } };
            var def2 = new Definition() { File = "test1.txt", ModName = "2" };
            var def3 = new Definition() { File = "test1.txt", ModName = "Game", IsFromGame = true };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def3, def, def2 });
            result.Definition.Should().Be(def);
            result.PriorityType.Should().Be(DefinitionPriorityType.ModOverride);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_object_due_to_override_but_priority_should_be_fios.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_object_due_to_override_but_priority_should_be_fios()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_object_due_to_override_but_priority_should_be_fios",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = "test1.txt", ModName = "1", Dependencies = new List<string>() { "2" } };
            var def2 = new Definition() { File = "test2.txt", ModName = "2" };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2 });
            result.Definition.Should().Be(def);
            result.PriorityType.Should().Be(DefinitionPriorityType.FIOS);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_object_due_to_override_and_ignore_non_game_object_and_priority_should_be_fios.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_object_due_to_override_and_ignore_non_game_object_and_priority_should_be_fios()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_object_due_to_override_and_ignore_non_game_object_and_priority_should_be_fios",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = "test1.txt", ModName = "1", Dependencies = new List<string>() { "2" } };
            var def2 = new Definition() { File = "test2.txt", ModName = "2" };
            var def3 = new Definition() { File = "test2.txt", ModName = "Game", IsFromGame = true };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def3, def, def2 });
            result.Definition.Should().Be(def);
            result.PriorityType.Should().Be(DefinitionPriorityType.FIOS);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_first_object_due_to_LIOS.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_first_object_due_to_LIOS()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_first_object_due_to_LIOS",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = "test1.txt", ModName = "1" };
            var def2 = new Definition() { File = "test2.txt", ModName = "2" };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2 });
            result.Definition.Should().Be(def2);
            result.PriorityType.Should().Be(DefinitionPriorityType.LIOS);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_first_object_due_to_LIOS_and_ignore_non_game_object.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_first_object_due_to_LIOS_and_ignore_non_game_object()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_first_object_due_to_LIOS_and_ignore_non_game_object",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = "test1.txt", ModName = "1" };
            var def2 = new Definition() { File = "test2.txt", ModName = "2" };
            var def3 = new Definition() { File = "test2.txt", ModName = "Game", IsFromGame = true };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def3, def, def2 });
            result.Definition.Should().Be(def2);
            result.PriorityType.Should().Be(DefinitionPriorityType.LIOS);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_localization_override.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_localization_override()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_localization_override",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = @"localisation\test.yml", ModName = "1" };
            var def2 = new Definition() { File = @"localisation\replace\test.yml", ModName = "2" };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2 });
            result.Definition.Should().Be(def2);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_localization.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_localization()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_localization",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = @"localisation\test.yml", ModName = "1" };
            var def2 = new Definition() { File = @"localisation\test.yml", ModName = "2" };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2 });
            result.Definition.Should().Be(def2);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_localization_override_with_custom_priority.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_localization_override_with_custom_priority()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_localization_override_with_custom_priority",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = @"localisation\test.yml", ModName = "1" };
            var def2 = new Definition() { File = @"localisation\replace\test.yml", ModName = "2", CustomPriorityOrder = 5 };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2 });
            result.Definition.Should().Be(def2);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_localization_with_custom_priority.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_localization_with_custom_priority()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_localization_with_custom_priority",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = @"localisation\test.yml", ModName = "1" };
            var def2 = new Definition() { File = @"localisation\test.yml", ModName = "2", CustomPriorityOrder = 5 };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2 });
            result.Definition.Should().Be(def2);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_localization_override_and_filename_priority.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_localization_override_and_filename_priority()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_localization_override_and_filename_priority",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = @"localisation\test.yml", ModName = "1" };
            var def2 = new Definition() { File = @"localisation\replace\test.yml", ModName = "2" };
            var def3 = new Definition() { File = @"localisation\replace\test2.yml", ModName = "3" };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2, def3 });
            result.Definition.Should().Be(def3);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_localization_and_filename_priority.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_localization_and_filename_priority()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_localization_and_filename_priority",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = @"localisation\test.yml", ModName = "1" };
            var def2 = new Definition() { File = @"localisation\test.yml", ModName = "2" };
            var def3 = new Definition() { File = @"localisation\test2.yml", ModName = "3" };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2, def3 });
            result.Definition.Should().Be(def3);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_localization_override_with_multiple_custom_priority.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_localization_override_with_multiple_custom_priority()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_localization_override_with_multiple_custom_priority",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = @"localisation\test.yml", ModName = "1" };
            var def2 = new Definition() { File = @"localisation\replace\test.yml", ModName = "2", CustomPriorityOrder = 1000 };
            var def3 = new Definition() { File = @"localisation\replace\test2.yml", ModName = "3", CustomPriorityOrder = 100 };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2, def3 });
            result.Definition.Should().Be(def2);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_localization_with_multiple_custom_priority.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_localization_with_multiple_custom_priority()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_localization_with_multiple_custom_priority",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = @"localisation\test.yml", ModName = "1" };
            var def2 = new Definition() { File = @"localisation\test.yml", ModName = "2", CustomPriorityOrder = 1000 };
            var def3 = new Definition() { File = @"localisation\test2.yml", ModName = "3", CustomPriorityOrder = 100 };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2, def3 });
            result.Definition.Should().Be(def2);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_localization_override_with_custom_priority_and_filename_priority.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_localization_override_with_custom_priority_and_filename_priority()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_localization_override_with_custom_priority_and_filename_priority",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = @"localisation\test.yml", ModName = "1" };
            var def2 = new Definition() { File = @"localisation\replace\test.yml", ModName = "2", CustomPriorityOrder = 1000 };
            var def3 = new Definition() { File = @"localisation\replace\test2.yml", ModName = "3", CustomPriorityOrder = 1000 };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2, def3 });
            result.Definition.Should().Be(def3);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_localization_with_custom_priority_and_filename_priority.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_localization_with_custom_priority_and_filename_priority()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_localization_with_custom_priority_and_filename_priority",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = @"localisation\test.yml", ModName = "1" };
            var def2 = new Definition() { File = @"localisation\replace\test.yml", ModName = "2", CustomPriorityOrder = 1000 };
            var def3 = new Definition() { File = @"localisation\replace\test2.yml", ModName = "3", CustomPriorityOrder = 1000 };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def, def2, def3 });
            result.Definition.Should().Be(def3);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_localization_override_and_ignore_non_game_definitions.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_localization_override_and_ignore_non_game_definitions()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_localization_override_and_ignore_non_game_definitions",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = @"localisation\test.yml", ModName = "1" };
            var def2 = new Definition() { File = @"localisation\replace\test2.yml", ModName = "2" };
            var def3 = new Definition() { File = @"localisation\replace\test.yml", ModName = "Game", IsFromGame = true };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def3, def, def2 });
            result.Definition.Should().Be(def2);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method EvalDefinitionPriority_should_return_localization_and_ignore_non_game_definitions.
        /// </summary>
        [Fact]
        public void EvalDefinitionPriority_should_return_localization_and_ignore_non_game_definitions()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "EvalDefinitionPriority_should_return_localization_and_ignore_non_game_definitions",
                UserDirectory = "C:\\Users\\Fake"
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(false);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, new List<IDefinitionInfoProvider>() { infoProvider.Object });

            var def = new Definition() { File = @"localisation\test.yml", ModName = "1" };            
            var def3 = new Definition() { File = @"localisation\test.yml", ModName = "Game", IsFromGame = true };
            var result = service.EvalDefinitionPriority(new List<IDefinition>() { def3, def });
            result.Definition.Should().Be(def);
            result.PriorityType.Should().Be(DefinitionPriorityType.None);
        }

        /// <summary>
        /// Defines the test method SaveIgnoredPathsAsync_should_be_false_when_game_is_null.
        /// </summary>
        [Fact]
        public async Task SaveIgnoredPathsAsync_should_be_false_when_game_is_null()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);
            modPatchExporter.Setup(p => p.SaveStateAsync(It.IsAny<ModPatchExporterParameters>())).Returns(Task.FromResult(false));
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var result = await service.SaveIgnoredPathsAsync(new ConflictResult(), "test");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method SaveIgnoredPathsAsync_should_be_false.
        /// </summary>
        [Fact]
        public async Task SaveIgnoredPathsAsync_should_be_false()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "SaveIgnoredPathsAsync_should_be_false",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\Fake" },
                CustomModDirectory = string.Empty
            });
            modPatchExporter.Setup(p => p.SaveStateAsync(It.IsAny<ModPatchExporterParameters>())).Returns(Task.FromResult(false));
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fake1.txt", "mod/fake2.txt"},
                    Name = "test",
                    Game = "SaveIgnoredPathsAsync_should_be_false"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var indexed = new IndexedDefinitions();
            var result = await service.SaveIgnoredPathsAsync(new ConflictResult() { AllConflicts = indexed, Conflicts = indexed }, "test");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method SaveIgnoredPathsAsync_should_be_true.
        /// </summary>
        [Fact]
        public async Task SaveIgnoredPathsAsync_should_be_true()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "SaveIgnoredPathsAsync_should_be_true",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\Fake" },
                CustomModDirectory = string.Empty
            });
            modPatchExporter.Setup(p => p.SaveStateAsync(It.IsAny<ModPatchExporterParameters>())).Returns(Task.FromResult(true));
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fake1.txt", "mod/fake2.txt"},
                    Name = "test",
                    Game = "SaveIgnoredPathsAsync_should_be_false"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var indexed = new IndexedDefinitions();
            var result = await service.SaveIgnoredPathsAsync(new ConflictResult() { AllConflicts = indexed, Conflicts = indexed }, "test");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method CopyPatchMod_should_be_false_when_no_game.
        /// </summary>
        [Fact]
        public async Task CopyPatchMod_should_be_false_when_no_game()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);
            modPatchExporter.Setup(p => p.CopyPatchModAsync(It.IsAny<ModPatchExporterParameters>())).Returns(Task.FromResult(false));
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var result = await service.CopyPatchCollectionAsync("t1", "t2");
            result.Should().BeFalse();
        }


        /// <summary>
        /// Defines the test method CopyPatchMod_should_be_false.
        /// </summary>
        [Fact]
        public async Task CopyPatchMod_should_be_false()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "CopyPatchMod_should_be_false",
                UserDirectory = "C:\\Users\\Fake"
            });
            modPatchExporter.Setup(p => p.CopyPatchModAsync(It.IsAny<ModPatchExporterParameters>())).Returns(Task.FromResult(false));
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var result = await service.CopyPatchCollectionAsync("t1", "t2");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method CopyPatchMod_should_be_true.
        /// </summary>
        [Fact]
        public async Task CopyPatchMod_should_be_true()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "CopyPatchMod_should_be_true",
                UserDirectory = "C:\\Users\\Fake"
            });
            modPatchExporter.Setup(p => p.CopyPatchModAsync(It.IsAny<ModPatchExporterParameters>())).Returns(Task.FromResult(true));
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var result = await service.CopyPatchCollectionAsync("t1", "t2");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method RenamePatchMod_should_be_false_when_no_game.
        /// </summary>
        [Fact]
        public async Task RenamePatchMod_should_be_false_when_no_game()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);
            modPatchExporter.Setup(p => p.RenamePatchModAsync(It.IsAny<ModPatchExporterParameters>())).Returns(Task.FromResult(false));
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var result = await service.RenamePatchCollectionAsync("t1", "t2");
            result.Should().BeFalse();
        }


        /// <summary>
        /// Defines the test method CopyPatchMod_should_be_false.
        /// </summary>
        [Fact]
        public async Task RenamePatchMod_should_be_false()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "RenamePatchMod_should_be_false",
                UserDirectory = "C:\\Users\\Fake"
            });
            modPatchExporter.Setup(p => p.RenamePatchModAsync(It.IsAny<ModPatchExporterParameters>())).Returns(Task.FromResult(false));
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var result = await service.RenamePatchCollectionAsync("t1", "t2");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method RenamePatchMod_should_be_true.
        /// </summary>
        [Fact]
        public async Task RenamePatchMod_should_be_true()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "RenamePatchMod_should_be_true",
                UserDirectory = "C:\\Users\\Fake"
            });
            modPatchExporter.Setup(p => p.RenamePatchModAsync(It.IsAny<ModPatchExporterParameters>())).Returns(Task.FromResult(true));
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var result = await service.RenamePatchCollectionAsync("t1", "t2");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method ResetCache_should_be_true.
        /// </summary>
        [Fact]
        public void ResetCache_should_be_true()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "ResetCache_should_be_true",
                UserDirectory = "C:\\Users\\Fake"
            });
            modPatchExporter.Setup(p => p.ResetCache());
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var result = service.ResetPatchStateCache();
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_get_patch_state_when_no_selected_game.
        /// </summary>
        [Fact]
        public async Task Should_not_get_patch_state_when_no_selected_game()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.GetPatchStateModeAsync("fake");
            result.Should().Be(IronyModManager.Models.Common.PatchStateMode.None);
        }

        /// <summary>
        /// Defines the test method Should_not_get_patch_state_when_no_collection.
        /// </summary>
        [Fact]
        public async Task Should_not_get_patch_state_when_no_collection()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.GetPatchStateModeAsync(null);
            result.Should().Be(IronyModManager.Models.Common.PatchStateMode.None);
        }

        /// <summary>
        /// Defines the test method Should_get_patch_state.
        /// </summary>
        [Fact]
        public async Task Should_get_patch_state()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).ReturnsAsync((ModPatchExporterParameters p, bool load) =>
            {
                var res = new PatchState()
                {
                    Conflicts = new List<IDefinition>(),
                    ResolvedConflicts = new List<IDefinition>(),
                    OrphanConflicts = new List<IDefinition>(),
                    OverwrittenConflicts = new List<IDefinition>(),
                    Mode = IO.Common.PatchStateMode.Default
                };
                return res;
            });
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_get_patch_state",
                UserDirectory = "C:\\Users\\Fake"
            });

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.GetPatchStateModeAsync("fake");
            result.Should().Be(IronyModManager.Models.Common.PatchStateMode.Default);
        }

        /// <summary>
        /// Defines the test method Should_not_resolve_full_definition_path_when_no_game_or_definition_null.
        /// </summary>
        [Fact]
        public void Should_not_resolve_full_definition_path_when_no_game()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.ResolveFullDefinitionPath(new Definition()
            {
                File = "events\\test.txt",
                ModName = "test"
            });
            result.Should().Be(string.Empty);
        }

        /// <summary>
        /// Defines the test method Should_not_resolve_full_definition_path_when_definition_null.
        /// </summary>
        [Fact]
        public void Should_not_resolve_full_definition_path_when_definition_null()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_not_resolve_full_definition_path_when_definition_null",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\fake" }
            });
            SetupMockCase(reader, parserManager, modParser);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.ResolveFullDefinitionPath(null);
            result.Should().Be(string.Empty);
        }

        /// <summary>
        /// Defines the test method Should_resolve_full_definition_path.
        /// </summary>
        [Fact]
        public void Should_resolve_full_definition_path()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_resolve_full_definition_path",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            SetupMockCase(reader, parserManager, modParser);
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fakemod.mod"},
                    Name = "test",
                    Game = "Should_resolve_full_definition_path"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            var fileInfos = new List<IFileInfo>()
            {
                new FileInfo()
                {
                    Content = new List<string>() { "1" },
                    FileName = "fakemod.mod",
                    IsBinary = false
                }
            };
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos);
            modParser.Setup(s => s.Parse(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> values) =>
            {
                return new ModObject()
                {
                    FileName = "fakemod",
                    Name = "1"
                };
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.ResolveFullDefinitionPath(new Definition()
            {
                File = "events\\test.txt",
                ModName = "1"
            });
            result.Should().Be(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod\\fakemod\\events\\test.txt"));
        }

        /// <summary>
        /// Defines the test method Should_resolve_full_definition_archive_path.
        /// </summary>
        [Fact]
        public void Should_resolve_full_definition_archive_path()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_resolve_full_definition_archive_path",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            SetupMockCase(reader, parserManager, modParser);
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fakemod.mod"},
                    Name = "test",
                    Game = "Should_resolve_full_definition_archive_path"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            var fileInfos = new List<IFileInfo>()
            {
                new FileInfo()
                {
                    Content = new List<string>() { "1" },
                    FileName = "fakemod.mod",
                    IsBinary = false
                }
            };
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos);
            modParser.Setup(s => s.Parse(It.IsAny<IEnumerable<string>>())).Returns((IEnumerable<string> values) =>
            {
                return new ModObject()
                {
                    FileName = "fakemod.zip",
                    Name = "1"
                };
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.ResolveFullDefinitionPath(new Definition()
            {
                File = "events\\test.txt",
                ModName = "1"
            });
            result.Should().Be(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod\\fakemod.zip"));
        }

        /// <summary>
        /// Defines the test method Should_add_mods_to_ignore_list.
        /// </summary>
        [Fact]
        public void Should_add_mods_to_ignore_list()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var c = new ConflictResult()
            {
                IgnoredPaths = "modName:a"
            };
            service.AddModsToIgnoreList(c, new List<string>() { "a", "b" });
            c.IgnoredPaths.Should().Be("modName:a" + Environment.NewLine + "modName:b");
        }

        /// <summary>
        /// Defines the test method Should_get_ignored_mods.
        /// </summary>
        [Fact]
        public void Should_get_ignored_mods()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.GetIgnoredMods(new ConflictResult()
            {
                IgnoredPaths = "modName:a" + Environment.NewLine + "modName:b"
            });
            result.Count.Should().Be(2);
            result[0].Should().Be("a");
            result[result.Count - 1].Should().Be("b");
        }

        /// <summary>
        /// Defines the test method Should_reset_resolved_conflict.
        /// </summary>
        [Fact]
        public async Task Should_reset_resolved_conflict()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_reset_resolved_conflict",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" }
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            var resolved = new IndexedDefinitions();
            resolved.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Type = "test",
                    Id = "1",
                    ModName = "test"
                }
            });

            var c = new ConflictResult()
            {
                AllConflicts = new IndexedDefinitions(),
                ResolvedConflicts = resolved
            };
            var result = await service.ResetResolvedConflictAsync(c, "test-1", "fake");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_reset_resolved_conflict.
        /// </summary>
        [Fact]
        public async Task Should_not_reset_resolved_conflict()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_not_reset_resolved_conflict",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" }
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var resolved = new IndexedDefinitions();
            resolved.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Type = "test",
                    Id = "1",
                    ModName = "test"
                }
            });

            var c = new ConflictResult()
            {
                ResolvedConflicts = resolved
            };
            var result = await service.ResetResolvedConflictAsync(c, "test-2", "fake");
            result.Should().BeFalse();
        }


        /// <summary>
        /// Defines the test method Should_reset_ignored_conflict.
        /// </summary>
        [Fact]
        public async Task Should_reset_ignored_conflict()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_reset_ignored_conflict",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" }
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            var ignored = new IndexedDefinitions();
            ignored.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Type = "test",
                    Id = "1",
                    ModName = "test"
                }
            });

            var c = new ConflictResult()
            {
                AllConflicts = new IndexedDefinitions(),
                IgnoredConflicts = ignored
            };
            var result = await service.ResetIgnoredConflictAsync(c, "test-1", "fake");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_reset_ignored_conflict.
        /// </summary>
        [Fact]
        public async Task Should_not_reset_ignored_conflict()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_not_reset_ignored_conflict",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" }
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            var ignored = new IndexedDefinitions();
            ignored.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Type = "test",
                    Id = "1",
                    ModName = "test"
                }
            });

            var c = new ConflictResult()
            {
                IgnoredConflicts = ignored
            };
            var result = await service.ResetIgnoredConflictAsync(c, "test-2", "fake");
            result.Should().BeFalse();
        }


        /// <summary>
        /// Defines the test method Should_not_apply_mod_custom_patch_when_no_game_selected_or_parameters_null.
        /// </summary>
        [Fact]
        public async Task Should_not_apply_mod_custom_patch_when_no_game_selected_or_parameters_null()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            var c = new ConflictResult();
            var result = await service.AddCustomModPatchAsync(c, new Definition(), "colname");
            result.Should().BeFalse();

            result = await service.AddCustomModPatchAsync(null, new Definition(), "colname");
            result.Should().BeFalse();

            result = await service.AddCustomModPatchAsync(c, null, "colname");
            result.Should().BeFalse();

            result = await service.AddCustomModPatchAsync(c, new Definition(), null);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_apply_custom_mod_patch_when_nothing_to_merge.
        /// </summary>
        [Fact]
        public async Task Should_not_apply_custom_mod_patch_when_nothing_to_merge()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_not_apply_custom_mod_patch_when_nothing_to_merge",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            SetupMockCase(reader, parserManager, modParser);
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>());
            var c = new ConflictResult()
            {
                AllConflicts = indexed,
                Conflicts = indexed,
                OrphanConflicts = indexed,
                ResolvedConflicts = indexed,
                CustomConflicts = indexed,
            };
            var result = await service.AddCustomModPatchAsync(c, new Definition() { ModName = "test", ValueType = ValueType.Object }, "colname");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_return_true_when_applying_custom_patches.
        /// </summary>
        [Fact]
        public async Task Should_return_true_when_applying_custom_patches()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_return_true_when_applying_custom_patches",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fake1.txt", "mod/fake2.txt"},
                    Name = "test",
                    Game = "Should_return_true_when_applying_custom_patches"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            modWriter.Setup(p => p.CreateModDirectoryAsync(It.IsAny<ModWriterParameters>())).Returns(Task.FromResult(true));
            modWriter.Setup(p => p.WriteDescriptorAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            modPatchExporter.Setup(p => p.SaveStateAsync(It.IsAny<ModPatchExporterParameters>())).Returns(Task.FromResult(true));
            modWriter.Setup(p => p.PurgeModDirectoryAsync(It.IsAny<ModWriterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult(true));
            modPatchExporter.Setup(p => p.ExportDefinitionAsync(It.IsAny<ModPatchExporterParameters>())).ReturnsAsync((ModPatchExporterParameters p) =>
            {
                if (p.CustomConflicts.Any())
                {
                    return true;
                }
                return false;
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = ValueType.Object
                },
                new Definition()
                {
                    File = "events\\2.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = ValueType.Object
                },
            };
            var all = new IndexedDefinitions();
            all.InitMap(definitions);

            var orphan = new IndexedDefinitions();
            orphan.InitMap(new List<IDefinition>());

            var resolved = new IndexedDefinitions();
            resolved.InitMap(new List<IDefinition>());

            var custom = new IndexedDefinitions();
            custom.InitMap(new List<IDefinition>());

            var c = new ConflictResult()
            {
                AllConflicts = all,
                Conflicts = all,
                OrphanConflicts = orphan,
                ResolvedConflicts = resolved,
                CustomConflicts = custom,
                OverwrittenConflicts = custom
            };
            var result = await service.AddCustomModPatchAsync(c, new Definition() { ModName = "1" }, "colname");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_reset_custom_conflict.
        /// </summary>
        [Fact]
        public async Task Should_reset_custom_conflict()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_reset_custom_conflict",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" }
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);

            var custom = new IndexedDefinitions();
            custom.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Type = "test",
                    Id = "1",
                    ModName = "test"
                }
            });

            var c = new ConflictResult()
            {
                AllConflicts = new IndexedDefinitions(),
                CustomConflicts = custom
            };
            var result = await service.ResetCustomConflictAsync(c, "test-1", "fake");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_reset_custom_conflict.
        /// </summary>
        [Fact]
        public async Task Should_not_reset_custom_conflict()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_not_reset_custom_conflict",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" }
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var custom = new IndexedDefinitions();
            custom.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Type = "test",
                    Id = "1",
                    ModName = "test"
                }
            });

            var c = new ConflictResult()
            {
                CustomConflicts = custom
            };
            var result = await service.ResetCustomConflictAsync(c, "test-2", "fake");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_invalidate_mod_patch_state_if_no_selected_game.
        /// </summary>
        [Fact]
        public void Should_not_invalidate_mod_patch_state_if_no_selected_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.ResetPatchStateCache();
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_invalidate_mod_patch_state.
        /// </summary>
        [Fact]
        public void Should_invalidate_mod_patch_state()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_invalidate_mod_patch_state",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" }
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.ResetPatchStateCache();
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Patch_mod_should_not_need_update_when_no_game.
        /// </summary>
        [Fact]
        public async Task Patch_mod_should_not_need_update_when_no_game()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.PatchModNeedsUpdateAsync("test", null);
            result.Should().BeFalse();
        }
        /// <summary>
        /// Defines the test method Patch_mod_should_not_need_update_when_no_collection.
        /// </summary>
        [Fact]
        public async Task Patch_mod_should_not_need_update_when_no_collection()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Patch_mod_should_not_need_update_when_no_collection",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" }
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.PatchModNeedsUpdateAsync(null, null);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Patch_mod_should_not_need_update_when_mod_not_present.
        /// </summary>
        [Fact]
        public async Task Patch_mod_should_need_update_when_mod_not_present()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Patch_mod_should_not_need_update_when_mod_not_present",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fake1.txt", "mod/fake2.txt"},
                    Name = "test",
                    Game = "Patch_mod_should_not_need_update_when_mod_not_present"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).ReturnsAsync((ModPatchExporterParameters p, bool load) =>
            {
                var res = new PatchState()
                {
                    Conflicts = new List<IDefinition>() { new Definition() { File = "1", Id = "test", Type = "events", Code = "ab", ModName = "1" } },
                    OverwrittenConflicts = new List<IDefinition>(),
                    OrphanConflicts = new List<IDefinition>(),
                    LoadOrder = new List<string>() { "mod/fake2.txt", "mod/fake1.txt" }
                };
                return res;
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.PatchModNeedsUpdateAsync("colname", new List<string>() { "mod/fake2.txt", "mod/fake1.txt" });
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Patch_mod_should_need_update_when_file_not_present.
        /// </summary>
        [Fact]
        public async Task Patch_mod_should_need_update_when_file_not_present()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Patch_mod_should_need_update_when_file_not_present",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fake1.txt", "mod/fake2.txt"},
                    Name = "test",
                    Game = "Patch_mod_should_need_update_when_file_not_present"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).ReturnsAsync((ModPatchExporterParameters p, bool load) =>
            {
                var res = new PatchState()
                {
                    Conflicts = new List<IDefinition>() { new Definition() { File = "1", Id = "test", Type = "events", Code = "ab", ModName = "1" } },
                    OverwrittenConflicts = new List<IDefinition>(),
                    OrphanConflicts = new List<IDefinition>(),
                    LoadOrder = new List<string>() { "mod/fake2.txt", "mod/fake1.txt" }
                };
                return res;
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.PatchModNeedsUpdateAsync("colname", new List<string>() { "mod/fake2.txt", "mod/fake1.txt" });
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Patch_mod_should_need_update_when_sha_not_same.
        /// </summary>
        [Fact]
        public async Task Patch_mod_should_need_update_when_sha_not_same()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Patch_mod_should_need_update_when_sha_not_same",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fake1.txt", "mod/fake2.txt"},
                    Name = "test",
                    Game = "Patch_mod_should_need_update_when_sha_not_same"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).ReturnsAsync((ModPatchExporterParameters p, bool load) =>
            {
                var res = new PatchState()
                {
                    Conflicts = new List<IDefinition>() { new Definition() { File = "1", Id = "test", Type = "events", Code = "ab", ModName = "1" } },
                    OverwrittenConflicts = new List<IDefinition>(),
                    OrphanConflicts = new List<IDefinition>(),
                    LoadOrder = new List<string>() { "mod/fake2.txt", "mod/fake1.txt" }
                };
                return res;
            });
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo()
            {
                ContentSHA = "2"
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.PatchModNeedsUpdateAsync("colname", new List<string>() { "mod/fake2.txt", "mod/fake1.txt" });
            result.Should().BeTrue();
        }

        /// <summary>
        /// Patches the mod should need update when overwritten sha not same.
        /// </summary>
        [Fact]
        public async Task Patch_mod_should_need_update_when_overwritten_sha_not_same()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Patch_mod_should_need_update_when_overwritten_sha_not_same",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fake1.txt", "mod/fake2.txt"},
                    Name = "test",
                    Game = "Patch_mod_should_need_update_when_overwritten_sha_not_same"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).ReturnsAsync((ModPatchExporterParameters p, bool load) =>
            {
                var res = new PatchState()
                {
                    Conflicts = new List<IDefinition>(),
                    OverwrittenConflicts = new List<IDefinition>() { new Definition() { File = "1", Id = "test", Type = "events", Code = "ab", ModName = "1", OriginalFileName = "1" } },
                    OrphanConflicts = new List<IDefinition>(),
                    LoadOrder = new List<string>() { "mod/fake2.txt", "mod/fake1.txt" }
                };
                return res;
            });
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo()
            {
                ContentSHA = "2"
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.PatchModNeedsUpdateAsync("colname", new List<string>() { "mod/fake2.txt", "mod/fake1.txt" });
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Patch_mod_should_need_update_when_orphan_sha_not_same.
        /// </summary>
        [Fact]
        public async Task Patch_mod_should_need_update_when_orphan_sha_not_same()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Patch_mod_should_need_update_when_overwritten_sha_not_same",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fake1.txt", "mod/fake2.txt"},
                    Name = "test",
                    Game = "Patch_mod_should_need_update_when_overwritten_sha_not_same"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).ReturnsAsync((ModPatchExporterParameters p, bool load) =>
            {
                var res = new PatchState()
                {
                    Conflicts = new List<IDefinition>(),
                    OverwrittenConflicts = new List<IDefinition>(),
                    OrphanConflicts = new List<IDefinition>() { new Definition() { File = "1", Id = "test", Type = "events", Code = "ab", ModName = "1" } },
                    LoadOrder = new List<string>() { "mod/fake2.txt", "mod/fake1.txt" }
                };
                return res;
            });
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo()
            {
                ContentSHA = "2"
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.PatchModNeedsUpdateAsync("colname", new List<string>() { "mod/fake2.txt", "mod/fake1.txt" });
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Patch_mod_should_need_update_when_load_order_not_same.
        /// </summary>
        [Fact]
        public async Task Patch_mod_should_need_update_when_load_order_not_same()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Patch_mod_should_need_update_when_load_order_not_same",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fake1.txt", "mod/fake2.txt"},
                    Name = "test",
                    Game = "Patch_mod_should_need_update_when_load_order_not_same"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).ReturnsAsync((ModPatchExporterParameters p, bool load) =>
            {
                var res = new PatchState()
                {
                    Conflicts = new List<IDefinition>() { new Definition() { File = "1", Id = "test", Type = "events", Code = "ab", ModName = "1" } },
                    OverwrittenConflicts = new List<IDefinition>(),
                    OrphanConflicts = new List<IDefinition>(),
                    LoadOrder = new List<string>() { "mod/fake1.txt", "mod/fake2.txt" }
                };
                return res;
            });
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo()
            {
                ContentSHA = "2"
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.PatchModNeedsUpdateAsync("colname", new List<string>() { "mod/fake2.txt", "mod/fake1.txt" });
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Patch_mod_should_not_need_update.
        /// </summary>
        [Fact]
        public async Task Patch_mod_should_not_need_update()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Patch_mod_should_not_need_update",
                UserDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mod"),
                WorkshopDirectory = new List<string>() { "C:\\fake" },
                CustomModDirectory = string.Empty
            });
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fake1.txt", "mod/fake2.txt"},
                    Name = "test",
                    Game = "Patch_mod_should_not_need_update"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).ReturnsAsync((ModPatchExporterParameters p, bool load) =>
            {
                var res = new PatchState()
                {
                    Conflicts = new List<IDefinition>() { new Definition() { File = "1", Id = "test", Type = "events", Code = "ab", ModName = "1", ContentSHA = "1" } },
                    OverwrittenConflicts = new List<IDefinition>(),
                    OrphanConflicts = new List<IDefinition>(),
                    LoadOrder = new List<string>() { "mod/fake2.txt", "mod/fake1.txt" }
                };
                return res;
            });
            reader.Setup(p => p.GetFileInfo(It.IsAny<string>(), It.IsAny<string>())).Returns(new FileInfo()
            {
                ContentSHA = "1"
            });
            modWriter.Setup(p => p.ModDirectoryExists(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return false;
            });
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.PatchModNeedsUpdateAsync("colname", new List<string>() { "mod/fake2.txt", "mod/fake1.txt" });
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method LoadDefinitionContent_should_be_null_when_no_game.
        /// </summary>
        [Fact]
        public async Task LoadDefinitionContent_should_be_null_when_no_game()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);
            modPatchExporter.Setup(p => p.LoadDefinitionContentsAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<string>())).Returns(Task.FromResult("test-response"));
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var result = await service.LoadDefinitionContentsAsync(new Definition() { File = "test.txt" }, "test");
            result.Should().BeNullOrWhiteSpace();
        }

        /// <summary>
        /// Defines the test method LoadDefinitionContent_should_be_null.
        /// </summary>
        [Fact]
        public async Task LoadDefinitionContent_should_be_null()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "LoadDefinitionContent_should_be_false_when_no_game",
                UserDirectory = "C:\\Users\\Fake"
            });
            modPatchExporter.Setup(p => p.LoadDefinitionContentsAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<string>())).Returns(Task.FromResult("test-response"));
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var result = await service.LoadDefinitionContentsAsync(null, "test");
            result.Should().BeNullOrWhiteSpace();

            result = await service.LoadDefinitionContentsAsync(new Definition() { File = "test.txt" }, string.Empty);
            result.Should().BeNullOrWhiteSpace();
        }

        /// <summary>
        /// Defines the test method LoadDefinitionContent_should_not_be_null.
        /// </summary>
        [Fact]
        public async Task LoadDefinitionContent_should_not_be_null()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            SetupMockCase(reader, parserManager, modParser);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "LoadDefinitionContent_should_be_false_when_no_game",
                UserDirectory = "C:\\Users\\Fake"
            });
            modPatchExporter.Setup(p => p.LoadDefinitionContentsAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<string>())).Returns(Task.FromResult("test-response"));
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter, null);

            var result = await service.LoadDefinitionContentsAsync(new Definition() { File = "test.txt" }, "test");
            result.Should().Be("test-response");
        }

        /// <summary>
        /// Defines the test method Should_not_have_game_files_included_when_no_selected_game.
        /// </summary>
        [Fact]
        public async Task Should_not_have_game_files_included_when_no_selected_game()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.PatchHasGameDefinitionsAsync("fake");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_have_game_files_included_when_no_collection.
        /// </summary>
        [Fact]
        public async Task Should_not_have_game_files_included_when_no_collection()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.PatchHasGameDefinitionsAsync(null);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_have_game_files_included.
        /// </summary>
        [Fact]
        public async Task Should_have_game_files_included()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).ReturnsAsync((ModPatchExporterParameters p, bool load) =>
            {
                var res = new PatchState()
                {
                    Conflicts = new List<IDefinition>(),
                    ResolvedConflicts = new List<IDefinition>(),
                    OrphanConflicts = new List<IDefinition>(),
                    OverwrittenConflicts = new List<IDefinition>(),
                    Mode = IO.Common.PatchStateMode.Default,
                    HasGameDefinitions = true
                };
                return res;
            });
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Should_have_game_files_included",
                UserDirectory = "C:\\Users\\Fake"
            });

            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = await service.PatchHasGameDefinitionsAsync("fake");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_parse_ignore_game_mods_anything_when_no_conflict_result.
        /// </summary>
        [Fact]
        public void Should_not_parse_ignore_game_mods_anything_when_no_conflict_result()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.ShouldIgnoreGameMods(null);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_toggle_ignore_game_mods.
        /// </summary>
        [Fact]
        public void Should_ignore_game_mods()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var c = new ConflictResult()
            {
                IgnoredPaths = ""
            };
            var result = service.ShouldIgnoreGameMods(c);
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_ignore_game_mods.
        /// </summary>
        [Fact]
        public void Should_not_ignore_game_mods()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var c = new ConflictResult()
            {
                IgnoredPaths = "--showGameMods"
            };
            var result = service.ShouldIgnoreGameMods(c);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_toggle_ignore_game_mods_anything_when_no_conflict_result.
        /// </summary>
        [Fact]
        public void Should_not_toggle_ignore_game_mods_anything_when_no_conflict_result()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.ToggleIgnoreGameMods(null);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_toggle_ignore_game_mods.
        /// </summary>
        [Fact]
        public void Should_not_toggle_ignore_game_mods()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var c = new ConflictResult()
            {
                IgnoredPaths = ""
            };
            var result = service.ToggleIgnoreGameMods(c);
            result.Should().BeFalse();
            c.IgnoredPaths.Should().Contain("--showGameMods");
        }

        /// <summary>
        /// Defines the test method Should_toggle_ignore_game_mods.
        /// </summary>
        [Fact]
        public void Should_toggle_ignore_game_mods()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var c = new ConflictResult()
            {
                IgnoredPaths = "--showGameMods"
            };
            var result = service.ToggleIgnoreGameMods(c);
            result.Should().BeTrue();
            c.IgnoredPaths.Should().BeNullOrWhiteSpace();
        }

        /// <summary>
        /// Defines the test method Should_not_parse_self_conflicts_when_no_conflict_result.
        /// </summary>
        [Fact]
        public void Should_not_parse_self_conflicts_when_no_conflict_result()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.ShouldShowSelfConflicts(null);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_show_self_conflicts.
        /// </summary>
        [Fact]
        public void Should_show_self_conflicts()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var c = new ConflictResult()
            {
                IgnoredPaths = "--showSelfConflicts"
            };
            var result = service.ShouldShowSelfConflicts(c);
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_show_self_conflicts.
        /// </summary>
        [Fact]
        public void Should_not_show_self_conflicts()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var c = new ConflictResult()
            {
                IgnoredPaths = ""
            };
            var result = service.ShouldShowSelfConflicts(c);
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_toggle_self_conflicts_when_no_conflict_result.
        /// </summary>
        [Fact]
        public void Should_not_toggle_self_conflicts_when_no_conflict_result()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var result = service.ToggleSelfModConflicts(null);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_toggle_self_conflicts.
        /// </summary>
        [Fact]
        public void Should_not_toggle_self_conflicts()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var c = new ConflictResult()
            {
                IgnoredPaths = "--showSelfConflicts"
            };
            var result = service.ToggleSelfModConflicts(c);
            result.Should().BeFalse();
            c.IgnoredPaths.Should().BeNullOrWhiteSpace();
        }

        /// <summary>
        /// Defines the test method Should_toggle_self_conflicts.
        /// </summary>
        [Fact]
        public void Should_toggle_self_conflicts()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var service = GetService(storageProvider, modParser, parserManager, reader, mapper, modWriter, gameService, modPatchExporter);
            var c = new ConflictResult()
            {
                IgnoredPaths = ""
            };
            var result = service.ToggleSelfModConflicts(c);
            result.Should().BeTrue();
            c.IgnoredPaths.Should().Contain("--showSelfConflicts");
        }

        /// <summary>
        /// Defines the test method Stellaris_Performance_profiling.
        /// </summary>

#if FUNCTIONAL_TEST
        [Fact(Timeout = 3000000)]
#else
        [Fact(Skip = "This is for functional testing only")]
#endif
        public async Task Stellaris_Performance_profiling()
        {
            DISetup.SetupContainer();

            var registration = new Services.Registrations.GameRegistration();
            registration.OnPostStartup();
            var game = DISetup.Container.GetInstance<IGameService>().Get().First(s => s.Type == "Stellaris");
            var mods = DISetup.Container.GetInstance<IModService>().GetInstalledMods(game);
            var defs = await DISetup.Container.GetInstance<IModPatchCollectionService>().GetModObjectsAsync(game, mods, string.Empty);
        }
    }
}
