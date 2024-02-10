
// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-10-2020
//
// Last Modified By : Mario
// Last Modified On : 02-10-2024
// ***********************************************************************
// <copyright file="App.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.DI;
using IronyModManager.Implementation;
using IronyModManager.Implementation.Actions;
using IronyModManager.Implementation.Overlay;
using IronyModManager.Localization;
using IronyModManager.Models.Common;
using IronyModManager.Platform.Fonts;
using IronyModManager.Platform.Themes;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.ViewModels;
using IronyModManager.Views;
using ReactiveUI;

namespace IronyModManager
{

    /// <summary>
    /// Class App.
    /// Implements the <see cref="Avalonia.Application" />
    /// </summary>
    /// <seealso cref="Avalonia.Application" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class App : Application
    {
        #region Fields

        /// <summary>
        /// The show fatal notification
        /// </summary>
        private bool showFatalNotification = false;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="App" /> class.
        /// </summary>
        public App()
        {
            DataContext = new AppViewModel();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the main window.
        /// </summary>
        /// <value>The main window.</value>
        public static MainWindow MainWindow { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Initializes the application by loading XAML etc.
        /// </summary>
        public override void Initialize()
        {
            showFatalNotification = StaticResources.CommandLineOptions.ShowFatalErrorNotification;
            AvaloniaXamlLoader.Load(this);
            if (!Design.IsDesignMode)
            {
                InitThemes();
                if (!showFatalNotification)
                {
                    HandleCommandLine();
                }
            }
        }

        /// <summary>
        /// Called when [framework initialization completed].
        /// </summary>
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                InitCulture();
                if (!showFatalNotification)
                {
                    InitApp(desktop);
                    InitAppTitle(desktop);
                    InitAppSizeDefaults(desktop);
                    VerifyWritePermissionsAsync().ConfigureAwait(false);
                }
                else
                {
                    InitFatalErrorMessage(desktop);
                }
            }

            base.OnFrameworkInitializationCompleted();
        }

        /// <summary>
        /// Handles the command line.
        /// </summary>
        protected virtual void HandleCommandLine()
        {
            static void setGame(bool raiseEvent = false)
            {
                if (!string.IsNullOrWhiteSpace(StaticResources.CommandLineOptions.GameAbrv))
                {
                    var gameService = DIResolver.Get<IGameService>();
                    var games = gameService.Get();
                    var game = games.FirstOrDefault(g => g.Abrv.Equals(StaticResources.CommandLineOptions.GameAbrv, StringComparison.OrdinalIgnoreCase));
                    if (game != null)
                    {
                        gameService.SetSelected(games, game);
                        if (raiseEvent)
                        {
                            var mbus = DIResolver.Get<Shared.MessageBus.IMessageBus>();
                            mbus.Publish(new ActiveGameRequestEvent(game));
                        }
                    }
                }
            }
            StaticResources.CommandLineArgsChanged += () =>
            {
                setGame(true);
            };
            setGame();
        }

        /// <summary>
        /// Initializes the application size defaults.
        /// </summary>
        /// <param name="desktop">The desktop.</param>
        protected virtual void InitAppSizeDefaults(IClassicDesktopStyleApplicationLifetime desktop)
        {
            static double validateSize(double minValue, double value)
            {
                if (double.IsNaN(value) || double.IsInfinity(value) || minValue > value)
                {
                    return minValue;
                }
                return value;
            }
            var stateService = DIResolver.Get<IWindowStateService>();
            desktop.MainWindow.Height = validateSize(desktop.MainWindow.MinHeight, desktop.MainWindow.Height) + desktop.MainWindow.ExtendClientAreaTitleBarHeightHint;
            desktop.MainWindow.Width = validateSize(desktop.MainWindow.MinWidth, desktop.MainWindow.Width);
            if (!stateService.IsDefined() && !stateService.IsMaximized())
            {
                desktop.MainWindow.SizeToContent = SizeToContent.Manual;
                desktop.MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
        }

        /// <summary>
        /// Initializes the application title.
        /// </summary>
        /// <param name="desktop">The desktop.</param>
        protected virtual void InitAppTitle(IClassicDesktopStyleApplicationLifetime desktop)
        {
            SetAppTitle(desktop);
            var listener = MessageBus.Current.Listen<LocaleChangedEventArgs>();
            listener.SubscribeObservable(x =>
            {
                SetAppTitle(desktop);
            });
        }

        /// <summary>
        /// Initializes the culture.
        /// </summary>
        protected virtual void InitCulture()
        {
            var langService = DIResolver.Get<ILanguagesService>();
            langService.ApplySelected();
        }

        /// <summary>
        /// Sets the application title.
        /// </summary>
        /// <param name="desktop">The desktop.</param>
        protected virtual void SetAppTitle(IClassicDesktopStyleApplicationLifetime desktop)
        {
            var appTitle = IronyFormatter.Format(DIResolver.Get<ILocalizationManager>().GetResource(LocalizationResources.App.Title),
            new
            {
                AppVersion = FileVersionInfo.GetVersionInfo(GetType().Assembly.Location).ProductVersion.Split("+")[0]
            });
            if (File.Exists(Constants.TitleSuffixFilename))
            {
                var suffix = File.ReadAllLines(Constants.TitleSuffixFilename);
                if (suffix.Length != 0)
                {
                    appTitle = $"{appTitle} ({suffix.FirstOrDefault()})";
                }
            }
            desktop.MainWindow.Title = appTitle;
        }

        /// <summary>
        /// verify write permissions as an asynchronous operation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        protected virtual async Task VerifyWritePermissionsAsync()
        {
            // Hopefully enough time for the UI to show up and such
            await Task.Delay(5000);
            var permissionService = DIResolver.Get<IPermissionCheckService>();
            var permissions = permissionService.VerifyPermissions();
            if (permissions.Count != 0 && permissions.Any(p => !p.Valid))
            {
                var notificationAction = DIResolver.Get<INotificationAction>();
                var locManager = DIResolver.Get<ILocalizationManager>();
                var title = locManager.GetResource(LocalizationResources.UnableToWriteError.Title);
                var message = IronyFormatter.Format(locManager.GetResource(LocalizationResources.UnableToWriteError.Message), new { Environment.NewLine, Paths = string.Join(Environment.NewLine, permissions.Where(p => !p.Valid).Select(p => p.Path).ToList()) });
                await notificationAction.ShowPromptAsync(title, title, message, NotificationType.Error, PromptType.OK);
            }
        }

        /// <summary>
        /// Reinitializes the application.
        /// </summary>
        /// <param name="desktop">The desktop.</param>
        private void InitApp(IClassicDesktopStyleApplicationLifetime desktop)
        {
            var resolver = DIResolver.Get<IViewResolver>();
            var mainWindow = DIResolver.Get<MainWindow>();
            SetFontFamily(mainWindow);
            var vm = (MainWindowViewModel)resolver.ResolveViewModel<MainWindow>();
            mainWindow.DataContext = vm;
            mainWindow.EnsureTitlebarSpacing();
            desktop.MainWindow = mainWindow;
            MainWindow = mainWindow;
        }

        /// <summary>
        /// Initializes the fatal error message.
        /// </summary>
        /// <param name="desktop">The desktop.</param>
        private void InitFatalErrorMessage(IClassicDesktopStyleApplicationLifetime desktop)
        {
            async Task close()
            {
                await Task.Delay(10000);
                var appAction = DIResolver.Get<IAppAction>();
                await Dispatcher.UIThread.SafeInvokeAsync(async () =>
                {
                    await appAction.ExitAppAsync();
                });
            }
            var logger = DIResolver.Get<ILogger>();
            var lastException = logger.GetLastFatalExceptionMessage();
            var locManager = DIResolver.Get<ILocalizationManager>();
            var title = locManager.GetResource(LocalizationResources.FatalError.Title);
            var message = string.IsNullOrWhiteSpace(lastException) ? locManager.GetResource(LocalizationResources.FatalError.Message) : locManager.GetResource(LocalizationResources.FatalError.MessageWithLastError).FormatIronySmart(new { Environment.NewLine, Message = string.Join(Environment.NewLine, lastException.SplitOnNewLine()) });
            var header = locManager.GetResource(LocalizationResources.FatalError.Header);
            var messageBox = MessageBoxes.GetFatalErrorWindow(title, header, message);

            SetFontFamily(messageBox);
            desktop.MainWindow = messageBox;

            var stateService = DIResolver.Get<IWindowStateService>();
            if (!stateService.IsDefined() || stateService.IsMaximized())
            {
                desktop.MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            if (string.IsNullOrWhiteSpace(lastException))
            {
                close().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Initializes the themes.
        /// </summary>
        private void InitThemes()
        {
            var currentTheme = DIResolver.Get<IThemeService>().GetSelected();
            var themeManager = DIResolver.Get<IThemeManager>();
            themeManager.ApplyTheme(currentTheme.Type);

            var themeListener = MessageBus.Current.Listen<ThemeChangedEventArgs>();
            themeListener.SubscribeObservable(x =>
            {
                OnThemeChanged().ConfigureAwait(true);
            });
            var idGenerator = DIResolver.Get<IIDGenerator>();
            var languageListener = MessageBus.Current.Listen<LocaleChangedEventArgs>();
            languageListener.SubscribeObservable(x =>
            {
                var window = (MainWindow)Helpers.GetMainWindow();
                var id = idGenerator.GetNextId();
                window.ViewModel.TriggerManualOverlay(id, true, string.Empty);
                SetFontFamily(window, x.Locale);
                window.ViewModel.TriggerManualOverlay(id, false, string.Empty);
            });
        }

        /// <summary>
        /// Called when [theme changed].
        /// </summary>
        /// <returns>Task.</returns>
        private async Task OnThemeChanged()
        {
            var notificationAction = DIResolver.Get<INotificationAction>();
            var locManager = DIResolver.Get<ILocalizationManager>();
            var title = locManager.GetResource(LocalizationResources.Themes.Restart_Title);
            var message = locManager.GetResource(LocalizationResources.Themes.Restart_Message);
            var header = locManager.GetResource(LocalizationResources.Themes.Restart_Header);
            if (await notificationAction.ShowPromptAsync(title, header, message, NotificationType.Info))
            {
                var path = Environment.ProcessPath;
                var appAction = DIResolver.Get<IAppAction>();
                if (await appAction.RunAsync(path))
                {
                    await appAction.ExitAppAsync();
                }
            }
        }

        /// <summary>
        /// Sets the font family.
        /// </summary>
        /// <param name="mainWindow">The main window.</param>
        /// <param name="locale">The locale.</param>
        private void SetFontFamily(Window mainWindow, string locale = Shared.Constants.EmptyParam)
        {
            var langService = DIResolver.Get<ILanguagesService>();
            ILanguage language;
            if (string.IsNullOrWhiteSpace(locale))
            {
                language = langService.GetSelected();
            }
            else
            {
                language = langService.Get().FirstOrDefault(p => p.Abrv.Equals(locale));
            }
            var fontResolver = DIResolver.Get<IFontFamilyManager>();
            var font = fontResolver.ResolveFontFamily(language.Font);
            mainWindow.FontFamily = font.GetFontFamily();
        }

        #endregion Methods
    }
}
