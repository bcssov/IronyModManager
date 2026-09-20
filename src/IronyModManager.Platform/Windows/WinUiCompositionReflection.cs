// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 09-19-2026
//
// Last Modified By : Mario
// Last Modified On : 09-19-2026
// ***********************************************************************
// <copyright file="WinUiCompositionReflection.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using Avalonia;

namespace IronyModManager.Platform.Windows
{
    /// <summary>
    /// Resolves the pinned Avalonia 0.10.22 WinUI Composition implementation used by the watchdog.
    /// </summary>
    internal sealed class WinUiCompositionReflection : IWinUiCompositionInterop
    {
        #region Constants

        private const string ConnectionTypeName = "Avalonia.Win32.WinRT.Composition.WinUICompositorConnection";
        private const string ActionProxyTypeName = "Avalonia.Win32.WinRT.Impl.__MicroComIAsyncActionProxy";

        #endregion Constants

        #region Fields

        private readonly MethodInfo actionDispose;
        private readonly MethodInfo actionGetResults;
        private readonly PropertyInfo actionNativePointer;
        private readonly MethodInfo actionSetCompleted;
        private readonly FieldInfo connectionCompositor;
        private readonly FieldInfo connectionPumpLock;
        private readonly FieldInfo connectionTick;
        private readonly FieldInfo handlerParent;
        private readonly FieldInfo handlerStopwatch;
        private readonly MethodInfo requestCommit;

        #endregion Fields

        #region Constructors

        private WinUiCompositionReflection(
            Assembly assembly,
            Type connectionType,
            Type handlerType,
            Type actionProxyType,
            ConstructorInfo handlerConstructor,
            MethodInfo handlerInvoke,
            MethodInfo actionSetCompleted,
            MethodInfo connectionRunLoop,
            FieldInfo handlerParent,
            FieldInfo handlerStopwatch,
            FieldInfo connectionCompositor,
            FieldInfo connectionPumpLock,
            FieldInfo connectionTick,
            MethodInfo requestCommit,
            PropertyInfo actionNativePointer,
            MethodInfo actionDispose,
            MethodInfo actionGetResults)
        {
            Assembly = assembly;
            ConnectionType = connectionType;
            HandlerType = handlerType;
            ActionProxyType = actionProxyType;
            HandlerConstructor = handlerConstructor;
            HandlerInvoke = handlerInvoke;
            ActionSetCompleted = actionSetCompleted;
            ConnectionRunLoop = connectionRunLoop;
            this.handlerParent = handlerParent;
            this.handlerStopwatch = handlerStopwatch;
            this.connectionCompositor = connectionCompositor;
            this.connectionPumpLock = connectionPumpLock;
            this.connectionTick = connectionTick;
            this.requestCommit = requestCommit;
            this.actionNativePointer = actionNativePointer;
            this.actionDispose = actionDispose;
            this.actionGetResults = actionGetResults;
            this.actionSetCompleted = actionSetCompleted;
        }

        #endregion Constructors

        #region Properties

        public Assembly Assembly { get; }

        public Type ConnectionType { get; }

        public Type HandlerType { get; }

        public Type ActionProxyType { get; }

        public ConstructorInfo HandlerConstructor { get; }

        public MethodInfo HandlerInvoke { get; }

        public MethodInfo ActionSetCompleted { get; }

        public MethodInfo ConnectionRunLoop { get; }

        #endregion Properties

        #region Methods

        public static bool TryCreate(out WinUiCompositionReflection reflection, out string failure)
        {
            reflection = null;
            failure = null;

            try
            {
                var assembly = typeof(Win32PlatformOptions).Assembly;
                var name = assembly.GetName();
                var publicKeyToken = name.GetPublicKeyToken();
                if (!string.Equals(name.Name, "Avalonia.Win32", StringComparison.Ordinal) ||
                    name.Version != new Version(0, 10, 22, 0) ||
                    publicKeyToken == null ||
                    Convert.ToHexString(publicKeyToken) != "C8D484A7012F9A8B")
                {
                    failure = $"Avalonia.Win32 identity {name.FullName} is not supported.";
                    return false;
                }

                var connectionType = RequireType(assembly, ConnectionTypeName);
                var handlerType = connectionType.GetNestedType("RunLoopHandler", BindingFlags.NonPublic)
                                  ?? throw new MissingMemberException(connectionType.FullName, "RunLoopHandler");
                var actionProxyType = RequireType(assembly, ActionProxyTypeName);
                var asyncActionType = RequireType(assembly, "Avalonia.Win32.WinRT.IAsyncAction");
                var asyncStatusType = RequireType(assembly, "Avalonia.Win32.WinRT.AsyncStatus");
                var completionHandlerType = RequireType(assembly, "Avalonia.Win32.WinRT.IAsyncActionCompletedHandler");
                var compositorType = RequireType(assembly, "Avalonia.Win32.WinRT.ICompositor5");

                const BindingFlags instance = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                var handlerConstructor = handlerType.GetConstructor(instance, null, new[] { connectionType }, null)
                                         ?? throw new MissingMethodException(handlerType.FullName, ".ctor");
                var handlerInvoke = handlerType.GetMethod("Invoke", instance, null, new[] { asyncActionType, asyncStatusType }, null)
                                    ?? throw new MissingMethodException(handlerType.FullName, "Invoke");
                var actionSetCompleted = actionProxyType.GetMethod("SetCompleted", instance, null, new[] { completionHandlerType }, null)
                                          ?? throw new MissingMethodException(actionProxyType.FullName, "SetCompleted");
                var connectionRunLoop = connectionType.GetMethod("RunLoop", instance, null, Type.EmptyTypes, null)
                                        ?? throw new MissingMethodException(connectionType.FullName, "RunLoop");

                var handlerParent = RequireField(handlerType, "_parent", connectionType);
                var handlerStopwatch = RequireField(handlerType, "_st", typeof(Stopwatch));
                var connectionCompositor = RequireField(connectionType, "_compositor5", compositorType);
                var connectionPumpLock = RequireField(connectionType, "_pumpLock", typeof(object));
                var connectionTick = RequireField(connectionType, "Tick", typeof(Action<TimeSpan>));
                var requestCommit = compositorType.GetMethod("RequestCommitAsync", instance, null, Type.EmptyTypes, null)
                                    ?? throw new MissingMethodException(compositorType.FullName, "RequestCommitAsync");
                if (requestCommit.ReturnType != asyncActionType)
                {
                    throw new MissingMethodException(compositorType.FullName, "RequestCommitAsync return type");
                }

                var actionNativePointer = actionProxyType.GetProperty("NativePointer", instance)
                                          ?? throw new MissingMemberException(actionProxyType.FullName, "NativePointer");
                var actionDispose = actionProxyType.GetMethod("Dispose", instance, null, Type.EmptyTypes, null)
                                    ?? throw new MissingMethodException(actionProxyType.FullName, "Dispose");
                var actionGetResults = actionProxyType.GetMethod("GetResults", instance, null, Type.EmptyTypes, null)
                                       ?? throw new MissingMethodException(actionProxyType.FullName, "GetResults");

                ValidateIl(handlerConstructor, "489D1848CB71BE6ECECED615423DD0C1854C1C9A2585BC48B48B1833A44BEA54");
                ValidateIl(handlerInvoke, "5EBB33020A137515BE50E4852DDEBDA89597FA1239A7331208637EFEFC40148E");
                ValidateIl(actionSetCompleted, "B78278BF3F5BC02095DA5FA1C49779C8B655FAEAE08991610F686A9E8012A44B");
                ValidateIl(connectionRunLoop, "9CB0637317AB45F25998B8FA2F7C868A3E2ED0B126E30D2E435E7C510BC3E43D");

                reflection = new WinUiCompositionReflection(
                    assembly,
                    connectionType,
                    handlerType,
                    actionProxyType,
                    handlerConstructor,
                    handlerInvoke,
                    actionSetCompleted,
                    connectionRunLoop,
                    handlerParent,
                    handlerStopwatch,
                    connectionCompositor,
                    connectionPumpLock,
                    connectionTick,
                    requestCommit,
                    actionNativePointer,
                    actionDispose,
                    actionGetResults);
                return true;
            }
            catch (Exception ex)
            {
                failure = ex.Message;
                return false;
            }
        }

        public object GetParent(object handler) => handlerParent.GetValue(handler);

        public object GetPumpLock(object connection) => connectionPumpLock.GetValue(connection);

        public TimeSpan GetElapsed(object handler) => ((Stopwatch)handlerStopwatch.GetValue(handler)).Elapsed;

        public IntPtr GetIdentity(object action) => (IntPtr)actionNativePointer.GetValue(action);

        public object RequestCommit(object connection)
        {
            var compositor = connectionCompositor.GetValue(connection);
            return Invoke(requestCommit, compositor, null);
        }

        public void SetCompleted(object action, object handler) => Invoke(actionSetCompleted, action, new[] { handler });

        public void GetResults(object action) => Invoke(actionGetResults, action, null);

        public void DisposeAction(object action) => Invoke(actionDispose, action, null);

        public void RaiseTick(object connection, TimeSpan elapsed)
        {
            ((Action<TimeSpan>)connectionTick.GetValue(connection))?.Invoke(elapsed);
        }

        private static Type RequireType(Assembly assembly, string name) =>
            assembly.GetType(name, false) ?? throw new TypeLoadException(name);

        private static FieldInfo RequireField(Type type, string name, Type fieldType)
        {
            var field = type.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        ?? throw new MissingFieldException(type.FullName, name);
            if (field.FieldType != fieldType)
            {
                throw new MissingFieldException(type.FullName, $"{name} ({field.FieldType.FullName})");
            }

            return field;
        }

        private static void ValidateIl(MethodBase method, string expectedHash)
        {
            var bytes = method.GetMethodBody()?.GetILAsByteArray()
                        ?? throw new InvalidOperationException($"{method} has no IL body.");
            var actualHash = Convert.ToHexString(SHA256.HashData(bytes));
            if (!actualHash.Equals(expectedHash, StringComparison.Ordinal))
            {
                throw new InvalidOperationException($"Unexpected IL fingerprint for {method.DeclaringType?.FullName}.{method.Name}: {actualHash}.");
            }
        }

        private static object Invoke(MethodInfo method, object instance, object[] arguments)
        {
            try
            {
                return method.Invoke(instance, arguments);
            }
            catch (TargetInvocationException ex) when (ex.InnerException != null)
            {
                throw ex.InnerException;
            }
        }

        #endregion Methods
    }

    /// <summary>
    /// Private Avalonia operations required by the deterministic watchdog state machine.
    /// </summary>
    internal interface IWinUiCompositionInterop
    {
        object GetParent(object handler);

        object GetPumpLock(object connection);

        TimeSpan GetElapsed(object handler);

        IntPtr GetIdentity(object action);

        object RequestCommit(object connection);

        void SetCompleted(object action, object handler);

        void GetResults(object action);

        void DisposeAction(object action);

        void RaiseTick(object connection, TimeSpan elapsed);
    }
}
