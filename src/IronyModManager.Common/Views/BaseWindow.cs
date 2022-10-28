// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 01-15-2020
//
// Last Modified By : Mario
// Last Modified On : 10-28-2022
// ***********************************************************************
// <copyright file="BaseWindow.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.ReactiveUI;
using IronyModManager.Common.ViewModels;
using IronyModManager.Shared;
using ReactiveUI;

namespace IronyModManager.Common.Views
{
    /// <summary>
    /// Class BaseWindow.
    /// Implements the <see cref="Avalonia.ReactiveUI.ReactiveWindow{TViewModel}" />
    /// Implements the <see cref="IronyModManager.Common.Views.IBaseWindow" />
    /// </summary>
    /// <typeparam name="TViewModel">The type of the t view model.</typeparam>
    /// <seealso cref="Avalonia.ReactiveUI.ReactiveWindow{TViewModel}" />
    /// <seealso cref="IronyModManager.Common.Views.IBaseWindow" />
    [ExcludeFromCoverage("This should be tested via functional testing.")]
    public abstract class BaseWindow<TViewModel> : ReactiveWindow<TViewModel>, IBaseWindow where TViewModel : BaseViewModel
    {
        #region Fields

        /// <summary>
        /// The is activated property
        /// </summary>
        public static readonly StyledProperty<bool> IsActivatedProperty = AvaloniaProperty.Register<BaseWindow<TViewModel>, bool>(nameof(IsActivated), true);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseWindow{TViewModel}" /> class.
        /// </summary>
        public BaseWindow()
        {
            if (!Design.IsDesignMode)
            {
                this.WhenActivated(disposables =>
                {
                    Disposables = disposables;
                    OnActivated(disposables);
                    IsActivated = true;
                });
                PropertyChanged += BaseWindow_PropertyChanged;
                PositionChanged += BaseWindow_PositionChanged;
            }
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is activated.
        /// </summary>
        /// <value><c>true</c> if this instance is activated; otherwise, <c>false</c>.</value>
        public bool IsActivated
        {
            get => GetValue(IsActivatedProperty);
            protected set => SetValue(IsActivatedProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is center screen.
        /// </summary>
        /// <value><c>true</c> if this instance is center screen; otherwise, <c>false</c>.</value>
        public bool IsCenterScreen { get; private set; }

        /// <summary>
        /// Gets the disposables.
        /// </summary>
        /// <value>The disposables.</value>
        protected CompositeDisposable Disposables { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Shows the window.
        /// </summary>
        public override void Show()
        {
            // If someome in the future is asking why is this overriden and then why it's using reflection in a private method.
            // 1. Window state in Avalonia version 0.10.18 is broken as in not working.
            // 2. It is reported on github but god forbid they include a backport to 0.10 branch,
            //    at the moment of implementing this fix this is only available on 0.11 preview version.
            //    Yes you read that right a preview version so who cares for people on stable version.
            ShowCore(null);
        }

        /// <summary>
        /// Shows the specified parent.
        /// </summary>
        /// <param name="parent">Window that will be a parent of the shown window.</param>
        /// <exception cref="System.ArgumentNullException">parent - Showing a child window requires valid parent.</exception>
        public new void Show(Window parent)
        {
            if (parent is null)
            {
                throw new ArgumentNullException(nameof(parent), "Showing a child window requires valid parent.");
            }
            ShowCore(parent);
        }

        /// <summary>
        /// Called when [activated].
        /// </summary>
        /// <param name="disposables">The disposables.</param>
        protected virtual void OnActivated(CompositeDisposable disposables)
        {
        }

        /// <summary>
        /// Handles the PositionChanged event of the BaseWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="PixelPointEventArgs" /> instance containing the event data.</param>
        private void BaseWindow_PositionChanged(object sender, PixelPointEventArgs e)
        {
            IsCenterScreen = false;
        }

        /// <summary>
        /// Handles the PropertyChanged event of the BaseWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="AvaloniaPropertyChangedEventArgs" /> instance containing the event data.</param>
        private void BaseWindow_PropertyChanged(object sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == WindowStartupLocationProperty)
            {
                IsCenterScreen = WindowStartupLocation == WindowStartupLocation.CenterScreen;
            }
        }

        /// <summary>
        /// Sets the window startup location.
        /// </summary>
        /// <param name="owner">The owner.</param>
        private void SetWindowStartupLocation(IWindowBaseImpl owner = null)
        {
            var startupLocation = WindowStartupLocation;

            if (startupLocation == WindowStartupLocation.CenterOwner &&
                Owner is Window ownerWindow &&
                ownerWindow.WindowState == WindowState.Minimized)
            {
                startupLocation = WindowStartupLocation.CenterScreen;
            }

            var scaling = owner?.DesktopScaling ?? PlatformImpl?.DesktopScaling ?? 1;

            var rect = FrameSize.HasValue ?
                new PixelRect(PixelSize.FromSize(FrameSize.Value, scaling)) :
                new PixelRect(PixelSize.FromSize(ClientSize, scaling));

            if (startupLocation == WindowStartupLocation.CenterScreen)
            {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
                Screen? screen = null;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

                if (owner is not null)
                {
                    screen = Screens.ScreenFromWindow(owner);

                    screen ??= Screens.ScreenFromPoint(owner.Position);
                }

#pragma warning disable IDE0074 // Use compound assignment
                if (screen is null)
                {
                    screen = Screens.ScreenFromPoint(Position);
                }
#pragma warning restore IDE0074 // Use compound assignment

                if (screen != null)
                {
                    Position = screen.WorkingArea.CenterRect(rect).Position;
                }
            }
            else if (startupLocation == WindowStartupLocation.CenterOwner)
            {
                if (owner != null)
                {
                    var ownerSize = owner.FrameSize ?? owner.ClientSize;
                    var ownerRect = new PixelRect(
                        owner.Position,
                        PixelSize.FromSize(ownerSize, scaling));
                    Position = ownerRect.CenterRect(rect).Position;
                }
            }
        }

        /// <summary>
        /// Shows the core.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <exception cref="System.InvalidOperationException">Cannot re-show a closed window.</exception>
        /// <exception cref="System.InvalidOperationException">Cannot show a window with a closed parent.</exception>
        /// <exception cref="System.InvalidOperationException">A Window cannot be its own parent.</exception>
        /// <exception cref="System.InvalidOperationException">Cannot show window with non-visible parent.</exception>
        private void ShowCore(Window parent)
        {
            //TODO: Remove this override once upgraded to 0.11
            if (PlatformImpl == null)
            {
                throw new InvalidOperationException("Cannot re-show a closed window.");
            }

            if (parent != null)
            {
                if (parent.PlatformImpl == null)
                {
                    throw new InvalidOperationException("Cannot show a window with a closed parent.");
                }
                else if (parent == this)
                {
                    throw new InvalidOperationException("A Window cannot be its own parent.");
                }
                else if (!parent.IsVisible)
                {
                    throw new InvalidOperationException("Cannot show window with non-visible parent.");
                }
            }

            if (IsVisible)
            {
                return;
            }

            var windowState = WindowState;

            RaiseEvent(new RoutedEventArgs(WindowOpenedEvent));

            EnsureInitialized();
            IsVisible = true;

            var initialSize = new Size(
                double.IsNaN(Width) ? Math.Max(MinWidth, ClientSize.Width) : Width,
                double.IsNaN(Height) ? Math.Max(MinHeight, ClientSize.Height) : Height);

            if (initialSize != ClientSize)
            {
                PlatformImpl?.Resize(initialSize, PlatformResizeReason.Layout);
            }
            if (windowState != WindowState)
            {
                WindowState = windowState;
            }

            LayoutManager.ExecuteInitialLayoutPass();

            if (parent != null)
            {
                PlatformImpl?.SetParent(parent.PlatformImpl);
            }

            Owner = parent;
            if (parent != null)
            {
                var addChild = typeof(Window).GetMethod("AddChild", BindingFlags.NonPublic | BindingFlags.Instance);
                addChild.Invoke(this, new object[] { this, false });
            }

            SetWindowStartupLocation(Owner?.PlatformImpl);

            PlatformImpl?.Show(ShowActivated, false);
            Renderer?.Start();
            OnOpened(EventArgs.Empty);
        }

        #endregion Methods
    }
}
