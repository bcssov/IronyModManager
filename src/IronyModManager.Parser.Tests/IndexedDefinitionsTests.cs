// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 04-25-2020
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
using IronyModManager.Parser.Common.Definitions;
using IronyModManager.Parser.Definitions;
using IronyModManager.Tests.Common;
using Xunit;

namespace IronyModManager.Parser.Tests
{
    /// <summary>
    /// Class IndexedDefinitionsTests.
    /// </summary>
    public class IndexedDefinitionsTests
    {
        /// <summary>
        /// Defines the test method Returns_all_definitions.
        /// </summary>
        [Fact]
        public void Returns_all_definitions()
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
            service.InitMap(defs);
            var results = service.GetAll();
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
        /// Defines the test method Returns_all_definitions_and_added_definitions.
        /// </summary>
        [Fact]
        public void Returns_all_definitions_and_added_definitions()
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
            service.InitMap(defs);
            service.AddToMap(new Definition()
            {
                Code = "a",
                Id = "14",
                Type = "14"
            });
            var results = service.GetAll();
            results.Count().Should().Be(defs.Count + 1);
        }

        /// <summary>
        /// Defines the test method Returns_by_file.
        /// </summary>
        [Fact]
        public void Returns_by_file()
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
            service.InitMap(defs);
            var results = service.GetByFile("file");
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
        /// Defines the test method Returns_by_value_type.
        /// </summary>
        [Fact]
        public void Returns_by_value_type()
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
                    ValueType = Common.ValueType.Object
                });
            }
            var service = new IndexedDefinitions();
            service.InitMap(defs);
            var results = service.GetByValueType(Common.ValueType.Object);
            results.Count().Should().Be(10);
        }

        /// <summary>
        /// Defines the test method Returns_by_parent_directory.
        /// </summary>
        [Fact]
        public void Returns_by_parent_directory()
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
            service.InitMap(defs);
            var results = service.GetByParentDirectory("events");
            results.Count().Should().Be(defs.Count());
        }

        /// <summary>
        /// Defines the test method Returns_all_file_keys.
        /// </summary>
        [Fact]
        public void Returns_all_file_keys()
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
            service.InitMap(defs);
            var results = service.GetAllFileKeys();
            results.Count().Should().Be(defs.Count);
        }

        /// <summary>
        /// Defines the test method Returns_by_type.
        /// </summary>
        [Fact]
        public void Returns_by_type()
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
            service.InitMap(defs);
            var results = service.GetByType("type");
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
        /// Defines the test method Returns_hierarchical_objects.
        /// </summary>
        [Fact]
        public void Returns_hierarchical_objects()
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
            service.InitMap(defs, true);
            var results = service.GetHierarchicalDefinitions();
            results.Count().Should().Be(1);
            results.First().Children.Count().Should().Be(10);
        }

        /// <summary>
        /// Defines the test method Returns_all_type_keys.
        /// </summary>
        [Fact]
        public void Returns_all_type_keys()
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
            service.InitMap(defs);
            var results = service.GetAllTypeKeys();
            results.Count().Should().Be(defs.Count);
        }

        /// <summary>
        /// Defines the test method Returns_by_type_and_id_constructed_key.
        /// </summary>
        [Fact]
        public void Returns_by_type_and_id_constructed_key()
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
            service.InitMap(defs);
            var results = service.GetByTypeAndId("type", "id");
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
        /// Defines the test method Returns_by_type_and_id_non_constructed_key.
        /// </summary>
        [Fact]
        public void Returns_by_type_and_id_non_constructed_key()
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
            service.InitMap(defs);
            var results = service.GetByTypeAndId("type-id");
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
        /// Defines the test method Returns_all_type_and_id_keys.
        /// </summary>
        [Fact]
        public void Returns_all_type_and_id_keys()
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
            service.InitMap(defs);
            var results = service.GetAllTypeAndIdKeys();
            results.Count().Should().Be(defs.Count);
        }

        /// <summary>
        /// Defines the test method Deletes_specified_definition.
        /// </summary>
        [Fact]
        public void Deletes_specified_definition()
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
            service.InitMap(defs, true);
            service.Remove(defs.First());
            var result = service.GetAll();
            result.Count().Should().Be(defs.Count - 1);
            result.FirstOrDefault(p => p.Id == "0").Should().BeNull();
            var hierarchalResult = service.GetHierarchicalDefinitions();
            hierarchalResult.First().Children.Count.Should().Be(defs.Count - 1);
            hierarchalResult.First().Children.FirstOrDefault(p => p.Key.StartsWith("0")).Should().BeNull();
            service.GetByTypeAndId(defs.First().TypeAndId).Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_find_definition.
        /// </summary>
        [Fact]
        public void Should_not_find_definition()
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
            service.InitMap(defs);
            var results = service.SearchDefinitions("1");
            results.Should().BeNull();
        }


        /// <summary>
        /// Defines the test method Should_find_definition.
        /// </summary>
        [Fact]
        public void Should_find_definition()
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
            service.InitMap(defs);
            service.InitSearch();
            var results = service.SearchDefinitions("1");
            results.Count().Should().Be(1);
            results.First().Id.Should().Be("1");
        }

    }
}
