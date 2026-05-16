// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Mario
// Created          : 05-16-2026
//
// Last Modified By : Mario
// Last Modified On : 05-16-2026
// ***********************************************************************
// <copyright file="SafeX11InputMethod.cs" company="Mario">
//     Mario
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.FreeDesktop;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;

namespace IronyModManager.Platform.Dbus
{
    /// <summary>
    /// Class SafeX11InputMethod. This class cannot be inherited.
    /// </summary>
    internal sealed class SafeX11InputMethod
    {
        #region Fields

        /// <summary>
        /// The dbus IME base type name
        /// </summary>
        private const string DbusImeBaseTypeName = "Avalonia.FreeDesktop.DBusIme.DBusTextInputMethodBase";

        #endregion Fields

        #region Methods

        /// <summary>
        /// Tries to install.
        /// </summary>
        public static void TryInstall()
        {
            var factory = AvaloniaLocator.Current.GetService<IX11InputMethodFactory>();
            switch (factory)
            {
                case null:

                case SafeX11InputMethodFactory:
                    return;
                default:
                    AvaloniaLocator.CurrentMutable.Bind<IX11InputMethodFactory>().ToConstant(new SafeX11InputMethodFactory(factory));
                    break;
            }
        }

        /// <summary>
        /// Clears the name of the current.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="dbusImeBaseType">Type of the dbus IME base.</param>
        private static void ClearCurrentName(object instance, Type dbusImeBaseType)
        {
            var currentNameField = dbusImeBaseType.GetField("_currentName", BindingFlags.Instance | BindingFlags.NonPublic);

            try
            {
                currentNameField?.SetValue(instance, null);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Disconnect safely as an asynchronous operation.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="dbusImeBaseType">Type of the dbus IME base.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private static async Task DisconnectSafelyAsync(object instance, Type dbusImeBaseType)
        {
            var disconnectMethod = dbusImeBaseType.GetMethod("Disconnect", BindingFlags.Instance | BindingFlags.NonPublic);

            if (disconnectMethod == null)
            {
                return;
            }

            try
            {
                if (disconnectMethod.Invoke(instance, null) is Task task)
                {
                    await task.ConfigureAwait(false);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Disposes the subscriptions.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="dbusImeBaseType">Type of the dbus IME base.</param>
        private static void DisposeSubscriptions(object instance, Type dbusImeBaseType)
        {
            var disposablesField = dbusImeBaseType.GetField("_disposables", BindingFlags.Instance | BindingFlags.NonPublic);

            if (disposablesField?.GetValue(instance) is not IEnumerable disposables)
            {
                return;
            }

            var snapshot = disposables.OfType<IDisposable>().ToArray();

            foreach (var disposable in snapshot)
            {
                try
                {
                    disposable.Dispose();
                }
                catch
                {
                }
            }

            try
            {
                disposablesField.GetValue(instance)?.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public)?.Invoke(disposablesField.GetValue(instance), null);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Safe dispose dbus IME as an asynchronous operation.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="dbusImeBaseType">Type of the dbus IME base.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private static async Task SafeDisposeDbusImeAsync(object instance, Type dbusImeBaseType)
        {
            try
            {
                DisposeSubscriptions(instance, dbusImeBaseType);

                await DisconnectSafelyAsync(instance, dbusImeBaseType).ConfigureAwait(false);

                ClearCurrentName(instance, dbusImeBaseType);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Tries the type of the find dbus IME base.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="baseType">Type of the base.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool TryFindDbusImeBaseType(Type type, out Type baseType)
        {
            for (var current = type; current != null; current = current.BaseType)
            {
                if (current.FullName == DbusImeBaseTypeName)
                {
                    baseType = current;
                    return true;
                }
            }

            baseType = null;
            return false;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class SafeX11InputMethodControl. This class cannot be inherited.
        /// Implements the <see cref="IX11InputMethodControl" />
        /// </summary>
        /// <seealso cref="IX11InputMethodControl" />
        private sealed class SafeX11InputMethodControl(IX11InputMethodControl proxy) : IX11InputMethodControl
        {
            #region Fields

            /// <summary>
            /// The proxy
            /// </summary>
            private readonly IX11InputMethodControl proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));

            #endregion Fields

            #region Events

            /// <summary>
            /// Occurs when [commit].
            /// </summary>
            public event Action<string> Commit
            {
                add => proxy.Commit += value;
                remove => proxy.Commit -= value;
            }

            /// <summary>
            /// Occurs when [forward key].
            /// </summary>
            public event Action<X11InputMethodForwardedKey> ForwardKey
            {
                add => proxy.ForwardKey += value;
                remove => proxy.ForwardKey -= value;
            }

            #endregion Events

            #region Properties

            /// <summary>
            /// Gets a value indicating whether this instance is enabled.
            /// </summary>
            /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
            public bool IsEnabled => proxy.IsEnabled;

            #endregion Properties

            #region Methods

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                if (TryFindDbusImeBaseType(proxy.GetType(), out var baseType))
                {
                    _ = SafeDisposeDbusImeAsync(proxy, baseType);
                    return;
                }

                proxy.Dispose();
            }

            /// <summary>
            /// Handles the event asynchronous.
            /// </summary>
            /// <param name="args">The <see cref="RawKeyEventArgs" /> instance containing the event data.</param>
            /// <param name="keyVal">The key value.</param>
            /// <param name="keyCode">The key code.</param>
            /// <returns>ValueTask{System.Boolean}.</returns>
            public ValueTask<bool> HandleEventAsync(RawKeyEventArgs args, int keyVal, int keyCode)
            {
                return proxy.HandleEventAsync(args, keyVal, keyCode);
            }

            /// <summary>
            /// Sets the window active.
            /// </summary>
            /// <param name="active">if set to <c>true</c> [active].</param>
            public void SetWindowActive(bool active)
            {
                proxy.SetWindowActive(active);
            }

            /// <summary>
            /// Updates the window information.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="scaling">The scaling.</param>
            public void UpdateWindowInfo(PixelPoint position, double scaling)
            {
                proxy.UpdateWindowInfo(position, scaling);
            }

            #endregion Methods
        }

        /// <summary>
        /// Class SafeX11InputMethodFactory. This class cannot be inherited.
        /// Implements the <see cref="IX11InputMethodFactory" />
        /// </summary>
        /// <seealso cref="IX11InputMethodFactory" />
        private sealed class SafeX11InputMethodFactory(IX11InputMethodFactory proxy) : IX11InputMethodFactory
        {
            #region Fields

            /// <summary>
            /// The proxy
            /// </summary>
            private readonly IX11InputMethodFactory proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));

            #endregion Fields

            #region Methods

            /// <summary>
            /// Creates the client.
            /// </summary>
            /// <param name="xid">The xid.</param>
            /// <returns>System.ValueTuple{ITextInputMethodImpl, IX11InputMethodControl}.</returns>
            public (ITextInputMethodImpl method, IX11InputMethodControl control) CreateClient(IntPtr xid)
            {
                var (method, control) = proxy.CreateClient(xid);

                if (control is null or SafeX11InputMethodControl)
                {
                    return (method, control);
                }

                return (method, new SafeX11InputMethodControl(control));
            }

            #endregion Methods
        }

        #endregion Classes
    }
}
