// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-04-2020
//
// Last Modified By : Mario
// Last Modified On : 03-16-2021
// ***********************************************************************
// <copyright file="ThemeServiceTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using AwesomeAssertions;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Storage;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class ThemeServiceTests.
    /// </summary>
    public class ThemeServiceTests
    {
        /// <summary>
        /// Setups the mock case.
        /// </summary>
        /// <param name="preferencesService">The preferences service.</param>
        /// <param name="storageProvider">The storage provider.</param>
        private static void SetupMockCase(Mock<IPreferencesService> preferencesService, Mock<IStorageProvider> storageProvider)
        {
            DISetup.SetupContainer();
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return new Preferences()
                {
                    Theme = "MaterialDeepPurple"
                };
            });
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns(true);
            var themes = new List<IThemeType>
            {
                new ThemeType() { Name = "Dark", IsDefault = false },
                new ThemeType() { Name = "MaterialDark", IsDefault = false },
                new ThemeType() { Name = "MaterialLightGreen", IsDefault = false },
                new ThemeType() { Name = "MaterialDeepPurple", IsDefault = false },
                new ThemeType() { Name = "Light", IsDefault = true }
            };
            storageProvider.Setup(p => p.GetThemes()).Returns(themes);
        }

        /// <summary>
        /// Defines the test method Should_contain_all_themes.
        /// </summary>
        [Fact]
        public void Should_contain_all_themes()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new ThemeService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.Count().Should().Be(5);
            result.GroupBy(p => p.Type).Select(p => p.First()).Count().Should().Be(5);
        }

        /// <summary>
        /// Defines the test method Should_contain_selected_theme.
        /// </summary>
        [Fact]
        public void Should_contain_selected_theme()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new ThemeService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.Get();
            result.FirstOrDefault(p => p.IsSelected).Should().NotBeNull();
            result.FirstOrDefault(p => p.IsSelected).Type.Should().Be("MaterialDeepPurple");
        }

        /// <summary>
        /// Defines the test method Should_return_selected_theme.
        /// </summary>
        [Fact]
        public void Should_return_selected_theme()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new ThemeService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.GetSelected();
            result.Should().NotBeNull();
            result.Type.Should().Be("MaterialDeepPurple");
        }

        /// <summary>
        /// Defines the test method Should_save_selected_theme.
        /// </summary>
        [Fact]
        public void Should_save_selected_theme()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new ThemeService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var result = service.Save(new Theme()
            {
                IsSelected = true,
                Type = "MaterialDark"
            });
            result.Should().Be(true);
        }

        /// <summary>
        /// Defines the test method Should_throw_exception_when_saving_non_selected_theme.
        /// </summary>
        [Fact]
        public void Should_throw_exception_when_saving_non_selected_theme()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new ThemeService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            bool exceptionThrown = false;
            try
            {
                service.Save(new Theme()
                {
                    IsSelected = false,
                    Type = "MaterialDark"
                });
            }
            catch (Exception ex)
            {
                ex.GetType().Should().Be(typeof(InvalidOperationException));
                exceptionThrown = true;
            }
            exceptionThrown.Should().BeTrue();
        }

        /// <summary>
        /// Defines the test method Should_set_selected_theme.
        /// </summary>
        [Fact]
        public void Should_set_selected_theme()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new ThemeService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var themes = new List<ITheme>
            {
                new Theme()
                {
                    IsSelected = true,
                    Type = "Dark"
                },
                new Theme()
                {
                    IsSelected = false,
                    Type = "Light"
                }
            };
            var result = service.SetSelected(themes, new Theme()
            {
                IsSelected = true,
                Type = "Light"
            });
            result.Should().Be(true);
        }

        /// <summary>
        /// Defines the test method Should_throw_validation_errors_when_setting_selected_theme.
        /// </summary>
        [Fact]
        public void Should_throw_validation_errors_when_setting_selected_theme()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new ThemeService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var themes = new List<ITheme>
            {
                new Theme()
                {
                    IsSelected = true,
                    Type = "Dark"
                },
                new Theme()
                {
                    IsSelected = false,
                    Type = "Light"
                }
            };

            Exception exception = null;
            try
            {
                service.SetSelected(themes, null);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            exception.GetType().Should().Be(typeof(ArgumentNullException));
            exception = null;
            try
            {
                service.SetSelected(null, new Theme());
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            exception.GetType().Should().Be(typeof(ArgumentNullException));
            exception = null;

            try
            {
                service.SetSelected(new List<ITheme>(), new Theme());
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            exception.GetType().Should().Be(typeof(ArgumentNullException));
        }

        /// <summary>
        /// Defines the test method Should_not_set_selected_theme.
        /// </summary>
        [Fact]
        public void Should_not_set_selected_theme()
        {
            var storageProvider = new Mock<IStorageProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            SetupMockCase(preferencesService, storageProvider);

            var service = new ThemeService(storageProvider.Object, preferencesService.Object, new Mock<IMapper>().Object);
            var themes = new List<ITheme>
            {
                new Theme()
                {
                    IsSelected = true,
                    Type = "MaterialDeepPurple"
                },
                new Theme()
                {
                    IsSelected = false,
                    Type = "Light"
                }
            };
            var result = service.SetSelected(themes, new Theme()
            {
                IsSelected = true,
                Type = "MaterialDeepPurple"
            });
            result.Should().Be(false);
        }
    }
}
