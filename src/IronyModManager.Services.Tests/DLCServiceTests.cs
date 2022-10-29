// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-14-2021
//
// Last Modified By : Mario
// Last Modified On : 10-29-2022
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
using IronyModManager.IO.Common.DLC;
using IronyModManager.IO.Common.Readers;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Common;
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
            reader.Setup(s => s.Read(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<bool>())).Returns(fileInfos);
            var parser = new Mock<IDLCParser>();
            parser.Setup(s => s.Parse(It.IsAny<string>(), It.IsAny<IEnumerable<string>>(), It.IsAny<DescriptorModType>())).Returns((string path, IEnumerable<string> values, DescriptorModType t) =>
            {
                return new DLCObject()
                {
                    Path = path,
                    Name = values.First()
                };
            });
            var service = new DLCService(null, new Cache(), reader.Object, parser.Object, null, mapper.Object);
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
            cache.Set(new CacheAddParameters<List<IDLC>>() { Region = "DLC", Key = "Should_return_dlc_object_from_cache", Value = dlcs });

            var service = new DLCService(null, cache, null, null, null, null);
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
            cache.Set(new CacheAddParameters<List<IDLC>>() { Region = "DLC", Key = "Should_return_dlc_object_from_cache_when_exe_path_in_subfolder", Value = dlcs });            

            var service = new DLCService(null, cache, null, null, null, null);
            var result = await service.GetAsync(new Game()
            {
                ExecutableLocation = AppDomain.CurrentDomain.BaseDirectory + "\\subfolder\\test.exe",
                Type = "Should_return_dlc_object_from_cache_when_exe_path_in_subfolder"
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
            var service = new DLCService(null, null, null, null, null, null);
            var result = await service.GetAsync(null);
            result.Count.Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_return_dlc_objects_when_game_path_not_set.
        /// </summary>
        [Fact]
        public async Task Should_not_return_dlc_objects_when_game_path_not_set()
        {
            var service = new DLCService(null, new Cache(), null, null, null, null);
            var result = await service.GetAsync(new Game()
            {
                ExecutableLocation = string.Empty,
                Type = "Should_not_return_dlc_objects_when_game_path_not_set"
            });
            result.Count.Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_export_dlc_when_no_game.
        /// </summary>
        [Fact]
        public async Task Should_not_export_dlc_when_no_game()
        {
            var service = new DLCService(null, null, null, null, null, null);
            var result = await service.ExportAsync(null, new List<IDLC>()
            {
                new DLC()
                {
                    Name = "test",
                    Path = "dlc/dlc01.dlc"
                }
            });
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_export_dlc_when_no_dlc.
        /// </summary>
        [Fact]
        public async Task Should_not_export_dlc_when_no_dlc()
        {
            var service = new DLCService(null, null, null, null, null, null);
            var result = await service.ExportAsync(new Game()
            {
                ExecutableLocation = string.Empty,
                Type = "Should_not_return_dlc_objects_when_game_path_not_set"
            }, new List<IDLC>());
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_export_dlc.
        /// </summary>
        [Fact]
        public async Task Should_export_dlc()
        {
            var dlcExport = new Mock<IDLCExporter>();
            dlcExport.Setup(p => p.ExportDLCAsync(It.IsAny<DLCParameters>())).ReturnsAsync((DLCParameters p) => { return p.DLC.Any(); });
            var service = new DLCService(dlcExport.Object, null, null, null, null, null);
            var result = await service.ExportAsync(new Game()
            {
                ExecutableLocation = string.Empty,
                Type = "Should_export_dlc"
            }, new List<IDLC>()
            {
                new DLC()
                {
                    Name = "test",
                    Path = "dlc/dlc01.dlc",
                    IsEnabled = false
                }
            });
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_sync_dlc_when_no_game.
        /// </summary>
        [Fact]
        public async Task Should_not_sync_dlc_when_no_game()
        {
            var service = new DLCService(null, null, null, null, null, null);
            var result = await service.SyncStateAsync(null, new List<IDLC>()
            {
                new DLC()
                {
                    Name = "test",
                    Path = "dlc/dlc01.dlc"
                }
            });
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_not_sync_dlc_when_no_dlc.
        /// </summary>
        [Fact]
        public async Task Should_not_sync_dlc_when_no_dlc()
        {
            var service = new DLCService(null, null, null, null, null, null);
            var result = await service.SyncStateAsync(new Game()
            {
                ExecutableLocation = string.Empty,
                Type = "Should_not_sync_dlc_when_no_dlc"
            }, new List<IDLC>());
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_sync_dlc.
        /// </summary>
        [Fact]
        public async Task Should_sync_dlc()
        {
            var dlc = new DLC()
            {
                Name = "test",
                Path = "dlc/dlc01.dlc",
                IsEnabled = true
            };
            var dlc2 = new DLC()
            {
                Name = "test",
                Path = "dlc/dlc02.dlc",
                IsEnabled = true
            };
            var dlcExport = new Mock<IDLCExporter>();
            dlcExport.Setup(p => p.GetDisabledDLCAsync(It.IsAny<DLCParameters>())).ReturnsAsync(() => new List<IDLCObject>() { new DLCObject() { Path = "dlc/dlc01.dlc" } });
            var service = new DLCService(dlcExport.Object, null, null, null, null, null);
            var result = await service.SyncStateAsync(new Game()
            {
                ExecutableLocation = string.Empty,
                Type = "Should_sync_dlc"
            }, new List<IDLC>()
            {
                dlc, dlc2
            });
            result.Should().BeTrue();
            dlc.IsEnabled.Should().BeFalse();
            dlc2.IsEnabled.Should().BeTrue();
        }
    }
}
