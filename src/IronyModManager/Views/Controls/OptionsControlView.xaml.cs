// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 05-30-2020
//
// Last Modified By : Mario
// Last Modified On : 11-04-2021
// ***********************************************************************
// <copyright file="OptionsControlView.xaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
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
using IronyModManager.Platform.Themes;
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
            var themeManager = DIResolver.Get<IThemeManager>();
            changelog.BaseStylesheet = themeManager.GetHtmlBaseCSS("width:660px;");
            var md = new MarkdownSharp.Markdown();
            popup.Closed += (sender, args) =>
            {
                ViewModel.ForceClose();
            };
            popup.Opened += (sender, args) =>
            {
                popup.Host.ConfigurePosition(popup.PlacementTarget, popup.PlacementMode, new Avalonia.Point(popup.HorizontalOffset, 15),
                    Avalonia.Controls.Primitives.PopupPositioning.PopupAnchor.None, Avalonia.Controls.Primitives.PopupPositioning.PopupGravity.Bottom);
            };
            MessageBus.Current.Listen<ForceClosePopulsEventArgs>()
            .SubscribeObservable(x =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ViewModel.ForceClose();
                });
            }).DisposeWith(disposables);
            this.WhenAnyValue(p => p.ViewModel.IsActivated).Where(p => p).SubscribeObservable(s =>
            {
                this.WhenAnyValue(p => p.ViewModel.UpdateInfoVisible).Where(p => p).SubscribeObservable(s =>
                {
                    var html = new StringBuilder("<!DOCTYPE html><html><head><meta http-equiv='Content-Type' content='text/html;charset=UTF-8'/></head><body>");
                    var log = new StringBuilder();
                    log.AppendLine($"#{ViewModel.VersionTitle}: {ViewModel.VersionContent}");
                    log.AppendLine(ViewModel.Changelog);
                    html.AppendLine(md.Transform(log.ToString()));
                    html.AppendLine("</body></html>");
                    changelog.Text = html.ToString();
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
