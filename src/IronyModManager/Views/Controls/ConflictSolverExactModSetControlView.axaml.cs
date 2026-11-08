// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 09-19-2026
//
// Last Modified By : Mario
// Last Modified On : 09-19-2026
// ***********************************************************************
// <copyright file="ConflictSolverExactModSetControlView.axaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Reactive.Disposables;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using IronyModManager.Common;
using IronyModManager.Common.Events;
using IronyModManager.Common.Views;
using IronyModManager.ViewModels.Controls;
using ReactiveUI;

namespace IronyModManager.Views.Controls
{
    /// <summary>
    /// Displays the exact participating-mod set rule manager.
    /// </summary>
    public class ConflictSolverExactModSetControlView : BaseControl<ConflictSolverExactModSetControlViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictSolverExactModSetControlView"/> class.
        /// </summary>
        public ConflictSolverExactModSetControlView()
        {
            InitializeComponent();
        }

        /// <inheritdoc />
        protected override void OnActivated(CompositeDisposable disposables)
        {
            var popup = this.FindControl<Popup>("popup");
            popup.Closed += PopupClosed;
            Disposable.Create(() => popup.Closed -= PopupClosed).DisposeWith(disposables);

            MessageBus.Current.Listen<ForceClosePopulsEventArgs>()
                .SubscribeObservable(_ => Dispatcher.UIThread.SafeInvoke(() => ViewModel.ClearSurface()))
                .DisposeWith(disposables);
            base.OnActivated(disposables);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void PopupClosed(object sender, System.EventArgs e)
        {
            ViewModel.ClearSurface();
        }
    }
}
