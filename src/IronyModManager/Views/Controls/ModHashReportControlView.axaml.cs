// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 10-01-2020
//
// Last Modified By : Mario
// Last Modified On : 10-01-2020
// ***********************************************************************
// <copyright file="ModHashReportControlView.axaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.Views;
using IronyModManager.Shared;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Class ModHashReportControlView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ModHashReportControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ModHashReportControlViewModel}" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public class ModHashReportControlView : BaseControl<ModHashReportControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ModHashReportControlView" /> class.
        /// </summary>
        public ModHashReportControlView()
        {
            this.InitializeComponent();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected override void OnActivated(CompositeDisposable disposables)
        {
            base.OnActivated(disposables);

            var popup = this.FindControl<Popup>("popup");
            popup.Closed += (sender, args) =>
            {
                ViewModel.ForceClose();
            };
            popup.Opened += (sender, args) =>
            {
                var window = Helpers.GetMainWindow();
                var verticalOffset = window.Bounds.Height / 2;
                popup.Host.ConfigurePosition(window, popup.PlacementMode, new Avalonia.Point(popup.HorizontalOffset, verticalOffset),
                    Avalonia.Controls.Primitives.PopupPositioning.PopupPositioningEdge.Top, Avalonia.Controls.Primitives.PopupPositioning.PopupPositioningEdge.None);
            };
            MessageBus.Current.Listen<ForceClosePopulsEventArgs>()
            .SubscribeObservable(x =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ViewModel.ForceClose();
                });
            }).DisposeWith(disposables);
            var tree = this.FindControl<TreeView>("tree");
            this.WhenAnyValue(p => p.ViewModel.Reports).SubscribeObservable(reports =>
            {
                var ds = new List<TreeViewItem>();
                // Why is this set in code behind you ask? Because tree view control needs fixing... much fixing...
                if (reports?.Count() > 0)
                {
                    foreach (var report in reports)
                    {
                        if (report.Reports.Count > 0)
                        {
                            var treeViewItem = new TreeViewItem()
                            {
                                Header = report.Name
                            };
                            var children = new List<TreeViewItem>();
                            foreach (var item in report.Reports)
                            {
                                children.Add(new TreeViewItem()
                                {
                                    Header = item.File
                                });
                            }
                            treeViewItem.Items = children;
                            ds.Add(treeViewItem);
                        }
                    }
                }
                tree.Items = ds;
            }).DisposeWith(disposables);
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
