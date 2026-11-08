// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-30-2020
//
// Last Modified By : Mario
// Last Modified On : 09-18-2026
// ***********************************************************************
// <copyright file="OptionsControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using Avalonia.Controls;
using Avalonia.Controls.Html;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.Views;
using IronyModManager.DI;
using IronyModManager.Platform.Fonts;
using IronyModManager.Platform.Themes;
using IronyModManager.Services.Common;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class OptionsControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.OptionsControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.OptionsControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class OptionsControlView : BaseControl<OptionsControlViewModel>
    {
        #region Fields

        /// <summary>
        /// The popup viewport margin
        /// </summary>
        private const double PopupViewportMargin = 16;

        /// <summary>
        /// The preferred popup height
        /// </summary>
        private const double PreferredPopupHeight = 470;

        /// <summary>
        /// The preferred popup width
        /// </summary>
        private const double PreferredPopupWidth = 760;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsControlView" /> class.
        /// </summary>
        public OptionsControlView()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var popup = this.FindControl<Popup>("popup");
            var optionsSurface = this.FindControl<Border>("optionsSurface");
            var changelog = this.FindControl<HtmlLabel>("changelog");
            var md = new MarkdownSharp.Markdown();

            void updatePopupSize()
            {
                if (this.GetVisualRoot() is not Window window)
                {
                    return;
                }

                var screen = window.Screens.ScreenFromVisual(window);
                var scaling = window.PlatformImpl?.RenderScaling ?? screen.PixelDensity;
                var clientSize = window.ClientSize;
                var availableWidth = clientSize.Width;
                var availableHeight = clientSize.Height;
                if (screen != null && scaling > 0)
                {
                    var workingArea = screen.WorkingArea;
                    var logicalScreenWidth = workingArea.Width / scaling;
                    var logicalScreenHeight = workingArea.Height / scaling;
                    availableWidth = availableWidth > 0 ? Math.Min(availableWidth, logicalScreenWidth) : logicalScreenWidth;
                    availableHeight = availableHeight > 0 ? Math.Min(availableHeight, logicalScreenHeight) : logicalScreenHeight;
                }

                var width = Math.Max(0, Math.Min(PreferredPopupWidth, availableWidth - (PopupViewportMargin * 2)));
                var height = Math.Max(0, Math.Min(PreferredPopupHeight, availableHeight - (PopupViewportMargin * 2)));
                if (double.IsNaN(optionsSurface.Width) || Math.Abs(optionsSurface.Width - width) > 0.5)
                {
                    optionsSurface.Width = width;
                }

                if (double.IsNaN(optionsSurface.Height) || Math.Abs(optionsSurface.Height - height) > 0.5)
                {
                    optionsSurface.Height = height;
                }
            }

            string getVersionHtml()
            {
                var html = new StringBuilder("<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/></head><body>");
                var log = new StringBuilder();
                log.AppendLine($"#{IronyFormatter.Format(ViewModel!.VersionTitle, new { Version = ViewModel.VersionContent })}");
                log.AppendLine(ViewModel.Changelog);
                html.AppendLine(md.Transform(log.ToString()));
                html.AppendLine("</body></html>");
                return html.ToString();
            }

            void setFont(string locale = Shared.Constants.EmptyParam)
            {
                var langService = DIResolver.Get<ILanguagesService>();
                var language = string.IsNullOrWhiteSpace(locale) ? langService.GetSelected() : langService.Get().FirstOrDefault(p => p.Abrv.Equals(locale));

                var themeManager = DIResolver.Get<IThemeManager>();
                var fontResolver = DIResolver.Get<IFontFamilyManager>();
                var font = fontResolver.ResolveFontFamily(language!.Font);
                changelog.BaseStylesheet = themeManager.GetHtmlBaseCSS($"width:100%; font-family:\"{font.Name}\";");
                changelog.Text = getVersionHtml();
            }

            var listener = MessageBus.Current.Listen<LocaleChangedEventArgs>();
            listener.SubscribeObservable(x =>
            {
                setFont(x.Locale);
            });
            setFont();

            popup.Closed += (_, _) =>
            {
                ViewModel!.ForceClose();
            };
            EventHandler popupOpened = (_, _) => updatePopupSize();
            EventHandler layoutUpdated = (_, _) =>
            {
                updatePopupSize();
            };
            popup.Opened += popupOpened;
            LayoutUpdated += layoutUpdated;
            Disposable.Create(() =>
            {
                popup.Opened -= popupOpened;
                LayoutUpdated -= layoutUpdated;
            }).DisposeWith(disposables);
            updatePopupSize();
            MessageBus.Current.Listen<ForceClosePopulsEventArgs>()
                .SubscribeObservable(_ =>
                {
                    Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        ViewModel!.ForceClose();
                    });
                }).DisposeWith(disposables);

            this.WhenAnyValue(p => p.ViewModel.IsActivated).Where(p => p).SubscribeObservable(_ =>
            {
                this.WhenAnyValue(p => p.ViewModel.UpdateInfoVisible).Where(p => p).SubscribeObservable(_ =>
                {
                    changelog.Text = getVersionHtml();
                }).DisposeWith(disposables);
            }).DisposeWith(disposables);

            base.OnActivated(disposables);
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        #endregion Methods
    }
}
