// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="Glx.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.OpenGL;
using Avalonia.Platform.Interop;
using IronyModManager.Shared;

// ReSharper disable UnassignedGetOnlyAutoProperty

namespace IronyModManager.Platform.x11.Glx
{
    /// <summary>
    /// Class GlxInterface.
    /// Implements the <see cref="Avalonia.OpenGL.GlInterfaceBase" />
    /// </summary>
    /// <seealso cref="Avalonia.OpenGL.GlInterfaceBase" />
    [ExcludeFromCoverage("External component.")]
    internal unsafe class GlxInterface : GlInterfaceBase
    {
        #region Fields

        /// <summary>
        /// The library gl
        /// </summary>
        private const string LibGL = "libGL.so.1";

        /// <summary>
        /// The GLX converted
        /// </summary>
        private static readonly Func<string, bool, IntPtr> GlxConverted = ConvertNative(GlxGetProcAddress);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GlxInterface" /> class.
        /// </summary>
        public GlxInterface() : base(SafeGetProcAddress)
        {
        }

        #endregion Constructors

        #region Delegates

        /// <summary>
        /// Delegate GlGetError
        /// </summary>
        /// <returns>System.Int32.</returns>
        public delegate int GlGetError();

        /// <summary>
        /// Delegate GlxChooseFBConfig
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="screen">The screen.</param>
        /// <param name="attrib_list">The attribute list.</param>
        /// <param name="nelements">The nelements.</param>
        /// <returns>IntPtr.</returns>
        public delegate IntPtr* GlxChooseFBConfig(IntPtr dpy, int screen, int[] attrib_list, out int nelements);

        /// <summary>
        /// Delegate GlxChooseVisual
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="screen">The screen.</param>
        /// <param name="attribList">The attribute list.</param>
        /// <returns>XVisualInfo.</returns>
        public delegate XVisualInfo* GlxChooseVisual(IntPtr dpy, int screen, int[] attribList);

        /// <summary>
        /// Delegate GlxCreateContext
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="vis">The vis.</param>
        /// <param name="shareList">The share list.</param>
        /// <param name="direct">if set to <c>true</c> [direct].</param>
        /// <returns>IntPtr.</returns>
        public delegate IntPtr GlxCreateContext(IntPtr dpy, XVisualInfo* vis, IntPtr shareList, bool direct);

        /// <summary>
        /// Delegate GlxCreateContextAttribsARB
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="fbconfig">The fbconfig.</param>
        /// <param name="shareList">The share list.</param>
        /// <param name="direct">if set to <c>true</c> [direct].</param>
        /// <param name="attribs">The attribs.</param>
        /// <returns>IntPtr.</returns>
        public delegate IntPtr GlxCreateContextAttribsARB(IntPtr dpy, IntPtr fbconfig, IntPtr shareList,
            bool direct, int[] attribs);

        /// <summary>
        /// Delegate GlxCreatePbuffer
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="fbc">The FBC.</param>
        /// <param name="attrib_list">The attribute list.</param>
        /// <returns>IntPtr.</returns>
        public delegate IntPtr GlxCreatePbuffer(IntPtr dpy, IntPtr fbc, int[] attrib_list);

        /// <summary>
        /// Delegate GlxDestroyContext
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="ctx">The CTX.</param>
        public delegate void GlxDestroyContext(IntPtr dpy, IntPtr ctx);

        /// <summary>
        /// Delegate GlxGetFBConfigAttrib
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="attribute">The attribute.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        public delegate int GlxGetFBConfigAttrib(IntPtr dpy, IntPtr config, int attribute, out int value);

        /// <summary>
        /// Delegate GlxGetVisualFromFBConfig
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="config">The configuration.</param>
        /// <returns>XVisualInfo.</returns>
        public delegate XVisualInfo* GlxGetVisualFromFBConfig(IntPtr dpy, IntPtr config);

        /// <summary>
        /// Delegate GlxMakeContextCurrent
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="draw">The draw.</param>
        /// <param name="read">The read.</param>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public delegate bool GlxMakeContextCurrent(IntPtr display, IntPtr draw, IntPtr read, IntPtr context);

        /// <summary>
        /// Delegate GlxSwapBuffers
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="drawable">The drawable.</param>
        public delegate void GlxSwapBuffers(IntPtr dpy, IntPtr drawable);

        /// <summary>
        /// Delegate GlxWaitGL
        /// </summary>
        public delegate void GlxWaitGL();

        /// <summary>
        /// Delegate GlxWaitX
        /// </summary>
        public delegate void GlxWaitX();

        #endregion Delegates

        #region Properties

        /// <summary>
        /// Gets the choose fb configuration.
        /// </summary>
        /// <value>The choose fb configuration.</value>
        [GlEntryPointAttribute("glXChooseFBConfig")]
        public GlxChooseFBConfig ChooseFBConfig { get; }

        /// <summary>
        /// Gets the choose visual.
        /// </summary>
        /// <value>The choose visual.</value>
        [GlEntryPointAttribute("glXChooseVisual")]
        public GlxChooseVisual ChooseVisual { get; }

        /// <summary>
        /// Gets the create context.
        /// </summary>
        /// <value>The create context.</value>
        [GlEntryPointAttribute("glXCreateContext")]
        public GlxCreateContext CreateContext { get; }

        /// <summary>
        /// Gets the create context attribs arb.
        /// </summary>
        /// <value>The create context attribs arb.</value>
        [GlEntryPointAttribute("glXCreateContextAttribsARB")]
        public GlxCreateContextAttribsARB CreateContextAttribsARB { get; }

        /// <summary>
        /// Gets the create pbuffer.
        /// </summary>
        /// <value>The create pbuffer.</value>
        [GlEntryPoint("glXCreatePbuffer")]
        public GlxCreatePbuffer CreatePbuffer { get; }

        /// <summary>
        /// Gets the destroy context.
        /// </summary>
        /// <value>The destroy context.</value>
        [GlEntryPointAttribute("glXDestroyContext")]
        public GlxDestroyContext DestroyContext { get; }

        /// <summary>
        /// Gets the get error.
        /// </summary>
        /// <value>The get error.</value>
        [GlEntryPoint("glGetError")]
        public GlGetError GetError { get; }

        /// <summary>
        /// Gets the get fb configuration attribute.
        /// </summary>
        /// <value>The get fb configuration attribute.</value>
        [GlEntryPointAttribute("glXGetFBConfigAttrib")]
        public GlxGetFBConfigAttrib GetFBConfigAttrib { get; }

        /// <summary>
        /// Gets the get visual from fb configuration.
        /// </summary>
        /// <value>The get visual from fb configuration.</value>
        [GlEntryPointAttribute("glXGetVisualFromFBConfig")]
        public GlxGetVisualFromFBConfig GetVisualFromFBConfig { get; }

        /// <summary>
        /// Gets the make context current.
        /// </summary>
        /// <value>The make context current.</value>
        [GlEntryPointAttribute("glXMakeContextCurrent")]
        public GlxMakeContextCurrent MakeContextCurrent { get; }

        /// <summary>
        /// Gets the swap buffers.
        /// </summary>
        /// <value>The swap buffers.</value>
        [GlEntryPointAttribute("glXSwapBuffers")]
        public GlxSwapBuffers SwapBuffers { get; }

        /// <summary>
        /// Gets the wait gl.
        /// </summary>
        /// <value>The wait gl.</value>
        [GlEntryPointAttribute("glXWaitGL")]
        public GlxWaitGL WaitGL { get; }

        /// <summary>
        /// Gets the wait x.
        /// </summary>
        /// <value>The wait x.</value>
        [GlEntryPointAttribute("glXWaitX")]
        public GlxWaitX WaitX { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// GLXs the get proc address.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(LibGL, EntryPoint = "glXGetProcAddress")]
        public static extern IntPtr GlxGetProcAddress(Utf8Buffer buffer);

        // Ignores egl functions.
        // On some Linux systems, glXGetProcAddress will return valid pointers for even EGL functions.
        // This makes Skia try to load some data from EGL,
        // which can then cause segmentation faults because they return garbage.
        /// <summary>
        /// Safes the get proc address.
        /// </summary>
        /// <param name="proc">The proc.</param>
        /// <param name="optional">if set to <c>true</c> [optional].</param>
        /// <returns>IntPtr.</returns>
        public static IntPtr SafeGetProcAddress(string proc, bool optional)
        {
            if (proc.StartsWith("egl", StringComparison.InvariantCulture))
            {
                return IntPtr.Zero;
            }

            return GlxConverted(proc, optional);
        }

        /// <summary>
        /// GLXs the choose fb configuration.
        /// </summary>
        /// <param name="dpy">The dpy.</param>
        /// <param name="screen">The screen.</param>
        /// <param name="attribs">The attribs.</param>
        /// <param name="nelements">The nelements.</param>
        /// <returns>IntPtr.</returns>
        public IntPtr* GlxChooseFbConfig(IntPtr dpy, int screen, IEnumerable<int> attribs, out int nelements)
        {
            var arr = attribs.Concat(new[] { 0 }).ToArray();
            return ChooseFBConfig(dpy, screen, arr, out nelements);
        }

        #endregion Methods
    }
}
