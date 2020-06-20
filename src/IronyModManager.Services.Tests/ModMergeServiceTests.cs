// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 06-19-2020
//
// Last Modified By : Mario
// Last Modified On : 06-19-2020
// ***********************************************************************
// <copyright file="ModMergeServiceTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using IronyModManager.IO;
using IronyModManager.IO.Common.Mods;
using IronyModManager.IO.Common.Mods.Models;
using IronyModManager.IO.Common.Readers;
using IronyModManager.IO.Mods.Models;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common;
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Common.Mod;
using IronyModManager.Parser.Definitions;
using IronyModManager.Parser.Mod;
using IronyModManager.Services.Common;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class ModMergeServiceTests.
    /// </summary>
    public class ModMergeServiceTests
    {
        /// <summary>
        /// Defines the test method Should_not_create_merge_mod.
        /// </summary>
        [Fact]
        public async Task Should_not_create_merge_mod()
        {
            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            var modMergeExporter = new Mock<IModMergeExporter>();
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            gameService.Setup(p => p.GetSelected()).Returns((IGame)null);

            var service = new ModMergeService(messageBus.Object, modPatchExporter.Object, modMergeExporter.Object,
                new List<IDefinitionInfoProvider>() { infoProvider.Object }, reader.Object, modWriter.Object,
                modParser.Object, gameService.Object, storageProvider.Object, mapper.Object);

            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Code = "test = {test}",
                    File = "fake.txt"
                }
            });
            var empty = new IndexedDefinitions();
            empty.InitMap(new List<IDefinition>());

            var result = await service.MergeCollectionAsync(new ConflictResult()
            {
                AllConflicts = indexed,
                Conflicts = empty,
                OverwrittenConflicts = empty,
                ResolvedConflicts = empty
            }, new List<string>(), "fake copy");

            result.Should().BeNull();
        }


        /// <summary>
        /// Defines the test method Should_merge_same_file_content.
        /// </summary>
        [Fact]
        public async Task Should_merge_same_file_content()
        {
            DISetup.SetupContainer();

            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult((IPatchState)new PatchState()
            {
                ConflictHistory = new List<IDefinition>()
            }));
            IDefinition definition = null;
            var modMergeExporter = new Mock<IModMergeExporter>();
            modMergeExporter.Setup(p => p.ExportDefinitionsAsync(It.IsAny<ModMergeExporterParameters>())).Returns((ModMergeExporterParameters p) =>
            {
                definition = p.Definitions.FirstOrDefault();
                return Task.FromResult(true);
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Fake",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = "C:\\Fake"
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fakemod.mod"},
                    Name = "test",
                    Game = "Fake"
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
                    Content = new List<string>() { "a" },
                    FileName = "fakemod.mod",
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
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });

            var service = new ModMergeService(messageBus.Object, modPatchExporter.Object, modMergeExporter.Object,
                new List<IDefinitionInfoProvider>() { infoProvider.Object }, reader.Object, modWriter.Object,
                modParser.Object, gameService.Object, storageProvider.Object, mapper.Object);

            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Code = "test = {test}",
                    File = "events\\fake.txt",
                    ModName = "a",
                    Id = "test1",
                    IsFirstLevel = true
                },
                new Definition()
                {
                    Code = "test = {test2}",
                    File = "events\\fake.txt",
                    ModName = "a",
                    Id = "test2",
                    IsFirstLevel = true
                }
            });
            var empty = new IndexedDefinitions();
            empty.InitMap(new List<IDefinition>());

            var result = await service.MergeCollectionAsync(new ConflictResult()
            {
                AllConflicts = indexed,
                Conflicts = empty,
                OverwrittenConflicts = empty,
                ResolvedConflicts = empty
            }, new List<string>() { "a" }, "fake copy");

            result.Should().NotBeNull();
            definition.Code.Should().Be("test = {test}" + Environment.NewLine + "test = {test2}");
        }

        /// <summary>
        /// Defines the test method Should_merge_same_file_non_first_level_content.
        /// </summary>
        [Fact]
        public async Task Should_merge_same_file_non_first_level_content()
        {
            DISetup.SetupContainer();

            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult((IPatchState)new PatchState()
            {
                ConflictHistory = new List<IDefinition>()
            }));
            IDefinition definition = null;
            var modMergeExporter = new Mock<IModMergeExporter>();
            modMergeExporter.Setup(p => p.ExportDefinitionsAsync(It.IsAny<ModMergeExporterParameters>())).Returns((ModMergeExporterParameters p) =>
            {
                definition = p.Definitions.FirstOrDefault();
                return Task.FromResult(true);
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Fake",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = "C:\\Fake"
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fakemod.mod"},
                    Name = "test",
                    Game = "Fake"
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
                    Content = new List<string>() { "a" },
                    FileName = "fakemod.mod",
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
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });

            var service = new ModMergeService(messageBus.Object, modPatchExporter.Object, modMergeExporter.Object,
                new List<IDefinitionInfoProvider>() { infoProvider.Object }, reader.Object, modWriter.Object,
                modParser.Object, gameService.Object, storageProvider.Object, mapper.Object);

            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Code = "test = {test}",
                    File = "events\\fake.txt",
                    ModName = "a",
                    Id = "test1",
                    IsFirstLevel = false
                },
                new Definition()
                {
                    Code = "test = {test2}",
                    File = "events\\fake.txt",
                    ModName = "a",
                    Id = "test2",
                    IsFirstLevel = false
                }
            });
            var empty = new IndexedDefinitions();
            empty.InitMap(new List<IDefinition>());

            var result = await service.MergeCollectionAsync(new ConflictResult()
            {
                AllConflicts = indexed,
                Conflicts = empty,
                OverwrittenConflicts = empty,
                ResolvedConflicts = empty
            }, new List<string>() { "a" }, "fake copy");

            result.Should().NotBeNull();
            definition.Code.Should().Be("test = {" + Environment.NewLine + "test" + Environment.NewLine + "test2" + Environment.NewLine + "}");
        }

        /// <summary>
        /// Defines the test method Should_select_resolved_conflict_for_merge.
        /// </summary>
        [Fact]
        public async Task Should_select_resolved_conflict_for_merge()
        {
            DISetup.SetupContainer();

            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult((IPatchState)new PatchState()
            {
                ConflictHistory = new List<IDefinition>()
                {
                    new Definition()
                    {
                        Code = "test = {testfakestate}",
                        File = "events\\fake.txt",
                        ModName = "a",
                        Id = "test1",
                        IsFirstLevel = true
                    }
                },
                ResolvedConflicts = new List<IDefinition>()
                {
                    new Definition()
                    {
                        Code = "test = {testfakeresolved}",
                        File = "events\\fake.txt",
                        ModName = "a",
                        Id = "test1",
                        IsFirstLevel = true
                    }
                }
            }));
            IDefinition definition = null;
            var modMergeExporter = new Mock<IModMergeExporter>();
            modMergeExporter.Setup(p => p.ExportDefinitionsAsync(It.IsAny<ModMergeExporterParameters>())).Returns((ModMergeExporterParameters p) =>
            {
                definition = p.PatchDefinitions.FirstOrDefault();
                return Task.FromResult(true);
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Fake",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = "C:\\Fake"
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fakemod.mod"},
                    Name = "test",
                    Game = "Fake"
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
                    Content = new List<string>() { "a" },
                    FileName = "fakemod.mod",
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
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });

            var service = new ModMergeService(messageBus.Object, modPatchExporter.Object, modMergeExporter.Object,
                new List<IDefinitionInfoProvider>() { infoProvider.Object }, reader.Object, modWriter.Object,
                modParser.Object, gameService.Object, storageProvider.Object, mapper.Object);

            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Code = "test = {test}",
                    File = "events\\fake.txt",
                    ModName = "a",
                    Id = "test1",
                    IsFirstLevel = true
                }
            });

            var empty = new IndexedDefinitions();
            empty.InitMap(new List<IDefinition>());

            var result = await service.MergeCollectionAsync(new ConflictResult()
            {
                AllConflicts = indexed,
                Conflicts = empty,
                OverwrittenConflicts = empty,
                ResolvedConflicts = empty
            }, new List<string>() { "a" }, "fake copy");

            result.Should().NotBeNull();
            definition.Code.Should().Be("test = {testfakestate}");
        }

        [Fact]
        public async Task Should_select_overwritten_conflict_for_merge()
        {
            DISetup.SetupContainer();

            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));
            var storageProvider = new Mock<IStorageProvider>();
            var modParser = new Mock<IModParser>();
            var reader = new Mock<IReader>();
            var modWriter = new Mock<IModWriter>();
            var gameService = new Mock<IGameService>();
            var mapper = new Mock<IMapper>();
            var modPatchExporter = new Mock<IModPatchExporter>();
            modPatchExporter.Setup(p => p.GetPatchStateAsync(It.IsAny<ModPatchExporterParameters>(), It.IsAny<bool>())).Returns(Task.FromResult((IPatchState)new PatchState()
            {
                ConflictHistory = new List<IDefinition>()
            }));
            IDefinition definition = null;
            var modMergeExporter = new Mock<IModMergeExporter>();
            modMergeExporter.Setup(p => p.ExportDefinitionsAsync(It.IsAny<ModMergeExporterParameters>())).Returns((ModMergeExporterParameters p) =>
            {
                definition = p.PatchDefinitions.FirstOrDefault();
                return Task.FromResult(true);
            });
            var infoProvider = new Mock<IDefinitionInfoProvider>();
            infoProvider.Setup(p => p.DefinitionUsesFIOSRules(It.IsAny<IDefinition>())).Returns(true);
            infoProvider.Setup(p => p.CanProcess(It.IsAny<string>())).Returns(true);
            gameService.Setup(p => p.GetSelected()).Returns(new Game()
            {
                Type = "Fake",
                UserDirectory = "C:\\Users\\Fake",
                WorkshopDirectory = "C:\\Fake"
            });
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    IsSelected = true,
                    Mods = new List<string>() { "mod/fakemod.mod"},
                    Name = "test",
                    Game = "Fake"
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
                    Content = new List<string>() { "a" },
                    FileName = "fakemod.mod",
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
            mapper.Setup(s => s.Map<IMod>(It.IsAny<IModObject>())).Returns((IModObject o) =>
            {
                return new Mod()
                {
                    FileName = o.FileName,
                    Name = o.Name
                };
            });

            var service = new ModMergeService(messageBus.Object, modPatchExporter.Object, modMergeExporter.Object,
                new List<IDefinitionInfoProvider>() { infoProvider.Object }, reader.Object, modWriter.Object,
                modParser.Object, gameService.Object, storageProvider.Object, mapper.Object);

            var indexed = new IndexedDefinitions();
            indexed.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Code = "test = {test}",
                    File = "events\\fake.txt",
                    ModName = "a",
                    Id = "test1",
                    IsFirstLevel = true
                }
            });

            var overwritten = new IndexedDefinitions();
            overwritten.InitMap(new List<IDefinition>()
            {
                new Definition()
                {
                    Code = "test = {testfakeoverwritten}",
                    File = "events\\fake.txt",
                    ModName = "a",
                    Id = "test1",
                    IsFirstLevel = true
                }
            });

            var empty = new IndexedDefinitions();
            empty.InitMap(new List<IDefinition>());

            var result = await service.MergeCollectionAsync(new ConflictResult()
            {
                AllConflicts = indexed,
                Conflicts = empty,
                OverwrittenConflicts = overwritten,
                ResolvedConflicts = empty
            }, new List<string>() { "a" }, "fake copy");

            result.Should().NotBeNull();
            definition.Code.Should().Be("test = {testfakeoverwritten}");
        }
    }
}
