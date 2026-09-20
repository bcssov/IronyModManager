// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 09-19-2026
//
// Last Modified By : Mario
// Last Modified On : 09-19-2026
// ***********************************************************************
// <copyright file="WinUiCompositionWatchdogWindow.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace IronyModManager.Platform.Windows
{
    /// <summary>
    /// Hosts the watchdog timer on Avalonia's existing WinUI compositor STA message loop.
    /// </summary>
    internal static class WinUiCompositionWatchdogWindow
    {
        #region Constants

        private const uint WmClose = 0x0010;
        private const uint WmDestroy = 0x0002;
        private const uint WmTimer = 0x0113;
        private const uint TimerIntervalMilliseconds = 1000;
        private const string WindowClassName = "IronyModManager.WinUiCompositionWatchdog";

        #endregion Constants

        #region Fields

        private static readonly object ClassLock = new object();
        private static readonly ConcurrentDictionary<IntPtr, WinUiCompositionPatchState> States = new ConcurrentDictionary<IntPtr, WinUiCompositionPatchState>();
        private static readonly WindowProcedureCallback WindowProcedureDelegate = HandleWindowMessage;
        private static ushort windowClass;

        #endregion Fields

        #region Properties

        internal static int ActiveStateCount => States.Count;

        #endregion Properties

        #region Methods

        public static void Create(WinUiCompositionPatchState state)
        {
            EnsureWindowClass();
            var window = CreateWindowEx(
                0,
                WindowClassName,
                null,
                0,
                0,
                0,
                0,
                0,
                new IntPtr(-3),
                IntPtr.Zero,
                GetModuleHandle(null),
                IntPtr.Zero);
            if (window == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            if (!States.TryAdd(window, state))
            {
                DestroyWindow(window);
                throw new InvalidOperationException("Unable to register the WinUI watchdog window.");
            }

            state.AttachTimerWindow(window, GetCurrentThreadId());
            if (SetTimer(window, new UIntPtr(1), TimerIntervalMilliseconds, IntPtr.Zero) == UIntPtr.Zero)
            {
                States.TryRemove(window, out _);
                DestroyWindow(window);
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public static void RequestClose(IntPtr window)
        {
            if (window != IntPtr.Zero)
            {
                PostMessage(window, WmClose, IntPtr.Zero, IntPtr.Zero);
            }
        }

        public static void StopAll()
        {
            foreach (var state in States.Values)
            {
                state.BeginStop();
            }
        }

        private static void EnsureWindowClass()
        {
            if (windowClass != 0)
            {
                return;
            }

            lock (ClassLock)
            {
                if (windowClass != 0)
                {
                    return;
                }

                var windowClassInfo = new WindowClass
                {
                    Size = (uint)Marshal.SizeOf<WindowClass>(),
                    Instance = GetModuleHandle(null),
                    WindowProcedure = WindowProcedureDelegate,
                    ClassName = WindowClassName
                };
                windowClass = RegisterClassEx(ref windowClassInfo);
                if (windowClass == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
        }

        private static IntPtr HandleWindowMessage(IntPtr window, uint message, IntPtr wParam, IntPtr lParam)
        {
            if (message == WmTimer && States.TryGetValue(window, out var timerState))
            {
                try
                {
                    timerState.WatchdogTick();
                }
                catch (Exception ex)
                {
                    timerState.HandleUnexpectedFailure("Unexpected failure in the Avalonia WinUI watchdog timer.", ex);
                }

                return IntPtr.Zero;
            }

            if (message == WmClose)
            {
                KillTimer(window, new UIntPtr(1));
                if (States.TryRemove(window, out var closingState))
                {
                    closingState.CompleteStopOnOwnerThread();
                }

                DestroyWindow(window);
                return IntPtr.Zero;
            }

            if (message == WmDestroy)
            {
                States.TryRemove(window, out _);
            }

            return DefWindowProc(window, message, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern ushort RegisterClassEx(ref WindowClass windowClass);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr CreateWindowEx(
            uint extendedStyle,
            string className,
            string windowName,
            uint style,
            int x,
            int y,
            int width,
            int height,
            IntPtr parent,
            IntPtr menu,
            IntPtr instance,
            IntPtr parameter);

        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DestroyWindow(IntPtr window);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern UIntPtr SetTimer(IntPtr window, UIntPtr timerId, uint interval, IntPtr timerProcedure);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool KillTimer(IntPtr window, UIntPtr timerId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string moduleName);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        #endregion Methods

        #region Delegates

        [UnmanagedFunctionPointer(CallingConvention.Winapi)]
        private delegate IntPtr WindowProcedureCallback(IntPtr window, uint message, IntPtr wParam, IntPtr lParam);

        #endregion Delegates

        #region Structs

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WindowClass
        {
            public uint Size;
            public uint Style;
            public WindowProcedureCallback WindowProcedure;
            public int ClassExtra;
            public int WindowExtra;
            public IntPtr Instance;
            public IntPtr Icon;
            public IntPtr Cursor;
            public IntPtr Background;
            public string MenuName;
            public string ClassName;
            public IntPtr SmallIcon;
        }

        #endregion Structs
    }
}
