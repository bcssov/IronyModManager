// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 09-19-2026
//
// Last Modified By : Mario
// Last Modified On : 09-19-2026
// ***********************************************************************
// <copyright file="WinUiCompositionWatchdogCompatibility.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Logging;
using Avalonia.Win32;
using HarmonyLib;

namespace IronyModManager.Platform.Windows
{
    /// <summary>
    /// Backports the RequestCommitAsync watchdog from Avalonia PR #16393 to the pinned Avalonia 0.10.22 runtime.
    /// </summary>
    /// <remarks>
    /// Harmony is used because the pending commit and callback loop are private Avalonia implementation details.
    /// Stale callback rejection is mandatory: a timed-out action must never revive a second commit chain.
    /// <c>UseWgl</c> remains the full-backend compatibility fallback when this narrow patch is inapplicable.
    /// </remarks>
    internal static class WinUiCompositionWatchdogCompatibility
    {
        #region Constants

        internal const string HarmonyId = "IronyModManager.Avalonia.0.10.22.WinUiCompositionWatchdog";

        #endregion Constants

        #region Fields

        private static readonly object InstallationLock = new object();
        private static ConditionalWeakTable<object, WinUiCompositionPatchState> states = new ConditionalWeakTable<object, WinUiCompositionPatchState>();
        private static readonly WatchdogLog Log = new WatchdogLog();
        private static Harmony harmony;
        private static bool installed;
        private static WinUiCompositionReflection reflection;

        #endregion Fields

        #region Properties

        internal static bool IsInstalled
        {
            get
            {
                lock (InstallationLock)
                {
                    return installed;
                }
            }
        }

        #endregion Properties

        #region Methods

        public static void TryInstall()
        {
            try
            {
                var options = Win32Platform.Options;
                if (!IsApplicable(options))
                {
                    Logger.TryGet(LogEventLevel.Debug, LogArea.Visual)?.Log(
                        null,
                        "Avalonia WinUI composition watchdog is dormant because the applicable composition path is disabled.");
                    return;
                }

                lock (InstallationLock)
                {
                    if (installed)
                    {
                        return;
                    }

                    if (!WinUiCompositionReflection.TryCreate(out var resolved, out var failure))
                    {
                        Logger.TryGet(LogEventLevel.Warning, LogArea.Visual)?.Log(
                            null,
                            $"Avalonia WinUI composition watchdog validation failed: {failure}");
                        return;
                    }

                    if (!TryInstallResolved(resolved, out var installFailure))
                    {
                        Logger.TryGet(LogEventLevel.Warning, LogArea.Visual)?.Log(
                            null,
                            $"Avalonia WinUI composition watchdog installation failed; vanilla rendering will continue: {installFailure}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.TryGet(LogEventLevel.Warning, LogArea.Visual)?.Log(
                    null,
                    $"Avalonia WinUI composition watchdog was not installed: {ex}");
            }
        }

        public static void Stop() => WinUiCompositionWatchdogWindow.StopAll();

        internal static bool TryValidateTargets(out string failure) => WinUiCompositionReflection.TryCreate(out _, out failure);

        internal static bool IsApplicable(Win32PlatformOptions options) =>
            options != null && !options.UseWgl && options.UseWindowsUIComposition;

        internal static bool TryInstallValidatedForTesting(out string failure)
        {
            lock (InstallationLock)
            {
                if (installed)
                {
                    failure = null;
                    return true;
                }

                return WinUiCompositionReflection.TryCreate(out var resolved, out failure) &&
                       TryInstallResolved(resolved, out failure);
            }
        }

        internal static void UninstallForTesting()
        {
            lock (InstallationLock)
            {
                harmony?.UnpatchAll(HarmonyId);
                states = new ConditionalWeakTable<object, WinUiCompositionPatchState>();
                reflection = null;
                harmony = null;
                installed = false;
            }
        }

        private static bool TryInstallResolved(WinUiCompositionReflection resolved, out string failure)
        {
            var candidate = new Harmony(HarmonyId);
            try
            {
                var constructorPostfix = typeof(WinUiCompositionWatchdogCompatibility).GetMethod(
                    nameof(HandlerConstructorPostfix),
                    BindingFlags.Static | BindingFlags.NonPublic);
                var invokePrefix = typeof(WinUiCompositionWatchdogCompatibility).GetMethod(
                    nameof(HandlerInvokePrefix),
                    BindingFlags.Static | BindingFlags.NonPublic);
                var invokePostfix = typeof(WinUiCompositionWatchdogCompatibility).GetMethod(
                    nameof(HandlerInvokePostfix),
                    BindingFlags.Static | BindingFlags.NonPublic);
                candidate.Patch(resolved.HandlerConstructor, postfix: new HarmonyMethod(constructorPostfix));
                candidate.Patch(
                    resolved.HandlerInvoke,
                    prefix: new HarmonyMethod(invokePrefix),
                    postfix: new HarmonyMethod(invokePostfix));

                VerifyPatch(resolved.HandlerConstructor);
                VerifyPatch(resolved.HandlerInvoke);

                reflection = resolved;
                harmony = candidate;
                installed = true;
                failure = null;
                AppDomain.CurrentDomain.ProcessExit += (_, _) => Stop();
                Logger.TryGet(LogEventLevel.Debug, LogArea.Visual)?.Log(
                    null,
                    $"Installed Avalonia WinUI composition watchdog for {resolved.Assembly.GetName().Version}.");
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    candidate.UnpatchAll(HarmonyId);
                }
                catch (Exception unpatchException)
                {
                    ex = new AggregateException(ex, unpatchException);
                }

                states = new ConditionalWeakTable<object, WinUiCompositionPatchState>();
                reflection = null;
                harmony = null;
                installed = false;
                failure = ex.ToString();
                return false;
            }
        }

        private static void HandlerConstructorPostfix(object __instance)
        {
            var resolved = reflection;
            if (resolved == null || states.TryGetValue(__instance, out _))
            {
                return;
            }

            WinUiCompositionPatchState state = null;
            try
            {
                var connection = resolved.GetParent(__instance);
                state = new WinUiCompositionPatchState(__instance, connection, resolved, Log);
                states.Add(__instance, state);
                try
                {
                    WinUiCompositionWatchdogWindow.Create(state);
                }
                catch
                {
                    states.Remove(__instance);
                    throw;
                }

                state.ArmInitialCommit();
            }
            catch (Exception ex)
            {
                states.Remove(__instance);
                state?.BeginStop();
                Logger.TryGet(LogEventLevel.Error, LogArea.Visual)?.Log(
                    null,
                    $"Unable to initialize an Avalonia WinUI composition watchdog state; vanilla callback behavior remains active: {ex}");
            }
        }

        private static bool HandlerInvokePrefix(object __instance, object[] __args)
        {
            if (!states.TryGetValue(__instance, out var state))
            {
                return true;
            }

            try
            {
                return state.HandleCompletion(__args[0]) == CompletionHandling.AllowOriginal;
            }
            catch (Exception ex)
            {
                return state.HandleUnexpectedFailure("Unexpected failure in the Avalonia WinUI commit callback patch.", ex) == CompletionHandling.AllowOriginal;
            }
        }

        private static void HandlerInvokePostfix(object __instance)
        {
            if (states.TryGetValue(__instance, out var state))
            {
                state.RearmAfterVanillaInvoke();
            }
        }

        private static void VerifyPatch(MethodBase method)
        {
            var patchInfo = Harmony.GetPatchInfo(method);
            if (patchInfo == null || !patchInfo.Owners.Contains(HarmonyId, StringComparer.Ordinal))
            {
                throw new InvalidOperationException($"Harmony did not report ownership of {method.DeclaringType?.FullName}.{method.Name}.");
            }
        }

        #endregion Methods

        #region Classes

        private sealed class WatchdogLog : IWinUiCompositionWatchdogLog
        {
            public void Warning(string message) => Logger.TryGet(LogEventLevel.Warning, LogArea.Visual)?.Log(this, message);

            public void Error(string message, Exception exception) =>
                Logger.TryGet(LogEventLevel.Error, LogArea.Visual)?.Log(this, $"{message} {exception}");
        }

        #endregion Classes
    }
}
