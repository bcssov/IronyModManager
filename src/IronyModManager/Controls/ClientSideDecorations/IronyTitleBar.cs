// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-23-2022
//
// Last Modified By : Mario
// Last Modified On : 11-23-2022
// ***********************************************************************
// <copyright file="IronyTitleBar.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Taken from avalonia.</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace IronyModManager.Controls.ClientSideDecorations
{
#nullable enable

    /// <summary>
    /// Class IronyTitleBar.
    /// </summary>
    [TemplatePart("PART_CaptionButtons", typeof(IronyCaptionButtons))]
    [PseudoClasses(":minimized", ":normal", ":maximized", ":fullscreen")]
    public class IronyTitleBar : TemplatedControl, IStyleable
    {
        #region Fields

        /// <summary>
        /// The caption buttons
        /// </summary>
        private IronyCaptionButtons? _captionButtons;

        /// <summary>
        /// The container
        /// </summary>
        private Panel? _container;

        /// <summary>
        /// The disposables
        /// </summary>
        private CompositeDisposable? _disposables;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the type by which the control is styled.
        /// </summary>
        /// <value>The style key.</value>
        Type IStyleable.StyleKey => typeof(IronyTitleBar);

        #endregion Properties

        #region Methods

        /// <summary>
        /// Handles the <see cref="E:ApplyTemplate" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TemplateAppliedEventArgs" /> instance containing the event data.</param>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _captionButtons?.Detach();

            _container = e.NameScope.Get<Panel>("PART_Container");

            _captionButtons = e.NameScope.Get<IronyCaptionButtons>("PART_CaptionButtons");

            if (VisualRoot is Window window)
            {
                _captionButtons?.Attach(window);
                _container.PointerPressed += (_, args) =>
                {
                    if (args.ClickCount == 1)
                        window.PlatformImpl?.BeginMoveDrag(args);
                    else
                        window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
                };

                UpdateSize(window);
            }
        }

        /// <summary>
        /// Called when the control is added to a rooted visual tree.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (VisualRoot is Window window)
            {
                _disposables = new CompositeDisposable
                {
                    window.GetObservable(Window.WindowDecorationMarginProperty)
                        .Subscribe(x => UpdateSize(window)),
                    window.GetObservable(Window.ExtendClientAreaTitleBarHeightHintProperty)
                        .Subscribe(x => UpdateSize(window)),
                    window.GetObservable(Window.OffScreenMarginProperty)
                        .Subscribe(x => UpdateSize(window)),
                    window.GetObservable(Window.ExtendClientAreaChromeHintsProperty)
                        .Subscribe(x => UpdateSize(window)),
                    window.GetObservable(Window.WindowStateProperty)
                        .Subscribe(x =>
                        {
                            PseudoClasses.Set(":minimized", x == WindowState.Minimized);
                            PseudoClasses.Set(":normal", x == WindowState.Normal);
                            PseudoClasses.Set(":maximized", x == WindowState.Maximized);
                            PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);
                        }),
                    window.GetObservable(Window.IsExtendedIntoWindowDecorationsProperty)
                        .Subscribe(x => UpdateSize(window))
                };
            }
        }

        /// <summary>
        /// Called when the control is removed from a rooted visual tree.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            _disposables?.Dispose();

            _container = null;
            _captionButtons?.Detach();
            _captionButtons = null;
        }

        /// <summary>
        /// Updates the size.
        /// </summary>
        /// <param name="window">The window.</param>
        private void UpdateSize(Window window)
        {
            if (window != null)
            {
                Margin = new Thickness(
                    window.OffScreenMargin.Left,
                    window.OffScreenMargin.Top,
                    window.OffScreenMargin.Right,
                    window.OffScreenMargin.Bottom);

                if (window.WindowState != WindowState.FullScreen)
                {
                    Height = window.WindowDecorationMargin.Top;

                    if (_captionButtons != null)
                    {
                        _captionButtons.Height = Height;
                    }
                }

                IsVisible = window.PlatformImpl?.NeedsManagedDecorations ?? false;
            }
        }

        #endregion Methods
    }
}
