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
// ***********************************************************************l
#pragma warning disable 414

namespace IronyModManager.Platform.x11.Glx
{
    /// <summary>
    /// Class GlxConsts.
    /// </summary>
    internal class GlxConsts
    {
        #region Fields

        /// <summary>
        /// The GLX accum alpha size
        /// </summary>
        public const int GLX_ACCUM_ALPHA_SIZE = 17;

        /// <summary>
        /// The GLX accum blue size
        /// </summary>
        public const int GLX_ACCUM_BLUE_SIZE = 16;

        /// <summary>
        /// The GLX accum buffer bit
        /// </summary>
        public const int GLX_ACCUM_BUFFER_BIT = 0x00000080;

        /// <summary>
        /// The GLX accum green size
        /// </summary>
        public const int GLX_ACCUM_GREEN_SIZE = 15;

        /// <summary>
        /// The GLX accum red size
        /// </summary>
        public const int GLX_ACCUM_RED_SIZE = 14;

        /// <summary>
        /// The GLX alpha size
        /// </summary>
        public const int GLX_ALPHA_SIZE = 11;

        /// <summary>
        /// The GLX aux buffers
        /// </summary>
        public const int GLX_AUX_BUFFERS = 7;

        /// <summary>
        /// The GLX aux buffers bit
        /// </summary>
        public const int GLX_AUX_BUFFERS_BIT = 0x00000010;

        /// <summary>
        /// The GLX back left buffer bit
        /// </summary>
        public const int GLX_BACK_LEFT_BUFFER_BIT = 0x00000004;

        /// <summary>
        /// The GLX back right buffer bit
        /// </summary>
        public const int GLX_BACK_RIGHT_BUFFER_BIT = 0x00000008;

        /// <summary>
        /// The GLX bad attribute
        /// </summary>
        public const int GLX_BAD_ATTRIBUTE = 2;

        /// <summary>
        /// The GLX bad context
        /// </summary>
        public const int GLX_BAD_CONTEXT = 5;

        /// <summary>
        /// The GLX bad enum
        /// </summary>
        public const int GLX_BAD_ENUM = 7;

        /// <summary>
        /// The GLX bad screen
        /// </summary>
        public const int GLX_BAD_SCREEN = 1;

        /// <summary>
        /// The GLX bad value
        /// </summary>
        public const int GLX_BAD_VALUE = 6;

        /// <summary>
        /// The GLX bad visual
        /// </summary>
        public const int GLX_BAD_VISUAL = 4;

        /// <summary>
        /// The GLX blue size
        /// </summary>
        public const int GLX_BLUE_SIZE = 10;

        /// <summary>
        /// The GLX buffer size
        /// </summary>
        public const int GLX_BUFFER_SIZE = 2;

        /// <summary>
        /// The GLX buffer swap complete
        /// </summary>
        public const int GLX_BufferSwapComplete = 1;

        /// <summary>
        /// The GLX color index bit
        /// </summary>
        public const int GLX_COLOR_INDEX_BIT = 0x00000002;

        /// <summary>
        /// The GLX color index type
        /// </summary>
        public const int GLX_COLOR_INDEX_TYPE = 0x8015;

        /// <summary>
        /// The GLX configuration caveat
        /// </summary>
        public const int GLX_CONFIG_CAVEAT = 0x20;

        /// <summary>
        /// The GLX context compatibility profile bit arb
        /// </summary>
        public const int GLX_CONTEXT_COMPATIBILITY_PROFILE_BIT_ARB = 0x00000002;

        /// <summary>
        /// The GLX context core profile bit arb
        /// </summary>
        public const int GLX_CONTEXT_CORE_PROFILE_BIT_ARB = 0x00000001;

        /// <summary>
        /// The GLX context debug bit arb
        /// </summary>
        public const int GLX_CONTEXT_DEBUG_BIT_ARB = 0x00000001;

        /// <summary>
        /// The GLX context flags arb
        /// </summary>
        public const int GLX_CONTEXT_FLAGS_ARB = 0x2094;

        /// <summary>
        /// The GLX context forward compatible bit arb
        /// </summary>
        public const int GLX_CONTEXT_FORWARD_COMPATIBLE_BIT_ARB = 0x00000002;

        /// <summary>
        /// The GLX context major version arb
        /// </summary>
        public const int GLX_CONTEXT_MAJOR_VERSION_ARB = 0x2091;

        /// <summary>
        /// The GLX context minor version arb
        /// </summary>
        public const int GLX_CONTEXT_MINOR_VERSION_ARB = 0x2092;

        /// <summary>
        /// The GLX context profile mask arb
        /// </summary>
        public const int GLX_CONTEXT_PROFILE_MASK_ARB = 0x9126;

        /// <summary>
        /// The GLX damaged
        /// </summary>
        public const int GLX_DAMAGED = 0x8020;

        /// <summary>
        /// The GLX depth buffer bit
        /// </summary>
        public const int GLX_DEPTH_BUFFER_BIT = 0x00000020;

        /// <summary>
        /// The GLX depth size
        /// </summary>
        public const int GLX_DEPTH_SIZE = 12;

        /// <summary>
        /// The GLX direct color
        /// </summary>
        public const int GLX_DIRECT_COLOR = 0x8003;

        /// <summary>
        /// The GLX dont care
        /// </summary>
        public const int GLX_DONT_CARE = unchecked((int)0xFFFFFFFF);

        /// <summary>
        /// The GLX doublebuffer
        /// </summary>
        public const int GLX_DOUBLEBUFFER = 5;

        /// <summary>
        /// The GLX drawable type
        /// </summary>
        public const int GLX_DRAWABLE_TYPE = 0x8010;

        /// <summary>
        /// The GLX event mask
        /// </summary>
        public const int GLX_EVENT_MASK = 0x801F;

        /// <summary>
        /// The GLX extensions
        /// </summary>
        public const int GLX_EXTENSIONS = 3;

        /// <summary>
        /// The GLX fbconfig identifier
        /// </summary>
        public const int GLX_FBCONFIG_ID = 0x8013;

        /// <summary>
        /// The GLX front left buffer bit
        /// </summary>
        public const int GLX_FRONT_LEFT_BUFFER_BIT = 0x00000001;

        /// <summary>
        /// The GLX front right buffer bit
        /// </summary>
        public const int GLX_FRONT_RIGHT_BUFFER_BIT = 0x00000002;

        /// <summary>
        /// The GLX gray scale
        /// </summary>
        public const int GLX_GRAY_SCALE = 0x8006;

        /// <summary>
        /// The GLX green size
        /// </summary>
        public const int GLX_GREEN_SIZE = 9;

        /// <summary>
        /// The GLX height
        /// </summary>
        public const int GLX_HEIGHT = 0x801E;

        /// <summary>
        /// The GLX largest pbuffer
        /// </summary>
        public const int GLX_LARGEST_PBUFFER = 0x801C;

        /// <summary>
        /// The GLX level
        /// </summary>
        public const int GLX_LEVEL = 3;

        /// <summary>
        /// The GLX maximum pbuffer height
        /// </summary>
        public const int GLX_MAX_PBUFFER_HEIGHT = 0x8017;

        /// <summary>
        /// The GLX maximum pbuffer pixels
        /// </summary>
        public const int GLX_MAX_PBUFFER_PIXELS = 0x8018;

        /// <summary>
        /// The GLX maximum pbuffer width
        /// </summary>
        public const int GLX_MAX_PBUFFER_WIDTH = 0x8016;

        /// <summary>
        /// The GLX no extension
        /// </summary>
        public const int GLX_NO_EXTENSION = 3;

        /// <summary>
        /// The GLX non conformant configuration
        /// </summary>
        public const int GLX_NON_CONFORMANT_CONFIG = 0x800D;

        /// <summary>
        /// The GLX none
        /// </summary>
        public const int GLX_NONE = 0x8000;

        /// <summary>
        /// The GLX pbuffer
        /// </summary>
        public const int GLX_PBUFFER = 0x8023;

        /// <summary>
        /// The GLX pbuffer bit
        /// </summary>
        public const int GLX_PBUFFER_BIT = 0x00000004;

        /// <summary>
        /// The GLX pbuffer clobber mask
        /// </summary>
        public const int GLX_PBUFFER_CLOBBER_MASK = 0x08000000;

        /// <summary>
        /// The GLX pbuffer height
        /// </summary>
        public const int GLX_PBUFFER_HEIGHT = 0x8040;

        /// <summary>
        /// The GLX pbuffer width
        /// </summary>
        public const int GLX_PBUFFER_WIDTH = 0x8041;

        /// <summary>
        /// The GLX pbuffer clobber
        /// </summary>
        public const int GLX_PbufferClobber = 0;

        /// <summary>
        /// The GLX pixmap bit
        /// </summary>
        public const int GLX_PIXMAP_BIT = 0x00000002;

        /// <summary>
        /// The GLX preserved contents
        /// </summary>
        public const int GLX_PRESERVED_CONTENTS = 0x801B;

        /// <summary>
        /// The GLX pseudo color
        /// </summary>
        public const int GLX_PSEUDO_COLOR = 0x8004;

        /// <summary>
        /// The GLX red size
        /// </summary>
        public const int GLX_RED_SIZE = 8;

        /// <summary>
        /// The GLX render type
        /// </summary>
        public const int GLX_RENDER_TYPE = 0x8011;

        /// <summary>
        /// The GLX rgba
        /// </summary>
        public const int GLX_RGBA = 4;

        /// <summary>
        /// The GLX rgba bit
        /// </summary>
        public const int GLX_RGBA_BIT = 0x00000001;

        /// <summary>
        /// The GLX rgba type
        /// </summary>
        public const int GLX_RGBA_TYPE = 0x8014;

        /// <summary>
        /// The GLX sample buffers
        /// </summary>
        public const int GLX_SAMPLE_BUFFERS = 0x186a0 /*100000*/;

        /// <summary>
        /// The GLX samples
        /// </summary>
        public const int GLX_SAMPLES = 0x186a1 /*100001*/;

        /// <summary>
        /// The GLX saved
        /// </summary>
        public const int GLX_SAVED = 0x8021;

        /// <summary>
        /// The GLX screen
        /// </summary>
        public const int GLX_SCREEN = 0x800C;

        /// <summary>
        /// The GLX slow configuration
        /// </summary>
        public const int GLX_SLOW_CONFIG = 0x8001;

        /// <summary>
        /// The GLX static color
        /// </summary>
        public const int GLX_STATIC_COLOR = 0x8005;

        /// <summary>
        /// The GLX static gray
        /// </summary>
        public const int GLX_STATIC_GRAY = 0x8007;

        /// <summary>
        /// The GLX stencil buffer bit
        /// </summary>
        public const int GLX_STENCIL_BUFFER_BIT = 0x00000040;

        /// <summary>
        /// The GLX stencil size
        /// </summary>
        public const int GLX_STENCIL_SIZE = 13;

        /// <summary>
        /// The GLX stereo
        /// </summary>
        public const int GLX_STEREO = 6;

        /// <summary>
        /// The GLX transparent alpha value
        /// </summary>
        public const int GLX_TRANSPARENT_ALPHA_VALUE = 0x28;

        /// <summary>
        /// The GLX transparent blue value
        /// </summary>
        public const int GLX_TRANSPARENT_BLUE_VALUE = 0x27;

        /// <summary>
        /// The GLX transparent green value
        /// </summary>
        public const int GLX_TRANSPARENT_GREEN_VALUE = 0x26;

        /// <summary>
        /// The GLX transparent index
        /// </summary>
        public const int GLX_TRANSPARENT_INDEX = 0x8009;

        /// <summary>
        /// The GLX transparent index value
        /// </summary>
        public const int GLX_TRANSPARENT_INDEX_VALUE = 0x24;

        /// <summary>
        /// The GLX transparent red value
        /// </summary>
        public const int GLX_TRANSPARENT_RED_VALUE = 0x25;

        /// <summary>
        /// The GLX transparent RGB
        /// </summary>
        public const int GLX_TRANSPARENT_RGB = 0x8008;

        /// <summary>
        /// The GLX transparent type
        /// </summary>
        public const int GLX_TRANSPARENT_TYPE = 0x23;

        /// <summary>
        /// The GLX true color
        /// </summary>
        public const int GLX_TRUE_COLOR = 0x8002;

        /// <summary>
        /// The GLX use gl
        /// </summary>
        public const int GLX_USE_GL = 1;

        /// <summary>
        /// The GLX vendor
        /// </summary>
        public const int GLX_VENDOR = 1;

        /// <summary>
        /// The GLX version
        /// </summary>
        public const int GLX_VERSION = 2;

        /// <summary>
        /// The GLX visual identifier
        /// </summary>
        public const int GLX_VISUAL_ID = 0x800B;

        /// <summary>
        /// The GLX width
        /// </summary>
        public const int GLX_WIDTH = 0x801D;

        /// <summary>
        /// The GLX window
        /// </summary>
        public const int GLX_WINDOW = 0x8022;

        /// <summary>
        /// The GLX window bit
        /// </summary>
        public const int GLX_WINDOW_BIT = 0x00000001;

        /// <summary>
        /// The GLX x renderable
        /// </summary>
        public const int GLX_X_RENDERABLE = 0x8012;

        /// <summary>
        /// The GLX x visual type
        /// </summary>
        public const int GLX_X_VISUAL_TYPE = 0x22;

        #endregion Fields
    }
}
