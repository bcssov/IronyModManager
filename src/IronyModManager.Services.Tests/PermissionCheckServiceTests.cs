// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 12-07-2020
//
// Last Modified By : Mario
// Last Modified On : 02-13-2021
// ***********************************************************************
// <copyright file="PermissionCheckServiceTests.cs" company="Mario">
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
    public class PermissionCheckServiceTests
    {
        /// <summary>
        /// Defines the test method Should_validate_due_to_non_existing_directory.
        /// </summary>
        [Fact]
        public void Should_validate_due_to_non_existing_directory()
        {
            DISetup.SetupContainer();
            DISetup.Container.Register<ITempFile, DummyTempFile>();
            DISetup.Container.RemoveTransientWarning<ITempFile>();
            DI.DIContainer.Finish(true);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.Get()).Returns(new List<IGame>()
            {
                new Game()
                {
                    UserDirectory = "test"
                }
            });
            var appStateService = new Mock<IAppStateService>();
            appStateService.Setup(p => p.Get()).Returns(new AppState()
            {
                LastWritableCheck = null
            });
            appStateService.Setup(p => p.Save(It.IsAny<IAppState>())).Returns(true);

            var service = new PermissionCheckService(gameService.Object, appStateService.Object, null, null);
            service.VerifyPermissions().Count.Should().Be(0);
        }


        /// <summary>
        /// Defines the test method Should_validate_due_to_last_check_being_valid.
        /// </summary>
        [Fact]
        public void Should_validate_due_to_last_check_being_valid()
        {
            DISetup.SetupContainer();
            DISetup.Container.Register<ITempFile, DummyTempFile>();
            DISetup.Container.RemoveTransientWarning<ITempFile>();
            DI.DIContainer.Finish(true);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.Get()).Returns(new List<IGame>()
            {
                new Game()
                {
                    UserDirectory = "test"
                }
            });
            var appStateService = new Mock<IAppStateService>();
            appStateService.Setup(p => p.Get()).Returns(new AppState()
            {
                LastWritableCheck = DateTime.Now.AddDays(-1)
            });
            appStateService.Setup(p => p.Save(It.IsAny<IAppState>())).Returns(true);

            var service = new PermissionCheckService(gameService.Object, appStateService.Object, null, null);
            service.VerifyPermissions().Count.Should().Be(0);
        }

        /// <summary>
        /// Defines the test method Should_validate_due_to_last_successful_file_creation.
        /// </summary>
        [Fact]
        public void Should_validate_due_to_last_successful_file_creation()
        {
            DISetup.SetupContainer();
            DISetup.Container.Register<ITempFile, DummyTempFile>();
            DISetup.Container.RemoveTransientWarning<ITempFile>();
            DI.DIContainer.Finish(true);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.Get()).Returns(new List<IGame>()
            {
                new Game()
                {
                    UserDirectory = AppDomain.CurrentDomain.BaseDirectory
                }
            });
            var appStateService = new Mock<IAppStateService>();
            appStateService.Setup(p => p.Get()).Returns(new AppState()
            {
                LastWritableCheck = DateTime.Now.AddDays(-10)
            });
            appStateService.Setup(p => p.Save(It.IsAny<IAppState>())).Returns(true);

            var service = new PermissionCheckService(gameService.Object, appStateService.Object, null, null);
            var result = service.VerifyPermissions();
            result.Count.Should().Be(1);
            foreach (var item in result)
            {
                item.Valid.Should().BeTrue();
            }
        }

        /// <summary>
        /// Defines the test method Should_validate_due_file_creation_error.
        /// </summary>
        [Fact]
        public void Should_not_validate_due_file_creation_error()
        {
            DISetup.SetupContainer();
            DISetup.Container.Register<ITempFile, DummyCreateExceptionTempFile>();
            DISetup.Container.RemoveTransientWarning<ITempFile>();
            DI.DIContainer.Finish(true);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.Get()).Returns(new List<IGame>()
            {
                new Game()
                {
                    UserDirectory = AppDomain.CurrentDomain.BaseDirectory
                }
            });
            var appStateService = new Mock<IAppStateService>();
            appStateService.Setup(p => p.Get()).Returns(new AppState()
            {
                LastWritableCheck = DateTime.Now.AddDays(-10)
            });
            appStateService.Setup(p => p.Save(It.IsAny<IAppState>())).Returns(true);

            var service = new PermissionCheckService(gameService.Object, appStateService.Object, null, null);
            var result = service.VerifyPermissions();
            result.Count.Should().Be(1);
            foreach (var item in result)
            {
                item.Valid.Should().BeFalse();
            }
        }

        /// <summary>
        /// Defines the test method Should_not_validate_due_file_update_error.
        /// </summary>
        [Fact]
        public void Should_not_validate_due_file_update_error()
        {
            DISetup.SetupContainer();
            DISetup.Container.Register<ITempFile, DummyUpdateExceptionTempFile>();
            DISetup.Container.RemoveTransientWarning<ITempFile>();
            DI.DIContainer.Finish(true);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.Get()).Returns(new List<IGame>()
            {
                new Game()
                {
                    UserDirectory = AppDomain.CurrentDomain.BaseDirectory
                }
            });
            var appStateService = new Mock<IAppStateService>();
            appStateService.Setup(p => p.Get()).Returns(new AppState()
            {
                LastWritableCheck = DateTime.Now.AddDays(-10)
            });
            appStateService.Setup(p => p.Save(It.IsAny<IAppState>())).Returns(true);

            var service = new PermissionCheckService(gameService.Object, appStateService.Object, null, null);
            var result = service.VerifyPermissions();
            result.Count.Should().Be(1);
            foreach (var item in result)
            {
                item.Valid.Should().BeFalse();
            }
        }

        /// <summary>
        /// Defines the test method Should_not_validate_due_file_delete_error.
        /// </summary>
        [Fact]
        public void Should_not_validate_due_file_delete_error()
        {
            DISetup.SetupContainer();
            DISetup.Container.Register<ITempFile, DummyDeleteExceptionTempFile>();
            DISetup.Container.RemoveTransientWarning<ITempFile>();
            DI.DIContainer.Finish(true);
            var gameService = new Mock<IGameService>();
            gameService.Setup(p => p.Get()).Returns(new List<IGame>()
            {
                new Game()
                {
                   UserDirectory = AppDomain.CurrentDomain.BaseDirectory
                }
            });
            var appStateService = new Mock<IAppStateService>();
            appStateService.Setup(p => p.Get()).Returns(new AppState()
            {
                LastWritableCheck = DateTime.Now.AddDays(-10)
            });
            appStateService.Setup(p => p.Save(It.IsAny<IAppState>())).Returns(true);

            var service = new PermissionCheckService(gameService.Object, appStateService.Object, null, null);
            var result = service.VerifyPermissions();
            result.Count.Should().Be(1);
            foreach (var item in result)
            {
                item.Valid.Should().BeFalse();
            }
        }

        /// <summary>
        /// Class DummyTempFile.
        /// Implements the <see cref="IronyModManager.Shared.ITempFile" />
        /// </summary>
        /// <seealso cref="IronyModManager.Shared.ITempFile" />
        public class DummyCreateExceptionTempFile : ITempFile
        {
            /// <summary>
            /// Gets the file.
            /// </summary>
            /// <value>The file.</value>
            public string File => "test";

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
            /// <exception cref="UnauthorizedAccessException"></exception>
            public string Create(string fileName = "")
            {
                throw new UnauthorizedAccessException();
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
        }

        /// <summary>
        /// Class DummyTempFile.
        /// Implements the <see cref="IronyModManager.Shared.ITempFile" />
        /// </summary>
        /// <seealso cref="IronyModManager.Shared.ITempFile" />
        public class DummyUpdateExceptionTempFile : ITempFile
        {
            /// <summary>
            /// Gets the file.
            /// </summary>
            /// <value>The file.</value>
            public string File => "test";

            /// <summary>
            /// Gets the text.
            /// </summary>
            /// <value>The text.</value>
            /// <exception cref="UnauthorizedAccessException"></exception>
            public string Text
            {
                get
                {
                    return string.Empty;
                }

                set
                {
                    throw new UnauthorizedAccessException();
                }
            }

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
        }

        /// <summary>
        /// Class DummyTempFile.
        /// Implements the <see cref="IronyModManager.Shared.ITempFile" />
        /// </summary>
        /// <seealso cref="IronyModManager.Shared.ITempFile" />
        public class DummyDeleteExceptionTempFile : ITempFile
        {
            /// <summary>
            /// Gets the file.
            /// </summary>
            /// <value>The file.</value>
            public string File => "test";

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
                return File;
            }

            /// <summary>
            /// Deletes this instance.
            /// </summary>
            /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
            /// <exception cref="UnauthorizedAccessException"></exception>
            public bool Delete()
            {
                throw new UnauthorizedAccessException();
            }

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }
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
            public string File => "test";

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
        }
    }
}
