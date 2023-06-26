// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 06-25-2023
// ***********************************************************************
// <copyright file="IndexedDefinitionsTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using IronyModManager.Parser.Definitions;
using IronyModManager.Tests.Common;
using IronyModManager.Shared.Models;
using Xunit;
using ValueType = IronyModManager.Shared.Models.ValueType;
using System.Threading.Tasks;
using System.IO;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class IndexedDefinitionsTests.
    /// </summary>
    public class IndexedDefinitionsTests
    {
        /// <summary>
        /// Gets the store path.
        /// </summary>
        /// <returns>System.String.</returns>
        private string GetStorePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dummyPath");
        }

        /// <summary>
        /// Defines the test method Returns_all_definitions.
        /// </summary>
        [Fact]
        public async Task Returns_all_definitions()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.GetAllAsync();
            results.Count().Should().Be(defs.Count);
            int match = 0;
            foreach (var item in defs)
            {
                if (results.Contains(item))
                {
                    match++;
                }
            }
            match.Should().Be(defs.Count);
        }

        /// <summary>
        /// Defines the test method Returns_all_store_definitions.
        /// </summary>
        [Fact]
        public async Task Returns_all_store_definitions()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.GetAllAsync();
            results.Count().Should().Be(defs.Count);
            int match = 0;
            foreach (var item in defs)
            {
                if (results.Any(p => p.ModName == item.ModName && p.File == item.File && p.TypeAndId == item.TypeAndId))
                {
                    match++;
                }
            }
            match.Should().Be(defs.Count);
        }

        /// <summary>
        /// Defines the test method Returns_all_definitions_and_added_definitions.
        /// </summary>
        [Fact]
        public async Task Returns_all_definitions_and_added_definitions()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            await service.AddToMapAsync(new Definition()
            {
                Code = "a",
                Id = "14",
                Type = "14"
            });
            var results = await service.GetAllAsync();
            results.Count().Should().Be(defs.Count + 1);
        }

        /// <summary>
        /// Defines the test method Returns_all_definitions_and_added_definitions_from_store.
        /// </summary>
        [Fact]
        public async Task Returns_all_definitions_and_added_definitions_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            await service.AddToMapAsync(new Definition()
            {
                Code = "a",
                Id = "14",
                Type = "14"
            });
            var results = await service.GetAllAsync();
            results.Count().Should().Be(defs.Count + 1);
        }

        /// <summary>
        /// Defines the test method Returns_by_file.
        /// </summary>
        [Fact]
        public async Task Returns_by_file()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 5 ? "file" : i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.GetByFileAsync("file");
            results.Count().Should().Be(defs.Where(s => s.File == "file").Count());
            int match = 0;
            foreach (var item in defs.Where(s => s.File == "file"))
            {
                if (results.Contains(item))
                {
                    match++;
                }
            }
            match.Should().Be(defs.Where(s => s.File == "file").Count());
        }

        /// <summary>
        /// Defines the test method Returns_by_file_from_store.
        /// </summary>
        [Fact]
        public async Task Returns_by_file_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 5 ? "file" : i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.GetByFileAsync("file");
            results.Count().Should().Be(defs.Where(s => s.File == "file").Count());
            int match = 0;
            foreach (var item in defs.Where(s => s.File == "file"))
            {
                if (results.Any(p => p.ModName == item.ModName && p.File == item.File && p.TypeAndId == item.TypeAndId))
                {
                    match++;
                }
            }
            match.Should().Be(defs.Where(s => s.File == "file").Count());
        }

        /// <summary>
        /// Defines the test method Returns_by_disk_file.
        /// </summary>
        [Fact]
        public async Task Returns_by_disk_file()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 5 ? "file" : i.ToString(),
                    DiskFile = i < 5 ? "diskfile" : i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.GetByDiskFileAsync("diskfile");
            results.Count().Should().Be(defs.Where(s => s.DiskFile == "diskfile").Count());
            int match = 0;
            foreach (var item in defs.Where(s => s.DiskFile == "diskfile"))
            {
                if (results.Contains(item))
                {
                    match++;
                }
            }
            match.Should().Be(defs.Where(s => s.DiskFile == "diskfile").Count());
        }

        /// <summary>
        /// Defines the test method Returns_by_disk_file_from_store.
        /// </summary>
        [Fact]
        public async Task Returns_by_disk_file_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 5 ? "file" : i.ToString(),
                    DiskFile = i < 5 ? "diskfile" : i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.GetByDiskFileAsync("diskfile");
            results.Count().Should().Be(defs.Where(s => s.DiskFile == "diskfile").Count());
            int match = 0;
            foreach (var item in defs.Where(s => s.DiskFile == "diskfile"))
            {
                if (results.Any(p => p.ModName == item.ModName && p.File == item.File && p.TypeAndId == item.TypeAndId))
                {
                    match++;
                }
            }
            match.Should().Be(defs.Where(s => s.DiskFile == "diskfile").Count());
        }

        /// <summary>
        /// Defines the test method Returns_by_value_type.
        /// </summary>
        [Fact]
        public async Task Returns_by_value_type()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 5 ? "file" : i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    ValueType = ValueType.Object
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = (await service.GetByValueTypeAsync(ValueType.Object));
            results.Count().Should().Be(10);
        }

        /// <summary>
        /// Defines the test method Returns_by_value_type_from_store.
        /// </summary>
        [Fact]
        public async Task Returns_by_value_type_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 5 ? "file" : i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    ValueType = ValueType.Object
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = (await service.GetByValueTypeAsync(ValueType.Object));
            results.Count().Should().Be(10);
        }

        /// <summary>
        /// Defines the test method Returns_by_parent_directory.
        /// </summary>
        [Fact]
        public async Task Returns_by_parent_directory()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 3; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "events\\" + i.ToString() + ".txt",
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.GetByParentDirectoryAsync("events");
            results.Count().Should().Be(defs.Count);
        }

        /// <summary>
        /// Defines the test method Returns_by_parent_directory_from_store.
        /// </summary>
        [Fact]
        public async Task Returns_by_parent_directory_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 3; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "events\\" + i.ToString() + ".txt",
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.GetByParentDirectoryAsync("events");
            results.Count().Should().Be(defs.Count);
        }

        /// <summary>
        /// Defines the test method Returns_all_file_keys.
        /// </summary>
        [Fact]
        public async Task Returns_all_file_keys()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.GetAllFileKeysAsync();
            results.Count().Should().Be(defs.Count);
        }

        /// <summary>
        /// Defines the test method Returns_all_file_keys_from_store.
        /// </summary>
        [Fact]
        public async Task Returns_all_file_keys_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.GetAllFileKeysAsync();
            results.Count().Should().Be(defs.Count);
        }

        /// <summary>
        /// Defines the test method Returns_all_directory_keys.
        /// </summary>
        [Fact]
        public async Task Returns_all_directory_keys()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.GetAllDirectoryKeysAsync();
            results.Count().Should().Be(1);
        }

        /// <summary>
        /// Defines the test method Returns_all_directory_keys_from_store.
        /// </summary>
        [Fact]
        public async Task Returns_all_directory_keys_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.GetAllDirectoryKeysAsync();
            results.Count().Should().Be(1);
        }

        /// <summary>
        /// Defines the test method HasGameDefinitions_should_be_false.
        /// </summary>
        [Fact]
        public async Task HasGameDefinitions_should_be_false()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.HasGameDefinitionsAsync();
            results.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method HasGameDefinitions_should_be_false_from_store.
        /// </summary>
        [Fact]
        public async Task HasGameDefinitions_should_be_false_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.HasGameDefinitionsAsync();
            results.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method HasGameDefinitions_should_be_true.
        /// </summary>
        [Fact]
        public async Task HasGameDefinitions_should_be_true()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    IsFromGame = true
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.HasGameDefinitionsAsync();
            results.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method HasGameDefinitions_should_be_true_from_store.
        /// </summary>
        [Fact]
        public async Task HasGameDefinitions_should_be_true_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    IsFromGame = true
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.HasGameDefinitionsAsync();
            results.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method HasResetDefinitions_should_be_false.
        /// </summary>
        [Fact]
        public async Task HasResetDefinitions_should_be_false()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.HasResetDefinitionsAsync();
            results.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method HasResetDefinitions_should_be_false_from_store.
        /// </summary>
        [Fact]
        public async Task HasResetDefinitions_should_be_false_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.HasResetDefinitionsAsync();
            results.Should().BeFalse();
        }


        /// <summary>
        /// Defines the test method HasResetDefinitions_should_be_false_due_to_no_hierarchical_definition.
        /// </summary>
        [Fact]
        public async Task HasResetDefinitions_should_be_false_due_to_no_hierarchical_definition()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    ResetType = ResetType.Resolved
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.HasResetDefinitionsAsync();
            results.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method HasResetDefinitions_should_be_false_due_to_no_hierarchical_definition_from_store.
        /// </summary>
        [Fact]
        public async Task HasResetDefinitions_should_be_false_due_to_no_hierarchical_definition_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    ResetType = ResetType.Resolved
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.HasResetDefinitionsAsync();
            results.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method HasResetDefinitions_should_be_true.
        /// </summary>
        [Fact]
        public async Task HasResetDefinitions_should_be_true()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    ResetType = ResetType.Resolved
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs, true);
            var results = await service.HasResetDefinitionsAsync();
            results.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method HasResetDefinitions_should_be_true_from_store.
        /// </summary>
        [Fact]
        public async Task HasResetDefinitions_should_be_true_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    ResetType = ResetType.Resolved
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs, true);
            var results = await service.HasResetDefinitionsAsync();
            results.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method ChangeHierarchicalResetState_should_be_false_due_to_no_map_initalized.
        /// </summary>
        [Fact]
        public async Task ChangeHierarchicalResetState_should_be_false_due_to_no_map_initalized()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    ResetType = ResetType.Resolved
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs, false);
            var results = await service.ChangeHierarchicalResetStateAsync(new Definition() { ResetType = ResetType.Resolved, Id = "1", Type = "1", File = "test\\1" });
            results.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method ChangeHierarchicalResetState_should_be_false_due_to_no_map_initalized_from_store.
        /// </summary>
        [Fact]
        public async Task ChangeHierarchicalResetState_should_be_false_due_to_no_map_initalized_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    ResetType = ResetType.Resolved
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs, false);
            var results = await service.ChangeHierarchicalResetStateAsync(new Definition() { ResetType = ResetType.Resolved, Id = "1", Type = "1", File = "test\\1" });
            results.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method ChangeHierarchicalResetState_should_be_false_due_to_no_definition.
        /// </summary>
        [Fact]
        public async Task ChangeHierarchicalResetState_should_be_false_due_to_no_definition()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    ResetType = ResetType.Resolved
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs, true);
            var results = await service.ChangeHierarchicalResetStateAsync(null);
            results.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method ChangeHierarchicalResetState_should_be_false_due_to_no_definition_from_store.
        /// </summary>
        [Fact]
        public async Task ChangeHierarchicalResetState_should_be_false_due_to_no_definition_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    ResetType = ResetType.Resolved
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs, true);
            var results = await service.ChangeHierarchicalResetStateAsync(null);
            results.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method ChangeHierarchicalResetState_should_be_true.
        /// </summary>
        [Fact]
        public async Task ChangeHierarchicalResetState_should_be_true()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    ResetType = ResetType.Resolved
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs, true);
            var results = await service.ChangeHierarchicalResetStateAsync(new Definition() { ResetType = ResetType.None, Id = "1", Type = "1", File = "test\\1" });
            results.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method ChangeHierarchicalResetState_should_be_true_from_store.
        /// </summary>
        [Fact]
        public async Task ChangeHierarchicalResetState_should_be_true_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = "test\\" + i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    ResetType = ResetType.Resolved
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs, true);
            var results = await service.ChangeHierarchicalResetStateAsync(new Definition() { ResetType = ResetType.None, Id = "1", Type = "1", File = "test\\1" });
            results.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Returns_by_type.
        /// </summary>
        [Fact]
        public async Task Returns_by_type()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 3 ? "file" : i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i > 3 && i < 6 ? "type" : i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.GetByTypeAsync("type");
            results.Count().Should().Be(defs.Where(s => s.Type == "type").Count());
            int match = 0;
            foreach (var item in defs.Where(s => s.Type == "type"))
            {
                if (results.Contains(item))
                {
                    match++;
                }
            }
            match.Should().Be(defs.Where(s => s.Type == "type").Count());
        }

        /// <summary>
        /// Defines the test method Returns_by_type_from_store.
        /// </summary>
        [Fact]
        public async Task Returns_by_type_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 3 ? "file" : i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i > 3 && i < 6 ? "type" : i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.GetByTypeAsync("type");
            results.Count().Should().Be(defs.Where(s => s.Type == "type").Count());
            int match = 0;
            foreach (var item in defs.Where(s => s.Type == "type"))
            {
                if (results.Any(p => p.ModName == item.ModName && p.File == item.File && p.TypeAndId == item.TypeAndId))
                {
                    match++;
                }
            }
            match.Should().Be(defs.Where(s => s.Type == "type").Count());
        }

        /// <summary>
        /// Defines the test method Returns_hierarchical_objects.
        /// </summary>
        [Fact]
        public async Task Returns_hierarchical_objects()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 3 ? "file" : i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i > 3 && i < 6 ? "type" : i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs, true);
            var results = service.GetHierarchicalDefinitions();
            results.Count().Should().Be(1);
            results.First().Children.Count.Should().Be(10);
        }

        /// <summary>
        /// Defines the test method Returns_hierarchical_objects_from_store.
        /// </summary>
        [Fact]
        public async Task Returns_hierarchical_objects_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 3 ? "file" : i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i > 3 && i < 6 ? "type" : i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs, true);
            var results = service.GetHierarchicalDefinitions();
            results.Count().Should().Be(1);
            results.First().Children.Count.Should().Be(10);
        }

        /// <summary>
        /// Defines the test method Returns_all_type_keys.
        /// </summary>
        [Fact]
        public async Task Returns_all_type_keys()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.GetAllTypeKeysAsync();
            results.Count().Should().Be(defs.Count);
        }

        /// <summary>
        /// Defines the test method Returns_by_type_and_id_constructed_key.
        /// </summary>
        [Fact]
        public async Task Returns_by_type_and_id_constructed_key()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 3 ? "file" : i.ToString(),
                    Id = i > 4 && i < 7 ? "id" : i.ToString(),
                    ModName = i.ToString(),
                    Type = i > 3 && i < 6 ? "type" : i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.GetByTypeAndIdAsync("type", "id");
            results.Count().Should().Be(defs.Where(s => s.Type == "type" && s.Id == "id").Count());
            int match = 0;
            foreach (var item in defs.Where(s => s.Type == "type" && s.Id == "id"))
            {
                if (results.Contains(item))
                {
                    match++;
                }
            }
            match.Should().Be(defs.Where(s => s.Type == "type" && s.Id == "id").Count());
        }

        /// <summary>
        /// Defines the test method Returns_by_type_and_id_constructed_key_from_store.
        /// </summary>
        [Fact]
        public async Task Returns_by_type_and_id_constructed_key_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 3 ? "file" : i.ToString(),
                    Id = i > 4 && i < 7 ? "id" : i.ToString(),
                    ModName = i.ToString(),
                    Type = i > 3 && i < 6 ? "type" : i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.GetByTypeAndIdAsync("type", "id");
            results.Count().Should().Be(defs.Where(s => s.Type == "type" && s.Id == "id").Count());
            int match = 0;
            foreach (var item in defs.Where(s => s.Type == "type" && s.Id == "id"))
            {
                if (results.Any(p => p.ModName == item.ModName && p.File == item.File && p.TypeAndId == item.TypeAndId))
                {
                    match++;
                }
            }
            match.Should().Be(defs.Where(s => s.Type == "type" && s.Id == "id").Count());
        }

        /// <summary>
        /// Defines the test method Returns_by_type_and_id_non_constructed_key.
        /// </summary>
        [Fact]
        public async Task Returns_by_type_and_id_non_constructed_key()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 3 ? "file" : i.ToString(),
                    Id = i > 4 && i < 7 ? "id" : i.ToString(),
                    ModName = i.ToString(),
                    Type = i > 3 && i < 6 ? "type" : i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.GetByTypeAndIdAsync("type-id");
            results.Count().Should().Be(defs.Where(s => s.Type == "type" && s.Id == "id").Count());
            int match = 0;
            foreach (var item in defs.Where(s => s.Type == "type" && s.Id == "id"))
            {
                if (results.Contains(item))
                {
                    match++;
                }
            }
            match.Should().Be(defs.Where(s => s.Type == "type" && s.Id == "id").Count());
        }

        /// <summary>
        /// Defines the test method Returns_by_type_and_id_non_constructed_key_from_store.
        /// </summary>
        [Fact]
        public async Task Returns_by_type_and_id_non_constructed_key_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i < 3 ? "file" : i.ToString(),
                    Id = i > 4 && i < 7 ? "id" : i.ToString(),
                    ModName = i.ToString(),
                    Type = i > 3 && i < 6 ? "type" : i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.GetByTypeAndIdAsync("type-id");
            results.Count().Should().Be(defs.Where(s => s.Type == "type" && s.Id == "id").Count());
            int match = 0;
            foreach (var item in defs.Where(s => s.Type == "type" && s.Id == "id"))
            {
                if (results.Any(p => p.ModName == item.ModName && p.File == item.File && p.TypeAndId == item.TypeAndId))
                {
                    match++;
                }
            }
            match.Should().Be(defs.Where(s => s.Type == "type" && s.Id == "id").Count());
        }

        /// <summary>
        /// Defines the test method Returns_all_type_and_id_keys.
        /// </summary>
        [Fact]
        public async Task Returns_all_type_and_id_keys()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.GetAllTypeAndIdKeysAsync();
            results.Count().Should().Be(defs.Count);
        }

        /// <summary>
        /// Defines the test method Returns_all_type_and_id_keys_from_store.
        /// </summary>
        [Fact]
        public async Task Returns_all_type_and_id_keys_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.GetAllTypeAndIdKeysAsync();
            results.Count().Should().Be(defs.Count);
        }

        /// <summary>
        /// Defines the test method Deletes_specified_definition.
        /// </summary>
        [Fact]
        public async Task Deletes_specified_definition()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs, true);
            await service.RemoveAsync(defs.First());
            var result = await service.GetAllAsync();
            result.Count().Should().Be(defs.Count - 1);
            result.FirstOrDefault(p => p.Id == "0").Should().BeNull();
            var hierarchalResult = service.GetHierarchicalDefinitions();
            hierarchalResult.First().Children.Count.Should().Be(defs.Count - 1);
            hierarchalResult.First().Children.FirstOrDefault(p => p.Key.StartsWith("0")).Should().BeNull();            
            (await service.GetByTypeAndIdAsync(defs.First().TypeAndId)).Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Deletes_specified_definition_from_store.
        /// </summary>
        [Fact]
        public async Task Deletes_specified_definition_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs, true);
            await service.RemoveAsync(defs.First());
            var result = await service.GetAllAsync();
            result.Count().Should().Be(defs.Count - 1);
            result.FirstOrDefault(p => p.Id == "0").Should().BeNull();
            var hierarchalResult = service.GetHierarchicalDefinitions();
            hierarchalResult.First().Children.Count.Should().Be(defs.Count - 1);
            hierarchalResult.First().Children.FirstOrDefault(p => p.Key.StartsWith("0")).Should().BeNull();
            (await service.GetByTypeAndIdAsync(defs.First().TypeAndId)).Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_find_definition.
        /// </summary>
        [Fact]
        public async Task Should_not_find_definition()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.SearchDefinitionsAsync("1");
            results.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_find_definition_from_store.
        /// </summary>
        [Fact]
        public async Task Should_not_find_definition_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.SearchDefinitionsAsync("1");
            results.Should().BeNull();
        }


        /// <summary>
        /// Defines the test method Should_find_definition.
        /// </summary>
        [Fact]
        public async Task Should_find_definition()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    Tags = new List<string>() { i.ToString() }
                });
            }
            var service = new IndexedDefinitions();
            service.UseSearch();
            await service.InitMapAsync(defs);
            await service.InitializeSearchAsync(defs);
            var results = await service.SearchDefinitionsAsync("1");
            results.Count().Should().Be(1);
            results.First().Should().StartWith("1 -");
        }

        /// <summary>
        /// Defines the test method Should_find_definition_from_store.
        /// </summary>
        [Fact]
        public async Task Should_find_definition_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    Tags = new List<string>() { i.ToString() }
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            service.UseSearch();
            await service.InitializeSearchAsync(defs);
            await service.InitMapAsync(defs);
            var results = await service.SearchDefinitionsAsync("1");
            results.Count().Should().Be(1);
            results.First().Should().StartWith("1 -");
        }

        /// <summary>
        /// Defines the test method Should_find_definition_from_search_store.
        /// </summary>
        [Fact]
        public async Task Should_find_definition_from_search_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString(),
                    Tags = new List<string>() { i.ToString() }
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            service.UseSearch(GetStorePath());
            await service.InitializeSearchAsync(defs);
            await service.InitMapAsync(defs);
            var results = await service.SearchDefinitionsAsync("1");
            results.Count().Should().Be(1);
            results.First().Should().StartWith("1 -");
        }

        /// <summary>
        /// Defines the test method Should_exist_by_file.
        /// </summary>
        [Fact]
        public async Task Should_exist_by_file()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.ExistsByFileAsync("1");
            results.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_exist_by_file_from_store.
        /// </summary>
        [Fact]
        public async Task Should_exist_by_file_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.ExistsByFileAsync("1");
            results.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_update_definitions.
        /// </summary>
        [Fact]
        public async Task Should_update_definitions()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.UpdateDefinitionsAsync(defs);
            results.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_update_definitions_from_store.
        /// </summary>
        [Fact]
        public async Task Should_update_definitions_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.UpdateDefinitionsAsync(defs);
            results.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_update_definitions.
        /// </summary>
        [Fact]
        public async Task Should_not_update_definitions()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            var results = await service.UpdateDefinitionsAsync(null);
            results.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_update_definitions_from_store.
        /// </summary>
        [Fact]
        public async Task Should_not_update_definitions_from_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            service.UseDiskStore(GetStorePath());
            await service.InitMapAsync(defs);
            var results = await service.UpdateDefinitionsAsync(null);
            results.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_switch_to_store.
        /// </summary>
        [Fact]
        public void Should_switch_to_store()
        {
            DISetup.SetupContainer();
            var service = new IndexedDefinitions();
            Exception ex = null;
            try
            {
                service.UseDiskStore(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dummy"));
            }
            catch (Exception e)
            {
                ex = e;                
            }
            ex.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_switch_to_store.
        /// </summary>
        [Fact]
        public async Task Should_not_switch_to_store()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            Exception ex = null;
            try
            {
                service.UseDiskStore(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dummy"));
            }
            catch (Exception e)
            {
                ex = e;
            }
            ex.Should().NotBeNull();
            ex.GetType().Should().Be(typeof(InvalidOperationException));
        }

        /// <summary>
        /// Defines the test method Should_switch_to_allowed_type.
        /// </summary>
        [Fact]
        public void Should_switch_to_allowed_type()
        {
            DISetup.SetupContainer();
            var service = new IndexedDefinitions();
            Exception ex = null;
            try
            {
                service.SetAllowedType(AddToMapAllowedType.InvalidAndSpecial);
            }
            catch (Exception e)
            {
                ex = e;
            }
            ex.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_switch_to_allowed_type.
        /// </summary>
        [Fact]
        public async Task Should_not_switch_to_allowed_type()
        {
            DISetup.SetupContainer();
            var defs = new List<IDefinition>();
            for (int i = 0; i < 10; i++)
            {
                defs.Add(new Definition()
                {
                    Code = i.ToString(),
                    ContentSHA = i.ToString(),
                    Dependencies = new List<string> { i.ToString() },
                    File = i.ToString(),
                    Id = i.ToString(),
                    ModName = i.ToString(),
                    Type = i.ToString()
                });
            }
            var service = new IndexedDefinitions();
            await service.InitMapAsync(defs);
            Exception ex = null;
            try
            {
                service.SetAllowedType(AddToMapAllowedType.InvalidAndSpecial);
            }
            catch (Exception e)
            {
                ex = e;
            }
            ex.Should().NotBeNull();
            ex.GetType().Should().Be(typeof(InvalidOperationException));
        }
    }
}
