// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11PlatformThreading.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Platform;
using Avalonia.Threading;
using static IronyModManager.Platform.x11.XLib;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class X11PlatformThreading.
    /// Implements the <see cref="Avalonia.Platform.IPlatformThreadingInterface" />
    /// </summary>
    /// <seealso cref="Avalonia.Platform.IPlatformThreadingInterface" />
    internal unsafe class X11PlatformThreading : IPlatformThreadingInterface
    {
        #region Fields

        /// <summary>
        /// The epoll control add
        /// </summary>
        private const int EPOLL_CTL_ADD = 1;

        /// <summary>
        /// The epollin
        /// </summary>
        private const int EPOLLIN = 1;

        /// <summary>
        /// The o nonblock
        /// </summary>
        private const int O_NONBLOCK = 2048;

        /// <summary>
        /// The clock
        /// </summary>
        private readonly Stopwatch _clock = Stopwatch.StartNew();

        /// <summary>
        /// The display
        /// </summary>
        private readonly IntPtr _display;

        /// <summary>
        /// The epoll
        /// </summary>
        private readonly int _epoll;

        /// <summary>
        /// The event handlers
        /// </summary>
        private readonly Dictionary<IntPtr, Action<XEvent>> _eventHandlers;

        /// <summary>
        /// The lock
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The main thread
        /// </summary>
        private readonly Thread _mainThread;

        /// <summary>
        /// The platform
        /// </summary>
        private readonly AvaloniaX11Platform _platform;

        /// <summary>
        /// The sigread
        /// </summary>
        private readonly int _sigread;

        /// <summary>
        /// The sigwrite
        /// </summary>
        private readonly int _sigwrite;

        /// <summary>
        /// The timers
        /// </summary>
        private readonly List<X11Timer> _timers = new List<X11Timer>();

        /// <summary>
        /// The signaled
        /// </summary>
        private bool _signaled;

        /// <summary>
        /// The signaled priority
        /// </summary>
        private DispatcherPriority _signaledPriority;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11PlatformThreading" /> class.
        /// </summary>
        /// <param name="platform">The platform.</param>
        /// <exception cref="IronyModManager.Platform.x11.X11Exception">epoll_create1 failed
        /// or
        /// Unable to attach X11 connection handle to epoll
        /// or
        /// Unable to attach signal pipe to epoll</exception>
        public X11PlatformThreading(AvaloniaX11Platform platform)
        {
            _platform = platform;
            _display = platform.Display;
            _eventHandlers = platform.Windows;
            _mainThread = Thread.CurrentThread;
            var fd = XLib.XConnectionNumber(_display);
            var ev = new epoll_event()
            {
                events = EPOLLIN,
                data = { u32 = (int)EventCodes.X11 }
            };
            _epoll = epoll_create1(0);
            if (_epoll == -1)
                throw new X11Exception("epoll_create1 failed");

            if (epoll_ctl(_epoll, EPOLL_CTL_ADD, fd, ref ev) == -1)
                throw new X11Exception("Unable to attach X11 connection handle to epoll");

            var fds = stackalloc int[2];
            pipe2(fds, O_NONBLOCK);
            _sigread = fds[0];
            _sigwrite = fds[1];

            ev = new epoll_event
            {
                events = EPOLLIN,
                data = { u32 = (int)EventCodes.Signal }
            };
            if (epoll_ctl(_epoll, EPOLL_CTL_ADD, _sigread, ref ev) == -1)
                throw new X11Exception("Unable to attach signal pipe to epoll");
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Occurs when [signaled].
        /// </summary>
        public event Action<DispatcherPriority?> Signaled;

        #endregion Events

        #region Enums

        /// <summary>
        /// Enum EventCodes
        /// </summary>
        private enum EventCodes
        {
            /// <summary>
            /// The X11
            /// </summary>
            X11 = 1,

            /// <summary>
            /// The signal
            /// </summary>
            Signal = 2
        }

        #endregion Enums

        #region Properties

        /// <summary>
        /// Gets a value indicating whether [current thread is loop thread].
        /// </summary>
        /// <value><c>true</c> if [current thread is loop thread]; otherwise, <c>false</c>.</value>
        public bool CurrentThreadIsLoopThread => Thread.CurrentThread == _mainThread;

        #endregion Properties

        #region Methods

        /// <summary>
        /// Runs the loop.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        public void RunLoop(CancellationToken cancellationToken)
        {
            var readyTimers = new List<X11Timer>();
            while (!cancellationToken.IsCancellationRequested)
            {
                var now = _clock.Elapsed;
                TimeSpan? nextTick = null;
                readyTimers.Clear();
                lock (_timers)
                    foreach (var t in _timers)
                    {
                        if (nextTick == null || t.NextTick < nextTick.Value)
                            nextTick = t.NextTick;
                        if (t.NextTick < now)
                            readyTimers.Add(t);
                    }

                readyTimers.Sort(TimerComparer);

                foreach (var t in readyTimers)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    t.Tick();
                    if (!t.Disposed)
                    {
                        t.Reschedule();
                        if (nextTick == null || t.NextTick < nextTick.Value)
                            nextTick = t.NextTick;
                    }
                }

                if (cancellationToken.IsCancellationRequested)
                    return;
                //Flush whatever requests were made to XServer
                XFlush(_display);
                epoll_event ev;
                if (XPending(_display) == 0)
                    epoll_wait(_epoll, &ev, 1,
                        nextTick == null ? -1 : Math.Max(1, (int)(nextTick.Value - _clock.Elapsed).TotalMilliseconds));
                if (cancellationToken.IsCancellationRequested)
                    return;
                CheckSignaled();
                HandleX11(cancellationToken);
            }
        }

        /// <summary>
        /// Signals the specified priority.
        /// </summary>
        /// <param name="priority">The priority.</param>
        public void Signal(DispatcherPriority priority)
        {
            lock (_lock)
            {
                if (priority > _signaledPriority)
                    _signaledPriority = priority;

                if (_signaled)
                    return;
                _signaled = true;
                int buf = 0;
                write(_sigwrite, &buf, new IntPtr(1));
            }
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="tick">The tick.</param>
        /// <returns>IDisposable.</returns>
        /// <exception cref="InvalidOperationException">StartTimer can be only called from UI thread</exception>
        /// <exception cref="ArgumentException">Interval must be positive - interval</exception>
        public IDisposable StartTimer(DispatcherPriority priority, TimeSpan interval, Action tick)
        {
            if (_mainThread != Thread.CurrentThread)
                throw new InvalidOperationException("StartTimer can be only called from UI thread");
            if (interval <= TimeSpan.Zero)
                throw new ArgumentException("Interval must be positive", nameof(interval));

            // We assume that we are on the main thread and outside of epoll_wait, so there is no need for wakeup signal

            var timer = new X11Timer(this, priority, interval, tick);
            lock (_timers)
                _timers.Add(timer);
            return timer;
        }

        /// <summary>
        /// Epolls the create1.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <returns>System.Int32.</returns>
        [DllImport("libc")]
        private static extern int epoll_create1(int size);

        /// <summary>
        /// Epolls the control.
        /// </summary>
        /// <param name="epfd">The epfd.</param>
        /// <param name="op">The op.</param>
        /// <param name="fd">The fd.</param>
        /// <param name="__event">The event.</param>
        /// <returns>System.Int32.</returns>
        [DllImport("libc")]
        private static extern int epoll_ctl(int epfd, int op, int fd, ref epoll_event __event);

        /// <summary>
        /// Epolls the wait.
        /// </summary>
        /// <param name="epfd">The epfd.</param>
        /// <param name="events">The events.</param>
        /// <param name="maxevents">The maxevents.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns>System.Int32.</returns>
        [DllImport("libc")]
        private static extern int epoll_wait(int epfd, epoll_event* events, int maxevents, int timeout);

        /// <summary>
        /// Pipe2s the specified FDS.
        /// </summary>
        /// <param name="fds">The FDS.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>System.Int32.</returns>
        [DllImport("libc")]
        private static extern int pipe2(int* fds, int flags);

        /// <summary>
        /// Reads the specified fd.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="buf">The buf.</param>
        /// <param name="count">The count.</param>
        /// <returns>IntPtr.</returns>
        [DllImport("libc")]
        private static extern IntPtr read(int fd, void* buf, IntPtr count);

        /// <summary>
        /// Writes the specified fd.
        /// </summary>
        /// <param name="fd">The fd.</param>
        /// <param name="buf">The buf.</param>
        /// <param name="count">The count.</param>
        /// <returns>IntPtr.</returns>
        [DllImport("libc")]
        private static extern IntPtr write(int fd, void* buf, IntPtr count);

        /// <summary>
        /// Checks the signaled.
        /// </summary>
        private void CheckSignaled()
        {
            int buf = 0;
            while (read(_sigread, &buf, new IntPtr(4)).ToInt64() > 0)
            {
            }

            DispatcherPriority prio;
            lock (_lock)
            {
                if (!_signaled)
                    return;
                _signaled = false;
                prio = _signaledPriority;
                _signaledPriority = DispatcherPriority.MinValue;
            }

            Signaled?.Invoke(prio);
        }

        /// <summary>
        /// Handles the X11.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        private void HandleX11(CancellationToken cancellationToken)
        {
            while (XPending(_display) != 0)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;
                XNextEvent(_display, out var xev);
                if (xev.type == XEventName.GenericEvent)
                    XGetEventData(_display, &xev.GenericEventCookie);
                try
                {
                    if (xev.type == XEventName.GenericEvent)
                    {
                        if (_platform.XI2 != null && _platform.Info.XInputOpcode ==
                            xev.GenericEventCookie.extension)
                        {
                            _platform.XI2.OnEvent((XIEvent*)xev.GenericEventCookie.data);
                        }
                    }
                    else if (_eventHandlers.TryGetValue(xev.AnyEvent.window, out var handler))
                        handler(xev);
                }
                finally
                {
                    if (xev.type == XEventName.GenericEvent && xev.GenericEventCookie.data != null)
                        XFreeEventData(_display, &xev.GenericEventCookie);
                }
            }

            Dispatcher.UIThread.RunJobs();
        }

        /// <summary>
        /// Timers the comparer.
        /// </summary>
        /// <param name="t1">The t1.</param>
        /// <param name="t2">The t2.</param>
        /// <returns>System.Int32.</returns>
        private int TimerComparer(X11Timer t1, X11Timer t2)
        {
            return t2.Priority - t1.Priority;
        }

        #endregion Methods

        #region Structs

        /// <summary>
        /// Struct epoll_data
        /// </summary>
        [StructLayout(LayoutKind.Explicit)]
        private struct epoll_data
        {
            /// <summary>
            /// The PTR
            /// </summary>
            [FieldOffset(0)]
            public IntPtr ptr;

            /// <summary>
            /// The fd
            /// </summary>
            [FieldOffset(0)]
            public int fd;

            /// <summary>
            /// The u32
            /// </summary>
            [FieldOffset(0)]
            public uint u32;

            /// <summary>
            /// The u64
            /// </summary>
            [FieldOffset(0)]
            public ulong u64;
        }

        /// <summary>
        /// Struct epoll_event
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct epoll_event
        {
            /// <summary>
            /// The events
            /// </summary>
            public uint events;

            /// <summary>
            /// The data
            /// </summary>
            public epoll_data data;
        }

        #endregion Structs

        #region Classes

        /// <summary>
        /// Class X11Timer.
        /// Implements the <see cref="System.IDisposable" />
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        private class X11Timer : IDisposable
        {
            #region Fields

            /// <summary>
            /// The parent
            /// </summary>
            private readonly X11PlatformThreading _parent;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="X11Timer" /> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="prio">The prio.</param>
            /// <param name="interval">The interval.</param>
            /// <param name="tick">The tick.</param>
            public X11Timer(X11PlatformThreading parent, DispatcherPriority prio, TimeSpan interval, Action tick)
            {
                _parent = parent;
                Priority = prio;
                Tick = tick;
                Interval = interval;
                Reschedule();
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="X11Timer" /> is disposed.
            /// </summary>
            /// <value><c>true</c> if disposed; otherwise, <c>false</c>.</value>
            public bool Disposed { get; private set; }

            /// <summary>
            /// Gets the interval.
            /// </summary>
            /// <value>The interval.</value>
            public TimeSpan Interval { get; }

            /// <summary>
            /// Gets or sets the next tick.
            /// </summary>
            /// <value>The next tick.</value>
            public TimeSpan NextTick { get; private set; }

            /// <summary>
            /// Gets the priority.
            /// </summary>
            /// <value>The priority.</value>
            public DispatcherPriority Priority { get; }

            /// <summary>
            /// Gets the tick.
            /// </summary>
            /// <value>The tick.</value>
            public Action Tick { get; }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                Disposed = true;
                lock (_parent._lock)
                    _parent._timers.Remove(this);
            }

            /// <summary>
            /// Reschedules this instance.
            /// </summary>
            public void Reschedule()
            {
                NextTick = _parent._clock.Elapsed + Interval;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}
