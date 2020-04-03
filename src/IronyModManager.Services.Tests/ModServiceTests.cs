// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-24-2020
//
// Last Modified By : Mario
// Last Modified On : 04-01-2020
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
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName
                };
            });

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
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
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
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
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
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
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
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
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
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
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
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
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);

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
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);

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
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);

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
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "test"
            });
            modWriter.Setup(p => p.ApplyModsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
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
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            gameService.Setup(s => s.GetSelected()).Returns(() =>
            {
                return null;
            });
            modWriter.Setup(p => p.ApplyModsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var result = await service.ExportModsAsync(new List<IMod> { new Mod() });
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
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            gameService.Setup(s => s.GetSelected()).Returns(new Game()
            {
                Type = "test"
            });
            modWriter.Setup(p => p.ApplyModsAsync(It.IsAny<ModWriterParameters>())).Returns((ModWriterParameters p) =>
            {
                return Task.FromResult(true);
            });

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var result = await service.ExportModsAsync(null);
            result.Should().BeFalse();
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

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed);
            result.Conflicts.GetAll().Count().Should().Be(2);
            result.Conflicts.GetAllFileKeys().Count().Should().Be(1);
            result.OrphanConflicts.GetAll().Count().Should().Be(0);
            result.Conflicts.GetAll().All(p => p.ModName == "test1" || p.ModName == "test2").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_find_filename_orphan_conflicts.
        /// </summary>
        [Fact]
        public void Should_find_filename_orphan_conflicts()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "c",
                    Id = "b",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed);
            result.Conflicts.GetAll().Count().Should().Be(2);
            result.Conflicts.GetAllFileKeys().Count().Should().Be(1);
            result.OrphanConflicts.GetAll().Count().Should().Be(1);
            result.Conflicts.GetAll().All(p => p.ModName == "test1" || p.ModName == "test2").Should().BeTrue();
            result.OrphanConflicts.GetAll().All(p => p.ModName == "test1").Should().BeTrue();
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

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\2.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed);
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

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test3",
                    ValueType = Parser.Common.ValueType.Object,
                    Dependencies = new List<string>() { "test1", "test2" }
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed);
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

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test3",
                    ValueType = Parser.Common.ValueType.Object,
                    Dependencies = new List<string>() { "test1" }
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed);
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

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test3",
                    ValueType = Parser.Common.ValueType.Object,
                    Dependencies = new List<string>() { "test2" }
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed);
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

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test3",
                    ValueType = Parser.Common.ValueType.Object,
                    Dependencies = new List<string>() { "test1", "test2" }
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "f",
                    Type = "events",
                    Id = "a",
                    ModName = "test4",
                    ValueType = Parser.Common.ValueType.Object
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed);
            result.Conflicts.GetAll().Count().Should().Be(2);
            result.Conflicts.GetAllFileKeys().Count().Should().Be(1);
            result.OrphanConflicts.GetAll().Count().Should().Be(0);
            result.Conflicts.GetAll().All(p => p.ModName == "test3" || p.ModName == "test4").Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_find_variable_conflicts_in_different_filenames.
        /// </summary>
        [Fact]
        public void Should_find_variable_conflicts_in_different_filenames()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a1",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a2",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Variable
                },
                new Definition()
                {
                    File = "events\\2.txt",
                    Code = "a",
                    Type = "events",
                    Id = "a1",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object                    
                },
                new Definition()
                {
                    File = "events\\2.txt",
                    Code = "b",
                    Id = "a2",
                    Type= "events",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Variable
                },
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed);
            result.Conflicts.GetAll().Count().Should().Be(2);
            result.Conflicts.GetAllFileKeys().Count().Should().Be(2);
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

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var result = service.FindConflicts(indexed);
            result.AllConflicts.GetAll().Count().Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_return_definitions_to_write.
        /// </summary>
        [Fact]
        public void Should_return_definitions_to_write()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "c",
                    Type = "events",
                    Id = "ab",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Variable
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var conflict = service.FindConflicts(indexed);
            var result = service.GetDefinitionsToWrite(conflict, definitions.First());
            result.Count().Should().Be(2);
            result.First().Id.Should().Be("a");
            result.Last().Id.Should().Be("ab");
        }

        /// <summary>
        /// Defines the test method Should_return_definitions_to_write_and_include_same_variables.
        /// </summary>
        [Fact]
        public void Should_return_definitions_to_write_and_include_same_variables()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "c",
                    Type = "events",
                    Id = "ab",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Variable
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "c",
                    Type = "events",
                    Id = "ab",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Variable
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var conflict = service.FindConflicts(indexed);
            var result = service.GetDefinitionsToWrite(conflict, definitions.First());
            result.Count().Should().Be(2);
            result.First().Id.Should().Be("a");
            result.Last().Id.Should().Be("ab");
        }

        /// <summary>
        /// Defines the test method Should_return_definitions_to_write_and_exclude_conflicted_variables.
        /// </summary>
        [Fact]
        public void Should_return_definitions_to_write_and_exclude_conflicted_variables()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "cd",
                    Type = "events",
                    Id = "ab",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Variable
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "b",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "c",
                    Type = "events",
                    Id = "ab",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Variable
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var conflict = service.FindConflicts(indexed);
            var result = service.GetDefinitionsToWrite(conflict, definitions.First());
            result.Count().Should().Be(1);
            result.First().Id.Should().Be("a");
        }

        [Fact]
        public void Should_return_valid_definitions_to_write_based_on_variable()
        {
            DISetup.SetupContainer();

            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var parserManager = new Mock<IParserManager>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();

            SetupMockCase(reader, parserManager, modParser);

            var service = new ModService(reader.Object, parserManager.Object, modParser.Object, modWriter.Object, gameService.Object, storageProvider.Object, mapper.Object);
            var definitions = new List<IDefinition>()
            {
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Id = "a",
                    Type= "events",
                    ModName = "test1",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Type = "events",
                    Id = "c",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "a",
                    Type = "events",
                    Id = "a",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Object
                },
                new Definition()
                {
                    File = "events\\1.txt",
                    Code = "c",
                    Type = "events",
                    Id = "ab",
                    ModName = "test2",
                    ValueType = Parser.Common.ValueType.Variable
                }
            };
            var indexed = new IndexedDefinitions();
            indexed.InitMap(definitions);
            var conflict = service.FindConflicts(indexed);
            var result = service.GetDefinitionsToWrite(conflict, definitions.Last());
            result.Count().Should().Be(3);
            result.First().Id.Should().Be("ab");
            result.ToList()[1].Id.Should().Be("a");
            result.Last().Id.Should().Be("c");
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
