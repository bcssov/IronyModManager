// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="XI2Manager.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Raw;
using static IronyModManager.Platform.x11.XLib;

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Interface IXI2Client
    /// </summary>
    internal interface IXI2Client
    {
        #region Properties

        /// <summary>
        /// Gets the input root.
        /// </summary>
        /// <value>The input root.</value>
        IInputRoot InputRoot { get; }

        /// <summary>
        /// Gets the mouse device.
        /// </summary>
        /// <value>The mouse device.</value>
        IMouseDevice MouseDevice { get; }

        /// <summary>
        /// Gets the touch device.
        /// </summary>
        /// <value>The touch device.</value>
        TouchDevice TouchDevice { get; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Schedules the x i2 input.
        /// </summary>
        /// <param name="args">The <see cref="RawInputEventArgs" /> instance containing the event data.</param>
        void ScheduleXI2Input(RawInputEventArgs args);

        #endregion Methods
    }

    /// <summary>
    /// Class ParsedDeviceEvent.
    /// </summary>
    internal unsafe class ParsedDeviceEvent
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsedDeviceEvent" /> class.
        /// </summary>
        /// <param name="ev">The ev.</param>
        public ParsedDeviceEvent(XIDeviceEvent* ev)
        {
            Type = ev->evtype;
            Timestamp = (ulong)ev->time.ToInt64();
            var state = (XModifierMask)ev->mods.Effective;
            if (state.HasFlag(XModifierMask.ShiftMask))
                Modifiers |= RawInputModifiers.Shift;
            if (state.HasFlag(XModifierMask.ControlMask))
                Modifiers |= RawInputModifiers.Control;
            if (state.HasFlag(XModifierMask.Mod1Mask))
                Modifiers |= RawInputModifiers.Alt;
            if (state.HasFlag(XModifierMask.Mod4Mask))
                Modifiers |= RawInputModifiers.Meta;

            if (ev->buttons.MaskLen > 0)
            {
                var buttons = ev->buttons.Mask;
                if (XIMaskIsSet(buttons, 1))
                    Modifiers |= RawInputModifiers.LeftMouseButton;

                if (XIMaskIsSet(buttons, 2))
                    Modifiers |= RawInputModifiers.MiddleMouseButton;

                if (XIMaskIsSet(buttons, 3))
                    Modifiers |= RawInputModifiers.RightMouseButton;
            }

            Valuators = new Dictionary<int, double>();
            Position = new Point(ev->event_x, ev->event_y);
            var values = ev->valuators.Values;
            for (var c = 0; c < ev->valuators.MaskLen * 8; c++)
                if (XIMaskIsSet(ev->valuators.Mask, c))
                    Valuators[c] = *values++;
            if (Type == XiEventType.XI_ButtonPress || Type == XiEventType.XI_ButtonRelease)
                Button = ev->detail;
            Detail = ev->detail;
            Emulated = ev->flags.HasFlag(XiDeviceEventFlags.XIPointerEmulated);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the button.
        /// </summary>
        /// <value>The button.</value>
        public int Button { get; set; }

        /// <summary>
        /// Gets or sets the detail.
        /// </summary>
        /// <value>The detail.</value>
        public int Detail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ParsedDeviceEvent" /> is emulated.
        /// </summary>
        /// <value><c>true</c> if emulated; otherwise, <c>false</c>.</value>
        public bool Emulated { get; set; }

        /// <summary>
        /// Gets the modifiers.
        /// </summary>
        /// <value>The modifiers.</value>
        public RawInputModifiers Modifiers { get; }

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        public Point Position { get; }

        /// <summary>
        /// Gets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public ulong Timestamp { get; }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public XiEventType Type { get; }

        /// <summary>
        /// Gets the valuators.
        /// </summary>
        /// <value>The valuators.</value>
        public Dictionary<int, double> Valuators { get; }

        #endregion Properties
    }

    /// <summary>
    /// Class XI2Manager.
    /// </summary>
    internal unsafe class XI2Manager
    {
        #region Fields

        /// <summary>
        /// The default event types
        /// </summary>
        private static readonly XiEventType[] DefaultEventTypes = new XiEventType[]
        {
            XiEventType.XI_Motion,
            XiEventType.XI_ButtonPress,
            XiEventType.XI_ButtonRelease
        };

        /// <summary>
        /// The multi touch event types
        /// </summary>
        private static readonly XiEventType[] MultiTouchEventTypes = new XiEventType[]
        {
            XiEventType.XI_TouchBegin,
            XiEventType.XI_TouchUpdate,
            XiEventType.XI_TouchEnd
        };

        /// <summary>
        /// The clients
        /// </summary>
        private readonly Dictionary<IntPtr, IXI2Client> _clients = new Dictionary<IntPtr, IXI2Client>();

        /// <summary>
        /// The multitouch
        /// </summary>
        private bool _multitouch;

        /// <summary>
        /// The platform
        /// </summary>
        private AvaloniaX11Platform _platform;

        /// <summary>
        /// The pointer device
        /// </summary>
        private PointerDeviceInfo _pointerDevice;

        /// <summary>
        /// The X11
        /// </summary>
        private X11Info _x11;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Adds the window.
        /// </summary>
        /// <param name="xid">The xid.</param>
        /// <param name="window">The window.</param>
        /// <returns>XEventMask.</returns>
        public XEventMask AddWindow(IntPtr xid, IXI2Client window)
        {
            _clients[xid] = window;

            var eventsLength = DefaultEventTypes.Length;

            if (_multitouch)
                eventsLength += MultiTouchEventTypes.Length;

            var events = new List<XiEventType>(eventsLength);

            events.AddRange(DefaultEventTypes);

            if (_multitouch)
                events.AddRange(MultiTouchEventTypes);

            XiSelectEvents(_x11.Display, xid,
                new Dictionary<int, List<XiEventType>> { [_pointerDevice.Id] = events });

            // We are taking over mouse input handling from here
            return XEventMask.PointerMotionMask
                   | XEventMask.ButtonMotionMask
                   | XEventMask.Button1MotionMask
                   | XEventMask.Button2MotionMask
                   | XEventMask.Button3MotionMask
                   | XEventMask.Button4MotionMask
                   | XEventMask.Button5MotionMask
                   | XEventMask.ButtonPressMask
                   | XEventMask.ButtonReleaseMask;
        }

        /// <summary>
        /// Initializes the specified platform.
        /// </summary>
        /// <param name="platform">The platform.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public bool Init(AvaloniaX11Platform platform)
        {
            _platform = platform;
            _x11 = platform.Info;
            _multitouch = platform.Options?.EnableMultiTouch ?? false;
            var devices = (XIDeviceInfo*)XIQueryDevice(_x11.Display,
                (int)XiPredefinedDeviceId.XIAllMasterDevices, out int num);
            for (var c = 0; c < num; c++)
            {
                if (devices[c].Use == XiDeviceType.XIMasterPointer)
                {
                    _pointerDevice = new PointerDeviceInfo(devices[c]);
                    break;
                }
            }
            if (_pointerDevice == null)
                return false;
            /*
            int mask = 0;

            XISetMask(ref mask, XiEventType.XI_DeviceChanged);
            var emask = new XIEventMask
            {
                Mask = &mask,
                Deviceid = _pointerDevice.Id,
                MaskLen = XiEventMaskLen
            };

            if (XISelectEvents(_x11.Display, _x11.RootWindow, &emask, 1) != Status.Success)
                return false;
            return true;
            */
            return XiSelectEvents(_x11.Display, _x11.RootWindow, new Dictionary<int, List<XiEventType>>
            {
                [_pointerDevice.Id] = new List<XiEventType>
                {
                    XiEventType.XI_DeviceChanged
                }
            }) == Status.Success;
        }

        /// <summary>
        /// Called when [event].
        /// </summary>
        /// <param name="xev">The xev.</param>
        public void OnEvent(XIEvent* xev)
        {
            if (xev->evtype == XiEventType.XI_DeviceChanged)
            {
                var changed = (XIDeviceChangedEvent*)xev;
                _pointerDevice.Update(changed->Classes, changed->NumClasses);
            }

            if ((xev->evtype >= XiEventType.XI_ButtonPress && xev->evtype <= XiEventType.XI_Motion)
                || (xev->evtype >= XiEventType.XI_TouchBegin && xev->evtype <= XiEventType.XI_TouchEnd))
            {
                var dev = (XIDeviceEvent*)xev;
                if (_clients.TryGetValue(dev->EventWindow, out var client))
                    OnDeviceEvent(client, new ParsedDeviceEvent(dev));
            }
        }

        /// <summary>
        /// Called when [window destroyed].
        /// </summary>
        /// <param name="xid">The xid.</param>
        public void OnWindowDestroyed(IntPtr xid) => _clients.Remove(xid);

        /// <summary>
        /// Called when [device event].
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="ev">The ev.</param>
        private void OnDeviceEvent(IXI2Client client, ParsedDeviceEvent ev)
        {
            if (ev.Type == XiEventType.XI_TouchBegin
                || ev.Type == XiEventType.XI_TouchUpdate
                || ev.Type == XiEventType.XI_TouchEnd)
            {
                var type = ev.Type == XiEventType.XI_TouchBegin ?
                    RawPointerEventType.TouchBegin :
                    (ev.Type == XiEventType.XI_TouchUpdate ?
                        RawPointerEventType.TouchUpdate :
                        RawPointerEventType.TouchEnd);
                client.ScheduleXI2Input(new RawTouchEventArgs(client.TouchDevice,
                    ev.Timestamp, client.InputRoot, type, ev.Position, ev.Modifiers, ev.Detail));
                return;
            }

            if (_multitouch && ev.Emulated)
                return;

            if (ev.Type == XiEventType.XI_Motion)
            {
                Vector scrollDelta = default;
                foreach (var v in ev.Valuators)
                {
                    foreach (var scroller in _pointerDevice.Scrollers)
                    {
                        if (scroller.Number == v.Key)
                        {
                            var old = _pointerDevice.Valuators[scroller.Number].Value;
                            // Value was zero after reset, ignore the event and use it as a reference next time
                            if (old == 0)
                                continue;
                            var diff = (old - v.Value) / scroller.Increment;
                            if (scroller.ScrollType == XiScrollType.Horizontal)
                                scrollDelta = scrollDelta.WithX(scrollDelta.X + diff);
                            else
                                scrollDelta = scrollDelta.WithY(scrollDelta.Y + diff);
                        }
                    }
                }

                if (scrollDelta != default)
                    client.ScheduleXI2Input(new RawMouseWheelEventArgs(client.MouseDevice, ev.Timestamp,
                        client.InputRoot, ev.Position, scrollDelta, ev.Modifiers));
                if (_pointerDevice.HasMotion(ev))
                    client.ScheduleXI2Input(new RawPointerEventArgs(client.MouseDevice, ev.Timestamp, client.InputRoot,
                        RawPointerEventType.Move, ev.Position, ev.Modifiers));
            }

            if (ev.Type == XiEventType.XI_ButtonPress || ev.Type == XiEventType.XI_ButtonRelease)
            {
                var down = ev.Type == XiEventType.XI_ButtonPress;
                var type =
                    ev.Button == 1 ? (down ? RawPointerEventType.LeftButtonDown : RawPointerEventType.LeftButtonUp)
                    : ev.Button == 2 ? (down ? RawPointerEventType.MiddleButtonDown : RawPointerEventType.MiddleButtonUp)
                    : ev.Button == 3 ? (down ? RawPointerEventType.RightButtonDown : RawPointerEventType.RightButtonUp)
                    : (RawPointerEventType?)null;
                if (type.HasValue)
                    client.ScheduleXI2Input(new RawPointerEventArgs(client.MouseDevice, ev.Timestamp, client.InputRoot,
                        type.Value, ev.Position, ev.Modifiers));
            }

            _pointerDevice.UpdateValuators(ev.Valuators);
        }

        #endregion Methods

        #region Classes

        /// <summary>
        /// Class DeviceInfo.
        /// </summary>
        private class DeviceInfo
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="DeviceInfo" /> class.
            /// </summary>
            /// <param name="info">The information.</param>
            public DeviceInfo(XIDeviceInfo info)
            {
                Id = info.Deviceid;
                Update(info.Classes, info.NumClasses);
            }

            #endregion Constructors

            #region Properties

            /// <summary>
            /// Gets the identifier.
            /// </summary>
            /// <value>The identifier.</value>
            public int Id { get; }

            /// <summary>
            /// Gets or sets the scrollers.
            /// </summary>
            /// <value>The scrollers.</value>
            public XIScrollClassInfo[] Scrollers { get; private set; }

            /// <summary>
            /// Gets or sets the valuators.
            /// </summary>
            /// <value>The valuators.</value>
            public XIValuatorClassInfo[] Valuators { get; private set; }

            #endregion Properties

            #region Methods

            /// <summary>
            /// Updates the specified classes.
            /// </summary>
            /// <param name="classes">The classes.</param>
            /// <param name="num">The number.</param>
            public virtual void Update(XIAnyClassInfo** classes, int num)
            {
                var valuators = new List<XIValuatorClassInfo>();
                var scrollers = new List<XIScrollClassInfo>();
                for (var c = 0; c < num; c++)
                {
                    if (classes[c]->Type == XiDeviceClass.XIValuatorClass)
                        valuators.Add(*((XIValuatorClassInfo**)classes)[c]);
                    if (classes[c]->Type == XiDeviceClass.XIScrollClass)
                        scrollers.Add(*((XIScrollClassInfo**)classes)[c]);
                }

                Valuators = valuators.ToArray();
                Scrollers = scrollers.ToArray();
            }

            /// <summary>
            /// Updates the valuators.
            /// </summary>
            /// <param name="valuators">The valuators.</param>
            public void UpdateValuators(Dictionary<int, double> valuators)
            {
                foreach (var v in valuators)
                {
                    if (Valuators.Length > v.Key)
                        Valuators[v.Key].Value = v.Value;
                }
            }

            #endregion Methods
        }

        /// <summary>
        /// Class PointerDeviceInfo.
        /// Implements the <see cref="IronyModManager.Platform.x11.XI2Manager.DeviceInfo" />
        /// </summary>
        /// <seealso cref="IronyModManager.Platform.x11.XI2Manager.DeviceInfo" />
        private class PointerDeviceInfo : DeviceInfo
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="PointerDeviceInfo" /> class.
            /// </summary>
            /// <param name="info">The information.</param>
            public PointerDeviceInfo(XIDeviceInfo info) : base(info)
            {
            }

            #endregion Constructors

            #region Methods

            /// <summary>
            /// Determines whether the specified ev has motion.
            /// </summary>
            /// <param name="ev">The ev.</param>
            /// <returns><c>true</c> if the specified ev has motion; otherwise, <c>false</c>.</returns>
            public bool HasMotion(ParsedDeviceEvent ev)
            {
                foreach (var val in ev.Valuators)
                    if (Scrollers.All(s => s.Number != val.Key))
                        return true;

                return false;
            }

            /// <summary>
            /// Determines whether the specified ev has scroll.
            /// </summary>
            /// <param name="ev">The ev.</param>
            /// <returns><c>true</c> if the specified ev has scroll; otherwise, <c>false</c>.</returns>
            public bool HasScroll(ParsedDeviceEvent ev)
            {
                foreach (var val in ev.Valuators)
                    if (Scrollers.Any(s => s.Number == val.Key))
                        return true;

                return false;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}
