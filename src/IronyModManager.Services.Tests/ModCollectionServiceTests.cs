// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 03-04-2020
//
// Last Modified By : Mario
// Last Modified On : 03-04-2020
// ***********************************************************************
// <copyright file="ModCollectionServiceTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using FluentAssertions;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class ModCollectionServiceTests.
    /// </summary>
    public class ModCollectionServiceTests
    {
        /// <summary>
        /// Setups the mock case.
        /// </summary>
        /// <param name="storageProvider">The storage provider.</param>
        private void SetupMockCase(Mock<IStorageProvider> storageProvider)
        {
            var collections = new List<IModCollection>()
            {
                new ModCollection()
                {
                    Mods = new List<string>() { "1", "2"},
                    Name = "test"
                },
                new ModCollection()
                {
                    Mods = new List<string>() { "2"},
                    Name = "test2"
                }
            };
            storageProvider.Setup(s => s.GetModCollections()).Returns(() =>
            {
                return collections;
            });
            storageProvider.Setup(s => s.SetModCollections(It.IsAny<IEnumerable<IModCollection>>())).Returns((IEnumerable<IModCollection> col) =>
            {
                collections = col.ToList();
                return true;
            });
        }

        /// <summary>
        /// Defines the test method Should_create_empty_mod_collection_object.
        /// </summary>
        [Fact]
        public void Should_create_empty_mod_collection_object()
        {
            DISetup.SetupContainer();
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();

            var service = new ModCollectionService(storageProvider.Object, mapper.Object);
            var result = service.Create();
            result.Name.Should().BeNullOrEmpty();
            result.Mods.Should().NotBeNull();
            result.Mods.Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_delete_mod_collection.
        /// </summary>
        [Fact]
        public void Should_delete_mod_collection()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            SetupMockCase(storageProvider);

            var service = new ModCollectionService(storageProvider.Object, mapper.Object);
            var result = service.Delete("test");
            result.Should().BeTrue();
            service.GetNames().Count().Should().Be(1);
        }

        /// <summary>
        /// Defines the test method Should_not_delete_mod_collection.
        /// </summary>
        [Fact]
        public void Should_not_delete_mod_collection()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            SetupMockCase(storageProvider);

            var service = new ModCollectionService(storageProvider.Object, mapper.Object);
            var result = service.Delete("test3");
            result.Should().BeFalse();
            service.GetNames().Count().Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_not_delete_mod_collection_when_collection_empty.
        /// </summary>
        [Fact]
        public void Should_not_delete_mod_collection_when_collection_empty()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            SetupMockCase(storageProvider);
            storageProvider.Setup(s => s.GetModCollections()).Returns(new List<IModCollection>());

            var service = new ModCollectionService(storageProvider.Object, mapper.Object);
            var result = service.Delete("test");
            result.Should().BeFalse();
            service.GetNames().Count().Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_return_mod_collection_object.
        /// </summary>
        [Fact]
        public void Should_return_mod_collection_object()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            SetupMockCase(storageProvider);

            var service = new ModCollectionService(storageProvider.Object, mapper.Object);
            var result = service.Get("test");
            result.Should().NotBeNull();
            result.Name.Should().Be("test");
            result.Mods.Count().Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_not_return_mod_collection_object.
        /// </summary>
        [Fact]
        public void Should_not_return_mod_collection_object()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            SetupMockCase(storageProvider);

            var service = new ModCollectionService(storageProvider.Object, mapper.Object);
            var result = service.Get("test3");
            result.Should().BeNull();            
        }

        /// <summary>
        /// Defines the test method Should_not_return_mod_collection_object_when_collection_empty.
        /// </summary>
        [Fact]
        public void Should_not_return_mod_collection_object_when_collection_empty()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            SetupMockCase(storageProvider);
            storageProvider.Setup(s => s.GetModCollections()).Returns(new List<IModCollection>());

            var service = new ModCollectionService(storageProvider.Object, mapper.Object);
            var result = service.Get("test2");
            result.Should().BeNull();
        }

        /// <summary>
        /// Defines the test method Should_throw_exception_when_saving_empty_mod_object.
        /// </summary>
        [Fact]
        public void Should_throw_exception_when_saving_empty_mod_object()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            SetupMockCase(storageProvider);
            var service = new ModCollectionService(storageProvider.Object, mapper.Object);
            Exception exception = null;
            try
            {
                service.Save(null);
            }
            catch (Exception ex)
            {
                exception = ex;                
            }
            exception.GetType().Should().Be(typeof(ArgumentNullException));
        }

        /// <summary>
        /// Defines the test method Should_save_new_mod_object.
        /// </summary>
        [Fact]
        public void Should_save_new_mod_object()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            SetupMockCase(storageProvider);

            var service = new ModCollectionService(storageProvider.Object, mapper.Object);
            var result = service.Save(new ModCollection()
            {
                Name = "fake"
            });
            result.Should().BeTrue();
            service.GetNames().Count().Should().Be(3);
        }

        /// <summary>
        /// Defines the test method Should_overwrite_existing_mod_object.
        /// </summary>
        [Fact]
        public void Should_overwrite_existing_mod_object()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            SetupMockCase(storageProvider);

            var service = new ModCollectionService(storageProvider.Object, mapper.Object);
            var result = service.Save(new ModCollection()
            {
                Name = "test"
            });
            result.Should().BeTrue();
            service.GetNames().Count().Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should_return_mod_names.
        /// </summary>
        [Fact]
        public void Should_return_mod_names()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            SetupMockCase(storageProvider);

            var service = new ModCollectionService(storageProvider.Object, mapper.Object);
            service.GetNames().Count().Should().Be(2);
        }

        /// <summary>
        /// Defines the test method Should__not_return_mod_names.
        /// </summary>
        [Fact]
        public void Should__not_return_mod_names()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            SetupMockCase(storageProvider);
            storageProvider.Setup(s => s.GetModCollections()).Returns(new List<IModCollection>());

            var service = new ModCollectionService(storageProvider.Object, mapper.Object);
            service.GetNames().Count().Should().Be(0);
        }
    }
}
