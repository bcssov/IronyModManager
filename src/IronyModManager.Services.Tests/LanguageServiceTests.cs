// ***********************************************************************
// Assembly         : IronyModManager.Services.Tests
// Author           : Mario
// Created          : 02-04-2020
//
// Last Modified By : Mario
// Last Modified On : 03-25-2021
// ***********************************************************************
// <copyright file="LanguageServiceTests.cs" company="Mario">
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
using IronyModManager.Localization;
using IronyModManager.Localization.ResourceProviders;
using IronyModManager.Models;
using IronyModManager.Models.Common;
using IronyModManager.Services.Common;
using IronyModManager.Shared.Cache;
using IronyModManager.Storage.Common;
using IronyModManager.Tests.Common;
using Moq;
using Xunit;

namespace IronyModManager.Services.Tests
{
    /// <summary>
    /// Class LanguageServiceTests.
    /// </summary>
    public class LanguageServiceTests
    {

        /// <summary>
        /// Setups the mock success case.
        /// </summary>
        /// <param name="locManager">The loc manager.</param>
        /// <param name="resourceProvider">The resource provider.</param>
        /// <param name="preferencesService">The preferences service.</param>
        private void SetupMockSuccessCase(Mock<ILocalizationManager> locManager, Mock<IDefaultLocalizationResourceProvider> resourceProvider, Mock<IPreferencesService> preferencesService)
        {
            DISetup.SetupContainer();
            CurrentLocale.SetCurrent("en");
            locManager.Setup(p => p.GetResource(It.IsAny<string>(), It.IsAny<string>())).Returns("Roboto");
            resourceProvider.Setup(p => p.GetAvailableLocales()).Returns(() =>
            {
                return new List<string>() { "en", "de" };
            });
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return new Preferences()
                {
                    Locale = "en"
                };
            });
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns(true);
        }

        /// <summary>
        /// Setups the mock fail case.
        /// </summary>
        /// <param name="locManager">The loc manager.</param>
        /// <param name="resourceProvider">The resource provider.</param>
        /// <param name="preferencesService">The preferences service.</param>
        private void SetupMockFailCase(Mock<ILocalizationManager> locManager, Mock<IDefaultLocalizationResourceProvider> resourceProvider, Mock<IPreferencesService> preferencesService)
        {
            DISetup.SetupContainer();
            CurrentLocale.SetCurrent("en");
            locManager.Setup(p => p.GetResource(It.IsAny<string>(), It.IsAny<string>())).Returns("Roboto");
            resourceProvider.Setup(p => p.GetAvailableLocales()).Returns(() =>
            {
                return new List<string>() { "es", "de" };
            });
            preferencesService.Setup(p => p.Get()).Returns(() =>
            {
                return new Preferences()
                {
                    Locale = "en"
                };
            });
            preferencesService.Setup(p => p.Save(It.IsAny<IPreferences>())).Returns(true);
        }

        /// <summary>
        /// Defines the test method Should_apply_selected_language.
        /// </summary>
        [Fact]
        public void Should_apply_selected_language()
        {
            var resourceProvider = new Mock<IDefaultLocalizationResourceProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            var locManager = new Mock<ILocalizationManager>();
            SetupMockSuccessCase(locManager, resourceProvider, preferencesService);

            var languageService = new LanguagesService(new Cache(), locManager.Object, resourceProvider.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
            languageService.ApplySelected().Should().Be(true);
            CurrentLocale.CultureName.Should().Be("en");
        }

        /// <summary>
        /// Shoulds the contain selected language.
        /// </summary>
        [Fact] 
        public void Should_contain_selected_language()
        {
            var resourceProvider = new Mock<IDefaultLocalizationResourceProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            var locManager = new Mock<ILocalizationManager>();
            SetupMockSuccessCase(locManager, resourceProvider, preferencesService);

            var languageService = new LanguagesService(new Cache(), locManager.Object, resourceProvider.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
            var result = languageService.Get();
            result.Count().Should().Be(2);
            result.Count(p => p.IsSelected).Should().Be(1);
            result.FirstOrDefault(p => p.IsSelected).Abrv.Should().Be("en");
        }

        /// <summary>
        /// Defines the test method Should_return_selected_language.
        /// </summary>
        [Fact]
        public void Should_return_selected_language()
        {
            var resourceProvider = new Mock<IDefaultLocalizationResourceProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            var locManager = new Mock<ILocalizationManager>();
            SetupMockSuccessCase(locManager, resourceProvider, preferencesService);

            var languageService = new LanguagesService(new Cache(), locManager.Object, resourceProvider.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
            var result = languageService.GetSelected();            
            result.Abrv.Should().Be("en");
        }

        /// <summary>
        /// Defines the test method Should_save_selected_language.
        /// </summary>
        [Fact]
        public void Should_save_selected_language()
        {
            var resourceProvider = new Mock<IDefaultLocalizationResourceProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            var locManager = new Mock<ILocalizationManager>();
            SetupMockSuccessCase(locManager, resourceProvider, preferencesService);

            var languageService = new LanguagesService(new Cache(), locManager.Object, resourceProvider.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
            var result = languageService.Save(new Language()
            {
                Abrv = "de",
                IsSelected = true,
                Name = "German"
            });
            result.Should().Be(true);
            CurrentLocale.CultureName.Should().Be("de");
        }

        /// <summary>
        /// Defines the test method Should_throw_exception_when_saving_non_selected_language.
        /// </summary>
        [Fact]
        public void Should_throw_exception_when_saving_non_selected_language()
        {
            var resourceProvider = new Mock<IDefaultLocalizationResourceProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            var locManager = new Mock<ILocalizationManager>();
            SetupMockFailCase(locManager, resourceProvider, preferencesService);

            var languageService = new LanguagesService(new Cache(), locManager.Object, resourceProvider.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
            try
            {
                languageService.Save(new Language()
                {
                    Abrv = "de",
                    IsSelected = false,
                    Name = "German"
                });
            }
            catch (Exception ex)
            {
                ex.GetType().Should().Be(typeof(InvalidOperationException));
                CurrentLocale.CultureName.Should().Be("en");                
            }                        
        }

        /// <summary>
        /// Shoulds the set selected language.
        /// </summary>
        [Fact] 
        public void Should_set_selected_language()
        {
            var resourceProvider = new Mock<IDefaultLocalizationResourceProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            var locManager = new Mock<ILocalizationManager>();
            SetupMockSuccessCase(locManager, resourceProvider, preferencesService);

            var languageService = new LanguagesService(new Cache(), locManager.Object, resourceProvider.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
            var languages = new List<ILanguage>
            {
                new Language()
                {
                    Abrv = "en",
                    IsSelected = true
                },
                new Language()
                {
                    Abrv = "de",
                    IsSelected = false
                }
            };
            var result = languageService.SetSelected(languages, new Language()
            {
                Abrv = "de",
                IsSelected = true
            });
            result.Should().Be(true);
            CurrentLocale.CultureName.Should().Be("de");
        }

        /// <summary>
        /// Defines the test method Should_throw_validation_errors_when_setting_selected_language.
        /// </summary>
        [Fact]
        public void Should_throw_validation_errors_when_setting_selected_language()
        {
            var resourceProvider = new Mock<IDefaultLocalizationResourceProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            var locManager = new Mock<ILocalizationManager>();
            SetupMockFailCase(locManager, resourceProvider, preferencesService);

            var languageService = new LanguagesService(new Cache(), locManager.Object, resourceProvider.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
            var languages = new List<ILanguage>
            {
                new Language()
                {
                    Abrv = "en",
                    IsSelected = true
                },
                new Language()
                {
                    Abrv = "de",
                    IsSelected = false
                }
            };

            Exception exception = null;

            try
            {
                languageService.SetSelected(languages, null);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            exception.GetType().Should().Be(typeof(ArgumentNullException));
            CurrentLocale.CultureName.Should().Be("en");
            exception = null;

            try
            {
                languageService.SetSelected(null, new Language()
                {
                    Abrv =  "de",
                    IsSelected = true
                });
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            exception.GetType().Should().Be(typeof(ArgumentNullException));
            CurrentLocale.CultureName.Should().Be("en");
            exception = null;

            try
            {
                languageService.SetSelected(new List<ILanguage>(), new Language()
                {
                    Abrv = "de",
                    IsSelected = true
                });
            }
            catch (Exception ex)
            {
                exception = ex;                
            }
            exception.GetType().Should().Be(typeof(ArgumentNullException));
            CurrentLocale.CultureName.Should().Be("en");
        }

        /// <summary>
        /// Defines the test method Should_not_set_selected_language.
        /// </summary>
        [Fact]
        public void Should_not_set_selected_language()
        {
            var resourceProvider = new Mock<IDefaultLocalizationResourceProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            var locManager = new Mock<ILocalizationManager>();
            SetupMockSuccessCase(locManager, resourceProvider, preferencesService);

            var languageService = new LanguagesService(new Cache(), locManager.Object, resourceProvider.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
            var languages = new List<ILanguage>
            {
                new Language()
                {
                    Abrv = "en",
                    IsSelected = true
                },
                new Language()
                {
                    Abrv = "de",
                    IsSelected = false
                }
            };
            var result = languageService.SetSelected(languages, new Language()
            {
                Abrv = "en",
                IsSelected = true
            });
            result.Should().Be(false);
            CurrentLocale.CultureName.Should().Be("en");
        }

        /// <summary>
        /// Defines the test method Should_return_language_by_supported_name_block.
        /// </summary>
        [Fact]
        public void Should_return_language_by_supported_name_block()
        {
            var resourceProvider = new Mock<IDefaultLocalizationResourceProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            var locManager = new Mock<ILocalizationManager>();
            SetupMockSuccessCase(locManager, resourceProvider, preferencesService);
            locManager.Setup(p => p.GetResource(It.IsAny<string>(), It.IsAny<string>())).Returns((string l, string r) =>
            {
                if (r.Contains("SupportedNameBlock"))
                {
                    if (l.Equals("jp"))
                    {
                        return "IsKatakana";
                    }
                    return string.Empty;
                }
                return "Roboto";
            });
            resourceProvider.Setup(p => p.GetAvailableLocales()).Returns(() =>
            {
                return new List<string>() { "en", "de", "jp" };
            });

            var languageService = new LanguagesService(new Cache(), locManager.Object, resourceProvider.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);            
            var l = languageService.GetLanguageBySupportedNameBlock("テスト");
            l.Abrv.Should().Be("jp");
        }

        /// <summary>
        /// Defines the test method Should__not_return_language_by_supported_name_block.
        /// </summary>
        [Fact]
        public void Should_return_selected_language_by_supported_name_block()
        {
            var resourceProvider = new Mock<IDefaultLocalizationResourceProvider>();
            var preferencesService = new Mock<IPreferencesService>();
            var locManager = new Mock<ILocalizationManager>();
            SetupMockSuccessCase(locManager, resourceProvider, preferencesService);
            locManager.Setup(p => p.GetResource(It.IsAny<string>(), It.IsAny<string>())).Returns((string l, string r) =>
            {
                if (r.Contains("SupportedNameBlock"))
                {
                    if (l.Equals("jp"))
                    {
                        return "IsKatakana";
                    }
                    return string.Empty;
                }
                return "Roboto";
            });
            resourceProvider.Setup(p => p.GetAvailableLocales()).Returns(() =>
            {
                return new List<string>() { "en", "de", "jp" };
            });

            var languageService = new LanguagesService(new Cache(), locManager.Object, resourceProvider.Object, preferencesService.Object, new Mock<IStorageProvider>().Object, new Mock<IMapper>().Object);
            var l = languageService.GetLanguageBySupportedNameBlock("test");
            l.Abrv.Should().Be("en");
        }
    }
}
