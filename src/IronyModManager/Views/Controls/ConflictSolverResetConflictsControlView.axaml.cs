// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 06-11-2020
//
// Last Modified By : Mario
// Last Modified On : 01-27-2022
// ***********************************************************************
// <copyright file="ConflictSolverResetConflictsControlView.axaml.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    /// Class ConflictSolverResetConflictsView.
    /// Implements the <see cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ConflictSolverResetConflictsControlViewModel}" />
    /// </summary>
    /// <seealso cref="IronyModManager.Common.Views.BaseControl{IronyModManager.ViewModels.Controls.ConflictSolverResetConflictsControlViewModel}" />
    public class ConflictSolverResetConflictsControlView : BaseControl<ConflictSolverResetConflictsControlViewModel>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ConflictSolverResetConflictsView" /> class.
        /// </summary>
        public ConflictSolverResetConflictsControlView()
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
            popup.Closed += (sender, args) =>
            {
                ViewModel.ForceClosePopup();
            };
            MessageBus.Current.Listen<ForceClosePopulsEventArgs>()
            .SubscribeObservable(x =>
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    ViewModel.ForceClosePopup();
                });
            }).DisposeWith(disposables);
            var conflictList = this.FindControl<Avalonia.Controls.ListBox>("conflictList");
            this.WhenAnyValue(p => p.ViewModel.IsActivated).Where(p => p).SubscribeObservable(s =>
            {
                int? index = null;
                ViewModel.ResetCommand.IsExecuting.SubscribeObservable(s =>
                {
                    if (s)
                    {
                        if (conflictList.ItemContainerGenerator != null && conflictList.ItemContainerGenerator.Containers != null)
                        {
                            var info = conflictList.ItemContainerGenerator.Containers.ToList().LastOrDefault();
                            if (info != null)
                            {
                                index = info.Index;
                            }
                        }
                    }
                    else
                    {
                        if (index.HasValue)
                        {
                            if (index.GetValueOrDefault() >= conflictList.ItemCount)
                            {
                                index = conflictList.ItemCount - 1;
                            }
                            conflictList.ScrollIntoView(index.GetValueOrDefault());
                        }
                        index = null;
                    }
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
