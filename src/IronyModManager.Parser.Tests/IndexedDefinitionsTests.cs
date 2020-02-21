// ***********************************************************************
// Assembly         : IronyModManager.Parser.Tests
// Author           : Mario
// Created          : 02-17-2020
//
// Last Modified By : Mario
// Last Modified On : 02-18-2020
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
        /// Defines the test method Returns_by_file.
        /// </summary>
        [Fact]
        public void Returns_by_file()
        {
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
        /// Defines the test method Returns_all_file_keys.
        /// </summary>
        [Fact]
        public void Returns_all_file_keys()
        {
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
        /// Defines the test method Returns_all_type_keys.
        /// </summary>
        [Fact]
        public void Returns_all_type_keys()
        {
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
        /// Defines the test method Returns_by_type_and_id.
        /// </summary>
        [Fact]
        public void Returns_by_type_and_id()
        {
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
        /// Defines the test method Returns_all_type_and_id_keys.
        /// </summary>
        [Fact]
        public void Returns_all_type_and_id_keys()
        {
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
    }
}
