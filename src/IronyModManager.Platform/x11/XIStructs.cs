// ***********************************************************************
// Assembly         : IronyModManager.Platform
// Author           : Avalonia
// Created          : 10-29-2020
//
// Last Modified By : Mario
// Last Modified On : 10-29-2020
// ***********************************************************************
// <copyright file="XIStructs.cs" company="Avalonia">
//     Avalonia
// </copyright>
// <summary>Rip of Avalonia.x11 so that I don't have to upgrade to 0.10 at this time</summary>
// ***********************************************************************
using System;
using System.Runtime.InteropServices;
using IronyModManager.Shared;
using Bool = System.Boolean;

// ReSharper disable IdentifierTypo
// ReSharper disable FieldCanBeMadeReadOnly.Global
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable 649

namespace IronyModManager.Platform.x11
{
    /// <summary>
    /// Enum XiDeviceEventFlags
    /// </summary>
    [Flags]
    [ExcludeFromCoverage("External component.")]
    public enum XiDeviceEventFlags : int
    {
        /// <summary>
        /// The none
        /// </summary>
        None = 0,

        /// <summary>
        /// The xi pointer emulated
        /// </summary>
        XIPointerEmulated = (1 << 16)
    }

    /// <summary>
    /// Enum XiDeviceClass
    /// </summary>
    [ExcludeFromCoverage("External component.")]
    internal enum XiDeviceClass
    {
        /// <summary>
        /// The xi key class
        /// </summary>
        XIKeyClass = 0,

        /// <summary>
        /// The xi button class
        /// </summary>
        XIButtonClass = 1,

        /// <summary>
        /// The xi valuator class
        /// </summary>
        XIValuatorClass = 2,

        /// <summary>
        /// The xi scroll class
        /// </summary>
        XIScrollClass = 3,

        /// <summary>
        /// The xi touch class
        /// </summary>
        XITouchClass = 8,
    }

    /// <summary>
    /// Enum XiDeviceType
    /// </summary>
    internal enum XiDeviceType
    {
        /// <summary>
        /// The xi master pointer
        /// </summary>
        XIMasterPointer = 1,

        /// <summary>
        /// The xi master keyboard
        /// </summary>
        XIMasterKeyboard = 2,

        /// <summary>
        /// The xi slave pointer
        /// </summary>
        XISlavePointer = 3,

        /// <summary>
        /// The xi slave keyboard
        /// </summary>
        XISlaveKeyboard = 4,

        /// <summary>
        /// The xi floating slave
        /// </summary>
        XIFloatingSlave = 5
    }

    /// <summary>
    /// Enum XiEventType
    /// </summary>
    internal enum XiEventType
    {
        /// <summary>
        /// The xi device changed
        /// </summary>
        XI_DeviceChanged = 1,

        /// <summary>
        /// The xi key press
        /// </summary>
        XI_KeyPress = 2,

        /// <summary>
        /// The xi key release
        /// </summary>
        XI_KeyRelease = 3,

        /// <summary>
        /// The xi button press
        /// </summary>
        XI_ButtonPress = 4,

        /// <summary>
        /// The xi button release
        /// </summary>
        XI_ButtonRelease = 5,

        /// <summary>
        /// The xi motion
        /// </summary>
        XI_Motion = 6,

        /// <summary>
        /// The xi enter
        /// </summary>
        XI_Enter = 7,

        /// <summary>
        /// The xi leave
        /// </summary>
        XI_Leave = 8,

        /// <summary>
        /// The xi focus in
        /// </summary>
        XI_FocusIn = 9,

        /// <summary>
        /// The xi focus out
        /// </summary>
        XI_FocusOut = 10,

        /// <summary>
        /// The xi hierarchy changed
        /// </summary>
        XI_HierarchyChanged = 11,

        /// <summary>
        /// The xi property event
        /// </summary>
        XI_PropertyEvent = 12,

        /// <summary>
        /// The xi raw key press
        /// </summary>
        XI_RawKeyPress = 13,

        /// <summary>
        /// The xi raw key release
        /// </summary>
        XI_RawKeyRelease = 14,

        /// <summary>
        /// The xi raw button press
        /// </summary>
        XI_RawButtonPress = 15,

        /// <summary>
        /// The xi raw button release
        /// </summary>
        XI_RawButtonRelease = 16,

        /// <summary>
        /// The xi raw motion
        /// </summary>
        XI_RawMotion = 17,

        /// <summary>
        /// The xi touch begin
        /// </summary>
        XI_TouchBegin = 18 /* XI 2.2 */,

        /// <summary>
        /// The xi touch update
        /// </summary>
        XI_TouchUpdate = 19,

        /// <summary>
        /// The xi touch end
        /// </summary>
        XI_TouchEnd = 20,

        /// <summary>
        /// The xi touch ownership
        /// </summary>
        XI_TouchOwnership = 21,

        /// <summary>
        /// The xi raw touch begin
        /// </summary>
        XI_RawTouchBegin = 22,

        /// <summary>
        /// The xi raw touch update
        /// </summary>
        XI_RawTouchUpdate = 23,

        /// <summary>
        /// The xi raw touch end
        /// </summary>
        XI_RawTouchEnd = 24,

        /// <summary>
        /// The xi barrier hit
        /// </summary>
        XI_BarrierHit = 25 /* XI 2.3 */,

        /// <summary>
        /// The xi barrier leave
        /// </summary>
        XI_BarrierLeave = 26,

        /// <summary>
        /// The xi lastevent
        /// </summary>
        XI_LASTEVENT = XI_BarrierLeave,
    }

    /// <summary>
    /// Enum XiPredefinedDeviceId
    /// </summary>
    internal enum XiPredefinedDeviceId : int
    {
        /// <summary>
        /// The xi all devices
        /// </summary>
        XIAllDevices = 0,

        /// <summary>
        /// The xi all master devices
        /// </summary>
        XIAllMasterDevices = 1
    }

    /// <summary>
    /// Enum XiScrollType
    /// </summary>
    internal enum XiScrollType
    {
        /// <summary>
        /// The vertical
        /// </summary>
        Vertical = 1,

        /// <summary>
        /// The horizontal
        /// </summary>
        Horizontal = 2
    }

    /// <summary>
    /// Struct XIAddMasterInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XIAddMasterInfo
    {
        /// <summary>
        /// The type
        /// </summary>
        public int Type;

        /// <summary>
        /// The name
        /// </summary>
        public IntPtr Name;

        /// <summary>
        /// The send core
        /// </summary>
        public Bool SendCore;

        /// <summary>
        /// The enable
        /// </summary>
        public Bool Enable;
    }

    /// <summary>
    /// Struct XIAnyClassInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XIAnyClassInfo
    {
        /// <summary>
        /// The type
        /// </summary>
        public XiDeviceClass Type;

        /// <summary>
        /// The sourceid
        /// </summary>
        public int Sourceid;
    };

    /// <summary>
    /// Struct XIAnyHierarchyChangeInfo
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct XIAnyHierarchyChangeInfo
    {
        /// <summary>
        /// The type
        /// </summary>
        [FieldOffset(0)]
        public int type; /* must be first element */

        /// <summary>
        /// The add
        /// </summary>
        [FieldOffset(4)]
        public XIAddMasterInfo add;

        /// <summary>
        /// The remove
        /// </summary>
        [FieldOffset(4)]
        public XIRemoveMasterInfo remove;

        /// <summary>
        /// The attach
        /// </summary>
        [FieldOffset(4)]
        public XIAttachSlaveInfo attach;

        /// <summary>
        /// The detach
        /// </summary>
        [FieldOffset(4)]
        public XIDetachSlaveInfo detach;
    };

    /// <summary>
    /// Struct XIAttachSlaveInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XIAttachSlaveInfo
    {
        /// <summary>
        /// The type
        /// </summary>
        public int Type;

        /// <summary>
        /// The deviceid
        /// </summary>
        public int Deviceid;

        /// <summary>
        /// Creates new master.
        /// </summary>
        public int NewMaster;
    };

    /// <summary>
    /// Struct XIButtonClassInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct XIButtonClassInfo
    {
        /// <summary>
        /// The type
        /// </summary>
        public int Type;

        /// <summary>
        /// The sourceid
        /// </summary>
        public int Sourceid;

        /// <summary>
        /// The number buttons
        /// </summary>
        public int NumButtons;

        /// <summary>
        /// The labels
        /// </summary>
        public IntPtr* Labels;

        /// <summary>
        /// The state
        /// </summary>
        public XIButtonState State;
    };

    /// <summary>
    /// Struct XIButtonState
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct XIButtonState
    {
        /// <summary>
        /// The mask length
        /// </summary>
        public int MaskLen;

        /// <summary>
        /// The mask
        /// </summary>
        public byte* Mask;
    };

    /// <summary>
    /// Struct XIDetachSlaveInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XIDetachSlaveInfo
    {
        /// <summary>
        /// The type
        /// </summary>
        public int Type;

        /// <summary>
        /// The deviceid
        /// </summary>
        public int Deviceid;
    };

    /// <summary>
    /// Struct XIDeviceChangedEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct XIDeviceChangedEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        public int Type; /* GenericEvent */

        /// <summary>
        /// The serial
        /// </summary>
        public UIntPtr Serial; /* # of last request processed by server */

        /// <summary>
        /// The send event
        /// </summary>
        public Bool SendEvent; /* true if this came from a SendEvent request */

        /// <summary>
        /// The display
        /// </summary>
        public IntPtr Display; /* Display the event was read from */

        /// <summary>
        /// The extension
        /// </summary>
        public int Extension; /* XI extension offset */

        /// <summary>
        /// The evtype
        /// </summary>
        public int Evtype; /* XI_DeviceChanged */

        /// <summary>
        /// The time
        /// </summary>
        public IntPtr Time;

        /// <summary>
        /// The deviceid
        /// </summary>
        public int Deviceid; /* id of the device that changed */

        /// <summary>
        /// The sourceid
        /// </summary>
        public int Sourceid; /* Source for the new classes. */

        /// <summary>
        /// The reason
        /// </summary>
        public int Reason; /* Reason for the change */

        /// <summary>
        /// The number classes
        /// </summary>
        public int NumClasses;

        /// <summary>
        /// The classes
        /// </summary>
        public XIAnyClassInfo** Classes; /* same as in XIDeviceInfo */
    }

    /// <summary>
    /// Struct XIDeviceEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XIDeviceEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        public XEventName type; /* GenericEvent */

        /// <summary>
        /// The serial
        /// </summary>
        public UIntPtr serial; /* # of last request processed by server */

        /// <summary>
        /// The send event
        /// </summary>
        public Bool send_event; /* true if this came from a SendEvent request */

        /// <summary>
        /// The display
        /// </summary>
        public IntPtr display; /* Display the event was read from */

        /// <summary>
        /// The extension
        /// </summary>
        public int extension; /* XI extension offset */

        /// <summary>
        /// The evtype
        /// </summary>
        public XiEventType evtype;

        /// <summary>
        /// The time
        /// </summary>
        public IntPtr time;

        /// <summary>
        /// The deviceid
        /// </summary>
        public int deviceid;

        /// <summary>
        /// The sourceid
        /// </summary>
        public int sourceid;

        /// <summary>
        /// The detail
        /// </summary>
        public int detail;

        /// <summary>
        /// The root window
        /// </summary>
        public IntPtr RootWindow;

        /// <summary>
        /// The event window
        /// </summary>
        public IntPtr EventWindow;

        /// <summary>
        /// The child window
        /// </summary>
        public IntPtr ChildWindow;

        /// <summary>
        /// The root x
        /// </summary>
        public double root_x;

        /// <summary>
        /// The root y
        /// </summary>
        public double root_y;

        /// <summary>
        /// The event x
        /// </summary>
        public double event_x;

        /// <summary>
        /// The event y
        /// </summary>
        public double event_y;

        /// <summary>
        /// The flags
        /// </summary>
        public XiDeviceEventFlags flags;

        /// <summary>
        /// The buttons
        /// </summary>
        public XIButtonState buttons;

        /// <summary>
        /// The valuators
        /// </summary>
        public XIValuatorState valuators;

        /// <summary>
        /// The mods
        /// </summary>
        public XIModifierState mods;

        /// <summary>
        /// The group
        /// </summary>
        public XIModifierState group;
    }

    /// <summary>
    /// Struct XIDeviceInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct XIDeviceInfo
    {
        /// <summary>
        /// The deviceid
        /// </summary>
        public int Deviceid;

        /// <summary>
        /// The name
        /// </summary>
        public IntPtr Name;

        /// <summary>
        /// The use
        /// </summary>
        public XiDeviceType Use;

        /// <summary>
        /// The attachment
        /// </summary>
        public int Attachment;

        /// <summary>
        /// The enabled
        /// </summary>
        public Bool Enabled;

        /// <summary>
        /// The number classes
        /// </summary>
        public int NumClasses;

        /// <summary>
        /// The classes
        /// </summary>
        public XIAnyClassInfo** Classes;
    }

    /// <summary>
    /// Struct XIEvent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct XIEvent
    {
        /// <summary>
        /// The type
        /// </summary>
        public int type; /* GenericEvent */

        /// <summary>
        /// The serial
        /// </summary>
        public UIntPtr serial; /* # of last request processed by server */

        /// <summary>
        /// The send event
        /// </summary>
        public Bool send_event; /* true if this came from a SendEvent request */

        /// <summary>
        /// The display
        /// </summary>
        public IntPtr display; /* Display the event was read from */

        /// <summary>
        /// The extension
        /// </summary>
        public int extension; /* XI extension offset */

        /// <summary>
        /// The evtype
        /// </summary>
        public XiEventType evtype;

        /// <summary>
        /// The time
        /// </summary>
        public IntPtr time;
    }

    /// <summary>
    /// Struct XIEventMask
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct XIEventMask
    {
        /// <summary>
        /// The deviceid
        /// </summary>
        public int Deviceid;

        /// <summary>
        /// The mask length
        /// </summary>
        public int MaskLen;

        /// <summary>
        /// The mask
        /// </summary>
        public int* Mask;
    };

    /// <summary>
    /// Struct XIKeyClassInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct XIKeyClassInfo
    {
        /// <summary>
        /// The type
        /// </summary>
        public int Type;

        /// <summary>
        /// The sourceid
        /// </summary>
        public int Sourceid;

        /// <summary>
        /// The number keycodes
        /// </summary>
        public int NumKeycodes;

        /// <summary>
        /// The keycodes
        /// </summary>
        public int* Keycodes;
    };

    /// <summary>
    /// Struct XIModifierState
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XIModifierState
    {
        /// <summary>
        /// The base
        /// </summary>
        public int Base;

        /// <summary>
        /// The latched
        /// </summary>
        public int Latched;

        /// <summary>
        /// The locked
        /// </summary>
        public int Locked;

        /// <summary>
        /// The effective
        /// </summary>
        public int Effective;
    };

    /// <summary>
    /// Struct XIRemoveMasterInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XIRemoveMasterInfo
    {
        /// <summary>
        /// The type
        /// </summary>
        public int Type;

        /// <summary>
        /// The deviceid
        /// </summary>
        public int Deviceid;

        /// <summary>
        /// The return mode
        /// </summary>
        public int ReturnMode; /* AttachToMaster, Floating */

        /// <summary>
        /// The return pointer
        /// </summary>
        public int ReturnPointer;

        /// <summary>
        /// The return keyboard
        /// </summary>
        public int ReturnKeyboard;
    };

    /// <summary>
    /// Struct XIScrollClassInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XIScrollClassInfo
    {
        /// <summary>
        /// The type
        /// </summary>
        public int Type;

        /// <summary>
        /// The sourceid
        /// </summary>
        public int Sourceid;

        /// <summary>
        /// The number
        /// </summary>
        public int Number;

        /// <summary>
        /// The scroll type
        /// </summary>
        public XiScrollType ScrollType;

        /// <summary>
        /// The increment
        /// </summary>
        public double Increment;

        /// <summary>
        /// The flags
        /// </summary>
        public int Flags;
    };

    /// <summary>
    /// Struct XITouchClassInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XITouchClassInfo
    {
        /// <summary>
        /// The type
        /// </summary>
        public int Type;

        /// <summary>
        /// The sourceid
        /// </summary>
        public int Sourceid;

        /// <summary>
        /// The mode
        /// </summary>
        public int Mode;

        /// <summary>
        /// The number touches
        /// </summary>
        public int NumTouches;
    };

    /// <summary>
    /// Struct XIValuatorClassInfo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct XIValuatorClassInfo
    {
        /// <summary>
        /// The type
        /// </summary>
        public int Type;

        /// <summary>
        /// The sourceid
        /// </summary>
        public int Sourceid;

        /// <summary>
        /// The number
        /// </summary>
        public int Number;

        /// <summary>
        /// The label
        /// </summary>
        public IntPtr Label;

        /// <summary>
        /// Determines the minimum of the parameters.
        /// </summary>
        public double Min;

        /// <summary>
        /// Determines the maximum of the parameters.
        /// </summary>
        public double Max;

        /// <summary>
        /// The value
        /// </summary>
        public double Value;

        /// <summary>
        /// The resolution
        /// </summary>
        public int Resolution;

        /// <summary>
        /// The mode
        /// </summary>
        public int Mode;
    };

    /// <summary>
    /// Struct XIValuatorState
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct XIValuatorState
    {
        /// <summary>
        /// The mask length
        /// </summary>
        public int MaskLen;

        /// <summary>
        /// The mask
        /// </summary>
        public byte* Mask;

        /// <summary>
        /// The values
        /// </summary>
        public double* Values;
    };

    /* new in XI 2.1 */
}
