// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 09-19-2026
//
// Last Modified By : Mario
// Last Modified On : 09-19-2026
// ***********************************************************************
// <copyright file="WinUiCompositionPatchState.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Threading;

namespace IronyModManager.Platform.Windows
{
    /// <summary>
    /// Owns the single WinUI commit chain associated with one Avalonia RunLoopHandler.
    /// </summary>
    internal sealed class WinUiCompositionPatchState
    {
        #region Constants

        internal static readonly TimeSpan CommitTimeout = TimeSpan.FromSeconds(1);

        #endregion Constants

        #region Fields

        private readonly object gate = new object();
        private readonly object connection;
        private readonly object handler;
        private readonly IWinUiCompositionInterop interop;
        private readonly IWinUiCompositionWatchdogLog log;
        private bool advancing;
        private bool awaitingInitialCompletion;
        private object currentAction;
        private IntPtr currentIdentity;
        private bool disabled;
        private TimeSpan dueAt;
        private long generation;
        private bool ownsChain;
        private int recoveryCount;
        private bool staleLogged;
        private bool stopping;
        private IntPtr timerWindow;
        private int vanillaInvokeDepth;

        #endregion Fields

        #region Constructors

        public WinUiCompositionPatchState(
            object handler,
            object connection,
            IWinUiCompositionInterop interop,
            IWinUiCompositionWatchdogLog log)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.interop = interop ?? throw new ArgumentNullException(nameof(interop));
            this.log = log ?? throw new ArgumentNullException(nameof(log));
        }

        #endregion Constructors

        #region Properties

        public uint OwnerThreadId { get; private set; }

        public IntPtr TimerWindow
        {
            get
            {
                lock (gate)
                {
                    return timerWindow;
                }
            }
        }

        public bool OwnsChain
        {
            get
            {
                lock (gate)
                {
                    return ownsChain;
                }
            }
        }

        internal long Generation
        {
            get
            {
                lock (gate)
                {
                    return generation;
                }
            }
        }

        internal int RecoveryCount
        {
            get
            {
                lock (gate)
                {
                    return recoveryCount;
                }
            }
        }

        #endregion Properties

        #region Methods

        public void AttachTimerWindow(IntPtr window, uint ownerThreadId)
        {
            lock (gate)
            {
                timerWindow = window;
                OwnerThreadId = ownerThreadId;
            }
        }

        public void ArmInitialCommit()
        {
            lock (gate)
            {
                if (stopping || disabled || ownsChain)
                {
                    return;
                }

                awaitingInitialCompletion = true;
                dueAt = interop.GetElapsed(handler) + CommitTimeout;
                generation++;
                ownsChain = true;
            }
        }

        public CompletionHandling HandleCompletion(object callbackAction)
        {
            bool allowOriginal;
            lock (gate)
            {
                if (!ownsChain)
                {
                    return CompletionHandling.AllowOriginal;
                }

                if (stopping || disabled)
                {
                    return CompletionHandling.SuppressOriginal;
                }

                if (awaitingInitialCompletion)
                {
                    awaitingInitialCompletion = false;
                    vanillaInvokeDepth++;
                    allowOriginal = true;
                }
                else if (vanillaInvokeDepth > 0)
                {
                    vanillaInvokeDepth++;
                    allowOriginal = true;
                }
                else
                {
                    allowOriginal = false;
                }
            }

            if (allowOriginal)
            {
                return CompletionHandling.AllowOriginal;
            }

            IntPtr callbackIdentity;
            try
            {
                callbackIdentity = interop.GetIdentity(callbackAction);
            }
            catch (Exception ex)
            {
                return HandleUnexpectedFailure("Unable to identify an Avalonia WinUI commit callback.", ex);
            }

            object completedAction;
            lock (gate)
            {
                if (stopping || disabled)
                {
                    return CompletionHandling.SuppressOriginal;
                }

                if (advancing || currentAction == null || callbackIdentity != currentIdentity)
                {
                    if (!staleLogged)
                    {
                        staleLogged = true;
                        log.Warning($"Ignored stale Avalonia WinUI commit callback 0x{callbackIdentity.ToInt64():X}; current generation is {generation}.");
                    }

                    return CompletionHandling.SuppressOriginal;
                }

                completedAction = currentAction;
                currentAction = null;
                currentIdentity = IntPtr.Zero;
                advancing = true;
            }

            Advance(completedAction, false);
            return CompletionHandling.SuppressOriginal;
        }

        public void RearmAfterVanillaInvoke()
        {
            lock (gate)
            {
                if (stopping || disabled || vanillaInvokeDepth == 0)
                {
                    return;
                }

                vanillaInvokeDepth--;
                if (vanillaInvokeDepth == 0)
                {
                    awaitingInitialCompletion = true;
                    dueAt = interop.GetElapsed(handler) + CommitTimeout;
                    generation++;
                }
            }
        }

        public void WatchdogTick()
        {
            object overdueAction;
            int timeoutNumber;
            lock (gate)
            {
                if (!ownsChain || stopping || disabled || advancing || interop.GetElapsed(handler) <= dueAt)
                {
                    return;
                }

                if (!awaitingInitialCompletion && currentAction == null)
                {
                    return;
                }

                overdueAction = currentAction;
                awaitingInitialCompletion = false;
                currentAction = null;
                currentIdentity = IntPtr.Zero;
                advancing = true;
                timeoutNumber = ++recoveryCount;
            }

            log.Warning($"Avalonia WinUI RequestCommitAsync timed out; recovery attempt {timeoutNumber} is starting.");
            Advance(overdueAction, true);
        }

        public void BeginStop()
        {
            IntPtr window;
            lock (gate)
            {
                if (stopping)
                {
                    return;
                }

                stopping = true;
                window = timerWindow;
            }

            if (window != IntPtr.Zero)
            {
                WinUiCompositionWatchdogWindow.RequestClose(window);
            }
        }

        public void CompleteStopOnOwnerThread()
        {
            object action;
            lock (gate)
            {
                stopping = true;
                action = currentAction;
                currentAction = null;
                currentIdentity = IntPtr.Zero;
                timerWindow = IntPtr.Zero;
            }

            SafeDispose(action);
        }

        public CompletionHandling HandleUnexpectedFailure(string message, Exception exception)
        {
            bool suppress;
            lock (gate)
            {
                disabled = true;
                suppress = ownsChain;
            }

            log.Error(message, exception);
            BeginStop();
            return suppress ? CompletionHandling.SuppressOriginal : CompletionHandling.AllowOriginal;
        }

        private void Advance(object previousAction, bool timedOut)
        {
            if (timedOut && previousAction != null)
            {
                try
                {
                    interop.GetResults(previousAction);
                }
                catch (Exception ex)
                {
                    log.Error("Avalonia WinUI RequestCommitAsync returned an error during watchdog recovery.", ex);
                }
            }

            SafeDispose(previousAction);

            lock (gate)
            {
                if (stopping || disabled)
                {
                    advancing = false;
                    return;
                }
            }

            var elapsed = interop.GetElapsed(handler);
            try
            {
                interop.RaiseTick(connection, elapsed);
            }
            catch (Exception ex)
            {
                log.Error("An Avalonia render tick subscriber failed while advancing the WinUI commit chain.", ex);
            }

            object nextAction = null;
            var scheduled = false;
            long scheduledGeneration = 0;
            IntPtr nextIdentity = IntPtr.Zero;
            try
            {
                lock (interop.GetPumpLock(connection))
                {
                    nextAction = interop.RequestCommit(connection);
                    nextIdentity = interop.GetIdentity(nextAction);
                    lock (gate)
                    {
                        if (stopping || disabled)
                        {
                            advancing = false;
                        }
                        else
                        {
                            currentAction = nextAction;
                            currentIdentity = nextIdentity;
                            dueAt = interop.GetElapsed(handler) + CommitTimeout;
                            generation++;
                            scheduledGeneration = generation;
                            staleLogged = false;
                            advancing = false;
                            scheduled = true;
                        }
                    }

                    if (scheduled)
                    {
                        interop.SetCompleted(nextAction, handler);
                    }
                }

                if (!scheduled)
                {
                    SafeDispose(nextAction);
                }
            }
            catch (Exception ex)
            {
                object actionToDispose = null;
                lock (gate)
                {
                    if (!scheduled ||
                        generation == scheduledGeneration && currentIdentity == nextIdentity)
                    {
                        disabled = true;
                        advancing = false;
                        actionToDispose = currentAction ?? nextAction;
                        currentAction = null;
                        currentIdentity = IntPtr.Zero;
                    }
                }

                SafeDispose(actionToDispose);
                log.Error("Unable to schedule the next Avalonia WinUI commit; the watchdog state is disabled.", ex);
                BeginStop();
            }
        }

        private void SafeDispose(object action)
        {
            if (action == null)
            {
                return;
            }

            try
            {
                interop.DisposeAction(action);
            }
            catch (Exception ex)
            {
                log.Error("Unable to release an Avalonia WinUI commit action.", ex);
            }
        }

        #endregion Methods
    }

    internal enum CompletionHandling
    {
        AllowOriginal,
        SuppressOriginal
    }

    internal interface IWinUiCompositionWatchdogLog
    {
        void Warning(string message);

        void Error(string message, Exception exception);
    }
}
