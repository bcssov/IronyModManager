// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 12-07-2020
//
// Last Modified By : Mario
// Last Modified On : 03-25-2021
// ***********************************************************************
// <copyright file="ExternalEditorServiceTests.cs" company="Mario">
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
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Parser.Definitions;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class ExternalEditorServiceTests.
    /// </summary>
    public class ExternalEditorServiceTests
    {
        /// <summary>
        /// Defines the test method Should_return_external_editor.
        /// </summary>
        [Fact]
        public void Should_return_external_editor()
        {
            DISetup.SetupContainer();
            var pref = new Preferences()
            {
                ExternalEditorLocation = "test",
                ExternalEditorParameters = "{Left} {Right}"
            };
            var preferencesService = new Mock<IPreferencesService>();
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return pref;
            });
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            mapper.Setup(s => s.Map<IExternalEditor>(It.IsAny<IPreferences>())).Returns((IPreferences o) =>
            {
                return new ExternalEditor()
                {
                    ExternalEditorLocation = o.ExternalEditorLocation,
                    ExternalEditorParameters = o.ExternalEditorParameters
                };
            });

            var service = new ExternalEditorService(preferencesService.Object, storageProvider.Object, mapper.Object);
            var result = service.Get();
            result.Should().NotBeNull();
            result.ExternalEditorParameters.Should().Be(pref.ExternalEditorParameters);
            result.ExternalEditorLocation.Should().Be(pref.ExternalEditorLocation);
        }

        /// <summary>
        /// Defines the test method Should_save_external_editor.
        /// </summary>
        [Fact]
        public void Should_save_external_editor()
        {
            DISetup.SetupContainer();
            IPreferences saved = null;
            var preferencesService = new Mock<IPreferencesService>();
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns((IPreferences pref) =>
            {
                saved = pref;
                return true;
            });
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            mapper.Setup(s => s.Map(It.IsAny<IExternalEditor>(), It.IsAny<IPreferences>())).Returns((IExternalEditor o, IPreferences p) =>
            {
                return new Preferences()
                {
                    ExternalEditorLocation = o.ExternalEditorLocation,
                    ExternalEditorParameters = o.ExternalEditorParameters
                };
            });

            var service = new ExternalEditorService(preferencesService.Object, storageProvider.Object, mapper.Object);
            service.Save(new ExternalEditor() { ExternalEditorLocation = "test" }).Should().BeTrue();
            saved.ExternalEditorLocation.Should().Be("test");
        }

        /// <summary>
        /// Defines the test method Should_return_launch_args.
        /// </summary>
        [Fact]
        public void Should_return_launch_args()
        {
            DISetup.SetupContainer();
            var pref = new Preferences()
            {
                ExternalEditorLocation = "test",
                ExternalEditorParameters = "{Left} {Right}"
            };
            var preferencesService = new Mock<IPreferencesService>();
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return pref;
            });
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            mapper.Setup(s => s.Map<IExternalEditor>(It.IsAny<IPreferences>())).Returns((IPreferences o) =>
            {
                return new ExternalEditor()
                {
                    ExternalEditorLocation = o.ExternalEditorLocation,
                    ExternalEditorParameters = o.ExternalEditorParameters
                };
            });

            var service = new ExternalEditorService(preferencesService.Object, storageProvider.Object, mapper.Object);
            var result = service.GetLaunchArguments("test1", "test2");
            result.Should().Be("\"test1\" \"test2\"");
        }

        /// <summary>
        /// Defines the test method Should_return_files.
        /// </summary>
        [Fact]
        public void Should_return_files()
        {
            DISetup.SetupContainer();
            DISetup.Container.Register<ITempFile, DummyTempFile>();
            DISetup.Container.RemoveTransientWarning<ITempFile>();
            DI.DIContainer.Finish(true);
            var preferencesService = new Mock<IPreferencesService>();
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();

            var service = new ExternalEditorService(preferencesService.Object, storageProvider.Object, mapper.Object);
            var result = service.GetFiles(new Definition() { ModName = "m1", Id = "id1" }, new Definition() { ModName = "m2", Id = "id2" });
            result.LeftDiff.File.Should().Be("m1 - id1.tmp");
            result.RightDiff.File.Should().Be("m2 - id2.tmp");
        }

        /// <summary>
        /// Defines the test method Should_not_return_launch_args.
        /// </summary>
        [Fact]
        public void Should_not_return_launch_args()
        {
            DISetup.SetupContainer();
            var pref = new Preferences()
            {
                ExternalEditorLocation = "test",
            };
            var preferencesService = new Mock<IPreferencesService>();
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return pref;
            });
            var storageProvider = new Mock<IStorageProvider>();
            var mapper = new Mock<IMapper>();
            mapper.Setup(s => s.Map<IExternalEditor>(It.IsAny<IPreferences>())).Returns((IPreferences o) =>
            {
                return new ExternalEditor()
                {
                    ExternalEditorLocation = o.ExternalEditorLocation,
                    ExternalEditorParameters = o.ExternalEditorParameters
                };
            });

            var service = new ExternalEditorService(preferencesService.Object, storageProvider.Object, mapper.Object);
            var result = service.GetLaunchArguments("test1", "test2");
            result.Should().BeNullOrWhiteSpace();
        }

        /// <summary>
        /// Class DummyTempFile.
        /// Implements the <see cref="IronyModManager.Shared.ITempFile" />
        /// </summary>
        /// <seealso cref="IronyModManager.Shared.ITempFile" />
        public class DummyTempFile : ITempFile
        {
            /// <summary>
            /// Gets the file.
            /// </summary>
            /// <value>The file.</value>
            public string File { get; set; } = "test";

            /// <summary>
            /// Gets the text.
            /// </summary>
            /// <value>The text.</value>
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets the temporary directory.
            /// </summary>
            /// <value>The temporary directory.</value>
            public string TempDirectory { get; set; }

            /// <summary>
            /// Creates the specified path.
            /// </summary>
            /// <param name="fileName">Name of the file.</param>
            /// <returns>System.String.</returns>
            public string Create(string fileName = "")
            {
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    File = fileName;
                    return fileName;
                }
                return File;
            }

            /// <summary>
            /// Deletes this instance.
            /// </summary>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            public bool Delete()
            {
                return true;
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// Gets the name of the temporary file.
            /// </summary>
            /// <param name="desiredFilename">The desired filename.</param>
            /// <returns>System.String.</returns>
            public string GetTempFileName(string desiredFilename)
            {
                return desiredFilename + ".tmp";
            }
        }
    }
}
