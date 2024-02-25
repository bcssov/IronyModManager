// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-30-2020
//
// Last Modified By : Mario
// Last Modified On : 02-25-2024
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
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.Views;
using IronyModManager.DI;
using IronyModManager.Models.Common;
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
            var changelog = this.FindControl<HtmlLabel>("changelog");
            var md = new MarkdownSharp.Markdown();

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
                ILanguage language;
                if (string.IsNullOrWhiteSpace(locale))
                {
                    language = langService.GetSelected();
                }
                else
                {
                    language = langService.Get().FirstOrDefault(p => p.Abrv.Equals(locale));
                }

                var themeManager = DIResolver.Get<IThemeManager>();
                var fontResolver = DIResolver.Get<IFontFamilyManager>();
                var font = fontResolver.ResolveFontFamily(language!.Font);
                changelog.BaseStylesheet = themeManager.GetHtmlBaseCSS($"width:660px; font-family:\"{font.Name}\";");
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
