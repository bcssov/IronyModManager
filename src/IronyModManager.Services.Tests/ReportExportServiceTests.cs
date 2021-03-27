// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 03-27-2021
//
// Last Modified By : Mario
// Last Modified On : 03-27-2021
// ***********************************************************************
// <copyright file="ReportExportServiceTests.cs" company="Mario">
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
using IronyModManager.IO.Common;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Shared.MessageBus;
using IronyModManager.Storage.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class ReportExportServiceTests.
    /// </summary>
    public class ReportExportServiceTests
    {
        /// <summary>
        /// Defines the test method Should_import_report.
        /// </summary>
        [Fact]
        public async Task Should_import_report()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            var innerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\1", Hash = "2" } };
            var outerReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = innerReports } };
            exporter.Setup(p => p.ImportAsync(It.IsAny<string>())).ReturnsAsync(outerReports);

            var service = new ReportExportService(null, exporter.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportAsync("test");
            result.Should().NotBeNull();
        }

        /// <summary>
        /// Defines the test method Should_not_import_report.
        /// </summary>
        [Fact]
        public async Task Should_not_import_report()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            exporter.Setup(p => p.ImportAsync(It.IsAny<string>())).ReturnsAsync((IEnumerable<IHashReport>)null);

            var service = new ReportExportService(null, exporter.Object, storageProvider.Object, mapper.Object);
            var result = await service.ImportAsync("test");
            result.Should().BeNull();
        }


        /// <summary>
        /// Defines the test method Should_export_report.
        /// </summary>
        [Fact]
        public async Task Should_export_report()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            exporter.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync(true);

            var innerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\1", Hash = "2" } };
            var outerReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = innerReports } };
            var service = new ReportExportService(null, exporter.Object, storageProvider.Object, mapper.Object);
            var result = await service.ExportAsync(outerReports, "test");
            result.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_not_export_report.
        /// </summary>
        [Fact]
        public async Task Should_not_export_report()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            exporter.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync(false);

            var innerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\1", Hash = "2" } };
            var outerReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = innerReports } };
            var service = new ReportExportService(null, exporter.Object, storageProvider.Object, mapper.Object);
            var result = await service.ExportAsync(outerReports, "test");
            result.Should().BeFalse();
        }

        /// <summary>
        /// Defines the test method Should_return_collection_report.
        /// </summary>
        [Fact]
        public void Should_return_collection_report()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            exporter.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync(true);

            var innerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\1", Hash = "2" } };
            var outerReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = innerReports } };
            var service = new ReportExportService(null, exporter.Object, storageProvider.Object, mapper.Object);
            var result = service.GetCollectionReports(outerReports.ToList());
            result.Count().Should().Be(1);
        }

        /// <summary>
        /// Defines the test method Should_not_return_collection_report.
        /// </summary>
        [Fact]
        public void Should_not_return_collection_report()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            exporter.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync(true);

            var innerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\1", Hash = "2" } };
            var outerReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = innerReports, ReportType = HashReportType.Game } };
            var service = new ReportExportService(null, exporter.Object, storageProvider.Object, mapper.Object);
            var result = service.GetCollectionReports(outerReports.ToList());
            result.Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_return_collection_report_when_report_null.
        /// </summary>
        [Fact]
        public void Should_not_return_collection_report_when_report_null()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            exporter.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync(true);
            
            var service = new ReportExportService(null, exporter.Object, storageProvider.Object, mapper.Object);
            var result = service.GetCollectionReports(null);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_return_game_report.
        /// </summary>
        [Fact]
        public void Should_return_game_report()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            exporter.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync(true);

            var innerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\1", Hash = "2" } };
            var outerReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = innerReports, ReportType = HashReportType.Game } };
            var service = new ReportExportService(null, exporter.Object, storageProvider.Object, mapper.Object);
            var result = service.GetGameReports(outerReports.ToList());
            result.Count().Should().Be(1);
        }

        /// <summary>
        /// Defines the test method Should_not_return_game_report.
        /// </summary>
        [Fact]
        public void Should_not_return_game_report()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            exporter.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync(true);

            var innerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\1", Hash = "2" } };
            var outerReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = innerReports, ReportType = HashReportType.Collection } };
            var service = new ReportExportService(null, exporter.Object, storageProvider.Object, mapper.Object);
            var result = service.GetGameReports(outerReports.ToList());
            result.Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_not_return_collection_report_when_report_null.
        /// </summary>
        [Fact]
        public void Should_not_return_game_report_when_report_null()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            exporter.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync(true);

            var service = new ReportExportService(null, exporter.Object, storageProvider.Object, mapper.Object);
            var result = service.GetGameReports(null);
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_return_hash_from_both_sources.
        /// </summary>
        [Fact]
        public void Should_return_hash_from_both_sources()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            exporter.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync(true);

            var firstInnerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\1", Hash = "3" } };
            var firstOuterReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = firstInnerReports, ReportType = HashReportType.Game } };

            var secondInnerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\test", Hash = "2" } };
            var secondOuterReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = secondInnerReports, ReportType = HashReportType.Game } };

            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));

            var service = new ReportExportService(messageBus.Object, exporter.Object, storageProvider.Object, mapper.Object);
            var result = service.CompareReports(firstOuterReports, secondOuterReports);
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result.FirstOrDefault().Reports.Count.Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_return_import_hash_with_diff_only.
        /// </summary>
        [Fact]
        public void Should_return_hash_with_diff_only()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            exporter.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync(true);

            var firstInnerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\test", Hash = "3" } };
            var firstOuterReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = firstInnerReports, ReportType = HashReportType.Game } };

            var secondInnerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\test", Hash = "2" } };
            var secondOuterReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = secondInnerReports, ReportType = HashReportType.Game } };

            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));

            var service = new ReportExportService(messageBus.Object, exporter.Object, storageProvider.Object, mapper.Object);
            var result = service.CompareReports(firstOuterReports, secondOuterReports);
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result.FirstOrDefault().Reports.Count.Should().Be(1);
        }

        /// <summary>
        /// Defines the test method Should_return_hash_when_hashes_same.
        /// </summary>
        [Fact]
        public void Should_return_hash_when_hashes_same()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            var exporter = new Mock<IReportExporter>();
            exporter.Setup(p => p.ExportAsync(It.IsAny<IEnumerable<IHashReport>>(), It.IsAny<string>())).ReturnsAsync(true);

            var firstInnerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\test", Hash = "2" } };
            var firstOuterReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = firstInnerReports, ReportType = HashReportType.Game } };

            var secondInnerReports = new List<IHashFileReport>() { new HashFileReport() { File = "test\\test", Hash = "2" } };
            var secondOuterReports = new List<IHashReport>() { new HashReport() { Name = "testreport", Reports = secondInnerReports, ReportType = HashReportType.Game } };

            var messageBus = new Mock<IMessageBus>();
            messageBus.Setup(p => p.PublishAsync(It.IsAny<IMessageBusEvent>()));
            messageBus.Setup(p => p.Publish(It.IsAny<IMessageBusEvent>()));

            var service = new ReportExportService(messageBus.Object, exporter.Object, storageProvider.Object, mapper.Object);
            var result = service.CompareReports(firstOuterReports, secondOuterReports);
            result.Should().NotBeNull();
            result.Count.Should().Be(0);
        }
    }
}
