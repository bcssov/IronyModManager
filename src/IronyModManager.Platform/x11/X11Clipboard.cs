// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="X11Clipboard.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Input.Platform;
using IronyModManager.Shared;
using static IronyModManager.Platform.x11.XLib;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Class X11Clipboard.
    /// Implements the <see cref="Avalonia.Input.Platform.IClipboard" />
    /// </summary>
    /// <seealso cref="Avalonia.Input.Platform.IClipboard" />
    [ExcludeFromCoverage("External component.")]
    internal class X11Clipboard : IClipboard
    {
        #region Fields

        /// <summary>
        /// The avalonia save targets atom
        /// </summary>
        private readonly IntPtr _avaloniaSaveTargetsAtom;

        /// <summary>
        /// The handle
        /// </summary>
        private readonly IntPtr _handle;

        /// <summary>
        /// The text atoms
        /// </summary>
        private readonly IntPtr[] _textAtoms;

        /// <summary>
        /// The X11
        /// </summary>
        private readonly X11Info _x11;

        /// <summary>
        /// The requested formats TCS
        /// </summary>
        private TaskCompletionSource<IntPtr[]> _requestedFormatsTcs;

        /// <summary>
        /// The requested text TCS
        /// </summary>
        private TaskCompletionSource<string> _requestedTextTcs;

        /// <summary>
        /// The stored string
        /// </summary>
        private string _storedString;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="X11Clipboard" /> class.
        /// </summary>
        /// <param name="platform">The platform.</param>
        public X11Clipboard(AvaloniaX11Platform platform)
        {
            _x11 = platform.Info;
            _handle = CreateEventWindow(platform, OnEvent);
            _avaloniaSaveTargetsAtom = XInternAtom(_x11.Display, "AVALONIA_SAVE_TARGETS_PROPERTY_ATOM", false);
            _textAtoms = new[]
            {
                _x11.Atoms.XA_STRING,
                _x11.Atoms.OEMTEXT,
                _x11.Atoms.UTF8_STRING,
                _x11.Atoms.UTF16_STRING
            }.Where(a => a != IntPtr.Zero).ToArray();
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Clears the asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public Task ClearAsync()
        {
            return SetTextAsync(null);
        }

        /// <summary>
        /// get text as an asynchronous operation.
        /// </summary>
        /// <returns>System.String.</returns>
        public async Task<string> GetTextAsync()
        {
            if (XGetSelectionOwner(_x11.Display, _x11.Atoms.CLIPBOARD) == IntPtr.Zero)
                return null;
            var res = await SendFormatRequest();
            var target = _x11.Atoms.UTF8_STRING;
            if (res != null)
            {
                var preferredFormats = new[] { _x11.Atoms.UTF16_STRING, _x11.Atoms.UTF8_STRING, _x11.Atoms.XA_STRING };
                foreach (var pf in preferredFormats)
                    if (res.Contains(pf))
                    {
                        target = pf;
                        break;
                    }
            }

            return await SendTextRequest(target);
        }

        /// <summary>
        /// Sets the text asynchronous.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>Task.</returns>
        public Task SetTextAsync(string text)
        {
            _storedString = text;
            XSetSelectionOwner(_x11.Display, _x11.Atoms.CLIPBOARD, _handle, IntPtr.Zero);
            StoreAtomsInClipboardManager(_textAtoms);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the string encoding.
        /// </summary>
        /// <param name="atom">The atom.</param>
        /// <returns>Encoding.</returns>
        private Encoding GetStringEncoding(IntPtr atom)
        {
            return (atom == _x11.Atoms.XA_STRING
                    || atom == _x11.Atoms.OEMTEXT)
                ? Encoding.ASCII
                : atom == _x11.Atoms.UTF8_STRING
                    ? Encoding.UTF8
                    : atom == _x11.Atoms.UTF16_STRING
                        ? Encoding.Unicode
                        : null;
        }

        /// <summary>
        /// Called when [event].
        /// </summary>
        /// <param name="ev">The ev.</param>
        private unsafe void OnEvent(XEvent ev)
        {
            if (ev.type == XEventName.SelectionRequest)
            {
                var sel = ev.SelectionRequestEvent;
                var resp = new XEvent
                {
                    SelectionEvent =
                    {
                        type = XEventName.SelectionNotify,
                        send_event = true,
                        display = _x11.Display,
                        selection = sel.selection,
                        target = sel.target,
                        requestor = sel.requestor,
                        time = sel.time,
                        property = IntPtr.Zero
                    }
                };
                if (sel.selection == _x11.Atoms.CLIPBOARD)
                {
                    resp.SelectionEvent.property = WriteTargetToProperty(sel.target, sel.requestor, sel.property);
                }

                XSendEvent(_x11.Display, sel.requestor, false, new IntPtr((int)EventMask.NoEventMask), ref resp);
            }

            IntPtr WriteTargetToProperty(IntPtr target, IntPtr window, IntPtr property)
            {
                Encoding textEnc;
                if (target == _x11.Atoms.TARGETS)
                {
                    var atoms = _textAtoms;
                    atoms = atoms.Concat(new[] { _x11.Atoms.TARGETS, _x11.Atoms.MULTIPLE })
                        .ToArray();
                    XChangeProperty(_x11.Display, window, property,
                        _x11.Atoms.XA_ATOM, 32, PropertyMode.Replace, atoms, atoms.Length);
                    return property;
                }
                else if (target == _x11.Atoms.SAVE_TARGETS && _x11.Atoms.SAVE_TARGETS != IntPtr.Zero)
                {
                    return property;
                }
                else if ((textEnc = GetStringEncoding(target)) != null)
                {
                    var data = textEnc.GetBytes(_storedString ?? "");
                    fixed (void* pdata = data)
                        XChangeProperty(_x11.Display, window, property, target, 8,
                            PropertyMode.Replace,
                            pdata, data.Length);
                    return property;
                }
                else if (target == _x11.Atoms.MULTIPLE && _x11.Atoms.MULTIPLE != IntPtr.Zero)
                {
                    XGetWindowProperty(_x11.Display, window, property, IntPtr.Zero, new IntPtr(0x7fffffff), false,
                        _x11.Atoms.ATOM_PAIR, out _, out var actualFormat, out var nitems, out _, out var prop);
                    if (nitems == IntPtr.Zero)
                        return IntPtr.Zero;
                    if (actualFormat == 32)
                    {
                        var data = (IntPtr*)prop.ToPointer();
                        for (var c = 0; c < nitems.ToInt32(); c += 2)
                        {
                            var subTarget = data[c];
                            var subProp = data[c + 1];
                            var converted = WriteTargetToProperty(subTarget, window, subProp);
                            data[c + 1] = converted;
                        }

                        XChangeProperty(_x11.Display, window, property, _x11.Atoms.ATOM_PAIR, 32, PropertyMode.Replace,
                            prop.ToPointer(), nitems.ToInt32());
                    }

                    XFree(prop);

                    return property;
                }
                else
                    return IntPtr.Zero;
            }

            if (ev.type == XEventName.SelectionNotify && ev.SelectionEvent.selection == _x11.Atoms.CLIPBOARD)
            {
                var sel = ev.SelectionEvent;
                if (sel.property == IntPtr.Zero)
                {
                    _requestedFormatsTcs?.TrySetResult(null);
                    _requestedTextTcs?.TrySetResult(null);
                }
                XGetWindowProperty(_x11.Display, _handle, sel.property, IntPtr.Zero, new IntPtr(0x7fffffff), true, (IntPtr)Atom.AnyPropertyType,
                    out var actualAtom, out var actualFormat, out var nitems, out var bytes_after, out var prop);
                Encoding textEnc = null;
                if (nitems == IntPtr.Zero)
                {
                    _requestedFormatsTcs?.TrySetResult(null);
                    _requestedTextTcs?.TrySetResult(null);
                }
                else
                {
                    if (sel.property == _x11.Atoms.TARGETS)
                    {
                        if (actualFormat != 32)
                            _requestedFormatsTcs?.TrySetResult(null);
                        else
                        {
                            var formats = new IntPtr[nitems.ToInt32()];
                            Marshal.Copy(prop, formats, 0, formats.Length);
                            _requestedFormatsTcs?.TrySetResult(formats);
                        }
                    }
                    else if ((textEnc = GetStringEncoding(sel.property)) != null)
                    {
                        var text = textEnc.GetString((byte*)prop.ToPointer(), nitems.ToInt32());
                        _requestedTextTcs?.TrySetResult(text);
                    }
                }

                XFree(prop);
            }
        }

        /// <summary>
        /// Sends the format request.
        /// </summary>
        /// <returns>Task&lt;IntPtr[]&gt;.</returns>
        private Task<IntPtr[]> SendFormatRequest()
        {
            if (_requestedFormatsTcs == null || _requestedFormatsTcs.Task.IsCompleted)
                _requestedFormatsTcs = new TaskCompletionSource<IntPtr[]>();
            XConvertSelection(_x11.Display, _x11.Atoms.CLIPBOARD, _x11.Atoms.TARGETS, _x11.Atoms.TARGETS, _handle,
                IntPtr.Zero);
            return _requestedFormatsTcs.Task;
        }

        /// <summary>
        /// Sends the text request.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>Task&lt;System.String&gt;.</returns>
        private Task<string> SendTextRequest(IntPtr format)
        {
            if (_requestedTextTcs == null || _requestedFormatsTcs.Task.IsCompleted)
                _requestedTextTcs = new TaskCompletionSource<string>();
            XConvertSelection(_x11.Display, _x11.Atoms.CLIPBOARD, format, format, _handle, IntPtr.Zero);
            return _requestedTextTcs.Task;
        }

        /// <summary>
        /// Stores the atoms in clipboard manager.
        /// </summary>
        /// <param name="atoms">The atoms.</param>
        private void StoreAtomsInClipboardManager(IntPtr[] atoms)
        {
            if (_x11.Atoms.CLIPBOARD_MANAGER != IntPtr.Zero && _x11.Atoms.SAVE_TARGETS != IntPtr.Zero)
            {
                var clipboardManager = XGetSelectionOwner(_x11.Display, _x11.Atoms.CLIPBOARD_MANAGER);
                if (clipboardManager != IntPtr.Zero)
                {
                    XChangeProperty(_x11.Display, _handle, _avaloniaSaveTargetsAtom, _x11.Atoms.XA_ATOM, 32,
                        PropertyMode.Replace,
                        atoms, atoms.Length);
                    XConvertSelection(_x11.Display, _x11.Atoms.CLIPBOARD_MANAGER, _x11.Atoms.SAVE_TARGETS,
                        _avaloniaSaveTargetsAtom, _handle, IntPtr.Zero);
                }
            }
        }

        #endregion Methods
    }
}
