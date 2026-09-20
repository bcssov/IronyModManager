// ***********************************************************************
// Assembly         : IronyModManager.Tests
// Author           : Mario
// Created          : 09-19-2026
//
// Last Modified By : Mario
// Last Modified On : 09-19-2026
// ***********************************************************************
// <copyright file="WinUiCompositionWatchdogTests.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Win32;
using IronyModManager.Platform.Windows;
using Xunit;

namespace IronyModManager.Tests.Platform
{
    [CollectionDefinition(Name, DisableParallelization = true)]
    public sealed class WinUiCompositionWatchdogCollection
    {
        public const string Name = "WinUI composition watchdog";
    }

    [Collection(WinUiCompositionWatchdogCollection.Name)]
    public class WinUiCompositionWatchdogTests
    {
        [Fact]
        public void TargetValidationMatchesPinnedAvaloniaBinary()
        {
            Assert.True(
                WinUiCompositionWatchdogCompatibility.TryValidateTargets(out var failure),
                failure);
        }

        [Fact]
        public void HarmonyPatchInstallsAndUninstallsAtomically()
        {
            try
            {
                Assert.True(
                    WinUiCompositionWatchdogCompatibility.TryInstallValidatedForTesting(out var failure),
                    failure);
                Assert.True(WinUiCompositionWatchdogCompatibility.IsInstalled);
            }
            finally
            {
                WinUiCompositionWatchdogCompatibility.UninstallForTesting();
            }

            Assert.False(WinUiCompositionWatchdogCompatibility.IsInstalled);
        }

        [Fact]
        public void WglAndDisabledCompositionLeaveWatchdogDormant()
        {
            Assert.False(WinUiCompositionWatchdogCompatibility.IsApplicable(null));
            Assert.False(WinUiCompositionWatchdogCompatibility.IsApplicable(
                new Win32PlatformOptions { UseWgl = true, UseWindowsUIComposition = true }));
            Assert.False(WinUiCompositionWatchdogCompatibility.IsApplicable(
                new Win32PlatformOptions { UseWgl = false, UseWindowsUIComposition = false }));
            Assert.True(WinUiCompositionWatchdogCompatibility.IsApplicable(
                new Win32PlatformOptions { UseWgl = false, UseWindowsUIComposition = true }));
        }

        [Fact]
        public void NormalInitialCompletionRemainsOnVanillaPath()
        {
            var fixture = new StateFixture();
            var initial = fixture.Action(1);

            fixture.State.ArmInitialCommit();
            Assert.Equal(CompletionHandling.AllowOriginal, fixture.State.HandleCompletion(initial));
            fixture.State.RearmAfterVanillaInvoke();

            Assert.Equal(0, fixture.Interop.RequestCount);
            Assert.Equal(0, fixture.Interop.TickCount);
            Assert.Equal(0, fixture.Interop.DisposeCount(1));
            Assert.Equal(2, fixture.State.Generation);
            Assert.Equal(0, fixture.State.RecoveryCount);
        }

        [Fact]
        public void TimeoutReplacesCommitAndLateCompletionCannotForkChain()
        {
            var fixture = new StateFixture();
            var initial = fixture.Action(1);
            var replacement = fixture.QueueAction(2);
            var afterReplacement = fixture.QueueAction(3);
            fixture.State.ArmInitialCommit();

            fixture.Interop.Elapsed = WinUiCompositionPatchState.CommitTimeout + TimeSpan.FromMilliseconds(1);
            fixture.State.WatchdogTick();

            Assert.Equal(1, fixture.Interop.RequestCount);
            Assert.Equal(1, fixture.State.RecoveryCount);
            Assert.Equal(0, fixture.Interop.GetResultsCount);
            Assert.Equal(CompletionHandling.SuppressOriginal, fixture.State.HandleCompletion(initial));
            Assert.Equal(1, fixture.Interop.RequestCount);

            Assert.Equal(CompletionHandling.SuppressOriginal, fixture.State.HandleCompletion(replacement));
            Assert.Equal(2, fixture.Interop.RequestCount);
            Assert.Same(afterReplacement, fixture.Interop.LastCompletionAction);
            Assert.Equal(0, fixture.Interop.DisposeCount(1));
            Assert.Equal(1, fixture.Interop.DisposeCount(2));
        }

        [Fact]
        public void RepeatedTimeoutsKeepOneCurrentGeneration()
        {
            var fixture = new StateFixture();
            fixture.State.ArmInitialCommit();
            fixture.QueueAction(2);
            fixture.QueueAction(3);

            fixture.Interop.Elapsed = TimeSpan.FromSeconds(2);
            fixture.State.WatchdogTick();
            fixture.Interop.Elapsed = TimeSpan.FromSeconds(4);
            fixture.State.WatchdogTick();

            Assert.Equal(2, fixture.State.RecoveryCount);
            Assert.Equal(2, fixture.Interop.RequestCount);
            Assert.Equal(3, fixture.State.Generation);
            Assert.Equal(0, fixture.Interop.DisposeCount(1));
            Assert.Equal(1, fixture.Interop.DisposeCount(2));
            Assert.Equal(0, fixture.Interop.DisposeCount(3));
        }

        [Fact]
        public void RepeatedVanillaCompletionsOnlyRearmTheDeadline()
        {
            var fixture = new StateFixture();
            fixture.State.ArmInitialCommit();

            for (var identity = 1; identity <= 3; identity++)
            {
                Assert.Equal(CompletionHandling.AllowOriginal, fixture.State.HandleCompletion(fixture.Action(identity)));
                fixture.State.RearmAfterVanillaInvoke();
            }

            Assert.Equal(4, fixture.State.Generation);
            Assert.Equal(0, fixture.Interop.RequestCount);
            Assert.Equal(0, fixture.Interop.TickCount);
        }

        [Fact]
        public void SeparateHandlersHaveIndependentRecoveryState()
        {
            var first = new StateFixture();
            var second = new StateFixture();
            first.State.ArmInitialCommit();
            second.State.ArmInitialCommit();
            first.QueueAction(11);
            first.Interop.Elapsed = TimeSpan.FromSeconds(2);

            first.State.WatchdogTick();

            Assert.Equal(1, first.Interop.RequestCount);
            Assert.Equal(1, first.State.RecoveryCount);
            Assert.Equal(0, second.Interop.RequestCount);
            Assert.Equal(0, second.State.RecoveryCount);
        }

        [Fact]
        public void FailedReplacementRegistrationDisablesFurtherRecovery()
        {
            var fixture = new StateFixture();
            fixture.State.ArmInitialCommit();
            fixture.QueueAction(2);
            fixture.Interop.ThrowOnSetCompleted = true;
            fixture.Interop.Elapsed = TimeSpan.FromSeconds(2);

            fixture.State.WatchdogTick();
            fixture.Interop.Elapsed = TimeSpan.FromSeconds(4);
            fixture.State.WatchdogTick();

            Assert.Equal(1, fixture.Interop.RequestCount);
            Assert.Equal(1, fixture.Interop.DisposeCount(2));
            Assert.Single(fixture.Log.Errors);
        }

        [Fact]
        public void TimedOutReplacementResultFailureStillStartsNextGeneration()
        {
            var fixture = new StateFixture();
            fixture.State.ArmInitialCommit();
            fixture.QueueAction(2);
            fixture.QueueAction(3);
            fixture.Interop.Elapsed = TimeSpan.FromSeconds(2);
            fixture.State.WatchdogTick();
            fixture.Interop.GetResultsException = new InvalidOperationException("device lost");
            fixture.Interop.Elapsed = TimeSpan.FromSeconds(4);

            fixture.State.WatchdogTick();

            Assert.Equal(2, fixture.Interop.RequestCount);
            Assert.Equal(1, fixture.Interop.GetResultsCount);
            Assert.Equal(1, fixture.Interop.DisposeCount(2));
            Assert.Single(fixture.Log.Errors);
        }

        [Fact]
        public async Task CompletionTimeoutRaceAdvancesOnlyOnce()
        {
            var fixture = new StateFixture();
            var initial = fixture.Action(1);
            fixture.State.ArmInitialCommit();
            fixture.QueueAction(2);
            fixture.Interop.Elapsed = TimeSpan.FromSeconds(2);

            await Task.WhenAll(
                Task.Run(() =>
                {
                    if (fixture.State.HandleCompletion(initial) == CompletionHandling.AllowOriginal)
                    {
                        fixture.State.RearmAfterVanillaInvoke();
                    }
                }),
                Task.Run(() => fixture.State.WatchdogTick()));

            Assert.InRange(fixture.Interop.RequestCount, 0, 1);
            Assert.InRange(fixture.Interop.TickCount, 0, 1);
            Assert.Equal(2, fixture.State.Generation);
            Assert.Equal(0, fixture.Interop.DisposeCount(1));
        }

        [Fact]
        public void ShutdownDisposesCurrentActionAndPreventsRecovery()
        {
            var fixture = new StateFixture();
            fixture.State.ArmInitialCommit();

            fixture.State.CompleteStopOnOwnerThread();
            fixture.Interop.Elapsed = TimeSpan.FromSeconds(2);
            fixture.State.WatchdogTick();

            Assert.Equal(0, fixture.Interop.DisposeCount(1));
            Assert.Equal(0, fixture.Interop.RequestCount);
            Assert.Equal(CompletionHandling.SuppressOriginal, fixture.State.HandleCompletion(fixture.Action(1)));
        }

        private sealed class StateFixture
        {
            public StateFixture()
            {
                Interop = new FakeInterop();
                Log = new FakeLog();
                State = new WinUiCompositionPatchState(new object(), new object(), Interop, Log);
            }

            public FakeInterop Interop { get; }

            public FakeLog Log { get; }

            public WinUiCompositionPatchState State { get; }

            public FakeAction Action(long identity) => new FakeAction(identity);

            public FakeAction QueueAction(long identity)
            {
                var action = Action(identity);
                Interop.PendingActions.Enqueue(action);
                return action;
            }
        }

        private sealed class FakeAction
        {
            public FakeAction(long identity)
            {
                Identity = new IntPtr(identity);
            }

            public IntPtr Identity { get; }
        }

        private sealed class FakeInterop : IWinUiCompositionInterop
        {
            private readonly ConcurrentDictionary<IntPtr, int> disposeCounts = new ConcurrentDictionary<IntPtr, int>();

            public TimeSpan Elapsed { get; set; }

            public int GetResultsCount { get; private set; }

            public Exception GetResultsException { get; set; }

            public object LastCompletionAction { get; private set; }

            public ConcurrentQueue<object> PendingActions { get; } = new ConcurrentQueue<object>();

            public int RequestCount { get; private set; }

            public int TickCount { get; private set; }

            public bool ThrowOnSetCompleted { get; set; }

            public void DisposeAction(object action) => disposeCounts.AddOrUpdate(GetIdentity(action), 1, (_, count) => count + 1);

            public int DisposeCount(long identity) => disposeCounts.TryGetValue(new IntPtr(identity), out var count) ? count : 0;

            public TimeSpan GetElapsed(object handler) => Elapsed;

            public IntPtr GetIdentity(object action) => ((FakeAction)action).Identity;

            public object GetParent(object handler) => throw new NotSupportedException();

            public object GetPumpLock(object connection) => this;

            public void GetResults(object action)
            {
                GetResultsCount++;
                if (GetResultsException != null)
                {
                    throw GetResultsException;
                }
            }

            public void RaiseTick(object connection, TimeSpan elapsed) => TickCount++;

            public object RequestCommit(object connection)
            {
                RequestCount++;
                Assert.True(PendingActions.TryDequeue(out var action));
                return action;
            }

            public void SetCompleted(object action, object handler)
            {
                if (ThrowOnSetCompleted)
                {
                    throw new InvalidOperationException("registration failed");
                }

                LastCompletionAction = action;
            }
        }

        private sealed class FakeLog : IWinUiCompositionWatchdogLog
        {
            public List<Exception> Errors { get; } = new List<Exception>();

            public List<string> Warnings { get; } = new List<string>();

            public void Error(string message, Exception exception) => Errors.Add(exception);

            public void Warning(string message) => Warnings.Add(message);
        }
    }
}
