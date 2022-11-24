// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-23-2022
//
// Last Modified By : Mario
// Last Modified On : 11-23-2022
// ***********************************************************************
// <copyright file="IronyCaptionButtons.cs" company="Avalonia">
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
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;

namespace IronyModManager.Controls.ClientSideDecorations
{
#nullable enable
    /// <summary>
    /// Class IronyCaptionButtons.
    /// </summary>
    [TemplatePart("PART_CloseButton", typeof(Button))]
    [TemplatePart("PART_RestoreButton", typeof(Button))]
    [TemplatePart("PART_MinimiseButton", typeof(Button))]
    [TemplatePart("PART_FullScreenButton", typeof(Button))]
    [PseudoClasses(":minimized", ":normal", ":maximized", ":fullscreen")]
    public class IronyCaptionButtons : TemplatedControl, IStyleable
    {
        #region Fields

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
        Type IStyleable.StyleKey => typeof(IronyCaptionButtons);

        /// <summary>
        /// Currently attached window.
        /// </summary>
        /// <value>The host window.</value>
        protected Window? HostWindow { get; private set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Attaches the specified host window.
        /// </summary>
        /// <param name="hostWindow">The host window.</param>
        public virtual void Attach(Window hostWindow)
        {
            if (_disposables == null)
            {
                HostWindow = hostWindow;

                _disposables = new CompositeDisposable
                {
                    HostWindow.GetObservable(Window.WindowStateProperty)
                    .Subscribe(x =>
                    {
                        PseudoClasses.Set(":minimized", x == WindowState.Minimized);
                        PseudoClasses.Set(":normal", x == WindowState.Normal);
                        PseudoClasses.Set(":maximized", x == WindowState.Maximized);
                        PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);
                    })
                };
            }
        }

        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public virtual void Detach()
        {
            if (_disposables != null)
            {
                _disposables.Dispose();
                _disposables = null;

                HostWindow = null;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:ApplyTemplate" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TemplateAppliedEventArgs"/> instance containing the event data.</param>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            var closeButton = e.NameScope.Get<Button>("PART_CloseButton");
            var restoreButton = e.NameScope.Get<Button>("PART_RestoreButton");
            var minimiseButton = e.NameScope.Get<Button>("PART_MinimiseButton");
            var fullScreenButton = e.NameScope.Get<Button>("PART_FullScreenButton");

            closeButton.Click += (sender, e) => OnClose();
            restoreButton.Click += (sender, e) => OnRestore();
            minimiseButton.Click += (sender, e) => OnMinimize();
            fullScreenButton.Click += (sender, e) => OnToggleFullScreen();
        }

        /// <summary>
        /// Raises the Close event.
        /// </summary>
        protected virtual void OnClose()
        {
            HostWindow?.Close();
        }

        /// <summary>
        /// Called when [minimize].
        /// </summary>
        protected virtual void OnMinimize()
        {
            if (HostWindow != null)
            {
                HostWindow.WindowState = WindowState.Minimized;
            }
        }

        /// <summary>
        /// Called when [restore].
        /// </summary>
        protected virtual void OnRestore()
        {
            if (HostWindow != null)
            {
                HostWindow.WindowState = HostWindow.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }
        }

        /// <summary>
        /// Called when [toggle full screen].
        /// </summary>
        protected virtual void OnToggleFullScreen()
        {
            if (HostWindow != null)
            {
                HostWindow.WindowState = HostWindow.WindowState == WindowState.FullScreen
                    ? WindowState.Normal
                    : WindowState.FullScreen;
            }
        }

        #endregion Methods
    }
}
