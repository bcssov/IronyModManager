// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 02-14-2021
// ***********************************************************************
// <copyright file="DLCServiceTests.cs" company="Mario">
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
using IronyModManager.DI.Extensions;
using IronyModManager.IO;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common.DLC;
using IronyModManager.Parser.DLC;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Shared.Cache;
using IronyModManager.Shared.Models;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class ExternalEditorServiceTests.
    /// </summary>
    public class DLCServiceTests
    {
        /// <summary>
        /// Defines the test method Should_validate_due_to_non_existing_directory.
        /// </summary>
        [Fact]
        public async Task Should_parse_dlc_object()
        {
            var mapper = new Mock<IMapper>();
            mapper.Setup(s => s.Map<IDLC>(It.IsAny<IDLCObject>())).Returns((IDLCObject o) =>
            {
                return new DLC()
                {
                    Path = o.Path
                };
            });
            var reader = new Mock<IReader>();
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
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns(fileInfos);
            var parser = new Mock<IDLCParser>();
            parser.Setup(s => s.Parse(It.IsAny<string>(), It.IsAny<IEnumerable<string>>())).Returns((string path, IEnumerable<string> values) =>
            {
                return new DLCObject()
                {
                    Path = path,
                    Name = values.First()
                };
            });
            var service = new DLCService(new Cache(), reader.Object, parser.Object, null, mapper.Object);
            var result = await service.GetAsync(new Game()
            {
                ExecutableLocation = AppDomain.CurrentDomain.BaseDirectory + "\\test.exe",
                Type = "Should_parse_dlc_object"
            });
            result.Count.Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_return_dlc_object_from_cache.
        /// </summary>
        [Fact]
        public async Task Should_return_dlc_object_from_cache()
        {
            var dlcs = new List<IDLC>() { new DLC()
            {
                Name = "test",
                Path = "test"
            } };
            var cache = new Cache();
            cache.Set("DLC", "Should_return_dlc_object_from_cache", dlcs);

            var service = new DLCService(cache, null, null, null, null);
            var result = await service.GetAsync(new Game()
            {
                ExecutableLocation = AppDomain.CurrentDomain.BaseDirectory + "\\test.exe",
                Type = "Should_return_dlc_object_from_cache"
            });
            result.Count.Should().Be(1);
            result.Should().BeEquivalentTo(dlcs);
        }

        /// <summary>
        /// Defines the test method Should_return_dlc_object_from_cache_when_exe_path_in_subfolder.
        /// </summary>
        [Fact]
        public async Task Should_return_dlc_object_from_cache_when_exe_path_in_subfolder()
        {
            var dlcs = new List<IDLC>() { new DLC()
            {
                Name = "test",
                Path = "test"
            } };
            var cache = new Cache();
            cache.Set("DLC", "Should_return_dlc_object_from_cache", dlcs);

            var service = new DLCService(cache, null, null, null, null);
            var result = await service.GetAsync(new Game()
            {
                ExecutableLocation = AppDomain.CurrentDomain.BaseDirectory + "\\subfolder\\test.exe",
                Type = "Should_return_dlc_object_from_cache"
            });
            result.Count.Should().Be(1);
            result.Should().BeEquivalentTo(dlcs);
        }

        /// <summary>
        /// Defines the test method Should_not_return_dlc_objects_when_game_null.
        /// </summary>
        [Fact]
        public async Task Should_not_return_dlc_objects_when_game_null()
        {
            var service = new DLCService(null, null, null, null, null);
            var result = await service.GetAsync(null);
            result.Count.Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_return_dlc_objects_when_game_path_not_set.
        /// </summary>
        [Fact]
        public async Task Should_not_return_dlc_objects_when_game_path_not_set()
        {
            var service = new DLCService(new Cache(), null, null, null, null);
            var result = await service.GetAsync(new Game()
            {
                ExecutableLocation = string.Empty,
                Type = "Should_not_return_dlc_objects_when_game_path_not_set"
            });
            result.Count.Should().Be(0);
        }
    }
}
