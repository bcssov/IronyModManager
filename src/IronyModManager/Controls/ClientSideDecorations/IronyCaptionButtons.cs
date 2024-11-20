// ***********************************************************************
// Assembly         : IronyModManager
// Author           : Mario
// Created          : 11-23-2022
//
// Last Modified By : Mario
// Last Modified On : 11-20-2024
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
using Avalonia.Controls.Shapes;
using Avalonia.Media;
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
        /// The full screen active
        /// </summary>
        private static readonly PathGeometry fullScreenActive = PathGeometry.Parse("M205 1024h819v-819h-205v469l-674 -674l-145 145l674 674h-469v205zM1374 1229h469v-205h-819v819h205v-469l674 674l145 -145z");

        /// <summary>
        /// The full screen normal
        /// </summary>
        private static readonly PathGeometry fullScreenNormal = PathGeometry.Parse("M2048 2048v-819h-205v469l-1493 -1493h469v-205h-819v819h205v-469l1493 1493h-469v205h819z");

        /// <summary>
        /// The restore screen active
        /// </summary>
        private static readonly PathGeometry restoreScreenActive = PathGeometry.Parse("M2048 410h-410v-410h-1638v1638h410v410h1638v-1638zM1434 1434h-1229v-1229h1229v1229zM1843 1843h-1229v-205h1024v-1024h205v1229z");

        /// <summary>
        /// The restore screen normal
        /// </summary>
        private static readonly PathGeometry restoreScreenNormal = PathGeometry.Parse("M2048 2048v-2048h-2048v2048h2048zM1843 1843h-1638v-1638h1638v1638z");

        /// <summary>
        /// The disposables
        /// </summary>
        private CompositeDisposable? disposables;

        /// <summary>
        /// The full screen button path
        /// </summary>
        private Path? fullScreenButtonPath;

        /// <summary>
        /// The restore button path
        /// </summary>
        private Path? restoreButtonPath;

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
            if (disposables == null)
            {
                HostWindow = hostWindow;

                disposables = new CompositeDisposable
                {
                    HostWindow.GetObservable(Window.WindowStateProperty)
                        .Subscribe(x =>
                        {
                            PseudoClasses.Set(":minimized", x == WindowState.Minimized);
                            PseudoClasses.Set(":normal", x == WindowState.Normal);
                            PseudoClasses.Set(":maximized", x == WindowState.Maximized);
                            PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);

                            // Why here? Because I'm tired of dealing with a bug in pseudo classes.
                            if (fullScreenButtonPath != null && restoreButtonPath != null)
                            {
                                // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                                switch (x)
                                {
                                    case WindowState.FullScreen:
                                        fullScreenButtonPath.Data = fullScreenActive;
                                        fullScreenButtonPath.IsVisible = true;
                                        restoreButtonPath.Data = restoreScreenNormal;
                                        break;
                                    case WindowState.Maximized:
                                        fullScreenButtonPath.Data = fullScreenNormal;
                                        fullScreenButtonPath.IsVisible = false;
                                        restoreButtonPath.Data = restoreScreenActive;
                                        break;
                                    default:
                                        fullScreenButtonPath.Data = fullScreenNormal;
                                        fullScreenButtonPath.IsVisible = true;
                                        restoreButtonPath.Data = restoreScreenNormal;
                                        break;
                                }
                            }
                        })
                };
            }
        }

        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public virtual void Detach()
        {
            if (disposables != null)
            {
                disposables.Dispose();
                disposables = null;

                HostWindow = null;
            }
        }

        /// <summary>
        /// Handles the <see cref="E:ApplyTemplate" /> event.
        /// </summary>
        /// <param name="e">The <see cref="TemplateAppliedEventArgs" /> instance containing the event data.</param>
        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            var closeButton = e.NameScope.Get<Button>("PART_CloseButton");
            var restoreButton = e.NameScope.Get<Button>("PART_RestoreButton");
            var minimiseButton = e.NameScope.Get<Button>("PART_MinimiseButton");
            var fullScreenButton = e.NameScope.Get<Button>("PART_FullScreenButton");
            fullScreenButtonPath = e.NameScope.Get<Path>("FullScreenButtonPath");
            restoreButtonPath = e.NameScope.Get<Path>("RestoreButtonPath");

            closeButton.Click += (_, _) => OnClose();
            restoreButton.Click += (_, _) => OnRestore();
            minimiseButton.Click += (_, _) => OnMinimize();
            fullScreenButton.Click += (_, _) => OnToggleFullScreen();
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
