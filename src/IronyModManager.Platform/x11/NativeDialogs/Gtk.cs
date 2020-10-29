// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="Gtk.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform.Interop;

// ReSharper disable IdentifierTypo
namespace IronyModManager.Platform.x11.NativeDialogs
{
    /// <summary>
    /// Enum GtkFileChooserAction
    /// </summary>
    internal enum GtkFileChooserAction
    {
        /// <summary>
        /// The open
        /// </summary>
        Open,

        /// <summary>
        /// The save
        /// </summary>
        Save,

        /// <summary>
        /// The select folder
        /// </summary>
        SelectFolder,
    }

    // ReSharper disable UnusedMember.Global
    /// <summary>
    /// Enum GtkResponseType
    /// </summary>
    internal enum GtkResponseType
    {
        /// <summary>
        /// The help
        /// </summary>
        Help = -11,

        /// <summary>
        /// The apply
        /// </summary>
        Apply = -10,

        /// <summary>
        /// The no
        /// </summary>
        No = -9,

        /// <summary>
        /// The yes
        /// </summary>
        Yes = -8,

        /// <summary>
        /// The close
        /// </summary>
        Close = -7,

        /// <summary>
        /// The cancel
        /// </summary>
        Cancel = -6,

        /// <summary>
        /// The ok
        /// </summary>
        Ok = -5,

        /// <summary>
        /// The delete event
        /// </summary>
        DeleteEvent = -4,

        /// <summary>
        /// The accept
        /// </summary>
        Accept = -3,

        /// <summary>
        /// The reject
        /// </summary>
        Reject = -2,

        /// <summary>
        /// The none
        /// </summary>
        None = -1,
    }

    /// <summary>
    /// Struct GSList
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct GSList
    {
        /// <summary>
        /// The data
        /// </summary>
        public readonly IntPtr Data;

        /// <summary>
        /// The next
        /// </summary>
        public readonly GSList* Next;
    }

    /// <summary>
    /// Class Glib.
    /// </summary>
    internal static unsafe class Glib
    {
        #region Fields

        /// <summary>
        /// The glib name
        /// </summary>
        private const string GlibName = "libglib-2.0.so.0";

        /// <summary>
        /// The g object name
        /// </summary>
        private const string GObjectName = "libgobject-2.0.so.0";

        /// <summary>
        /// The s pinned handler
        /// </summary>
        private static readonly timeout_callback s_pinnedHandler;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes static members of the <see cref="Glib" /> class.
        /// </summary>
        static Glib()
        {
            s_pinnedHandler = TimeoutHandler;
        }

        #endregion Constructors

        #region Delegates

        /// <summary>
        /// Delegate timeout_callback
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private delegate bool timeout_callback(IntPtr data);

        #endregion Delegates

        #region Methods

        /// <summary>
        /// Connects the signal.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="name">The name.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>IDisposable.</returns>
        /// <exception cref="ArgumentException">Unable to connect to signal " + name</exception>
        public static IDisposable ConnectSignal<T>(IntPtr obj, string name, T handler)
        {
            var handle = GCHandle.Alloc(handler);
            var ptr = Marshal.GetFunctionPointerForDelegate((Delegate)(object)handler);
            using var utf = new Utf8Buffer(name);
            var id = g_signal_connect_object(obj, utf, ptr, IntPtr.Zero, 0);
            if (id == 0)
                throw new ArgumentException("Unable to connect to signal " + name);
            return new ConnectedSignal(obj, handle, id);
        }

        /// <summary>
        /// gs the slist free.
        /// </summary>
        /// <param name="data">The data.</param>
        [DllImport(GlibName)]
        public static extern void g_slist_free(GSList* data);

        /// <summary>
        /// Runs the on glib thread.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns>Task&lt;T&gt;.</returns>
        public static Task<T> RunOnGlibThread<T>(Func<T> action)
        {
            var tcs = new TaskCompletionSource<T>();
            AddTimeout(0, 0, () =>
            {
                try
                {
                    tcs.SetResult(action());
                }
                catch (Exception e)
                {
                    tcs.TrySetException(e);
                }

                return false;
            });
            return tcs.Task;
        }

        /// <summary>
        /// Adds the timeout.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="callback">The callback.</param>
        private static void AddTimeout(int priority, uint interval, Func<bool> callback)
        {
            var handle = GCHandle.Alloc(callback);
            g_timeout_add_full(priority, interval, s_pinnedHandler, GCHandle.ToIntPtr(handle), IntPtr.Zero);
        }

        /// <summary>
        /// gs the object reference.
        /// </summary>
        /// <param name="instance">The instance.</param>
        [DllImport(GObjectName)]
        private static extern void g_object_ref(IntPtr instance);

        /// <summary>
        /// gs the object unref.
        /// </summary>
        /// <param name="instance">The instance.</param>
        [DllImport(GObjectName)]
        private static extern void g_object_unref(IntPtr instance);

        /// <summary>
        /// gs the signal connect object.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="signal">The signal.</param>
        /// <param name="handler">The handler.</param>
        /// <param name="userData">The user data.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>System.UInt64.</returns>
        [DllImport(GObjectName)]
        private static extern ulong g_signal_connect_object(IntPtr instance, Utf8Buffer signal,
            IntPtr handler, IntPtr userData, int flags);

        /// <summary>
        /// gs the signal handler disconnect.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="connectionId">The connection identifier.</param>
        /// <returns>System.UInt64.</returns>
        [DllImport(GObjectName)]
        private static extern ulong g_signal_handler_disconnect(IntPtr instance, ulong connectionId);

        /// <summary>
        /// gs the timeout add full.
        /// </summary>
        /// <param name="prio">The prio.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="callback">The callback.</param>
        /// <param name="data">The data.</param>
        /// <param name="destroy">The destroy.</param>
        /// <returns>System.UInt64.</returns>
        [DllImport(GlibName)]
        private static extern ulong g_timeout_add_full(int prio, uint interval, timeout_callback callback, IntPtr data,
            IntPtr destroy);

        /// <summary>
        /// Timeouts the handler.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool TimeoutHandler(IntPtr data)
        {
            var handle = GCHandle.FromIntPtr(data);
            var cb = (Func<bool>)handle.Target;
            if (!cb())
            {
                handle.Free();
                return false;
            }

            return true;
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class ConnectedSignal.
        /// Implements the <see cref="System.IDisposable" />
        /// </summary>
        /// <seealso cref="System.IDisposable" />
        private class ConnectedSignal : IDisposable
        {
            #region Fields

            /// <summary>
            /// The identifier
            /// </summary>
            private readonly ulong _id;

            /// <summary>
            /// The instance
            /// </summary>
            private readonly IntPtr _instance;

            /// <summary>
            /// The handle
            /// </summary>
            private GCHandle _handle;

            #endregion Fields

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectedSignal" /> class.
            /// </summary>
            /// <param name="instance">The instance.</param>
            /// <param name="handle">The handle.</param>
            /// <param name="id">The identifier.</param>
            public ConnectedSignal(IntPtr instance, GCHandle handle, ulong id)
            {
                _instance = instance;
                g_object_ref(instance);
                _handle = handle;
                _id = id;
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                if (_handle.IsAllocated)
                {
                    g_signal_handler_disconnect(_instance, _id);
                    g_object_unref(_instance);
                    _handle.Free();
                }
            }

            #endregion Methods
        }

        #endregion Classes
    }

    // ReSharper restore UnusedMember.Global

    /// <summary>
    /// Class Gtk.
    /// </summary>
    internal static unsafe class Gtk
    {
        #region Fields

        /// <summary>
        /// The GDK name
        /// </summary>
        private const string GdkName = "libgdk-3.so.0";

        /// <summary>
        /// The GTK name
        /// </summary>
        private const string GtkName = "libgtk-3.so.0";

        /// <summary>
        /// The s display
        /// </summary>
        private static IntPtr s_display;

        #endregion Fields

        #region Delegates

        /// <summary>
        /// Delegate signal_dialog_response
        /// </summary>
        /// <param name="gtkWidget">The GTK widget.</param>
        /// <param name="response">The response.</param>
        /// <param name="userData">The user data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public delegate bool signal_dialog_response(IntPtr gtkWidget, GtkResponseType response, IntPtr userData);

        /// <summary>
        /// Delegate signal_generic
        /// </summary>
        /// <param name="gtkWidget">The GTK widget.</param>
        /// <param name="userData">The user data.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public delegate bool signal_generic(IntPtr gtkWidget, IntPtr userData);

        #endregion Delegates

        #region Methods

        /// <summary>
        /// GDKs the window set transient for.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="parent">The parent.</param>
        [DllImport(GdkName)]
        public static extern void gdk_window_set_transient_for(IntPtr window, IntPtr parent);

        /// <summary>
        /// Gets the foreign window.
        /// </summary>
        /// <param name="xid">The xid.</param>
        /// <returns>IntPtr.</returns>
        public static IntPtr GetForeignWindow(IntPtr xid) => gdk_x11_window_foreign_new_for_display(s_display, xid);

        /// <summary>
        /// GTKs the dialog add button.
        /// </summary>
        /// <param name="raw">The raw.</param>
        /// <param name="button_text">The button text.</param>
        /// <param name="response_id">The response identifier.</param>
        [DllImport(GtkName)]
        public static extern void
            gtk_dialog_add_button(IntPtr raw, Utf8Buffer button_text, GtkResponseType response_id);

        /// <summary>
        /// GTKs the file chooser add filter.
        /// </summary>
        /// <param name="chooser">The chooser.</param>
        /// <param name="filter">The filter.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(GtkName)]
        public static extern IntPtr gtk_file_chooser_add_filter(IntPtr chooser, IntPtr filter);

        /// <summary>
        /// GTKs the file chooser dialog new.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="action">The action.</param>
        /// <param name="ignore">The ignore.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(GtkName)]
        public static extern IntPtr gtk_file_chooser_dialog_new(Utf8Buffer title, IntPtr parent,
            GtkFileChooserAction action, IntPtr ignore);

        /// <summary>
        /// GTKs the file chooser get filenames.
        /// </summary>
        /// <param name="chooser">The chooser.</param>
        /// <returns>GSList.</returns>
        [DllImport(GtkName)]
        public static extern GSList* gtk_file_chooser_get_filenames(IntPtr chooser);

        /// <summary>
        /// GTKs the file chooser get filter.
        /// </summary>
        /// <param name="chooser">The chooser.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(GtkName)]
        public static extern IntPtr gtk_file_chooser_get_filter(IntPtr chooser);

        /// <summary>
        /// GTKs the name of the file chooser set current.
        /// </summary>
        /// <param name="chooser">The chooser.</param>
        /// <param name="file">The file.</param>
        [DllImport(GtkName)]
        public static extern void gtk_file_chooser_set_current_name(IntPtr chooser, Utf8Buffer file);

        /// <summary>
        /// GTKs the file chooser set filename.
        /// </summary>
        /// <param name="chooser">The chooser.</param>
        /// <param name="file">The file.</param>
        [DllImport(GtkName)]
        public static extern void gtk_file_chooser_set_filename(IntPtr chooser, Utf8Buffer file);

        /// <summary>
        /// GTKs the file chooser set select multiple.
        /// </summary>
        /// <param name="chooser">The chooser.</param>
        /// <param name="allow">if set to <c>true</c> [allow].</param>
        [DllImport(GtkName)]
        public static extern void gtk_file_chooser_set_select_multiple(IntPtr chooser, bool allow);

        /// <summary>
        /// GTKs the file filter add pattern.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="pattern">The pattern.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(GtkName)]
        public static extern IntPtr gtk_file_filter_add_pattern(IntPtr filter, Utf8Buffer pattern);

        /// <summary>
        /// GTKs the file filter new.
        /// </summary>
        /// <returns>IntPtr.</returns>
        [DllImport(GtkName)]
        public static extern IntPtr gtk_file_filter_new();

        /// <summary>
        /// GTKs the name of the file filter set.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="name">The name.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(GtkName)]
        public static extern IntPtr gtk_file_filter_set_name(IntPtr filter, Utf8Buffer name);

        /// <summary>
        /// GTKs the widget get window.
        /// </summary>
        /// <param name="gtkWidget">The GTK widget.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(GtkName)]
        public static extern IntPtr gtk_widget_get_window(IntPtr gtkWidget);

        /// <summary>
        /// GTKs the widget hide.
        /// </summary>
        /// <param name="gtkWidget">The GTK widget.</param>
        [DllImport(GtkName)]
        public static extern void gtk_widget_hide(IntPtr gtkWidget);

        /// <summary>
        /// GTKs the widget realize.
        /// </summary>
        /// <param name="gtkWidget">The GTK widget.</param>
        [DllImport(GtkName)]
        public static extern void gtk_widget_realize(IntPtr gtkWidget);

        /// <summary>
        /// GTKs the window present.
        /// </summary>
        /// <param name="gtkWindow">The GTK window.</param>
        [DllImport(GtkName)]
        public static extern void gtk_window_present(IntPtr gtkWindow);

        /// <summary>
        /// GTKs the window set modal.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <param name="modal">if set to <c>true</c> [modal].</param>
        [DllImport(GtkName)]
        public static extern void gtk_window_set_modal(IntPtr window, bool modal);

        /// <summary>
        /// Starts the GTK.
        /// </summary>
        /// <returns>Task&lt;System.Boolean&gt;.</returns>
        public static Task<bool> StartGtk()
        {
            var tcs = new TaskCompletionSource<bool>();
            new Thread(() =>
            {
                try
                {
                    using var backends = new Utf8Buffer("x11");
                    gdk_set_allowed_backends(backends);
                }
                catch
                {
                    //Ignore
                }

                Environment.SetEnvironmentVariable("WAYLAND_DISPLAY",
                    "/proc/fake-display-to-prevent-wayland-initialization-by-gtk3");

                if (!gtk_init_check(0, IntPtr.Zero))
                {
                    tcs.SetResult(false);
                    return;
                }

                IntPtr app;
                using (var utf = new Utf8Buffer($"avalonia.app.a{Guid.NewGuid():N}"))
                    app = gtk_application_new(utf, 0);
                if (app == IntPtr.Zero)
                {
                    tcs.SetResult(false);
                    return;
                }

                s_display = gdk_display_get_default();
                tcs.SetResult(true);
                while (true)
                    gtk_main_iteration();
            })
            { Name = "GTK3THREAD", IsBackground = true }.Start();
            return tcs.Task;
        }

        /// <summary>
        /// GDKs the display get default.
        /// </summary>
        /// <returns>IntPtr.</returns>
        [DllImport(GdkName)]
        private static extern IntPtr gdk_display_get_default();

        /// <summary>
        /// GDKs the set allowed backends.
        /// </summary>
        /// <param name="backends">The backends.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(GdkName)]
        private static extern IntPtr gdk_set_allowed_backends(Utf8Buffer backends);

        /// <summary>
        /// GDKs the X11 window foreign new for display.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <param name="xid">The xid.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(GdkName)]
        private static extern IntPtr gdk_x11_window_foreign_new_for_display(IntPtr display, IntPtr xid);

        /// <summary>
        /// GTKs the application new.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="flags">The flags.</param>
        /// <returns>IntPtr.</returns>
        [DllImport(GtkName)]
        private static extern IntPtr gtk_application_new(Utf8Buffer appId, int flags);

        /// <summary>
        /// GTKs the initialize check.
        /// </summary>
        /// <param name="argc">The argc.</param>
        /// <param name="argv">The argv.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        [DllImport(GtkName)]
        private static extern bool gtk_init_check(int argc, IntPtr argv);

        /// <summary>
        /// GTKs the main iteration.
        /// </summary>
        [DllImport(GtkName)]
        private static extern void gtk_main_iteration();

        #endregion Methods
    }
}
