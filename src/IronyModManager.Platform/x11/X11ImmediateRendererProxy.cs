// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11ImmediateRendererProxy.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ************************************************************************

using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Rendering;
using Avalonia.Threading;
using Avalonia.VisualTree;
using IronyModManager.Shared;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class X11ImmediateRendererProxy.
    /// Implements the <see cref="Avalonia.Rendering.IRenderer" />
    /// Implements the <see cref="Avalonia.Rendering.IRenderLoopTask" />
    /// </summary>
    /// <seealso cref="Avalonia.Rendering.IRenderer" />
    /// <seealso cref="Avalonia.Rendering.IRenderLoopTask" />
    [ExcludeFromCoverage("External component.")]
    public class X11ImmediateRendererProxy : IRenderer, IRenderLoopTask
    {
        #region Fields

        /// <summary>
        /// The lock
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The loop
        /// </summary>
        private readonly IRenderLoop _loop;

        /// <summary>
        /// The renderer
        /// </summary>
        private readonly ImmediateRenderer _renderer;

        /// <summary>
        /// The invalidated
        /// </summary>
        private bool _invalidated;

        /// <summary>
        /// The running
        /// </summary>
        private bool _running;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11ImmediateRendererProxy"/> class.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="loop">The loop.</param>
        public X11ImmediateRendererProxy(IVisual root, IRenderLoop loop)
        {
            _loop = loop;
            _renderer = new ImmediateRenderer(root);
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when [scene invalidated].
        /// </summary>
        public event EventHandler<SceneInvalidatedEventArgs> SceneInvalidated
        {
            add => _renderer.SceneInvalidated += value;
            remove => _renderer.SceneInvalidated -= value;
        }

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [draw dirty rects].
        /// </summary>
        /// <value><c>true</c> if [draw dirty rects]; otherwise, <c>false</c>.</value>
        public bool DrawDirtyRects
        {
            get => _renderer.DrawDirtyRects;
            set => _renderer.DrawDirtyRects = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [draw FPS].
        /// </summary>
        /// <value><c>true</c> if [draw FPS]; otherwise, <c>false</c>.</value>
        public bool DrawFps
        {
            get => _renderer.DrawFps;
            set => _renderer.DrawFps = value;
        }

        /// <summary>
        /// Gets a value indicating whether [needs update].
        /// </summary>
        /// <value><c>true</c> if [needs update]; otherwise, <c>false</c>.</value>
        public bool NeedsUpdate => false;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds the dirty.
        /// </summary>
        /// <param name="visual">The visual.</param>
        public void AddDirty(IVisual visual)
        {
            lock (_lock)
                _invalidated = true;
            _renderer.AddDirty(visual);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _running = false;
            _renderer.Dispose();
        }

        /// <summary>
        /// Hits the test.
        /// </summary>
        /// <param name="p">The p.</param>
        /// <param name="root">The root.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>IEnumerable&lt;IVisual&gt;.</returns>
        public IEnumerable<IVisual> HitTest(Point p, IVisual root, Func<IVisual, bool> filter)
        {
            return _renderer.HitTest(p, root, filter);
        }

        /// <summary>
        /// Paints the specified rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        public void Paint(Rect rect)
        {
            _invalidated = false;
            _renderer.Paint(rect);
        }

        /// <summary>
        /// Recalculates the children.
        /// </summary>
        /// <param name="visual">The visual.</param>
        public void RecalculateChildren(IVisual visual)
        {
            _renderer.RecalculateChildren(visual);
        }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        public void Render()
        {
            if (_invalidated)
            {
                lock (_lock)
                    _invalidated = false;
                Dispatcher.UIThread.Post(() =>
                {
                    if (_running)
                        Paint(new Rect(0, 0, 100000, 100000));
                });
            }
        }

        /// <summary>
        /// Resizeds the specified size.
        /// </summary>
        /// <param name="size">The size.</param>
        public void Resized(Size size)
        {
            _renderer.Resized(size);
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            _running = true;
            _loop.Add(this);
            _renderer.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            _running = false;
            _loop.Remove(this);
            _renderer.Stop();
        }

        /// <summary>
        /// Updates the specified time.
        /// </summary>
        /// <param name="time">The time.</param>
        public void Update(TimeSpan time)
        {
        }

        #endregion Methods
    }
}
